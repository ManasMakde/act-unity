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
public class ReperformableAct : Act
{
    protected override void Setup()
    {
        _canReperform = true;
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
