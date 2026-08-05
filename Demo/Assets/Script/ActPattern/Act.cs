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
		LateTick = 1 << 2,
	}
	public enum Status
	{
		None = 0,
		Prologuing,
		Entering,
		Ticking,
		Exiting
	}
	public enum Outcome
	{
		Interrupted = -2,
		Failure = -1,
		Pending = 0,
		Success = 1,
		Retry = 2
	}
	public enum BlockType
	{
		Interrupt,
		Persistent
	}



	// Public
	public event Action<Act /* act */> OnPreSetup;
	public event Action<Act /* act */> OnPostSetup;
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

	public Func<Act, List<Act>> prologue = (act) => new List<Act>();  // List all acts to perform before this act, Return { null } for failure outcome
	public List<Func<Act, bool>> performConditions = new List<Func<Act, bool>>();  // Externally extendable conditions for CanPerform()
	public bool isVerbose = false;  // Toggle for warning messages

	public void Init(string newName = "", Theater newTheater = null, bool initiallyEnabled = true)
	{
		// Return if trying to reinitialize
		if (_hasInitialized)
		{
			WriteLog("Failed Init(), Already initialized!");
			return;
		}


		// Return if already initialized
		if (_isInitializing)
		{
			WriteLog("Failed Init(), Already initializing or deinitializing!");
			return;
		}


		// Mark as initialization started
		_isInitializing = true;


		// Assign new name
		if (newName != "")
		{
			_name = newName;
		}


		// Assign new owning theater
		if (newTheater != null)
		{
			_theater = newTheater;
			Theater.Friend.AddAct(_theater, this);
		}


		// Disable Initially
		if (!initiallyEnabled)
		{
			BlockSelf(this, BlockType.Persistent);
		}


		// Broadcast pre setup
		OnPreSetup?.Invoke(this);


		// Core setup
		Setup();


		// Mark as initialization completed
		_isInitializing = false;  // Intentionally before OnPostSetup DO NOT CHANGE
		_hasInitialized = true;

		// Broadcast post setup
		OnPostSetup?.Invoke(this);
	}
	public void Deinit()
	{
		// Return if trying to redeinitialize
		if (!_hasInitialized)
		{
			WriteLog("Failed Init(), Already deinitialized!");
			return;
		}


		// Return if not initialized
		if (_isInitializing)
		{
			WriteLog("Failed Deinit(), Already initializing or deinitializing!");
			return;
		}


		// Mark as deinitialization started
		_isInitializing = true;


		// Make sure act is not ongoing
		Abort();


		// Broadcast pre cleanup
		OnPreCleanup?.Invoke(this);


		// Core cleanup
		Cleanup();


		// Broadcast post cleanup
		OnPostCleanup?.Invoke(this);


		// Unassign owning theater
		if (_theater != null)
		{
			Theater.Friend.RemoveAct(_theater, this);
			_theater = null;
		}


		// Reset performed on ticks
		_performCount = 0;
		_performedOnTick = -1;
		_performedOnPhysicsTick = -1;
		_performedOnLateTick = -1;


		// Mark as deinitialization completed
		_isInitializing = false;
		_hasInitialized = false;
	}
	public void Perform()
	{
		if (CanPerformImpl())
		{
			PerformImpl();
		}
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
		if (IsOngoing())
		{
			Redirect(Status.Exiting, Outcome.Retry);
		}
		else
		{
			Perform();
		}
	}
	public void Abort()
	{
		Redirect(Status.Exiting, Outcome.Interrupted);


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
			// Skip if self (reserved for enable/disable)
			if (bAct == this)
			{
				WriteLog("Trying to block self!");
				continue;
			}


			// Add to block list
			_actsToBlock[bAct] = blockType;


			// Block if ongoing
			if (IsOngoing())
			{
				bAct.BlockSelf(this, blockType);
			}
		}
	}
	public void RemoveFromBlock(List<Act> acts)
	{
		foreach (Act bAct in acts)
		{
			// Skip if self (reserved for enable/disable)
			if (bAct == this)
			{
				WriteLog("Trying to unblock self!");
				continue;
			}


			// Unblock if ongoing
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
	public bool DidPerform(TickFlags tickFlag = TickFlags.PhysicsTick)
	{
		// Return false if no flag provided
		if (tickFlag == TickFlags.None)
		{
			return false;
		}


		// Check based on tick types
		var hasPerformed = false;
		if ((tickFlag & TickFlags.Tick) != 0)
		{
			hasPerformed = hasPerformed || _performedOnTick == Time.frameCount;
		}
		if ((tickFlag & TickFlags.PhysicsTick) != 0)
		{
			hasPerformed = hasPerformed || _performedOnPhysicsTick == Mathf.RoundToInt(Time.fixedTime / Time.fixedDeltaTime);
		}
		if ((tickFlag & TickFlags.LateTick) != 0)
		{
			hasPerformed = hasPerformed || _performedOnLateTick == Time.frameCount;
		}

		return hasPerformed;
	}
	public bool IsOngoing()
	{
		return _status != Status.None;
	}
	public bool IsActive()
	{
		return _status != Status.None && _status != Status.Prologuing;
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
	public bool CanTick(TickFlags type)
	{
		return (_tickFlags & type) != 0;
	}
	public Theater GetTheater()
	{
		return _theater;
	}
	public GameObject GetOwner()
	{
		return _theater?.gameObject;
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
	public Outcome GetOutcome()
	{
		return _outcome;
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
	public static List<Act> Seq(List<List<Act>> pArrays)  // Only use inside prologue
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
	protected bool _canReperform = false;  // Indicates if act can interrupt itself & restart perform, Only assign in Setup()
	protected TickFlags _tickFlags = TickFlags.None;  // Indicates if act will be "Ticking" after entering, Only assign in Setup()

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
		Redirect(Status.Exiting, newOutcome);
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


		// Finish interrupted incase ongoing
		Redirect(Status.Exiting, Outcome.Interrupted);


		// Add to blocked by list if persistent
		if (blockType == BlockType.Persistent)
		{
			_blockedByActs.Add(byAct);
		}


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
	protected virtual void BlockOthers()
	{
		foreach (Act act in _actsToBlock.Keys)
		{
			act.BlockSelf(this, _actsToBlock[act]);
		}
	}
	protected virtual void UnblockOthers()
	{
		foreach (Act act in _actsToBlock.Keys)
		{
			if (_actsToBlock[act] == BlockType.Persistent)
			{
				act.UnblockSelf(this);
			}
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
	private Theater _theater = null;  // Which theater this act belongs to
	private Status _status = Status.None;  // Keeps track of where in the perform life cycle the act is currently
	private Status _prevStatus = Status.None;
	private Outcome _outcome = Outcome.Pending;  // Denotes how the act ended
	private Dictionary<Act, BlockType> _actsToBlock = new Dictionary<Act, BlockType>();  // Which acts to block when performing this act
	private HashSet<Act> _blockedByActs = new();  // Which acts are blocking this act

	private HashSet<Act> _epilogueActs = new();
	private HashSet<Act> _pendingEpilogueActs = new();

	private HashSet<Act> _prologueActs = new();
	private HashSet<Act> _pendingPrologueActs = new();
	private HashSet<Act> _completedPrologueActs = new();

	private HashSet<Act> _resultTopEpilogues = new();
	private HashSet<Act> _visitedTopEpilogues = new();

	private bool _hasInitialized = false;
	private bool _isInitializing = false;
	private bool _hasPrecomputedPrologues = false;

	private int _performCount = 0;
	private int _tickCount = 0;
	private int _physicsTickCount = 0;
	private int _lateTickCount = 0;

	private int _tickReqCount = 0;
	private int _physicsTickReqCount = 0;
	private int _lateTickReqCount = 0;

	private int _performedOnTick = -1;
	private int _performedOnPhysicsTick = -1;
	private int _performedOnLateTick = -1;

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
			ofAct.Redirect(Status.Exiting, Outcome.Failure);
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
				ofAct.Redirect(Status.Exiting, Outcome.Failure);
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
	private static void ContinueEpilogues(Act ofAct, Outcome newOutcome)
	{
		// Continue and clear out epilogues
		while (ofAct._pendingEpilogueActs.Count != 0)
		{
			Act eAct = GetFirst(ofAct._pendingEpilogueActs);
			ofAct._pendingEpilogueActs.Remove(eAct);
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


			// Recurse down, Incase Seq() linked stale acts that were never performed
			if (pAct._epilogueActs.Count == 0)
			{
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
	private bool CanPerformImpl(bool isRetrying = false)
	{
		// Return if in between initialization
		if (_isInitializing)
		{
			WriteLog("Cannot perform, act is initializing or deinitializing!");
			return false;
		}


		// Return if exiting
		if (!isRetrying && _status == Status.Exiting)
		{
			WriteLog("Cannot perform, act is between exiting!");
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


		// Return if already ongoing
		if (!isRetrying && !_canReperform && IsOngoing())
		{
			WriteLog("Cannot perform, act is ongoing!");
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
	private void PerformImpl()
	{
		// Finish any ongoing perform
		if (_status != Status.None)
		{
			Finish(Outcome.Interrupted);
		}


		// Store during which tick act was performed
		_performCount++;
		_performedOnTick = Time.frameCount;
		_performedOnPhysicsTick = Mathf.RoundToInt(Time.fixedTime / Time.fixedDeltaTime);
		_performedOnLateTick = Time.frameCount;


		// Clear deferred
		if (_theater != null)
		{
			Theater.Friend.UnstageDeferred(_theater, this);
		}


		// Start prologuing
		Redirect(Status.Prologuing);
	}
	private void PrologueImpl()
	{
		// Broadcast perform start
		OnPerformStart?.Invoke(this);
		if (_status != Status.Prologuing)
		{
			return;  // Guard
		}


		// Let theater know this act has started
		if (_theater != null)
		{
			Theater.Friend.StageOngoing(_theater, this);
		}
		if (_status != Status.Prologuing)
		{
			return;  // Guard
		}


		// Precompute prologue chain
		if (!_hasPrecomputedPrologues)
		{
			PrecomputePrologueChain(this);
		}
		if (_status != Status.Prologuing)
		{
			return;  // Guard
		}


		// Assign self as pending epilogue
		foreach (Act pAct in _prologueActs)
		{
			pAct._pendingEpilogueActs.Add(this);
		}


		// Block
		BlockOthers();
		if (_status != Status.Prologuing)
		{
			return;  // Guard
		}


		// Skip if no prologues
		if (_prologueActs.Count == 0)
		{
			Redirect(Status.Entering);  // Intentional to skip pre prologue signal
			return;
		}


		// Broadcast pre prologue
		OnPrePrologue?.Invoke(this);
		if (_status != Status.Prologuing)
		{
			return;  // Guard
		}


		// Perform all prologues
		while (_prologueActs.Count != 0)
		{
			// Guard
			if (_status != Status.Prologuing)
			{
				return;
			}


			// Skip prologue if ongoing
			var pAct = GetFirst(_prologueActs);
			var isOngoing = pAct.IsOngoing();
			if (isOngoing)
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


			// Perform prologue
			if (pAct.CanPerformImpl())
			{
				_prologueActs.Remove(pAct);
				_pendingPrologueActs.Add(pAct);
				pAct.PerformImpl();
				continue;
			}


			// Exit with failure if failed to perform
			Redirect(Status.Exiting, Outcome.Failure);
			return;
		}
	}
	private void CompletedPrologue(Act pAct, Outcome newOutcome)
	{
		// Guard
		if (_status != Status.Prologuing)
		{
			return;
		}


		// Remove from pending and move to completed
		_pendingPrologueActs.Remove(pAct);


		// Broadcast prologue completed
		OnPrologueComplete?.Invoke(this, pAct, newOutcome);
		if (_status != Status.Prologuing)
		{
			return;
		}


		// Exit if prologue act did not succeed
		if (newOutcome != Outcome.Success)
		{
			Redirect(Status.Exiting, newOutcome);
			return;
		}


		// Wait for all prologues to complete
		if (_pendingPrologueActs.Count != 0 || _prologueActs.Count != 0)
		{
			return;
		}


		// Broadcast post prologue
		OnPostPrologue?.Invoke(this);
		if (_status != Status.Prologuing)
		{
			return;  // Guard
		}


		// Redirect to enter
		Redirect(Status.Entering);
	}
	private void EnterImpl()
	{
		// Broadcast pre enter
		OnPreEnter?.Invoke(this);
		if (_status != Status.Entering)
		{
			return;  // Guard
		}


		// Core enter
		var newOutcome = Enter();
		if (_status != Status.Entering)
		{
			return;  // Guard
		}


		// Broadcast post enter
		OnPostEnter?.Invoke(this);
		if (_status != Status.Entering)
		{
			return;  // Guard
		}


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
	private void HandleTickingImpl()
	{
		if (CanTick(TickFlags.Tick))
		{
			_tickReqCount++;
			Theater.Friend.StageTick(_theater, this);
		}
		if (CanTick(TickFlags.PhysicsTick))
		{
			_physicsTickReqCount++;
			Theater.Friend.StagePhysicsTick(_theater, this);
		}
		if (CanTick(TickFlags.LateTick))
		{
			_lateTickReqCount++;
			Theater.Friend.StageLateTick(_theater, this);
		}
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


		// Save tick request count
		int currTickReqCount = _tickReqCount;


		// Broadcast pre tick
		OnPreTick?.Invoke(this);
		if (_status != Status.Ticking || currTickReqCount != _tickReqCount)
		{
			return;  // Guard
		}


		// Core tick
		var newOutcome = Tick();
		if (_status != Status.Ticking || currTickReqCount != _tickReqCount)
		{
			return;  // Guard
		}


		// Broadcast post tick
		OnPostTick?.Invoke(this);
		if (_status != Status.Ticking || currTickReqCount != _tickReqCount)
		{
			return;  // Guard
		}


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


		// Save physics tick request count
		int currPhysicsTickReqCount = _physicsTickReqCount;


		// Broadcast pre physics tick
		OnPrePhysicsTick?.Invoke(this);
		if (_status != Status.Ticking || currPhysicsTickReqCount != _physicsTickReqCount)
		{
			return;  // Guard
		}


		// Core tick
		var newOutcome = PhysicsTick();
		if (_status != Status.Ticking || currPhysicsTickReqCount != _physicsTickReqCount)
		{
			return;  // Guard
		}


		// Broadcast post physics tick
		OnPostPhysicsTick?.Invoke(this);
		if (_status != Status.Ticking || currPhysicsTickReqCount != _physicsTickReqCount)
		{
			return;  // Guard
		}


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


		// Save late tick request count
		int currLateTickReqCount = _lateTickReqCount;


		// Broadcast pre late tick
		OnPreLateTick?.Invoke(this);
		if (_status != Status.Ticking || currLateTickReqCount != _lateTickReqCount)
		{
			return;  // Guard
		}


		// Core tick
		var newOutcome = LateTick();
		if (_status != Status.Ticking || currLateTickReqCount != _lateTickReqCount)
		{
			return;  // Guard
		}


		// Broadcast post late tick
		OnPostLateTick?.Invoke(this);
		if (_status != Status.Ticking || currLateTickReqCount != _lateTickReqCount)
		{
			return;  // Guard
		}


		// Check if exit was requested
		if (newOutcome != Outcome.Pending)
		{
			Redirect(Status.Exiting, newOutcome);
		}
	}
	private void ExitImpl()
	{
		// Only exit if coming from enter or tick
		if (_prevStatus == Status.Entering || _prevStatus == Status.Ticking)
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
		}


		// Cleanup prologues
		FinishPrologues(this, _outcome);
		ClearPrologueChain(this);
		_hasPrecomputedPrologues = false;
		_prologueActs.Clear();
		_pendingPrologueActs.Clear();
		_completedPrologueActs.Clear();


		// Retry
		if (_outcome == Outcome.Retry)
		{
			if (CanPerformImpl(true))
			{
				_status = Status.None;
				PerformImpl();
				return;
			}


			// Change outcome to failure since could not retry
			_outcome = Outcome.Failure;
		}


		// Unblock & Continue Epilogues
		UnblockOthers();
		ContinueEpilogues(this, _outcome);
		_epilogueActs.Clear();
		_pendingEpilogueActs.Clear();


		// Reset status
		_status = Status.None;


		// Let theater know this act has ended
		if (_theater != null)
		{
			Theater.Friend.UnstageOngoing(_theater, this);
		}


		// Broadcast perform end
		OnPerformEnd?.Invoke(this);
	}
	private void Redirect(Status newStatus, Outcome newOutcome = Outcome.Pending)
	{
		// None -> prologue
		if (_status == Status.None && newStatus == Status.Prologuing)
		{
			_prevStatus = _status;
			_status = Status.Prologuing;
			_outcome = Outcome.Pending;
			PrologueImpl();
		}

		// prologue -> Enter
		else if (_status == Status.Prologuing && newStatus == Status.Entering)
		{
			_prevStatus = _status;
			_status = Status.Entering;
			EnterImpl();
		}

		// Enter -> Tick
		else if (_status == Status.Entering && newStatus == Status.Ticking)
		{
			_prevStatus = _status;
			_status = Status.Ticking;
			HandleTickingImpl();
		}

		// prologue or Enter or Tick -> Exit
		else if ((_status == Status.Prologuing || _status == Status.Entering || _status == Status.Ticking) && newStatus == Status.Exiting)
		{
			_prevStatus = _status;
			_status = Status.Exiting;
			_outcome = newOutcome;
			ExitImpl();
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
		static public TickFlags GetTickFlags(Act act)
		{
			return act._tickFlags;
		}
		static public Status GetPrevStatus(Act act)
		{
			return act._prevStatus;
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
		static public bool GetIsInitializing(Act act)
		{
			return act._isInitializing;
		}
		static public bool GetHasPrecomputedPrologues(Act act)
		{
			return act._hasPrecomputedPrologues;
		}
		static public int GetTickReqCount(Act act)
		{
			return act._tickReqCount;
		}
		static public int GetPhysicsTickReqCount(Act act)
		{
			return act._physicsTickReqCount;
		}
		static public int GetLateTickReqCount(Act act)
		{
			return act._lateTickReqCount;
		}
		static public int GetPerformedOnTick(Act act)
		{
			return act._performedOnTick;
		}
		static public int GetPerformedOnPhysicsTick(Act act)
		{
			return act._performedOnPhysicsTick;
		}
		static public int GetPerformedOnLateTick(Act act)
		{
			return act._performedOnLateTick;
		}
	}
}
