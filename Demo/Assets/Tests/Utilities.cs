using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class Utilities
{
    public static void AssertChainCleared(Act act, string prefix = "", string suffix = "")
    {
        var name = act.GetName();
        Assert.IsTrue(Act.Friend.GetPrologueActs(act).Count == 0, $"{prefix}{name} _prologueActs not cleared{suffix}! _prologueActs count={Act.Friend.GetPrologueActs(act).Count}");
        Assert.IsTrue(Act.Friend.GetEpilogueActs(act).Count == 0, $"{prefix}{name} _epilogueActs not cleared{suffix}! _epilogueActs count={Act.Friend.GetEpilogueActs(act).Count}");
        Assert.IsTrue(Act.Friend.GetContinueEpilogueActs(act).Count == 0, $"{prefix}{name} _continueEpilogueActs not cleared{suffix}! _continueEpilogueActs count={Act.Friend.GetContinueEpilogueActs(act).Count}");
        Assert.IsTrue(Act.Friend.GetTopEpilogueActs(act).Count == 0, $"{prefix}{name} _topEpilogueActs not cleared{suffix}! _topEpilogueActs count={Act.Friend.GetTopEpilogueActs(act).Count}");
    }
}
public class SetupAct : Act
{
    public int callCount = 0;
    protected override void Setup()
    {
        callCount++;
    }
}
public class CanPerformAct : Act
{
    public int callCount = 0;
    protected override bool CanPerform()
    {
        callCount++;
        return base.CanPerform();
    }
}
public class EnterAct : Act
{
    public int callCount = 0;
    protected override Outcome Enter()
    {
        callCount++;
        return base.Enter();
    }
}
public class TickAct : Act
{
    public int callCount = 0;
    protected override void Setup()
    {
        _tickFlags = TickFlags.Tick;
    }
    protected override Outcome Tick()
    {
        callCount++;
        return Outcome.Pending;
    }
}
public class PhysicsTickAct : Act
{
    public int callCount = 0;
    protected override void Setup()
    {
        _tickFlags = TickFlags.PhysicsTick;
    }
    protected override Outcome PhysicsTick()
    {
        callCount++;
        return Outcome.Pending;
    }
}
public class LateTickAct : Act
{
    public int callCount = 0;
    protected override void Setup()
    {
        _tickFlags = TickFlags.LateTick;
    }
    protected override Outcome LateTick()
    {
        callCount++;
        return Outcome.Pending;
    }
}
public class ExitAct : Act
{
    public int callCount = 0;
    protected override void Exit()
    {
        callCount++;
    }
}
public class CleanupAct : Act
{
    public int callCount = 0;
    protected override void Cleanup()
    {
        callCount++;
    }
}
public class WaitInfiniAct : Act
{
    protected override Outcome Enter()
    {
        return Outcome.Pending;
    }
}
public class ReperformableAct : Act
{
    protected override void Setup()
    {
        _canReperform = true;
    }
}
public class NonReperformableInfiAct : Act
{
    protected override void Setup()
    {
        _canReperform = false;
    }

    protected override Outcome Enter()
    {
        return Outcome.Pending;
    }
}
public class ReperformableInfiAct : Act
{
    public TickFlags overrideTickFlag = TickFlags.None;
    protected override void Setup()
    {
        _canReperform = true;
        _tickFlags = overrideTickFlag;
    }
    protected override Outcome Enter()
    {
        return Outcome.Pending;
    }
}
public class FalseCanPerformAct : Act
{
    protected override bool CanPerform()
    {
        return false;
    }
    protected override Outcome Enter()
    {
        return Outcome.Pending;
    }
}
public class FailingAct : Act
{
    protected override Outcome Enter()
    {
        return Outcome.Failure;
    }
}
public class SingleTickAct : Act
{
    protected override void Setup()
    {
        _tickFlags = TickFlags.Tick;
    }

    protected override Outcome Tick()
    {
        return GetTickCount() >= 1 ? Outcome.Success : Outcome.Pending;
    }
}
public class ManualFinishAct : Act
{
    public bool canReperformOverride = false;
    public TickFlags overrideTickFlags = TickFlags.None;
    public void ManualFinish(Outcome outcome = Outcome.Success)
    {
        Finish(outcome);
    }
    protected override void Setup()
    {
        _canReperform = canReperformOverride;
        _tickFlags = overrideTickFlags;
    }
    protected override Outcome Enter()
    {
        return Outcome.Pending;
    }
}
public class NoneTickAct : Act
{
    protected override void Setup()
    {
        _tickFlags = TickFlags.None;
    }
}
public class RetryAct : Act
{
    public int enterCallCount = 0;
    public int retryLimit = 1;
    protected override Outcome Enter()
    {
        enterCallCount++;
        return enterCallCount <= retryLimit ? Outcome.Retry : Outcome.Success;
    }
}
public class RetryOnceThenFailAct : Act
{
    public int enterCallCount = 0;
    protected override Outcome Enter()
    {
        enterCallCount++;
        return enterCallCount == 1 ? Outcome.Retry : Outcome.Success;
    }
    protected override bool CanPerform()
    {
        return enterCallCount == 0;  // block retry attempt after first enter
    }
}
public class RetryingCheckAct : Act
{
    public bool retryingInSetup = false;
    public bool retryingInEnter = false;
    public bool retryingInTick = false;
    public bool retryingInPhysicsTick = false;
    public bool retryingInLateTick = false;
    public bool retryingInExit = false;
    public bool retryingInCleanup = false;
 

