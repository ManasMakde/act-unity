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
		Oneshot,
		Persistent
	}


	// Public
	public event Action<Act /* act */> OnPreSetup;
	public event Action<Act /* act */> OnPostSetup;
	public event Action<Act /* act */> OnPrePrologue;
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
	public event Action<Act /* act */> OnPreCleanup;
	public event Action<Act /* act */> OnPostCleanup;
	public event Action<Act /* act */, bool /* newIsEnabled */> OnEnableChanged;
	public event Action<Act /* act */, Act /* blockingAct */, BlockType /* blockType */, bool /* didBlock */> OnBlockChanged;

	public Func<Act, List<Act>> Prologue = (act) => new List<Act>();  // List all acts to perform before this act, return a list containing null for failure outcome
	public List<Func<Act, bool>> PerformConditions = new List<Func<Act, bool>>();  // Externally extendable conditions for CanPerformImpl

	public void Init(Theater theater, string name = "", bool initiallyEnabled = true)
	{
		// Warn if null theater provided
		if (theater == null)
		{
			Debug.LogError(name + " Null theater provided for initialization!");
			return;
		}


		// Assign new owning theater
		_theater = theater;
		_theater.AddAct(this);


		// Assign new name
		_name = name;


		// Disable Initially
		if (!initiallyEnabled)
		{
			BlockSelf(this, BlockType.Persistent);
		}


		// Broadcast pre setup
		OnPreSetup?.Invoke(this);


		// Core setup
		Setup();


		// Broadcast post setup
		OnPostSetup?.Invoke(this);
	}
	public void Deinit()
	{

		// Make sure act is not ongoing
		Abort();


		// Broadcast pre cleanup
		OnPreCleanup?.Invoke(this);


		// Core cleanup
		Cleanup();


		// Broadcast post cleanup
		OnPostCleanup?.Invoke(this);


		// Unassign owning theater
		_theater.RemoveAct(this);
		_theater = null;
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
		_theater?.StageDeferred(this, tickFlag);
	}
	public void Retry()
	{
		Finish(Outcome.Retry);
	}
	public void Abort()
	{
		Finish(Outcome.Interrupted);
	}
	public void AddToBlock(List<Act> acts, BlockType blockType = BlockType.Persistent)
	{
		foreach (Act bAct in acts)
		{
			// Skip if self reserved for enable disable
			if (bAct == this)
			{
				Debug.LogWarning(_name + " Trying to block self");
				continue;
			}


			// Add to block list
			_actsToBlock[bAct] = blockType;
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
	public bool DidPerform(TickFlags tickFlag = TickFlags.PhysicsTick)  // True if act was performed atleast once during current tick
	{
		// Return false if no flag provided
		if (tickFlag == TickFlags.None)
		{
			return false;
		}


		// Check based on tick types
		var performed = false;
		if ((tickFlag & TickFlags.Tick) != 0)
		{
			performed = performed || _performedOnTick == Time.frameCount;
		}
		if ((tickFlag & TickFlags.PhysicsTick) != 0)
		{
			performed = performed || _performedOnPhysicsTick == Mathf.RoundToInt(Time.fixedTime / Time.fixedDeltaTime);
		}
		if ((tickFlag & TickFlags.LateTick) != 0)
		{
			performed = performed || _performedOnLateTick == Time.frameCount;
		}

		return performed;
	}
	public bool DidPerformEver()  // True if act was performed atleast once since it was initialized
	{
		return _performedOnTick != -1 || _performedOnPhysicsTick != -1 || _performedOnLateTick != -1;
	}
	public bool IsOngoing()
	{
		return _status != Status.None;
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
	public bool DidEnter()
	{
		return _didEnter;
	}
	public bool CanTick(TickFlags type)
	{
		return (_tickFlags & type) != 0;
	}
	public Outcome GetOutcome()
	{
		return _outcome;
	}
	public Theater GetTheater()
	{
		return _theater;
	}
	public GameObject GetOwner()
	{
		return _theater?.gameObject;
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
	public static List<Act> Seq(List<List<Act>> pArrays)  // Only use inside Prologue
	{
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
	protected bool _canReperform = false;  // Indicates if act can interrupt itself and restart perform assign in Setup
	protected TickFlags _tickFlags = TickFlags.None;  // Indicates if act will be ticking after entering assign in Setup

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
		return Outcome.Pending;
	}
	protected virtual Outcome PhysicsTick()
	{
		return Outcome.Pending;
	}
	protected virtual Outcome LateTick()
	{
		return Outcome.Pending;
	}
	protected virtual void Exit()
	{
	}
	protected virtual void Cleanup()
	{
	}
	protected void Finish(Outcome newOutcome = Outcome.Success)  // Call in Enter if Exit needs to be delayed
	{
		// If currently prologuing
		if (_status == Status.Prologuing)
		{
			ContinuePrologue(null, newOutcome);
		}

		// If currently entering or ticking
		else if (_status == Status.Entering || _status == Status.Ticking)
		{
			Redirect(Status.Exiting, newOutcome);
		}
	}
	protected virtual void BlockSelf(Act byAct, BlockType blockType)
	{
		// Return if already blocked or top epilogues match up
		if (_blockedByActs.Contains(byAct) || HasMutualTopEpilogue(this, byAct))
		{
			return;
		}


		// Finish interrupted incase ongoing
		Finish(Outcome.Interrupted);


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
			act.UnblockSelf(this);
		}
	}



	// Private
	private Theater _theater = null;  // Which theater this act belongs to
	private Status _status = Status.None;  // Keeps track of where in the perform life cycle the act is currently
	private Outcome _outcome = Outcome.Pending;  // Denotes how the act ended
	private bool _didEnter = false;  // True if exit has been reached via enter
	private string _name = "";  // Useful for debugging
	private Dictionary<Act, BlockType> _actsToBlock = new Dictionary<Act, BlockType>();  // Which acts to block when performing this act
	private HashSet<Act> _blockedByActs = new();  // Which acts are blocking this act
	private HashSet<Act> _topEpilogueActs = new();
	private HashSet<Act> _epilogueActs = new();
	private HashSet<Act> _prologueActs = new();
	private int _prologueCompleteCount = 0;
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
				AssignPrologue(actB, actA);
			}
		}
	}
	private static bool HasMutualTopEpilogue(Act actA, Act actB)
	{
		// Incase both are the same acts
		if (actA == actB)
		{
			return false;
		}


		// Incase act a is a top epilogue
		if (actA._epilogueActs.Count == 0 && actB._topEpilogueActs.Contains(actA))
		{
			return true;
		}


		// Incase act b is a top epilogue
		if (actB._epilogueActs.Count == 0 && actA._topEpilogueActs.Contains(actB))
		{
			return true;
		}


		// Check for overlap in top epilogue of both
		foreach (Act eAct in actA._topEpilogueActs)
		{
			if (actB._topEpilogueActs.Contains(eAct))
			{
				return true;
			}
		}

		return false;
	}
	private static void FinishEpilogues(Act ofAct, Outcome newOutcome)
	{
		foreach (Act eAct in ofAct._epilogueActs)
		{
			eAct?.ContinuePrologue(ofAct, newOutcome);

			// Prevents false positive "mutation while iterating" error DO NOT REMOVE
			if (ofAct._epilogueActs.Count == 0)
			{
				break;
			}
		}
	}
	private static void FinishPrologues(Act ofAct, Outcome newOutcome)
	{
		foreach (Act pAct in ofAct._prologueActs)
		{
			pAct?.Finish(newOutcome);

			// Prevents false positive "mutation while iterating" error DO NOT REMOVE
			if (ofAct._prologueActs.Count == 0)
			{
				break;
			}
		}
	}
	private static void ClearPrologueChain(Act ofAct)
	{
		// Recurse clear
		foreach (Act pAct in ofAct._prologueActs)
		{
			if (pAct != null)
			{
				ClearPrologueChain(pAct);
			}
		}

		ofAct._epilogueActs.Clear();
		ofAct._topEpilogueActs.Clear();
		ofAct._prologueActs.Clear();
	}
	private static void AssignPrologue(Act eAct, Act pAct)
	{
		// Assign prologue
		eAct._prologueActs.Add(pAct);


		// Assign epilogue
		pAct._epilogueActs.Add(eAct);


		// Assign top epilogue
		if (eAct._epilogueActs.Count == 0)
		{
			pAct._topEpilogueActs.Add(eAct);
		}
		else
		{
			pAct._topEpilogueActs.UnionWith(new HashSet<Act>(eAct._topEpilogueActs));
		}
	}
	private bool CanPerformImpl()
	{

		// Return if null theater
		if (_theater == null)
		{
			Debug.LogWarning(_name + " Null theater found, initialize first!");
			return false;
		}


		// Return conditions
		if (!IsEnabled() || !_theater.IsEnabled() || IsBlocked() || (!_canReperform && IsOngoing()))
		{
			return false;
		}


		// Return if any external condition is false
		foreach (Func<Act, bool> cond in PerformConditions)
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
		// Store tick
		_performedOnTick = Time.frameCount;
		_performedOnPhysicsTick = Mathf.RoundToInt(Time.fixedTime / Time.fixedDeltaTime);
		_performedOnLateTick = Time.frameCount;


		// Finish any ongoing perform
		Finish(Outcome.Interrupted);


		// Redirect to prologue
		Redirect(Status.Prologuing);
	}
	private void PrologueImpl()
	{
		// Let theater know this act is now ongoing
		_theater.StageOngoing(this);
		if (_status != Status.Prologuing)
		{
			return;  // Guard
		}


		// Assign all prologues and epilogues
		foreach (Act pAct in Prologue.Invoke(this))
		{

			// Skip self
			if (pAct == this)
			{
				continue;
			}

			// Fail incase null
			if (pAct == null)
			{
				Redirect(Status.Exiting, Outcome.Failure);
				return;
			}

			// Assign prologue epilogue and top epilogue
			AssignPrologue(this, pAct);
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
		foreach (Act pAct in _prologueActs)
		{

			// Skip if ongoing
			if (pAct.IsOngoing())
			{
				continue;
			}

			// Fail incase cannot perform
			if (!pAct.CanPerformImpl())
			{
				Redirect(Status.Exiting, Outcome.Failure);
				return;
			}

			// Perform
			pAct.PerformImpl();
			if (_status != Status.Prologuing)
			{
				return;  // Guard
			}
		}
	}
	private void ContinuePrologue(Act pAct, Outcome newOutcome = Outcome.Pending)
	{
		// Guard
		if (_status != Status.Prologuing)
		{
			return;
		}


		// Wait for all prologues to complete
		var prologueSucceeded = newOutcome == Outcome.Success && pAct != null;
		if (prologueSucceeded && _prologueCompleteCount + 1 != _prologueActs.Count)
		{
			_prologueCompleteCount += 1;
			return;
		}


		// Broadcast post prologue
		if (prologueSucceeded)
		{
			OnPostPrologue?.Invoke(this);
		}
		if (_status != Status.Prologuing)
		{
			return;  // Guard
		}


		// If prologue succeeded goto enter otherwise exit
		Redirect(prologueSucceeded ? Status.Entering : Status.Exiting, newOutcome);
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


		// Start ticking
		if (CanTick(TickFlags.Tick))
		{
			_theater.StageTick(this);
		}
		if (_status != Status.Entering)
		{
			return;  // Guard
		}


		// Start physics ticking
		if (CanTick(TickFlags.PhysicsTick))
		{
			_theater.StagePhysicsTick(this);
		}
		if (_status != Status.Entering)
		{
			return;  // Guard
		}


		// Start late ticking
		if (CanTick(TickFlags.LateTick))
		{
			_theater.StageLateTick(this);
		}
		if (_status != Status.Entering)
		{
			return;  // Guard
		}


		// Redirect to ticking
		Redirect(Status.Ticking);
	}
	public void TickImpl()
	{
		// Guard
		if (_status != Status.Ticking)
		{
			return;
		}


		// Broadcast pre tick
		OnPreTick?.Invoke(this);
		if (_status != Status.Ticking)
		{
			return;  // Guard
		}


		// Core tick
		var newOutcome = Tick();
		if (_status != Status.Ticking)
		{
			return;  // Guard
		}


		// Broadcast post tick
		OnPostTick?.Invoke(this);
		if (_status != Status.Ticking)
		{
			return;  // Guard
		}


		// Check if exit was requested
		if (newOutcome != Outcome.Pending)
		{
			Redirect(Status.Exiting, newOutcome);
		}
	}
	public void PhysicsTickImpl()
	{
		// Guard
		if (_status != Status.Ticking)
		{
			return;
		}


		// Broadcast pre physics tick
		OnPrePhysicsTick?.Invoke(this);
		if (_status != Status.Ticking)
		{
			return;  // Guard
		}


		// Core tick
		var newOutcome = PhysicsTick();
		if (_status != Status.Ticking)
		{
			return;  // Guard
		}


		// Broadcast post physics tick
		OnPostPhysicsTick?.Invoke(this);
		if (_status != Status.Ticking)
		{
			return;  // Guard
		}


		// Check if exit was requested
		if (newOutcome != Outcome.Pending)
		{
			Redirect(Status.Exiting, newOutcome);
		}
	}
	public void LateTickImpl()
	{

		// Guard
		if (_status != Status.Ticking)
		{
			return;
		}


		// Broadcast pre late tick
		OnPreLateTick?.Invoke(this);
		if (_status != Status.Ticking)
		{
			return;  // Guard
		}


		// Core tick
		var newOutcome = LateTick();
		if (_status != Status.Ticking)
		{
			return;  // Guard
		}


		// Broadcast post late tick
		OnPostLateTick?.Invoke(this);
		if (_status != Status.Ticking)
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

		// Stop ticking
		if (CanTick(TickFlags.Tick))
		{
			_theater.UnstageTick(this);
		}
		if (_status != Status.Exiting)
		{
			return;  // Guard
		}


		// Stop physics ticking
		if (CanTick(TickFlags.PhysicsTick))
		{
			_theater.UnstagePhysicsTick(this);
		}
		if (_status != Status.Exiting)
		{
			return;  // Guard
		}


		// Stop late ticking
		if (CanTick(TickFlags.LateTick))
		{
			_theater.UnstageLateTick(this);
		}
		if (_status != Status.Exiting)
		{
			return;  // Guard
		}


		// Broadcast pre exit
		OnPreExit?.Invoke(this);
		if (_status != Status.Exiting)
		{
			return;  // Guard
		}


		// Core exit
		Exit();
		if (_status != Status.Exiting)
		{
			return;  // Guard
		}


		// Broadcast post exit
		OnPostExit?.Invoke(this);
		if (_status != Status.Exiting)
		{
			return;  // Guard
		}


		// Finish epilogues
		if (_outcome != Outcome.Retry)
		{
			FinishEpilogues(this, _outcome);
		}
		if (_status != Status.Exiting)
		{
			return;  // Guard
		}


		// Finish prologues
		FinishPrologues(this, _outcome == Outcome.Retry ? Outcome.Interrupted : _outcome);
		if (_status != Status.Exiting)
		{
			return;  // Guard
		}


		// Clear chain
		ClearPrologueChain(this);


		// Reset properties
		var toRetry = _outcome == Outcome.Retry;
		_status = Status.None;
		_outcome = Outcome.Pending;
		_didEnter = false;
		_prologueCompleteCount = 0;


		// Unblock
		UnblockOthers();


		// Retry performance
		if (toRetry)
		{
			Perform();
			return;
		}


		// Let theater know this act has ended
		_theater.UnstageOngoing(this);
	}
	private void Redirect(Status newStatus, Outcome newOutcome = Outcome.Pending)
	{

		// None -> Prologue
		if (_status == Status.None && newStatus == Status.Prologuing)
		{
			_status = Status.Prologuing;
			PrologueImpl();
		}

		// Prologue -> Enter
		else if (_status == Status.Prologuing && newStatus == Status.Entering)
		{
			_status = Status.Entering;
			EnterImpl();
		}

		// Enter -> Tick
		else if (_status == Status.Entering && newStatus == Status.Ticking)
		{
			_status = Status.Ticking;
		}

		// Prologue or Enter or Tick -> Exit
		else if ((_status == Status.Prologuing || _status == Status.Entering || _status == Status.Ticking) && newStatus == Status.Exiting)
		{
			_didEnter = _status == Status.Entering || _status == Status.Ticking;
			_status = Status.Exiting;
			_outcome = newOutcome;
			ExitImpl();
		}
	}
}
