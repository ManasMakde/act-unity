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

	public Func<Act, List<Act>> prologue = (act) => new List<Act>();
	public List<Func<Act, bool>> performConditions = new List<Func<Act, bool>>();
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
	public static List<Act> Seq(List<List<Act>> pArrays)
	{
		// Return if null
		if (pArrays == null)
		{
			return new List<Act> { null };
		}


		// Return if any null
		foreach (List<Act> pArray in pArrays)
		{
			if (pArray == null || pArray.Contains(null))
			{
				return new List<Act> { null };
			}
		}


		// Remove empty lists before chaining
		pArrays.RemoveAll(pArr => pArr.Count == 0);


		// Return if empty list
		var pLength = pArrays.Count;
		if (pLength == 0)
		{
			return new List<Act>();
		}


		// Chain all prologues
		for (int i = pLength - 1; i > 0; i--)
		{
			LinkPrologueArrays(pArrays[i], pArrays[i - 1]);
		}


		return pArrays[pLength - 1];  // Return last acts
	}



	// Protected
	protected string _name = "";
	protected bool _canReperform = false;
	protected TickFlags _tickFlags = TickFlags.None;

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
			WriteLog("Failed to block, null act provided!");
			return;
		}


		// Return if already blocked
		if (_blockedByActs.Contains(byAct))
		{
			return;
		}


		// Return if both acts are in the same prologue chain
		if (this != byAct && (_epilogueActs.Count != 0 || byAct._epilogueActs.Count != 0))
		{
			_resultTopEpilogues.Clear();
			_visitedTopEpilogues.Clear();
			byAct._resultTopEpilogues.Clear();
			byAct._visitedTopEpilogues.Clear();
			if (GetTopEpilogues(this, _resultTopEpilogues, _visitedTopEpilogues).Overlaps(GetTopEpilogues(byAct, byAct._resultTopEpilogues, byAct._visitedTopEpilogues)))
			{
				WriteLog("Failed to block, Both " + _name + " and " + byAct._name + " are in the same prologue chain!");
				return;
			}
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
			WriteLog("Failed to unblock, null act provided!");
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
	private Theater _theater = null;
	private Status _status = Status.None;
	private Status _prevStatus = Status.None;
	private Outcome _outcome = Outcome.Pending;
	private Dictionary<Act, BlockType> _actsToBlock = new Dictionary<Act, BlockType>();
	private HashSet<Act> _blockedByActs = new();

	private HashSet<Act> _epilogueActs = new();
	private HashSet<Act> _pendingEpilogueActs = new();
	private HashSet<Act> _continueEpilogueActs = new();

	private HashSet<Act> _prologueActs = new();
	private HashSet<Act> _pendingPrologueActs = new();
	private HashSet<Act> _completedPrologueActs = new();

	private HashSet<Act> _resultTopEpilogues = new();
	private HashSet<Act> _visitedTopEpilogues = new();

	private string _initName;
	private Theater _initTheater;

	private bool _hasInitialized = false;
	private bool _isRetrying = false;
	private bool _hasPrecomputedPrologues = false;

	private int _lifetimePerformCount = 0;
	private int _performCount = 0;
	private int _tickCount = 0;
	private int _physicsTickCount = 0;
	private int _lateTickCount = 0;

	private static void LinkPrologueArrays(List<Act> arrayB, List<Act> arrayA)
	{
		for (int i = 0; i < arrayB.Count; i++)
		{
			Act actB = arrayB[i];
			for (int j = 0; j < arrayA.Count; j++)
			{
				Act actA = arrayA[j];
				actB._prologueActs.Add(actA);
				actA._epilogueActs.Add(actB);
				actA._pendingEpilogueActs.Add(actB);
			}
		}
	}
	private static HashSet<Act> GetTopEpilogues(Act ofAct, HashSet<Act> result, HashSet<Act> visited)
	{
		// Skip if already visited
		if (visited.Contains(ofAct))
		{
			return result;
		}


		// Mark as visited
		visited.Add(ofAct);


		// Add if top epilogue
		if (ofAct._epilogueActs.Count == 0)
		{
			result.Add(ofAct);
			return result;
		}


		// Recurse into each epilogue
		foreach (Act eAct in ofAct._epilogueActs)
		{
			GetTopEpilogues(eAct, result, visited);
		}

		return result;
	}
	private static void PrecomputePrologueChain(Act ofAct)
	{
		// Fail Incase directly null provided
		var prologueActs = ofAct.prologue.Invoke(ofAct);
		if (prologueActs == null)
		{
			ofAct.Finish(Outcome.Failure);
			return;
		}


		// Iterate through prologue acts		
		foreach (Act pAct in prologueActs)
		{
			// Skip self
			if (pAct == ofAct)
			{
				continue;
			}


			// Fail incase null
			if (pAct == null)
			{
				ofAct.Finish(Outcome.Failure);
				return;
			}


			// Assign prologue and epilogue
			ofAct._prologueActs.Add(pAct);
			pAct._epilogueActs.Add(ofAct);


			// Recurse into prologue
			PrecomputePrologueChain(pAct);
		}


		// Mark as precomputed
		ofAct._hasPrecomputedPrologues = true;
	}
	private static void FinishPrologues(Act ofAct, Outcome newOutcome)
	{
		// Set outcome to iterrupted incase retrying
		var pOutcome = newOutcome == Outcome.Retry ? Outcome.Interrupted : newOutcome;


		// Finish all pending prologues
		while (ofAct._pendingPrologueActs.Count != 0)
		{
			Act pAct = GetFirst(ofAct._pendingPrologueActs);
			ofAct._pendingPrologueActs.Remove(pAct);
			pAct?.Finish(pOutcome);
		}
	}
	private static void ContinueEpilogues(Act ofAct, HashSet<Act> pendingEpilogueActs, Outcome newOutcome)
	{
		// Continue and clear out epilogues
		while (pendingEpilogueActs.Count != 0)
		{
			Act eAct = GetFirst(pendingEpilogueActs);
			pendingEpilogueActs.Remove(eAct);
			eAct._completedPrologueActs.Add(ofAct);
			eAct.CompletedPrologue(ofAct, newOutcome);
		}
	}
	private static void ClearPrologueChain(Act ofAct)
	{
		while (ofAct._prologueActs.Count != 0 || ofAct._completedPrologueActs.Count != 0)
		{
			// Get prologue act
			Act pAct;
			if (ofAct._prologueActs.Count == 0)
			{
				pAct = GetFirst(ofAct._completedPrologueActs);
				ofAct._completedPrologueActs.Remove(pAct);
			}
			else
			{
				pAct = GetFirst(ofAct._prologueActs);
				ofAct._prologueActs.Remove(pAct);
			}


			// Skip if null
			if (pAct == null)
			{
				continue;
			}


			// Remove self from epilogue
			pAct._epilogueActs.Remove(ofAct);
			pAct._pendingEpilogueActs.Remove(ofAct);


			// Recurse incase Seq() linked stale acts
			if (pAct._epilogueActs.Count == 0)
			{
				pAct._hasPrecomputedPrologues = false;
				ClearPrologueChain(pAct);
			}
		}
	}
	private static Act GetFirst(HashSet<Act> data)
	{
		if (data.Count == 0)
		{
			return null;
		}

		foreach (Act act in data)
		{
			return act;
		}

		return null;
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


		// Broadcast perform start
		OnPerformStart?.Invoke(this);
		if (_status != Status.Prologuing) { return; }  // Guard


		// Precompute prologue chain
		if (!_hasPrecomputedPrologues)
		{
			PrecomputePrologueChain(this);
		}
		if (_status != Status.Prologuing) { return; }  // Guard



		// Assign self as pending epilogue
		foreach (Act pAct in _prologueActs)
		{
			pAct._pendingEpilogueActs.Add(this);
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


		// Perform all prologues
		while (_prologueActs.Count != 0)
		{
			// Guard
			if (_status != Status.Prologuing)
			{
				return;
			}


			// Skip prologue if already performing
			var pAct = GetFirst(_prologueActs);
			if (pAct.IsPerforming())
			{
				_prologueActs.Remove(pAct);
				_pendingPrologueActs.Add(pAct);
				continue;
			}


			// Skip if already completed
			if (_completedPrologueActs.Contains(pAct))
			{
				_prologueActs.Remove(pAct);
				CompletedPrologue(pAct, Outcome.Success);
				continue;
			}


			// Mark prologue as pending
			_prologueActs.Remove(pAct);
			_pendingPrologueActs.Add(pAct);


			// Perform prologue
			if (!pAct.PerformImpl())
			{
				// Revert pending
				_pendingPrologueActs.Remove(pAct);
				_prologueActs.Add(pAct);


				// Conclude if failed to perform prologue
				Redirect(Status.Concluding, Outcome.Failure);
				return;
			}
		}
	}
	private void CompletedPrologue(Act pAct, Outcome newOutcome)
	{
		// Guard
		if (_status != Status.Prologuing)
		{
			return;
		}


		// Remove from pending
		_pendingPrologueActs.Remove(pAct);


		// Broadcast prologue completed
		OnPrologueComplete?.Invoke(this, pAct, newOutcome);
		if (_status != Status.Prologuing)
		{
			return;
		}


		// Concluding if prologue act did not succeed
		if (newOutcome != Outcome.Success)
		{
			Redirect(Status.Concluding, newOutcome);
			return;
		}


		// Wait for all prologues to complete
		if (_pendingPrologueActs.Count != 0 || _prologueActs.Count != 0)
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
		// Cleanup prologues
		FinishPrologues(this, _outcome == Outcome.Retry ? Outcome.Interrupted : _outcome);  // Set outcome to iterrupted incase retrying
		ClearPrologueChain(this);
		_prologueActs.Clear();
		_pendingPrologueActs.Clear();
		_completedPrologueActs.Clear();
		_hasPrecomputedPrologues = false;


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


		// Cleanup epilogues
		_epilogueActs.Clear();


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
			SetStatus(Status.None);
			_isRetrying = false;


			// Prerequisites for epilogue
			Outcome epilogueOutcome = _outcome;
			HashSet<Act> pendingEpilogueActs = _pendingEpilogueActs;
			_pendingEpilogueActs = _continueEpilogueActs;
			_continueEpilogueActs = pendingEpilogueActs;


			// Broadcast perform end
			OnPerformEnd?.Invoke(this);


			// Continue epilogues
			ContinueEpilogues(this, pendingEpilogueActs, epilogueOutcome);
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
		static public HashSet<Act> GetEpilogueActs(Act act)
		{
			return act._epilogueActs;
		}
		static public HashSet<Act> GetPendingEpilogueActs(Act act)
		{
			return act._pendingEpilogueActs;
		}
		static public HashSet<Act> GetPrologueActs(Act act)
		{
			return act._prologueActs;
		}
		static public HashSet<Act> GetPendingPrologueActs(Act act)
		{
			return act._pendingPrologueActs;
		}
		static public HashSet<Act> GetCompletedPrologueActs(Act act)
		{
			return act._completedPrologueActs;
		}
		static public bool GetHasPrecomputedPrologues(Act act)
		{
			return act._hasPrecomputedPrologues;
		}
	}
}
