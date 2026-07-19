using System;
using System.Linq;
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
	public event Action<Act /* act */> OnPerformStart;
	public event Action<Act /* act */> OnPerformEnd;
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

	public Func<Act, List<Act>> prologue = (act) => new List<Act>();  // List all acts to perform before this act, Return { null } for failure outcome
	public List<Func<Act, bool>> performConditions = new List<Func<Act, bool>>();  // Externally extendable conditions for CanPerform()
	public bool isVerbose = false;  // Toggle for warning messages

	public void Init(Theater theater, string name = "", bool initiallyEnabled = true)
	{
		// Warn if null theater provided
		if (theater == null)
		{
			WriteLog("Null theater provided for initialization!", name);
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
		if (_theater != null)
		{
			_theater.RemoveAct(this);
			_theater = null;
		}


		// Reset performed on ticks
		_performedOnTick = -1;
		_performedOnPhysicsTick = -1;
		_performedOnLateTick = -1;
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
			WriteLog("Cannot perform deferred, Theater is null! Have you initialized act?");
			return;
		}

		_theater.StageDeferred(this, tickFlag);
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
	public static List<Act> Seq(List<List<Act>> pArrays)  // Only use inside prologue
	{
		// Check for any null
		foreach (List<Act> pArray in pArrays)
		{
			if (pArray.Contains(null))
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
		if (this != byAct && GetTopEpilogues(this).Overlaps(GetTopEpilogues(byAct)))
		{
			WriteLog("Failed to block, Both " + _name + " and " + byAct._name + " are in the same prologue chain!");
			return;
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
			WriteLog("Failed to unblock, Act is not blocked by " + byAct._name);
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
			if (_actsToBlock[act] == BlockType.Persistent)  // Skip oneshot
			{
				act.UnblockSelf(this);
			}
		}
	}



	// Private
	private string _name = "";  // Useful for debugging
	private Theater _theater = null;  // Which theater this act belongs to
	private Status _status = Status.None;  // Keeps track of where in the perform life cycle the act is currently
	private Status _prevStatus = Status.None;
	private Outcome _outcome = Outcome.Pending;  // Denotes how the act ended
	private Dictionary<Act, BlockType> _actsToBlock = new Dictionary<Act, BlockType>();  // Which acts to block when performing this act
	private HashSet<Act> _blockedByActs = new();  // Which acts are blocking this act
	private HashSet<Act> _epilogueActs = new();
	private HashSet<Act> _prologueActs = new();
	private HashSet<Act> _pendingPrologueActs = new();
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
			}
		}
	}
	private static HashSet<Act> GetTopEpilogues(Act ofAct, HashSet<Act> result = null, HashSet<Act> visited = null)
	{
		result ??= new HashSet<Act>();
		visited ??= new HashSet<Act>();


		// Skip if already visited
		if (!visited.Add(ofAct))
		{
			return result;
		}


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
	private static void AssignPrologueChain(Act ofAct)  // Done
	{
		foreach (Act pAct in ofAct.prologue.Invoke(ofAct))
		{
			// Skip self
			if (pAct == ofAct)
			{
				continue;
			}


			// Assign prologue and epilogue
			ofAct._prologueActs.Add(pAct);
			pAct._epilogueActs.Add(ofAct);
		}
	}
	private static void FinishPrologues(Act ofAct, Outcome newOutcome)  // Done
	{
		// Set outcome to iterrupted incase retrying
		var pOutcome = newOutcome == Outcome.Retry ? Outcome.Interrupted : newOutcome;


		// Finish all pending prologues
		while (ofAct._pendingPrologueActs.Count != 0)
		{
			Act pAct = ofAct._pendingPrologueActs.First();
			ofAct._pendingPrologueActs.Remove(pAct);
			pAct?.Finish(pOutcome);
		}
	}
	private static void ContinueEpilogues(Act ofAct, Outcome newOutcome)  // Done
	{
		// Continue and clear out epilogues
		while (ofAct._epilogueActs.Count != 0)
		{
			Act eAct = ofAct._epilogueActs.First();
			ofAct._epilogueActs.Remove(eAct);
			eAct.CompletedPrologue(ofAct, newOutcome);
		}
	}
	private static void ClearPrologueChain(Act ofAct)
	{
		while (ofAct._prologueActs.Count != 0)
		{
			// Get prologue act
			Act pAct = ofAct._prologueActs.First();
			ofAct._prologueActs.Remove(pAct);


			// Skip if null
			if (pAct == null)
			{
				continue;
			}


			// Remove self from epilogue
			pAct._epilogueActs.Remove(ofAct);


			// Recurse down, Incase Seq() linked stale acts that were never performed
			if (pAct._epilogueActs.Count == 0)
			{
				ClearPrologueChain(pAct);
			}
		}
	}
	private bool CanPerformImpl()
	{
		// Return if null theater
		if (_theater == null)
		{
			WriteLog("Cannot perform, Theater is null! Have you initialized act?");
			return false;
		}


		// Return if disabled
		if (!IsEnabled() || !_theater.IsEnabled())
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
		if (!_canReperform && IsOngoing())
		{
			WriteLog("Cannot perform, act is ongoing!");
			return false;
		}


		// Return if any external condition is false
		foreach (Func<Act, bool> cond in performConditions)
		{
			if (!cond(this))
			{
				WriteLog("Cannot perform, failed an external perform condition!");
				return false;
			}
		}

		return CanPerform();
	}
	private void PerformImpl()
	{
		// Finish any ongoing perform
		Finish(Outcome.Interrupted);


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
			_theater.StageOngoing(this);
		}
		if (_status != Status.Prologuing)
		{
			return;  // Guard
		}


		// Store during which tick act was performed
		_performedOnTick = Time.frameCount;
		_performedOnPhysicsTick = Mathf.RoundToInt(Time.fixedTime / Time.fixedDeltaTime);
		_performedOnLateTick = Time.frameCount;


		// Assign prologues & epilogues
		AssignPrologueChain(this);
		if (_status != Status.Prologuing)
		{
			return;  // Guard
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


		// Reset pending prologues
		_pendingPrologueActs.Clear();


		// Perform all prologues
		while (_prologueActs.Count != 0)
		{
			// Pop a prologue & get prerequisites
			var pAct = _prologueActs.First();
			var isOngoing = pAct.IsOngoing();
			var canPerform = pAct.CanPerformImpl();


			// Fail incase cannot perform
			if (!isOngoing && !canPerform)
			{
				Redirect(Status.Exiting, Outcome.Failure);
				return;
			}


			// Shift to pending
			_prologueActs.Remove(pAct);
			_pendingPrologueActs.Add(pAct);


			// Perform if not already ongoing
			if (!isOngoing)
			{
				pAct.PerformImpl();
			}
			if (_status != Status.Prologuing)
			{
				return;  // Guard
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


		// Remove from pending and move to completed
		_pendingPrologueActs.Remove(pAct);


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


		// Start ticking
		if (CanTick(TickFlags.Tick) && _theater != null)
		{
			_theater.StageTick(this);
		}
		if (CanTick(TickFlags.PhysicsTick) && _theater != null)
		{
			_theater.StagePhysicsTick(this);
		}
		if (CanTick(TickFlags.LateTick) && _theater != null)
		{
			_theater.StageLateTick(this);
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
		// Only exit if coming from enter or tick
		if (_prevStatus == Status.Entering || _prevStatus == Status.Ticking)
		{
			// Stop ticking
			if (CanTick(TickFlags.Tick) && _theater != null)
			{
				_theater.UnstageTick(this);
			}
			if (CanTick(TickFlags.PhysicsTick) && _theater != null)
			{
				_theater.UnstagePhysicsTick(this);
			}
			if (CanTick(TickFlags.LateTick) && _theater != null)
			{
				_theater.UnstageLateTick(this);
			}


			// Broadcast pre exit
			OnPreExit?.Invoke(this);


			// Core exit
			Exit();


			// Broadcast post exit
			OnPostExit?.Invoke(this);
		}


		// Finish Prologues if any pending & Clear prologue chain 
		FinishPrologues(this, _outcome);
		ClearPrologueChain(this);


		// Do not continue epilogues or unblock if retrying 
		if (_outcome != Outcome.Retry)
		{
			ContinueEpilogues(this, _outcome);
			UnblockOthers();
		}


		// Retry
		if (_outcome == Outcome.Retry)
		{
			if (CanPerformImpl())
			{
				_status = Status.None;
				PerformImpl();
				return;
			}

			// Continue epilogues & unblock which were previously skipped
			ContinueEpilogues(this, _outcome);
			UnblockOthers();
		}


		// Reset status
		_status = Status.None;



		// Let theater know this act has ended
		if (_theater != null)
		{
			_theater.UnstageOngoing(this);
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
	private void WriteLog(string message, string overrideName = "")
	{
		if (!isVerbose)
		{
			return;
		}

		Debug.LogWarning((overrideName != "" ? overrideName : _name) + " " + message);
	}
}