    protected override void Setup()
    {
        _canReperform = true;
        _tickFlags = Act.TickFlags.Tick | Act.TickFlags.PhysicsTick | Act.TickFlags.LateTick;
        retryingInSetup = IsRetrying();
    }
    protected override Outcome Enter()
    {
        retryingInEnter = IsRetrying();
        return Outcome.Pending;
    }
    protected override Outcome Tick()
    {
        retryingInTick = IsRetrying();
        return Outcome.Pending;
    }
    protected override Outcome PhysicsTick()
    {
        retryingInPhysicsTick = IsRetrying();
        return Outcome.Pending;
    }
    protected override Outcome LateTick()
    {
        retryingInLateTick = IsRetrying();
        return Outcome.Pending;
    }
    protected override void Exit()
    {
        retryingInExit = IsRetrying();
    }
    protected override void Cleanup()
    {
        retryingInCleanup = IsRetrying();
    }
}
public class PerformingCheckAct : Act
{
    public bool performingInSetup = false;
    public bool performingInEnter = false;
    public bool performingInTick = false;
    public bool performingInPhysicsTick = false;
    public bool performingInLateTick = false;
    public bool performingInExit = false;
    public bool performingInCleanup = false;
    public void ForceFinish(Outcome outcome = Outcome.Success)
    {
        Finish(outcome);
    }


    protected override void Setup()
    {
        _tickFlags = Act.TickFlags.Tick | Act.TickFlags.PhysicsTick | Act.TickFlags.LateTick;
        performingInSetup = IsPerforming();
    }
    protected override Outcome Enter()
    {
        performingInEnter = IsPerforming();
        return Outcome.Pending;
    }
    protected override Outcome Tick()
    {
        performingInTick = IsPerforming();
        return Outcome.Pending;
    }
    protected override Outcome PhysicsTick()
    {
        performingInPhysicsTick = IsPerforming();
        return Outcome.Pending;
    }
    protected override Outcome LateTick()
    {
        performingInLateTick = IsPerforming();
        return Outcome.Pending;
    }
    protected override void Exit()
    {
        performingInExit = IsPerforming();
    }
    protected override void Cleanup()
    {
        performingInCleanup = IsPerforming();
    }
}
public class OngoingCheckAct : Act
{
    public bool ongoingInSetup = false;
    public bool ongoingInEnter = false;
    public bool ongoingInTick = false;
    public bool ongoingInPhysicsTick = false;
    public bool ongoingInLateTick = false;
    public bool ongoingInExit = false;
    public bool ongoingInCleanup = false;
    public void ForceFinish(Outcome outcome = Outcome.Success)
    {
        Finish(outcome);
    }


    protected override void Setup()
    {
        _tickFlags = Act.TickFlags.Tick | Act.TickFlags.PhysicsTick | Act.TickFlags.LateTick;
        ongoingInSetup = IsOngoing();
    }
    protected override Outcome Enter()
    {
        ongoingInEnter = IsOngoing();
        return Outcome.Pending;
    }
    protected override Outcome Tick()
    {
        ongoingInTick = IsOngoing();
        return Outcome.Pending;
    }
    protected override Outcome PhysicsTick()
    {
        ongoingInPhysicsTick = IsOngoing();
        return Outcome.Pending;
    }
    protected override Outcome LateTick()
    {
        ongoingInLateTick = IsOngoing();
        return Outcome.Pending;
    }
    protected override void Exit()
    {
        ongoingInExit = IsOngoing();
    }
    protected override void Cleanup()
    {
        ongoingInCleanup = IsOngoing();
    }
}
public class StatusCheckAct : Act
{
    public Status statusInSetup = Status.None;
    public Status statusInEnter = Status.None;
    public Status statusInTick = Status.None;
    public Status statusInPhysicsTick = Status.None;
    public Status statusInLateTick = Status.None;
    public Status statusInExit = Status.None;
    public Status statusInCleanup = Status.None;
    public void ForceFinish(Outcome outcome = Outcome.Success)
    {
        Finish(outcome);
    }


    protected override void Setup()
    {
        _tickFlags = Act.TickFlags.Tick | Act.TickFlags.PhysicsTick | Act.TickFlags.LateTick;
        statusInSetup = GetStatus();
    }
    protected override Outcome Enter()
    {
        statusInEnter = GetStatus();
        return Outcome.Pending;
    }
    protected override Outcome Tick()
    {
        statusInTick = GetStatus();
        return Outcome.Pending;
    }
    protected override Outcome PhysicsTick()
    {
        statusInPhysicsTick = GetStatus();
        return Outcome.Pending;
    }
    protected override Outcome LateTick()
    {
        statusInLateTick = GetStatus();
        return Outcome.Pending;
    }
    protected override void Exit()
    {
        statusInExit = GetStatus();
    }
    protected override void Cleanup()
    {
        statusInCleanup = GetStatus();
    }
}
