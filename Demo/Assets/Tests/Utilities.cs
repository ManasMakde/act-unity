using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;


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
public class ActiveCheckAct : Act
{
    public bool activeInSetup = false;
    public bool activeInEnter = false;
    public bool activeInTick = false;
    public bool activeInPhysicsTick = false;
    public bool activeInLateTick = false;
    public bool activeInExit = false;
    public bool activeInCleanup = false;
    public void ForceFinish(Outcome outcome = Outcome.Success)
    {
        Finish(outcome);
    }


    protected override void Setup()
    {
        _tickFlags = Act.TickFlags.Tick | Act.TickFlags.PhysicsTick | Act.TickFlags.LateTick;
        activeInSetup = IsActive();
    }
    protected override Outcome Enter()
    {
        activeInEnter = IsActive();
        return Outcome.Pending;
    }
    protected override Outcome Tick()
    {
        activeInTick = IsActive();
        return Outcome.Pending;
    }
    protected override Outcome PhysicsTick()
    {
        activeInPhysicsTick = IsActive();
        return Outcome.Pending;
    }
    protected override Outcome LateTick()
    {
        activeInLateTick = IsActive();
        return Outcome.Pending;
    }
    protected override void Exit()
    {
        activeInExit = IsActive();
    }
    protected override void Cleanup()
    {
        activeInCleanup = IsActive();
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
