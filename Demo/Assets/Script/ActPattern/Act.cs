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

	[NonSerialized] public Func<Act, bool> condition = null;
	[NonSerialized] public Func<Act, List<List<Act>>> prologue = null;
	[NonSerialized] public Func<Act, Dictionary<Act, BlockType>> block = null;
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
	}
	public void PerformDeferred(TickFlags tickFlag = TickFlags.PhysicsTick)
	{
		// Warn if null theater
		if (_theater == null)
		{
			WriteLog("Cannot perform deferred, Assign a theater first!");
			return;
		}

		Theater.Friend.PerformDeferred(_theater, this, tickFlag);
	}
	public void RetryDeferred(TickFlags tickFlag = TickFlags.PhysicsTick)
	{
		// Warn if null theater
		if (_theater == null)
		{
			WriteLog("Cannot retry deferred, Assign a theater first!");
			return;
		}

		Theater.Friend.RetryDeferred(_theater, this, tickFlag);
	}
	public void AbortDeferred(TickFlags tickFlag = TickFlags.PhysicsTick)
	{
		// Warn if null theater
		if (_theater == null)
		{
			WriteLog("Cannot abort deferred, Assign a theater first!");
			return;
		}

		Theater.Friend.AbortDeferred(_theater, this, tickFlag);
	}
	public void CancelPerformDeferred()
	{
		// Warn if null theater provided
		if (_theater == null)
		{
			WriteLog("Cannot cancel perform deferred, Assign a theater first!");
			return;
		}

		Theater.Friend.CancelPerformDeferred(_theater, this);
	}
	public void CancelRetryDeferred()
	{
		// Warn if null theater provided
		if (_theater == null)
		{
			WriteLog("Cannot cancel retry deferred, Assign a theater first!");
			return;
		}

		Theater.Friend.CancelRetryDeferred(_theater, this);
	}
	public void CancelAbortDeferred()
	{
		// Warn if null theater provided
		if (_theater == null)
		{
			WriteLog("Cannot cancel abort deferred, Assign a theater first!");
			return;
		}

		Theater.Friend.CancelAbortDeferred(_theater, this);
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
	public bool IsPerforming()
	{
		return _status != Status.Initializing && _status != Status.Deinitializing && _status != Status.None;
	}
	public bool IsOngoing()
	{
		return _status != Status.Initializing && _status != Status.Deinitializing && _status != Status.None && _status != Status.Prologuing;
	}
	public bool IsRetrying()
	{
		return _isRetrying;
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
		if (byAct == this)
		{
			return false;
		}


		return _blockedByActs.Contains(byAct);
	}
	public bool HasInitialized()
	{
		return _hasInitialized;
	}
	public bool CanTick(TickFlags type = TickFlags.Tick | TickFlags.PhysicsTick | TickFlags.LateTick)
	{
		return (_tickFlags & type) != 0;
	}
	public string GetName()
	{
		return _name;
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
	public TickFlags GetTickFlags()
	{
		return _tickFlags;
	}
	public IReadOnlyCollection<Act> GetBlockedByActs()
	{
		return _blockedByActs;
	}
	public IReadOnlyDictionary<Act, BlockType> GetActsToBlock()
	{
		return _actsToBlock;
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
	public int GetLifetimePerformCount()
	{
		return _lifetimePerformCount;
	}
	public int GetLifetimeTickCount()
	{
		return _lifetimeTickCount;
	}
	public int GetLifetimePhysicsTickCount()
	{
		return _lifetimePhysicsTickCount;
	}
	public int GetLifetimeLateTickCount()
	{
		return _lifetimeLateTickCount;
	}
	public Theater GetTheater()
	{
		return _theater;
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
	protected virtual void BlockOthers()
	{
		// Return if no block assigned
		if (block == null)
		{
			return;
		}


		// Get acts to block
		_actsToBlock = block?.Invoke(this);
		if (_actsToBlock == null)
		{
			return;
		}


		// Iterate and block others
		int currLifetimePerformCount = _lifetimePerformCount;
		foreach (Act act in _actsToBlock.Keys)
		{
			if (act == this)
			{
				continue;
			}

			act.BlockSelf(this, _actsToBlock[act]);
			if (_status != Status.Prologuing || currLifetimePerformCount != _lifetimePerformCount) { return; }  // Guard
		}
	}
	protected virtual void UnblockOthers()
	{
		// Return if nothing to unblock
		if (_actsToBlock == null)
		{
			return;
		}


		// Iterate and unblock others
		foreach (Act act in _actsToBlock.Keys)
		{
			act.UnblockSelf(this);
		}
	}
	protected virtual void WriteLog(string message)
	{
		if (!isVerbose)
		{
			return;
		}

		Theater.Friend.WriteLog(_theater, $"[{_name}] {message}");
	}


	// Private
	private class ActGroup
	{
		public Act ownerAct = null;
		public List<Act> acts = null;
		public ActGroup next = null;
		public ActGroup prev = null;
		public int _itr = 0;
		public int _completed = 0;

		static public bool HaveAllPerformed(ActGroup group)
		{
			return group.acts.Count <= group._itr && group.acts.Count <= group._completed;
		}
	}

	[NonSerialized] private Theater _theater = null;
	[NonSerialized] private Status _status = Status.None;
	[NonSerialized] private Status _prevStatus = Status.None;
	[NonSerialized] private Outcome _outcome = Outcome.Pending;

	[NonSerialized] private Dictionary<Act, BlockType> _actsToBlock = null;
	[NonSerialized] private HashSet<Act> _blockedByActs = new();

	[NonSerialized] private Dictionary<ActGroup, bool /* didPerform */> _belongsToGroups = new();
	[NonSerialized] private HashSet<ActGroup> _prologueGroups = new();
	[NonSerialized] private HashSet<ActGroup> _epilogueGroups = new();

	[NonSerialized] private string _initName;
	[NonSerialized] private Theater _initTheater;

	[NonSerialized] private bool _hasInitialized = false;
	[NonSerialized] private bool _isRetrying = false;

	[NonSerialized] private int _performCount = 0;
	[NonSerialized] private int _tickCount = 0;
	[NonSerialized] private int _physicsTickCount = 0;
	[NonSerialized] private int _lateTickCount = 0;

	[NonSerialized] private int _lifetimePerformCount = 0;
	[NonSerialized] private int _lifetimeTickCount = 0;
	[NonSerialized] private int _lifetimePhysicsTickCount = 0;
	[NonSerialized] private int _lifetimeLateTickCount = 0;

	static private ActGroup GetFirstKey(Dictionary<ActGroup, bool> collection)
	{
		foreach (var kvp in collection)
		{
			return kvp.Key;
		}

		return null;
	}
	static private ActGroup GetFirst(HashSet<ActGroup> collection)
	{
		foreach (var data in collection)
		{
			return data;
		}

		return null;
	}
	static private bool BuildActGroupChain(Act ofAct)
	{
		// Return if no prologues
		if (ofAct.prologue == null)
		{
			return true;
		}


		// Return if null prologue
		var prologueSeq = ofAct.prologue.Invoke(ofAct);
		if (prologueSeq == null)
		{
			ofAct.WriteLog("Cannot build prologue group chain, prologue returned null!");
			return false;
		}


		// Return if empty prologues
		if (prologueSeq.Count == 0)
		{
			return true;
		}


		// Create self group
		var selfGroup = new ActGroup { ownerAct = ofAct };


		// Create group chain
		ActGroup prevGroup = selfGroup;
		for (int i = prologueSeq.Count - 1; i >= 0; i--)
		{
			// Return if any null
			var pArray = prologueSeq[i];
			if (pArray == null || pArray.Contains(null))
			{
				ofAct.WriteLog($"Cannot build prologue group chain, List of index {i} contains a null!");
				return false;
			}


			// Skip if empty list
			if (pArray.Count == 0)
			{
				continue;
			}


			// Create group & link with previous
			var group = new ActGroup { acts = pArray, prev = prevGroup };
			prevGroup.next = group;


			// Iterate prologues
			foreach (var pAct in pArray)
			{
				// Return if already past prologuing phase
				if (pAct.IsOngoing() && i != 0)
				{
					return false;
				}


				// Add to group
				pAct._belongsToGroups.Add(group, false);
				pAct._epilogueGroups.Add(prevGroup);
			}


			// Save as previous group
			prevGroup = group;
		}


		// Return if only empty lists in prologues
		if (prevGroup == selfGroup)
		{
			return true;
		}


		// Assign self group
		ofAct._belongsToGroups.Add(selfGroup, false);


		return true;
	}
	static private void ClearPrologueGroupChain(Act ofAct)
	{
		// Clear belonging groups
		while (ofAct._belongsToGroups.Count != 0)
		{
			// Get group
			var group = GetFirstKey(ofAct._belongsToGroups);
			ofAct._belongsToGroups.Remove(group);


			// Get prologue group
			var pGroup = group.next;
			if (pGroup == null)
			{
				continue;
			}


			// Recurse into prologue
			foreach (var pAct in pGroup.acts)
			{
				ClearPrologueGroupChain(pAct);
			}
		}


		// Clear prologue groups
		ofAct._prologueGroups.Clear();
	}
	static private void PerformActGroup(Act ofAct, ActGroup group)
	{
		// Return if invalid group
		if (group == null)
		{
			return;
		}


		// Save life time count
		int currLifetimePerformCount = ofAct._lifetimePerformCount;


		// Perform all prologues
		while (group._itr < group.acts.Count)
		{
			// Get prologue act
			var pAct = group.acts[group._itr];
			group._itr++;


			// Try entering if already completed
			var didFailPerform = false;
			if (pAct._belongsToGroups[group])
			{
				group._completed++;
				ofAct.ContinuePrologue(pAct, Outcome.Success, true);
			}

			// Skip if already performing
			else if (pAct.IsPerforming())
			{
				pAct.PerformPrologues();
			}

			// Perform prologue
			else
			{
				pAct._belongsToGroups[group] = true;
				didFailPerform = !pAct.PerformImpl();
			}


			// Fail main act if prologue failed
			if (didFailPerform)
			{
				ofAct.Redirect(Status.Concluding, Outcome.Failure);
				return;
			}


			// Guard
			if (ofAct._status != Status.Prologuing || currLifetimePerformCount != ofAct._lifetimePerformCount) { return; }  // Guard
		}
	}
	static private void FinishActGroup(ActGroup group, Outcome outcome)
	{
		for (int i = 0; group != null && i < group.acts.Count; i++)
		{
			group.acts[i]?.Finish(outcome);
		}
	}
	static private void ContinueActGroup(Act ofAct, ActGroup group, Outcome outcome)
	{
		// Incase no acts in group
		if (group.acts == null)
		{
			group.ownerAct.ContinuePrologue(ofAct, outcome);
			return;
		}


		// Continue all acts
		foreach (var act in group.acts)
		{
			act.ContinuePrologue(ofAct, outcome);
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
		if (condition != null && !condition(this))
		{
			return false;
		}


		return CanPerform();
	}
	private bool PerformImpl(bool isRetrying = false)
	{
		// Store retrying status
		var wasRetrying = _isRetrying;
		_isRetrying = isRetrying;


		// Broadcast pre & post perform requested
		OnPrePerformReq?.Invoke(this);
		bool willPerform = CanPerformImpl(isRetrying);
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


		// Broadcast performing started
		int currLifetimePerformCount = _lifetimePerformCount;
		OnPerformStart?.Invoke(this);
		if (_status != Status.Prologuing || currLifetimePerformCount != _lifetimePerformCount) { return; }  // Guard


		// Create chain
		if (!BuildActGroupChain(this))
		{
			Redirect(Status.Concluding, Outcome.Failure);
			return;
		}
		if (_status != Status.Prologuing || currLifetimePerformCount != _lifetimePerformCount) { return; }


		// Block all other acts
		BlockOthers();
		if (_status != Status.Prologuing || currLifetimePerformCount != _lifetimePerformCount) { return; }


		// Skip if no prologues
		if (_belongsToGroups.Count == 0)
		{
			Redirect(Status.Entering);  // Intentional to skip pre prologue signal
			return;
		}


		// Broadcast pre prologue
		OnPrePrologue?.Invoke(this);
		if (_status != Status.Prologuing || currLifetimePerformCount != _lifetimePerformCount) { return; }


		// Perform all prologues
		PerformPrologues();
	}
	private void PerformPrologues()
	{
		// Guard
		if (_status != Status.Prologuing)
		{
			return;
		}


		// Perform prologues
		int currLifetimePerformCount = _lifetimePerformCount;
		while (_belongsToGroups.Count != 0)
		{
			// Get self group
			var group = GetFirstKey(_belongsToGroups);
			_belongsToGroups.Remove(group);


			// Get prologue group
			var pGroup = group?.next;
			if (pGroup != null)
			{
				_prologueGroups.Add(pGroup);
			}


			// Perform all in prologue group
			PerformActGroup(this, pGroup);
			if (_status != Status.Prologuing || currLifetimePerformCount != _lifetimePerformCount) { return; }  // Guard


			// Try entering
			ContinuePrologue(null, Outcome.Success, true);
			if (_status != Status.Prologuing || currLifetimePerformCount != _lifetimePerformCount) { return; }  // Guard
		}
	}
	private void ContinuePrologue(Act pAct, Outcome pOutcome, bool toCheckOnly = false)
	{
		// Guard
		if (_status != Status.Prologuing)
		{
			return;
		}


		// Save life time count
		int currLifetimePerformCount = _lifetimePerformCount;


		// Skip if only to try entering
		if (!toCheckOnly)
		{
			// Broadcast prologue completed
			OnPrologueComplete?.Invoke(this, pAct, pOutcome);
			if (_status != Status.Prologuing || currLifetimePerformCount != _lifetimePerformCount) { return; }  // Guard


			// Conclude if prologue act did not succeed
			if (pOutcome != Outcome.Success)
			{
				Redirect(Status.Concluding, pOutcome);
				return;
			}
		}


		// Wait to exhaust all belonging groups
		if (_belongsToGroups.Count != 0)
		{
			return;
		}


		// Wait for all prologue groups to complete
		foreach (var pGroup in _prologueGroups)
		{
			if (!ActGroup.HaveAllPerformed(pGroup))
			{
				return;
			}
		}


		// Broadcast post prologue
		OnPostPrologue?.Invoke(this);
		if (_status != Status.Prologuing || currLifetimePerformCount != _lifetimePerformCount) { return; }  // Guard


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
		int currLifetimePerformCount = _lifetimePerformCount;
		OnPreEnter?.Invoke(this);
		if (_status != Status.Entering || currLifetimePerformCount != _lifetimePerformCount) { return; }  // Guard


		// Core enter
		var newOutcome = Enter();
		if (_status != Status.Entering || currLifetimePerformCount != _lifetimePerformCount) { return; }  // Guard 


		// Broadcast post enter
		OnPostEnter?.Invoke(this);
		if (_status != Status.Entering || currLifetimePerformCount != _lifetimePerformCount) { return; }  // Guard


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
		_lifetimeTickCount++;


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
		_lifetimePhysicsTickCount++;


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
		_lifetimeLateTickCount++;


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
		if (_theater != null)
		{
			Theater.Friend.UnstageTick(_theater, this);
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
		foreach (var pGroup in _prologueGroups)
		{
			FinishActGroup(pGroup, pOutcome);
		}


		// Clear prologue chain
		ClearPrologueGroupChain(this);


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


		// Unblock all other acts
		UnblockOthers();


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

		// Prologue -> Enter
		else if (_status == Status.Prologuing && newStatus == Status.Entering)
		{
			SetStatus(Status.Entering);
			EnterImpl();
		}

		// Enter -> Tick
		else if (_status == Status.Entering && newStatus == Status.Ticking)
		{
			SetStatus(Status.Ticking);
			Theater.Friend.StageTick(_theater, this);
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


			// Continue all epilogues
			while (_epilogueGroups.Count != 0)
			{
				var eGroup = GetFirst(_epilogueGroups);
				_epilogueGroups.Remove(eGroup);
				eGroup.next._completed++;
				ContinueActGroup(this, eGroup, eOutcome);
			}
		}



		// None -> Initialize
		else if (_status == Status.None && newStatus == Status.Initializing && !_hasInitialized)
		{
			SetStatus(Status.Initializing);


			// Assign new name
			if (!string.IsNullOrEmpty(_initName))
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


			// Reset status
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


			// Reset all
			_name = "";
			condition = null;
			prologue = null;
			block = null;
			_actsToBlock = null;
			_belongsToGroups.Clear();
			_prologueGroups.Clear();
			_epilogueGroups.Clear();
			_hasInitialized = false;
			_performCount = 0;
			_tickCount = 0;
			_physicsTickCount = 0;
			_lateTickCount = 0;


			// Reset status
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
	}
}
