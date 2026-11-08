// v0.4.0-alpha
// 
// MIT License
//
// Copyright (c) 2025-present Manas Ravindra Makde
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.


using System;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class Act
{
	// Enums
	[Flags]
	public enum TickFlags
	{
		None = 0,
		Tick = 1 << 0,
		PhysicsTick = 1 << 1,
		LateTick = 1 << 2
	}
	public enum Status
	{
		None,
		Initializing,
		Prologuing,
		Entering,
		Ticking,
		Exiting,
		Concluding,
		Deinitializing
	}
	public enum Outcome
	{
		Interrupted,
		Failure,
		Pending,
		Success,
		Retry
	}
	public enum BlockType
	{
		Interrupt,
		Persistent
	}



	// Public
	public event Action<Act /* act */> OnPreSetup;
	public event Action<Act /* act */> OnPostSetup;
	public event Action<Act /* act */> OnPrePerformReq;
	public event Action<Act /* act */, bool /* willPerform */> OnPostPerformReq;
	public event Action<Act /* act */> OnPerformStart;
	public event Action<Act /* act */> OnPrePrologue;
	public event Action<Act /* act */, Act /* pAct */, Outcome /* pOutcome */> OnPrologueComplete;
	public event Action<Act /* act */> OnPostPrologue;
	public event Action<Act /* act */> OnPreEnter;
	public event Action<Act /* act */> OnPostEnter;
	public event Action<Act /* act */> OnPreTick;
	public event Action<Act /* act */> OnPostTick;
	public event Action<Act /* act */> OnPrePhysicsTick;
	public event Action<Act /* act */> OnPostPhysicsTick;
	public event Action<Act /* act */> OnPreLateTick;
	public event Action<Act /* act */> OnPostLateTick;
	public event Action<Act /* act */> OnPreExit;
	public event Action<Act /* act */> OnPostExit;
	public event Action<Act /* act */> OnPerformEnd;
	public event Action<Act /* act */> OnPreCleanup;
	public event Action<Act /* act */> OnPostCleanup;
	public event Action<Act /* act */, bool /* newIsEnabled */> OnEnableChanged;
	public event Action<Act /* act */, Act /* blockingAct */, BlockType /* blockType */, bool /* didBlock */> OnBlockChanged;

	[NonSerialized] public Func<Act, List<List<Act>>> prologue = null;
	[NonSerialized] public List<Func<Act, bool>> performConditions = new List<Func<Act, bool>>();
	public bool isVerbose = false;

	public void Init(string newName = "", Theater newTheater = null)
	{
		_initName = newName;
		_initTheater = newTheater;
		Redirect(Status.Initializing);
	}
	public void Deinit()
	{
		Abort();
		Redirect(Status.Deinitializing);
	}
	public bool Perform()
	{
		return PerformImpl();
	}
	public void PerformDeferred(TickFlags tickFlag = TickFlags.PhysicsTick)
	{
		// Warn if null theater provided
		if (_theater == null)
		{
			WriteLog("Cannot perform deferred, Assign a theater first!");
			return;
		}

		Theater.Friend.StageDeferred(_theater, this, tickFlag);
	}
	public void Retry()
	{
		if (IsPerforming())
		{
			Finish(Outcome.Retry);
		}
		else
		{
			Perform();
		}
	}
	public void Abort()
	{
		Finish(Outcome.Interrupted);


		// Clear deferred
		if (_theater != null)
		{
			Theater.Friend.UnstageDeferred(_theater, this);
			return;
		}
	}
	public void AddToBlock(List<Act> acts, BlockType blockType = BlockType.Persistent)
	{
		foreach (Act bAct in acts)
		{
			// Skip if self (reserved for enable/disable) or null
			if (bAct == this || bAct == null)
			{
				WriteLog("Trying to block self or another invalid act!");
				continue;
			}


			// Unblock if type changed to interrupted
			if (IsPerforming() && _actsToBlock.ContainsKey(bAct) && blockType == BlockType.Interrupt)
			{
				bAct.UnblockSelf(this);
			}


			// Add to block list
			_actsToBlock[bAct] = blockType;


			// Persistent block if performing
			if (IsPerforming() && blockType == BlockType.Persistent)
			{
				bAct.BlockSelf(this, blockType);
			}
		}
	}
	public void RemoveFromBlock(List<Act> acts)
	{
		foreach (Act bAct in acts)
		{
			// Skip if self (reserved for enable/disable) or null
			if (bAct == this || bAct == null)
			{
				WriteLog("Trying to unblock self or another invalid act!");
				continue;
			}


			// Unblock if performing
			bAct.UnblockSelf(this);


			// Remove from block list
			_actsToBlock.Remove(bAct);
		}
	}
	public void SetEnabled(bool newEnabled)
	{
		// Return if trying to reassign same value
		if (newEnabled == IsEnabled())
		{
			return;
		}


		// Block unblock self
		if (!newEnabled)
		{
			BlockSelf(this, BlockType.Persistent);
		}
		else
		{
			UnblockSelf(this);
		}


		// Broadcast enabled disabled
		OnEnableChanged?.Invoke(this, IsEnabled());
	}
	public bool HasInitialized()
	{
		return _hasInitialized;
	}
	public bool IsRetrying()
	{
		return _isRetrying;
	}
	public bool IsPerforming()
	{
		return _status != Status.Initializing && _status != Status.Deinitializing && _status != Status.None;
	}
	public bool IsOngoing()
	{
		return _status != Status.Initializing && _status != Status.Deinitializing && _status != Status.None && _status != Status.Prologuing;
	}
	public bool IsEnabled()
	{
		return !_blockedByActs.Contains(this);
	}
	public bool IsBlocked()
	{
		// Incase act is disabled
		if (_blockedByActs.Count == 1 && _blockedByActs.Contains(this))
		{
			return false;
		}

		return _blockedByActs.Count != 0;
	}
	public bool IsBlockedBy(Act byAct)
	{
		return _blockedByActs.Contains(byAct);
	}
	public bool CanTick(TickFlags type)
	{
		return (_tickFlags & type) != 0;
	}
	public TickFlags GetTickFlags()
	{
		return _tickFlags;
	}
	public Theater GetTheater()
	{
		return _theater;
	}
	public GameObject GetOwner()
	{
		return _theater != null ? _theater.gameObject : null;
	}
	public HashSet<Act> GetBlockedByActs()
	{
		return new HashSet<Act>(_blockedByActs);
	}
	public Dictionary<Act, BlockType> GetActsToBlock()
	{
		return new Dictionary<Act, BlockType>(_actsToBlock);
	}
	public Status GetStatus()
	{
		return _status;
	}
	public Status GetPrevStatus()
	{
		return _prevStatus;
	}
	public Outcome GetOutcome()
	{
		return _outcome;
	}
	public int GetLifetimePerformCount()
	{
		return _lifetimePerformCount;
	}
	public int GetPerformCount()
	{
		return _performCount;
	}
	public int GetTickCount()
	{
		return _tickCount;
	}
	public int GetPhysicsTickCount()
	{
		return _physicsTickCount;
	}
	public int GetLateTickCount()
	{
		return _lateTickCount;
	}
	static public float GetDelta()
	{
		return Time.deltaTime;
	}
	static public float GetPhysicsDelta()
	{
		return Time.fixedDeltaTime;
	}
	public string GetName()
	{
		return _name;
	}



	// Protected
	[NonSerialized] protected string _name = "";
	[NonSerialized] protected bool _canReperform = false;
	[NonSerialized] protected TickFlags _tickFlags = TickFlags.None;

	protected virtual void Setup()
	{
	}
	protected virtual bool CanPerform()
	{
		return true;
	}
	protected virtual Outcome Enter()
	{
		return _tickFlags != TickFlags.None ? Outcome.Pending : Outcome.Success;
	}
	protected virtual Outcome Tick()
	{
		return Outcome.Success;
	}
	protected virtual Outcome PhysicsTick()
	{
		return Outcome.Success;
	}
	protected virtual Outcome LateTick()
	{
		return Outcome.Success;
	}
	protected virtual void Exit()
	{
	}
	protected virtual void Cleanup()
	{
	}
	protected void Finish(Outcome newOutcome = Outcome.Success)
	{
		Redirect(_status == Status.Prologuing ? Status.Concluding : Status.Exiting, newOutcome);
	}
	protected virtual void BlockSelf(Act byAct, BlockType blockType)
	{
		// Return incase null act
		if (byAct == null)
		{
			WriteLog("Cannot block, null act provided!");
			return;
		}


		// Return if already blocked
		if (_blockedByActs.Contains(byAct))
		{
			return;
		}


		// Return if both acts in same prologue chain
		if (byAct != this && InSamePrologueChain(byAct, this))
		{
			WriteLog("Cannot block, Both " + _name + " & " + byAct._name + " are in the same prologue chain!");
			return;
		}


		// Add to blocked by list if persistent
		if (blockType == BlockType.Persistent)
		{
			_blockedByActs.Add(byAct);
		}


		// Finish interrupted incase performing
		Finish(Outcome.Interrupted);


		// Broadcast blocked
		if (byAct != this)
		{
			OnBlockChanged?.Invoke(this, byAct, blockType, true);
		}
	}
	protected virtual void UnblockSelf(Act byAct)
	{
		// Return incase null act
		if (byAct == null)
		{
			WriteLog("Cannot unblock, null act provided!");
			return;
		}


		// Return if not currently blocked by act
		if (!_blockedByActs.Contains(byAct))
		{
			return;
		}


		// Persistent unblocking
		_blockedByActs.Remove(byAct);


		// Broadcast unblocked
		if (byAct != this)
		{
			OnBlockChanged?.Invoke(this, byAct, BlockType.Persistent, false);
		}
	}
	protected virtual void WriteLog(string message, string overrideName = "")
	{
		if (!isVerbose)
		{
			return;
		}

		Debug.LogWarning("[" + (overrideName != "" ? overrideName : _name) + "] " + message);
	}



	// Private
	[NonSerialized] private Theater _theater = null;
	[NonSerialized] private Status _status = Status.None;
	[NonSerialized] private Status _prevStatus = Status.None;
	[NonSerialized] private Outcome _outcome = Outcome.Pending;

	[NonSerialized] private Dictionary<Act, BlockType> _actsToBlock = new Dictionary<Act, BlockType>();
	[NonSerialized] private HashSet<Act> _blockedByActs = new();

	[NonSerialized] private List<Act> _prologueActs = new();
	[NonSerialized] private Dictionary<Act, bool /* isInSequence */> _epilogueActs = new();
	[NonSerialized] private HashSet<Act> _continueEpilogueActs = new();
	[NonSerialized] private HashSet<Act> _topEpilogueActs = new();

	[NonSerialized] private string _initName;
	[NonSerialized] private Theater _initTheater;

	[NonSerialized] private bool _hasInitialized = false;
	[NonSerialized] private bool _isRetrying = false;

	[NonSerialized] private int _prologueIndex = 0;
	[NonSerialized] private int _prologueCompletedCount = 0;

	[NonSerialized] private int _lifetimePerformCount = 0;
	[NonSerialized] private int _performCount = 0;
	[NonSerialized] private int _tickCount = 0;
	[NonSerialized] private int _physicsTickCount = 0;
	[NonSerialized] private int _lateTickCount = 0;

	public static Act GetFirst(HashSet<Act> collection)
	{
		foreach (var data in collection)
		{
			return data;
		}

		return null;
	}
	public static bool InSamePrologueChain(Act actA, Act actB)
	{
		return actA._topEpilogueActs.Overlaps(actB._topEpilogueActs) || actA._topEpilogueActs.Contains(actB) || actB._topEpilogueActs.Contains(actA);
	}
	private static bool LinkPrologueArrays(List<Act> eArray, List<Act> pArray)
	{
		for (int i = 0; i < eArray.Count; i++)
		{
			for (int j = 0; j < pArray.Count; j++)
			{
				if (!LinkPrologue(eArray[i], pArray[j], true))
				{
					return false;
				}
			}
		}

		return true;
	}
	private static bool LinkPrologue(Act eAct, Act pAct, bool inSequence = false)
	{
		// Return if already past prologuing phase 
		if (eAct.IsOngoing())
		{
			return false;
		}

		eAct._prologueActs.Add(pAct);
		pAct._epilogueActs.TryAdd(eAct, inSequence);

		return true;
	}
	private static bool BuildPrologueChain(Act ofAct, Act newTopEpilogue = null, Act oldTopEpilogue = null)
	{
		// Assign top epilogue if not given
		if (newTopEpilogue == null)
		{
			newTopEpilogue = ofAct;
		}


		// Calculate prologues/epilogues
		if (ofAct._topEpilogueActs.Count == 0 && ofAct.prologue != null)
		{
			// Get prologue acts
			var prologueSeq = ofAct.prologue.Invoke(ofAct);
			if (prologueSeq == null)
			{
				ofAct.WriteLog("Cannot build prologue chain, prologue returned null!");
				return false;
			}


			// Iterate prologue lists
			List<Act> prevArray = null;
			for (int i = prologueSeq.Count - 1; i >= 0; i--)
			{
				// Return if any null
				var pArray = prologueSeq[i];
				if (pArray == null || pArray.Contains(null))
				{
					ofAct.WriteLog("Cannot build prologue chain, lists returned by prologue contains null!");
					return false;
				}


				// Skip if empty list
				if (pArray.Count == 0)
				{
					continue;
				}


				// Link with self
				if (prevArray == null)
				{
					foreach (var pAct in pArray)
					{
						LinkPrologue(ofAct, pAct);
					}
				}
				else if (!LinkPrologueArrays(prevArray, pArray))  // Link with prev
				{
					ofAct.WriteLog("Cannot build prologue chain, A prologue act is already ongoing!");
					return false;
				}


				// Store previous array
				prevArray = pArray;
			}
		}


		// Iterate through prologue acts
		foreach (Act pAct in ofAct._prologueActs)
		{
			// Recurse
			if (!BuildPrologueChain(pAct, newTopEpilogue, oldTopEpilogue))
			{
				return false;
			}


			// Change top epilogues
			pAct._topEpilogueActs.Add(newTopEpilogue);
			pAct._topEpilogueActs.Remove(oldTopEpilogue);
		}

		return true;
	}
	private static void ClearPrologueChain(Act ofAct, Act oldTopEpilogue = null)
	{
		// Assign top epilogue if not given
		if (oldTopEpilogue == null)
		{
			oldTopEpilogue = ofAct;
		}


		// Iterate over prologue acts
		for (int i = 0; i < ofAct._prologueActs.Count; i++)
		{
			// Get prologue act
			var pAct = ofAct._prologueActs[i];


			// Skip if null
			if (pAct == null)
			{
				continue;
			}


			// Remove old top epilogue
			pAct._topEpilogueActs.Remove(oldTopEpilogue);


			// Clean up epilogues
			if (ofAct._topEpilogueActs.Count == 0)
			{
				pAct._epilogueActs.Remove(ofAct);
			}


			// Recurse
			ClearPrologueChain(pAct, oldTopEpilogue);
		}


		// Clean up prologues
		if (ofAct._topEpilogueActs.Count == 0)
		{
			ofAct._prologueActs.Clear();
		}
	}
	private static void ShiftEpilogues(Act ofAct)
	{
		// Add all to continue epilogue
		foreach (var kvp in ofAct._epilogueActs)
		{
			var eAct = kvp.Key;
			var isInSequence = kvp.Value;

			// Mark as already performed in sequence or if waiting on prologue completion
			if (isInSequence || eAct._status == Status.Prologuing)
			{
				ofAct._continueEpilogueActs.Add(eAct);
			}
		}


		// Filter out sequence acts
		foreach (var eAct in ofAct._continueEpilogueActs)
		{
			var isInSequence = ofAct._epilogueActs[eAct];
			if (isInSequence)
			{
				ofAct._epilogueActs.Remove(eAct);
			}
		}
	}
	private static void ContinueEpilogues(Act ofAct, Outcome eOutcome)
	{
		while (ofAct._continueEpilogueActs.Count != 0)
		{
			Act eAct = GetFirst(ofAct._continueEpilogueActs);
			ofAct._continueEpilogueActs.Remove(eAct);
			eAct.CompletedPrologue(ofAct, eOutcome);
		}
	}
	private void SetStatus(Status newStatus)
	{
		// Return if reassigning same status
		if (newStatus == _status)
		{
			return;
		}


		// Store previous status & Assign new
		_prevStatus = _status;
		_status = newStatus;


		// Let theater know act status has changed
		if (_theater != null)
		{
			Theater.Friend.ActStatusChange(_theater, this);
		}
	}
	private bool CanPerformImpl(bool isRetrying = false)
	{
		// Return if in between initialization
		if (_status == Status.Initializing || _status == Status.Deinitializing)
		{
			WriteLog("Cannot perform, act is in between initializing or deinitializing!");
			return false;
		}


		// Return if exiting or concluding
		if (!isRetrying && (_status == Status.Exiting || _status == Status.Concluding))
		{
			WriteLog("Cannot perform, act is between exiting or concluding!");
			return false;
		}


		// Return if disabled or theater is disabled
		if (!IsEnabled() || (_theater != null && !_theater.IsEnabled()))
		{
			WriteLog("Cannot perform, act or theater is disabled!");
			return false;
		}


		// Return if blocked
		if (IsBlocked())
		{
			WriteLog("Cannot perform, act is blocked!");
			return false;
		}


		// Return if already performing
		if (!isRetrying && !_canReperform && IsPerforming())
		{
			WriteLog("Cannot perform, act is performing!");
			return false;
		}


		// Return if any external condition is false
		foreach (Func<Act, bool> cond in performConditions)
		{
			if (!cond(this))
			{
				return false;
			}
		}


		return CanPerform();
	}
	private bool PerformImpl(bool isRetrying = false)
	{
		// Store retrying status
		var wasRetrying = _isRetrying;
		_isRetrying = isRetrying;


		// Broadcast pre perform requested
		OnPrePerformReq?.Invoke(this);


		// Check perform condition
		bool willPerform = CanPerformImpl(isRetrying);


		// Broadcast post perform requested
		OnPostPerformReq?.Invoke(this, willPerform);


		// Return if perform condition failed
		if (!willPerform)
		{
			_isRetrying = wasRetrying;
			return false;
		}


		// Finish if in between performing
		Finish(Outcome.Interrupted);


		// Increment perform count & lifetime perform count
		_performCount++;
		_lifetimePerformCount++;


		// Clear deferred
		if (_theater != null)
		{
			Theater.Friend.UnstageDeferred(_theater, this);
		}


		// Start prologuing
		Redirect(Status.Prologuing, isRetrying ? Outcome.Retry : Outcome.Pending);


		return true;
	}
	private void PrologueImpl()
	{
		// Guard
		if (_status != Status.Prologuing)
		{
			return;
		}


		// Create chain
		if (_topEpilogueActs.Count == 0 && !BuildPrologueChain(this))
		{
			Redirect(Status.Concluding, Outcome.Failure);
			return;
		}


		// Block Others
		foreach (Act act in _actsToBlock.Keys)
		{
			act.BlockSelf(this, _actsToBlock[act]);
			if (_status != Status.Prologuing) { return; }  // Guard
		}


		// Skip if no prologues
		if (_prologueActs.Count == 0)
		{
			Redirect(Status.Entering);  // Intentional to skip pre prologue signal
			return;
		}


		// Broadcast pre prologue
		OnPrePrologue?.Invoke(this);
		if (_status != Status.Prologuing) { return; }  // Guard


		// Start performing prologues
		PerformPrologues();
	}
	private void PerformPrologues()
	{
		// Guard
		if (_status != Status.Prologuing)
		{
			return;
		}


		// Iterate and perform prologues
		while (_prologueIndex < _prologueActs.Count)
		{
			// Get prologue act
			var pAct = _prologueActs[_prologueIndex];


			// Increment prologue index
			_prologueIndex++;


			// When act has already performed in sequence
			if (!pAct._epilogueActs.ContainsKey(this))
			{
				CompletedPrologue(pAct, Outcome.Success, true);
				continue;
			}


			// Skip if already performing
			if (pAct.IsPerforming())
			{
				continue;
			}


			// Perform prologue act
			if (!pAct.PerformImpl())
			{
				Redirect(Status.Concluding, Outcome.Failure);
				return;
			}


			// Guard
			if (_status != Status.Prologuing) { return; }
		}


		// Cascade down incase sequenced with already ongoing act
		for (int i = 0; i < _prologueActs.Count && _status == Status.Prologuing; i++)
		{
			_prologueActs[i].PerformPrologues();
		}
	}
	private void CompletedPrologue(Act pAct, Outcome pOutcome, bool hadAlreadyCompleted = false)
	{
		// Guard
		if (_status != Status.Prologuing)
		{
			return;
		}


		// Broadcast prologue completed
		if (!hadAlreadyCompleted)
		{
			OnPrologueComplete?.Invoke(this, pAct, pOutcome);
		}
		if (_status != Status.Prologuing) { return; }  // Guard


		// Concluding if prologue act did not succeed
		if (pOutcome != Outcome.Success)
		{
			Redirect(Status.Concluding, pOutcome);
			return;
		}


		// Increment prologues completed
		_prologueCompletedCount++;


		// Wait for all to complete
		if (_prologueIndex < _prologueActs.Count || _prologueCompletedCount < _prologueActs.Count)
		{
			return;
		}


		// Broadcast post prologue
		OnPostPrologue?.Invoke(this);
		if (_status != Status.Prologuing) { return; }  // Guard


		// Redirect to enter
		Redirect(Status.Entering);
	}
	private void EnterImpl()
	{
		// Guard
		if (_status != Status.Entering)
		{
			return;
		}


		// Broadcast pre enter
		OnPreEnter?.Invoke(this);
		if (_status != Status.Entering) { return; }  // Guard


		// Core enter
		var newOutcome = Enter();
		if (_status != Status.Entering) { return; }  // Guard 


		// Broadcast post enter
		OnPostEnter?.Invoke(this);
		if (_status != Status.Entering) { return; }  // Guard


		// Redirect to exit
		if (newOutcome != Outcome.Pending)
		{
			Redirect(Status.Exiting, newOutcome);
			return;
		}


		// Return if no ticking
		if (_tickFlags == TickFlags.None)
		{
			return;
		}


		// Return if no theater assigned for ticking
		if (_theater == null)
		{
			WriteLog("Cannot tick, Assign a theater first!");
			return;
		}


		// Redirect to ticking
		Redirect(Status.Ticking);
	}
	private void TickImpl()
	{
		// Guard
		if (_status != Status.Ticking)
		{
			return;
		}


		// Increment tick count
		_tickCount++;


		// Save life time count
		int currLifetimePerformCount = _lifetimePerformCount;


		// Broadcast pre tick
		OnPreTick?.Invoke(this);
		if (_status != Status.Ticking || currLifetimePerformCount != _lifetimePerformCount) { return; }   // Guard


		// Core tick
		var newOutcome = Tick();
		if (_status != Status.Ticking || currLifetimePerformCount != _lifetimePerformCount) { return; }  // Guard


		// Broadcast post tick
		OnPostTick?.Invoke(this);
		if (_status != Status.Ticking || currLifetimePerformCount != _lifetimePerformCount) { return; }  // Guard


		// Check if exit was requested
		if (newOutcome != Outcome.Pending)
		{
			Redirect(Status.Exiting, newOutcome);
		}
	}
	private void PhysicsTickImpl()
	{
		// Guard
		if (_status != Status.Ticking)
		{
			return;
		}


		// Increment physics tick count
		_physicsTickCount++;


		// Save life time count
		int currLifetimePerformCount = _lifetimePerformCount;


		// Broadcast pre physics tick
		OnPrePhysicsTick?.Invoke(this);
		if (_status != Status.Ticking || currLifetimePerformCount != _lifetimePerformCount) { return; }  // Guard


		// Core tick
		var newOutcome = PhysicsTick();
		if (_status != Status.Ticking || currLifetimePerformCount != _lifetimePerformCount) { return; }  // Guard


		// Broadcast post physics tick
		OnPostPhysicsTick?.Invoke(this);
		if (_status != Status.Ticking || currLifetimePerformCount != _lifetimePerformCount) { return; }  // Guard


		// Check if exit was requested
		if (newOutcome != Outcome.Pending)
		{
			Redirect(Status.Exiting, newOutcome);
		}
	}
	private void LateTickImpl()
	{
		// Guard
		if (_status != Status.Ticking)
		{
			return;
		}


		// Increment late tick count
		_lateTickCount++;


		// Save life time count
		int currLifetimePerformCount = _lifetimePerformCount;


		// Broadcast pre late tick
		OnPreLateTick?.Invoke(this);
		if (_status != Status.Ticking || currLifetimePerformCount != _lifetimePerformCount) { return; }  // Guard


		// Core tick
		var newOutcome = LateTick();
		if (_status != Status.Ticking || currLifetimePerformCount != _lifetimePerformCount) { return; }  // Guard


		// Broadcast post late tick
		OnPostLateTick?.Invoke(this);
		if (_status != Status.Ticking || currLifetimePerformCount != _lifetimePerformCount) { return; }  // Guard


		// Check if exit was requested
		if (newOutcome != Outcome.Pending)
		{
			Redirect(Status.Exiting, newOutcome);
		}
	}
	private void ExitImpl()
	{
		// Stop ticking
		if (CanTick(TickFlags.Tick) && _theater != null)
		{
			Theater.Friend.UnstageTick(_theater, this);
		}
		if (CanTick(TickFlags.PhysicsTick) && _theater != null)
		{
			Theater.Friend.UnstagePhysicsTick(_theater, this);
		}
		if (CanTick(TickFlags.LateTick) && _theater != null)
		{
			Theater.Friend.UnstageLateTick(_theater, this);
		}


		// Broadcast pre exit
		OnPreExit?.Invoke(this);


		// Core exit
		Exit();


		// Broadcast post exit
		OnPostExit?.Invoke(this);


		// Redirect to concluding
		Redirect(Status.Concluding);
	}
	private void ConcludingImpl()
	{
		// Finish all prologues
		Outcome pOutcome = _outcome == Outcome.Retry ? Outcome.Interrupted : _outcome;
		for (int i = 0; i < _prologueActs.Count; i++)
		{
			_prologueActs[i]?.Finish(pOutcome);
		}


		// Clear any stales in the prologue chain
		if (_topEpilogueActs.Count == 0)
		{
			ClearPrologueChain(this);
		}


		// Reset indices
		_prologueIndex = 0;
		_prologueCompletedCount = 0;


		// Retry
		if (_outcome == Outcome.Retry)
		{
			// Retry performing
			if (PerformImpl(true))
			{
				return;
			}


			// Change outcome to failure
			_outcome = Outcome.Failure;
		}


		// Shift epilogues to allow independent continuing
		ShiftEpilogues(this);


		// Unblock others
		foreach (Act act in _actsToBlock.Keys)
		{
			act.UnblockSelf(this);
		}


		// Redirect to None
		Redirect(Status.None);
	}
	private void Redirect(Status newStatus, Outcome newOutcome = Outcome.Pending)
	{
		// None/Retrying Conclude -> prologue
		if ((_status == Status.None || (_status == Status.Concluding && newOutcome == Outcome.Retry)) && newStatus == Status.Prologuing)
		{
			_outcome = Outcome.Pending;
			SetStatus(Status.Prologuing);
			OnPerformStart?.Invoke(this);
			PrologueImpl();
		}

		// prologue -> Enter
		else if (_status == Status.Prologuing && newStatus == Status.Entering)
		{
			SetStatus(Status.Entering);
			EnterImpl();
		}

		// Enter -> Tick
		else if (_status == Status.Entering && newStatus == Status.Ticking)
		{
			SetStatus(Status.Ticking);
			if (CanTick(TickFlags.Tick))
			{
				Theater.Friend.StageTick(_theater, this);
			}
			if (CanTick(TickFlags.PhysicsTick))
			{
				Theater.Friend.StagePhysicsTick(_theater, this);
			}
			if (CanTick(TickFlags.LateTick))
			{
				Theater.Friend.StageLateTick(_theater, this);
			}
		}

		// Enter/Tick -> Exit
		else if ((_status == Status.Entering || _status == Status.Ticking) && newStatus == Status.Exiting)
		{
			_outcome = newOutcome;
			SetStatus(Status.Exiting);
			ExitImpl();
		}

		// Prologue/Exit -> Conclude
		else if ((_status == Status.Prologuing || _status == Status.Exiting) && newStatus == Status.Concluding)
		{
			_outcome = _status == Status.Prologuing ? newOutcome : _outcome;
			SetStatus(Status.Concluding);
			ConcludingImpl();
		}

		// Conclude -> None
		else if (_status == Status.Concluding && newStatus == Status.None)
		{
			Outcome eOutcome = _outcome;
			SetStatus(Status.None);
			_isRetrying = false;


			// Broadcast perform end
			OnPerformEnd?.Invoke(this);


			// Continue epilogues
			ContinueEpilogues(this, eOutcome);
		}



		// None -> Initialize
		else if (_status == Status.None && newStatus == Status.Initializing && !_hasInitialized)
		{
			SetStatus(Status.Initializing);


			// Assign new name
			if (_initName != "")
			{
				_name = _initName;
			}


			// Assign new owning theater
			if (_initTheater != null)
			{
				_theater = _initTheater;
				Theater.Friend.AddAct(_theater, this);
			}


			// Broadcast pre setup
			OnPreSetup?.Invoke(this);


			// Core setup
			Setup();


			// Mark as initialized
			_hasInitialized = true;


			SetStatus(Status.None);


			// Broadcast post setup
			OnPostSetup?.Invoke(this);
		}

		// None -> Deinitialize
		else if (_status == Status.None && newStatus == Status.Deinitializing && _hasInitialized)
		{
			SetStatus(Status.Deinitializing);


			// Broadcast pre cleanup
			OnPreCleanup?.Invoke(this);


			// Core cleanup
			Cleanup();


			// Unassign owning theater
			if (_theater != null)
			{
				Theater.Friend.RemoveAct(_theater, this);
				_theater = null;
			}


			// Reset perform count & Mark as deinitialized
			_performCount = 0;
			_hasInitialized = false;


			SetStatus(Status.None);


			// Broadcast post cleanup
			OnPostCleanup?.Invoke(this);
		}
	}



	// Friend Class
	public class Friend
	{
		static public void TickImpl(Act act)
		{
			act.TickImpl();
		}
		static public void PhysicsTickImpl(Act act)
		{
			act.PhysicsTickImpl();
		}
		static public void LateTickImpl(Act act)
		{
			act.LateTickImpl();
		}


		// For Testing Only
		static public bool GetCanReperform(Act act)
		{
			return act._canReperform;
		}
		static public List<Act> GetPrologueActs(Act act)
		{
			return act._prologueActs;
		}
		static public Dictionary<Act, bool> GetEpilogueActs(Act act)
		{
			return act._epilogueActs;
		}
		static public HashSet<Act> GetContinueEpilogueActs(Act act)
		{
			return act._continueEpilogueActs;
		}
		static public HashSet<Act> GetTopEpilogueActs(Act act)
		{
			return act._topEpilogueActs;
		}
	}
}
