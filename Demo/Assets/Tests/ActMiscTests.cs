using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;


// 1. Is IsRetrying() valid everywhere?
// 1. Is IsPerforming() valid everywhere?
// 1. Is IsOngoing() valid everywhere?
// 1. Is IsEnabled() valid in every combination?
// 1. Is IsBlocked() valid in every combination?

// 1. Is CanTick() valid in every combination?
// 1. Is CanTick() false when no tick flags?

// 1. Is GetTheater() return accurate theater?
// 1. Is GetTheater() return null when no theater is assigned?

// 1. Is GetOwner() return accurate owner?
// 1. Is GetOwner() return null when no theater is assigned?

// 1. Is GetBlockedByActs() return accurate acts?
// 1. Is GetActsToBlock() return accurate acts?

// 1. Is GetStatus() valid everywhere?
// 1. Is GetStatus() return None before & after perform?

// 1. Is GetOutcome() return accurate outcome after exiting? (Check for all outcomes)

// 1. Is GetPerformCount() give accurate perform counts?
// 1. Is GetTickCount() give accurate tick counts?
// 1. Is GetPhysicsTickCount() give accurate tick counts?
// 1. Is GetLateTickCount() give accurate tick counts?

// 1. GetName() return accurate value?


public class ActMiscTests
{
    [UnityTest]
    public IEnumerator IsRetryingValid()
    {
        // Prerequisites
        var theaterGO = new GameObject("Misc Retrying Theater");
        var theater = theaterGO.AddComponent<Theater>();
        var prologueAct = new Act();
        prologueAct.Init("Prologue Act");

        bool retryingInPreSetup = false;
        bool retryingInPostSetup = false;
        bool retryingInPerformStart = false;
        bool retryingInPrePrologue = false;
        bool retryingInPrologueComplete = false;
        bool retryingInPostPrologue = false;
        bool retryingInPreEnter = false;
        bool retryingInPostEnter = false;
        bool retryingInPreTick = false;
        bool retryingInPostTick = false;
        bool retryingInPrePhysicsTick = false;
        bool retryingInPostPhysicsTick = false;
        bool retryingInPreLateTick = false;
        bool retryingInPostLateTick = false;
        bool retryingInPreExit = false;
        bool retryingInPostExit = false;
        bool retryingInPerformEnd = false;
        bool retryingInPreCleanup = false;
        bool retryingInPostCleanup = false;
        bool retryingInEnableChanged = false;
        bool retryingInBlockChanged = false;


        // Perform Act
        var act = new RetryingCheckAct();
        act.OnPreSetup += (a) => { retryingInPreSetup = a.IsRetrying(); };
        act.OnPostSetup += (a) => { retryingInPostSetup = a.IsRetrying(); };
        act.OnPerformStart += (a) => { retryingInPerformStart = a.IsRetrying(); };
        act.OnPrePrologue += (a) => { retryingInPrePrologue = a.IsRetrying(); };
        act.OnPrologueComplete += (a, pAct, pOutcome) => { retryingInPrologueComplete = a.IsRetrying(); };
        act.OnPostPrologue += (a) => { retryingInPostPrologue = a.IsRetrying(); };
        act.OnPreEnter += (a) => { retryingInPreEnter = a.IsRetrying(); };
        act.OnPostEnter += (a) => { retryingInPostEnter = a.IsRetrying(); };
        act.OnPreTick += (a) => { retryingInPreTick = a.IsRetrying(); };
        act.OnPostTick += (a) => { retryingInPostTick = a.IsRetrying(); };
        act.OnPrePhysicsTick += (a) => { retryingInPrePhysicsTick = a.IsRetrying(); };
        act.OnPostPhysicsTick += (a) => { retryingInPostPhysicsTick = a.IsRetrying(); };
        act.OnPreLateTick += (a) => { retryingInPreLateTick = a.IsRetrying(); };
        act.OnPostLateTick += (a) => { retryingInPostLateTick = a.IsRetrying(); };
        act.OnPreExit += (a) => { retryingInPreExit = a.IsRetrying(); };
        act.OnPostExit += (a) => { retryingInPostExit = a.IsRetrying(); };
        act.OnPerformEnd += (a) => { retryingInPerformEnd = a.IsRetrying(); };
        act.OnPreCleanup += (a) => { retryingInPreCleanup = a.IsRetrying(); };
        act.OnPostCleanup += (a) => { retryingInPostCleanup = a.IsRetrying(); };
        act.OnEnableChanged += (a, newIsEnabled) => { retryingInEnableChanged = a.IsRetrying(); };
        act.OnBlockChanged += (a, blockingAct, blockType, didBlock) => { retryingInBlockChanged = a.IsRetrying(); };
        act.prologue += (a) => new() { prologueAct };
        act.Init("Misc Retrying Act", theater);

        act.Perform();

        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();
        yield return null;
        yield return null;

        act.Retry();

        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();
        yield return null;
        yield return null;

        act.Abort();

        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();
        yield return null;
        yield return null;

        var retryingAfterFinish = act.IsRetrying();

        act.Deinit();


        // Assertions
        Assert.IsTrue(!retryingInPreSetup, "IsRetrying() true in OnPreSetup!");
        Assert.IsTrue(!act.retryingInSetup, "IsRetrying() true in Setup()!");
        Assert.IsTrue(!retryingInPostSetup, "IsRetrying() true in OnPostSetup!");

        Assert.IsTrue(retryingInPerformStart, "IsRetrying() false in OnPerformStart!");
        Assert.IsTrue(retryingInPrePrologue, "IsRetrying() false in OnPrePrologue!");
        Assert.IsTrue(retryingInPrologueComplete, "IsRetrying() false in OnPrologueComplete!");
        Assert.IsTrue(retryingInPostPrologue, "IsRetrying() false in OnPostPrologue!");

        Assert.IsTrue(retryingInPreEnter, "IsRetrying() false in OnPreEnter!");
        Assert.IsTrue(act.retryingInEnter, "IsRetrying() false in Enter()!");
        Assert.IsTrue(retryingInPostEnter, "IsRetrying() false in OnPostEnter!");

        Assert.IsTrue(retryingInPreTick, "IsRetrying() false in OnPreTick!");
        Assert.IsTrue(act.retryingInTick, "IsRetrying() false in Tick()!");
        Assert.IsTrue(retryingInPostTick, "IsRetrying() false in OnPostTick!");

        Assert.IsTrue(retryingInPrePhysicsTick, "IsRetrying() false in OnPrePhysicsTick!");
        Assert.IsTrue(act.retryingInPhysicsTick, "IsRetrying() false in PhysicsTick()!");
        Assert.IsTrue(retryingInPostPhysicsTick, "IsRetrying() false in OnPostPhysicsTick!");

        Assert.IsTrue(retryingInPreLateTick, "IsRetrying() false in OnPreLateTick!");
        Assert.IsTrue(act.retryingInLateTick, "IsRetrying() false in LateTick()!");
        Assert.IsTrue(retryingInPostLateTick, "IsRetrying() false in OnPostLateTick!");

        Assert.IsTrue(retryingInPreExit, "IsRetrying() false in OnPreExit!");
        Assert.IsTrue(act.retryingInExit, "IsRetrying() false in Exit()!");
        Assert.IsTrue(retryingInPostExit, "IsRetrying() false in OnPostExit!");

        Assert.IsTrue(!retryingInPerformEnd, "IsRetrying() true in OnPerformEnd!");

        Assert.IsTrue(!retryingAfterFinish, "IsRetrying() true even after act has finished perform!");

        Assert.IsTrue(!retryingInPreCleanup, "IsRetrying() true in OnPreCleanup!");
        Assert.IsTrue(!act.retryingInCleanup, "IsRetrying() true in Cleanup()!");
        Assert.IsTrue(!retryingInPostCleanup, "IsRetrying() true in OnPostCleanup!");

        Assert.IsTrue(!retryingInEnableChanged, "IsRetrying() true in OnEnableChanged!");
        Assert.IsTrue(!retryingInBlockChanged, "IsRetrying() true in OnBlockChanged!");

        UnityEngine.Object.Destroy(theaterGO);

        yield return null;
    }
    [UnityTest]
    public IEnumerator IsPerformingValid()
    {
        // Prerequisites
        var theaterGO = new GameObject("Misc Performing Theater");
        var theater = theaterGO.AddComponent<Theater>();
        var prologueAct = new Act();
        prologueAct.Init("Prologue Act");

        bool performingInPreSetup = false;
        bool performingInPostSetup = false;
        bool performingInPerformStart = false;
        bool performingInPrePrologue = false;
        bool performingInPrologueComplete = false;
        bool performingInPostPrologue = false;
        bool performingInPreEnter = false;
        bool performingInPostEnter = false;
        bool performingInPreTick = false;
        bool performingInPostTick = false;
        bool performingInPrePhysicsTick = false;
        bool performingInPostPhysicsTick = false;
        bool performingInPreLateTick = false;
        bool performingInPostLateTick = false;
        bool performingInPreExit = false;
        bool performingInPostExit = false;
        bool performingInPerformEnd = false;
        bool performingInPreCleanup = false;
        bool performingInPostCleanup = false;
        bool performingInEnableChanged = false;
        bool performingInBlockChanged = false;


        // Perform Act
        var act = new PerformingCheckAct();
        act.OnPreSetup += (a) => { performingInPreSetup = a.IsPerforming(); };
        act.OnPostSetup += (a) => { performingInPostSetup = a.IsPerforming(); };
        act.OnPerformStart += (a) => { performingInPerformStart = a.IsPerforming(); };
        act.OnPrePrologue += (a) => { performingInPrePrologue = a.IsPerforming(); };
        act.OnPrologueComplete += (a, pAct, pOutcome) => { performingInPrologueComplete = a.IsPerforming(); };
        act.OnPostPrologue += (a) => { performingInPostPrologue = a.IsPerforming(); };
        act.OnPreEnter += (a) => { performingInPreEnter = a.IsPerforming(); };
        act.OnPostEnter += (a) => { performingInPostEnter = a.IsPerforming(); };
        act.OnPreTick += (a) => { performingInPreTick = a.IsPerforming(); };
        act.OnPostTick += (a) => { performingInPostTick = a.IsPerforming(); };
        act.OnPrePhysicsTick += (a) => { performingInPrePhysicsTick = a.IsPerforming(); };
        act.OnPostPhysicsTick += (a) => { performingInPostPhysicsTick = a.IsPerforming(); };
        act.OnPreLateTick += (a) => { performingInPreLateTick = a.IsPerforming(); };
        act.OnPostLateTick += (a) => { performingInPostLateTick = a.IsPerforming(); };
        act.OnPreExit += (a) => { performingInPreExit = a.IsPerforming(); };
        act.OnPostExit += (a) => { performingInPostExit = a.IsPerforming(); };
        act.OnPerformEnd += (a) => { performingInPerformEnd = a.IsPerforming(); };
        act.OnPreCleanup += (a) => { performingInPreCleanup = a.IsPerforming(); };
        act.OnPostCleanup += (a) => { performingInPostCleanup = a.IsPerforming(); };
        act.OnEnableChanged += (a, newIsEnabled) => { performingInEnableChanged = a.IsPerforming(); };
        act.OnBlockChanged += (a, blockingAct, blockType, didBlock) => { performingInBlockChanged = a.IsPerforming(); };
        act.prologue += (a) => new() { prologueAct };
        act.Init("Misc Performing Act", theater);

        act.Perform();

        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();
        yield return null;
        yield return null;

        act.ForceFinish();
        var performingAfterFinish = act.IsPerforming();

        act.Deinit();


        // Assertions
        Assert.IsTrue(!performingInPreSetup, "IsPerforming() true in OnPreSetup!");
        Assert.IsTrue(!act.performingInSetup, "IsPerforming() true in Setup()!");
        Assert.IsTrue(!performingInPostSetup, "IsPerforming() true in OnPostSetup!");

        Assert.IsTrue(performingInPerformStart, "IsPerforming() false in OnPerformStart!");
        Assert.IsTrue(performingInPrePrologue, "IsPerforming() false in OnPrePrologue!");
        Assert.IsTrue(performingInPrologueComplete, "IsPerforming() false in OnPrologueComplete!");
        Assert.IsTrue(performingInPostPrologue, "IsPerforming() false in OnPostPrologue!");

        Assert.IsTrue(performingInPreEnter, "IsPerforming() false in OnPreEnter!");
        Assert.IsTrue(act.performingInEnter, "IsPerforming() false in Enter()!");
        Assert.IsTrue(performingInPostEnter, "IsPerforming() false in OnPostEnter!");

        Assert.IsTrue(performingInPreTick, "IsPerforming() false in OnPreTick!");
        Assert.IsTrue(act.performingInTick, "IsPerforming() false in Tick()!");
        Assert.IsTrue(performingInPostTick, "IsPerforming() false in OnPostTick!");

        Assert.IsTrue(performingInPrePhysicsTick, "IsPerforming() false in OnPrePhysicsTick!");
        Assert.IsTrue(act.performingInPhysicsTick, "IsPerforming() false in PhysicsTick()!");
        Assert.IsTrue(performingInPostPhysicsTick, "IsPerforming() false in OnPostPhysicsTick!");

        Assert.IsTrue(performingInPreLateTick, "IsPerforming() false in OnPreLateTick!");
        Assert.IsTrue(act.performingInLateTick, "IsPerforming() false in LateTick()!");
        Assert.IsTrue(performingInPostLateTick, "IsPerforming() false in OnPostLateTick!");

        Assert.IsTrue(performingInPreExit, "IsPerforming() false in OnPreExit!");
        Assert.IsTrue(act.performingInExit, "IsPerforming() false in Exit()!");
        Assert.IsTrue(performingInPostExit, "IsPerforming() false in OnPostExit!");

        Assert.IsTrue(!performingInPerformEnd, "IsPerforming() true in OnPerformEnd!");

        Assert.IsTrue(!performingAfterFinish, "IsPerforming() true even after act has finished perform!");

        Assert.IsTrue(!performingInPreCleanup, "IsPerforming() true in OnPreCleanup!");
        Assert.IsTrue(!act.performingInCleanup, "IsPerforming() true in Cleanup()!");
        Assert.IsTrue(!performingInPostCleanup, "IsPerforming() true in OnPostCleanup!");

        Assert.IsTrue(!performingInEnableChanged, "IsPerforming() true in OnEnableChanged!");
        Assert.IsTrue(!performingInBlockChanged, "IsPerforming() true in OnBlockChanged!");

        UnityEngine.Object.Destroy(theaterGO);

        yield return null;
    }
    [UnityTest]
    public IEnumerator IsOngoingValid()
    {
        // Prerequisites
        var theaterGO = new GameObject("Misc Theater");
        var theater = theaterGO.AddComponent<Theater>();
        var prologueAct = new Act();
        prologueAct.Init("Prologue Act");

        bool ongoingInPreSetup = false;
        bool ongoingInPostSetup = false;
        bool ongoingInPerformStart = false;
        bool ongoingInPrePrologue = false;
        bool ongoingInPrologueComplete = false;
        bool ongoingInPostPrologue = false;
        bool ongoingInPreEnter = false;
        bool ongoingInPostEnter = false;
        bool ongoingInPreTick = false;
        bool ongoingInPostTick = false;
        bool ongoingInPrePhysicsTick = false;
        bool ongoingInPostPhysicsTick = false;
        bool ongoingInPreLateTick = false;
        bool ongoingInPostLateTick = false;
        bool ongoingInPreExit = false;
        bool ongoingInPostExit = false;
        bool ongoingInPerformEnd = false;
        bool ongoingInPreCleanup = false;
        bool ongoingInPostCleanup = false;
        bool ongoingInEnableChanged = false;
        bool ongoingInBlockChanged = false;


        // Perform Act
        var act = new OngoingCheckAct();
        act.OnPreSetup += (a) => { ongoingInPreSetup = a.IsOngoing(); };
        act.OnPostSetup += (a) => { ongoingInPostSetup = a.IsOngoing(); };
        act.OnPerformStart += (a) => { ongoingInPerformStart = a.IsOngoing(); };
        act.OnPrePrologue += (a) => { ongoingInPrePrologue = a.IsOngoing(); };
        act.OnPrologueComplete += (a, pAct, pOutcome) => { ongoingInPrologueComplete = a.IsOngoing(); };
        act.OnPostPrologue += (a) => { ongoingInPostPrologue = a.IsOngoing(); };
        act.OnPreEnter += (a) => { ongoingInPreEnter = a.IsOngoing(); };
        act.OnPostEnter += (a) => { ongoingInPostEnter = a.IsOngoing(); };
        act.OnPreTick += (a) => { ongoingInPreTick = a.IsOngoing(); };
        act.OnPostTick += (a) => { ongoingInPostTick = a.IsOngoing(); };
        act.OnPrePhysicsTick += (a) => { ongoingInPrePhysicsTick = a.IsOngoing(); };
        act.OnPostPhysicsTick += (a) => { ongoingInPostPhysicsTick = a.IsOngoing(); };
        act.OnPreLateTick += (a) => { ongoingInPreLateTick = a.IsOngoing(); };
        act.OnPostLateTick += (a) => { ongoingInPostLateTick = a.IsOngoing(); };
        act.OnPreExit += (a) => { ongoingInPreExit = a.IsOngoing(); };
        act.OnPostExit += (a) => { ongoingInPostExit = a.IsOngoing(); };
        act.OnPerformEnd += (a) => { ongoingInPerformEnd = a.IsOngoing(); };
        act.OnPreCleanup += (a) => { ongoingInPreCleanup = a.IsOngoing(); };
        act.OnPostCleanup += (a) => { ongoingInPostCleanup = a.IsOngoing(); };
        act.OnEnableChanged += (a, newIsEnabled) => { ongoingInEnableChanged = a.IsOngoing(); };
        act.OnBlockChanged += (a, blockingAct, blockType, didBlock) => { ongoingInBlockChanged = a.IsOngoing(); };
        act.prologue += (a) => new() { prologueAct };
        act.Init("Misc Ongoing Act", theater);

        act.Perform();

        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();
        yield return null;
        yield return null;

        act.ForceFinish();
        var ongoingAfterFinish = act.IsOngoing();

        act.Deinit();


        // Assertions
        Assert.IsTrue(!ongoingInPreSetup, "IsOngoing() true in OnPreSetup!");
        Assert.IsTrue(!act.ongoingInSetup, "IsOngoing() true in Setup()!");
        Assert.IsTrue(!ongoingInPostSetup, "IsOngoing() true in OnPostSetup!");

        Assert.IsTrue(!ongoingInPerformStart, "IsOngoing() true in OnPerformStart!");
        Assert.IsTrue(!ongoingInPrePrologue, "IsOngoing() true in OnPrePrologue!");
        Assert.IsTrue(!ongoingInPrologueComplete, "IsOngoing() true in OnPrologueComplete!");
        Assert.IsTrue(!ongoingInPostPrologue, "IsOngoing() true in OnPostPrologue!");

        Assert.IsTrue(ongoingInPreEnter, "IsOngoing() false in OnPreEnter!");
        Assert.IsTrue(act.ongoingInEnter, "IsOngoing() false in Enter()!");
        Assert.IsTrue(ongoingInPostEnter, "IsOngoing() false in OnPostEnter!");

        Assert.IsTrue(ongoingInPreTick, "IsOngoing() false in OnPreTick!");
        Assert.IsTrue(act.ongoingInTick, "IsOngoing() false in Tick()!");
        Assert.IsTrue(ongoingInPostTick, "IsOngoing() false in OnPostTick!");

        Assert.IsTrue(ongoingInPrePhysicsTick, "IsOngoing() false in OnPrePhysicsTick!");
        Assert.IsTrue(act.ongoingInPhysicsTick, "IsOngoing() false in PhysicsTick()!");
        Assert.IsTrue(ongoingInPostPhysicsTick, "IsOngoing() false in OnPostPhysicsTick!");

        Assert.IsTrue(ongoingInPreLateTick, "IsOngoing() false in OnPreLateTick!");
        Assert.IsTrue(act.ongoingInLateTick, "IsOngoing() false in LateTick()!");
        Assert.IsTrue(ongoingInPostLateTick, "IsOngoing() false in OnPostLateTick!");

        Assert.IsTrue(ongoingInPreExit, "IsOngoing() false in OnPreExit!");
        Assert.IsTrue(act.ongoingInExit, "IsOngoing() false in Exit()!");
        Assert.IsTrue(ongoingInPostExit, "IsOngoing() false in OnPostExit!");

        Assert.IsTrue(!ongoingInPerformEnd, "IsOngoing() true in OnPerformEnd!");

        Assert.IsTrue(!ongoingAfterFinish, "IsOngoing() true even after act has finished perform!");

        Assert.IsTrue(!ongoingInPreCleanup, "IsOngoing() true in OnPreCleanup!");
        Assert.IsTrue(!act.ongoingInCleanup, "IsOngoing() true in Cleanup()!");
        Assert.IsTrue(!ongoingInPostCleanup, "IsOngoing() true in OnPostCleanup!");

        Assert.IsTrue(!ongoingInEnableChanged, "IsOngoing() true in OnEnableChanged!");
        Assert.IsTrue(!ongoingInBlockChanged, "IsOngoing() true in OnBlockChanged!");

        UnityEngine.Object.Destroy(theaterGO);

        yield return null;
    }
    [UnityTest]
    public IEnumerator IsEnabledValid()
    {

        // Enabled by default
        {
            var act = new Act();
            act.Init("Enabled Act");
            act.SetEnabled(true);


            // Assertions
            Assert.IsTrue(act.IsEnabled(), "IsEnabled() false despite never being disabled!");

            yield return null;
        }


        // Disabled
        {
            var act = new Act();
            act.Init("Misc Disabled Act");
            act.SetEnabled(false);


            // Assertions
            Assert.IsFalse(act.IsEnabled(), "IsEnabled() true despite being disabled!");

            yield return null;
        }


        // Disabled then enabled
        {
            var act = new Act();
            act.Init("Misc Reenabled Act");
            act.SetEnabled(false);
            act.SetEnabled(true);


            // Assertions
            Assert.IsTrue(act.IsEnabled(), "IsEnabled() false despite being reenabled!");

            yield return null;
        }
    }
    [UnityTest]
    public IEnumerator IsBlockedValid()
    {
        var blockedAct = new Act();
        blockedAct.Init("Misc Target Act");

        var blocker = new ManualFinishAct();
        blocker.AddToBlock(new List<Act> { blockedAct }, Act.BlockType.Persistent);
        blocker.Init("Misc Blocker Act");

        blocker.Perform();

        var isBlockedWhileBlocking = blockedAct.IsBlocked();
        blocker.ManualFinish();
        var isBlockedAfterUnblock = blockedAct.IsBlocked();


        // Assertions
        Assert.IsTrue(isBlockedWhileBlocking, "IsBlocked() false while persistently blocked!");
        Assert.IsTrue(!isBlockedAfterUnblock, "IsBlocked() true after persistently unblocked!");

        yield return null;
    }



    [UnityTest]
    public IEnumerator CanTickFlagCombos()
    {
        // Prerequisites
        var allCombos = new Act.TickFlags[]
        {
            Act.TickFlags.Tick,
            Act.TickFlags.PhysicsTick,
            Act.TickFlags.LateTick,
            Act.TickFlags.Tick | Act.TickFlags.PhysicsTick,
            Act.TickFlags.Tick | Act.TickFlags.LateTick,
            Act.TickFlags.PhysicsTick | Act.TickFlags.LateTick,
            Act.TickFlags.Tick | Act.TickFlags.PhysicsTick | Act.TickFlags.LateTick,
        };

        // Perform Act
        foreach (var combo in allCombos)
        {
            var act = new ReperformableInfiAct();
            act.overrideTickFlag = combo;
            act.Init("Misc Tick Flag Act");

            var expectTick = (combo & Act.TickFlags.Tick) != 0;
            var expectPhysicsTick = (combo & Act.TickFlags.PhysicsTick) != 0;
            var expectLateTick = (combo & Act.TickFlags.LateTick) != 0;


            // Assertions
            Assert.IsTrue(act.CanTick(Act.TickFlags.Tick) == expectTick, $"CanTick(Tick) wrong for combo='{combo}'");
            Assert.IsTrue(act.CanTick(Act.TickFlags.PhysicsTick) == expectPhysicsTick, $"CanTick(PhysicsTick) wrong for combo='{combo}'");
            Assert.IsTrue(act.CanTick(Act.TickFlags.LateTick) == expectLateTick, $"CanTick(LateTick) wrong for combo='{combo}'");
        }

        yield return null;
    }
    [UnityTest]
    public IEnumerator CanTickNoFlags()
    {
        // Perform Act
        var act = new ReperformableInfiAct();
        act.overrideTickFlag = Act.TickFlags.None;
        act.Init("Misc No Tick Flag Act");


        // Assertions
        Assert.IsFalse(act.CanTick(Act.TickFlags.Tick), "CanTick(Tick) true despite no flags assigned!");
        Assert.IsFalse(act.CanTick(Act.TickFlags.PhysicsTick), "CanTick(PhysicsTick) true despite no flags assigned!");
        Assert.IsFalse(act.CanTick(Act.TickFlags.LateTick), "CanTick(LateTick) true despite no flags assigned!");

        yield return null;
    }



    [UnityTest]
    public IEnumerator GetTheaterAccurate()
    {
        // Prerequisites
        var theaterGO = new GameObject("Misc Theater Get Theater");
        var theater = theaterGO.AddComponent<Theater>();
        var act = new Act();
        act.Init("Misc Theater Act", theater);


        // Assertions
        Assert.IsTrue(act.GetTheater() == theater, "GetTheater() did not return assigned theater!");

        UnityEngine.Object.Destroy(theaterGO);

        yield return null;
    }
    [UnityTest]
    public IEnumerator GetTheaterNullWhenNotAssigned()
    {
        // Perform Act
        var act = new Act();
        act.Init("Misc No Theater Act");


        // Assertions
        Assert.IsTrue(act.GetTheater() == null, "GetTheater() not null despite no theater assigned!");

        yield return null;
    }



    [UnityTest]
    public IEnumerator GetOwnerAccurate()
    {
        // Prerequisites
        var theaterGO = new GameObject("Misc Owner Get Theater");
        var theater = theaterGO.AddComponent<Theater>();

        // Perform Act
        var act = new Act();
        act.Init("Misc Owner Act", theater);


        // Assertions
        Assert.IsTrue(act.GetOwner() == theaterGO, "GetOwner() did not return theater owner gameobject!");

        UnityEngine.Object.Destroy(theaterGO);

        yield return null;
    }
    [UnityTest]
    public IEnumerator GetOwnerNullWhenNotAssigned()
    {
        // Perform Act
        var act = new Act();
        act.Init("Misc No Owner Act");


        // Assertions
        Assert.IsTrue(act.GetOwner() == null, "GetOwner() not null despite no theater assigned!");

        yield return null;
    }



    [UnityTest]
    public IEnumerator GetBlockedByActsValid()
    {
        // Prerequisites
        var blockedAct = new Act();
        var blockerAct1 = new ManualFinishAct();
        var blockerAct2 = new ManualFinishAct();

        blockedAct.Init("Blocked Act");
        blockerAct1.Init("Blocker Act 1");
        blockerAct2.Init("Blocker Act 2");
        blockerAct1.AddToBlock(new() { blockedAct });
        blockerAct2.AddToBlock(new() { blockedAct });

        // Perform Act
        blockerAct1.Perform();
        blockerAct2.Perform();

        var blockedByActs = blockedAct.GetBlockedByActs();


        // Assertions
        Assert.IsTrue(blockedByActs.Contains(blockerAct1), "GetBlockedByActs() does not contain blockerAct1!");
        Assert.IsTrue(blockedByActs.Contains(blockerAct2), "GetBlockedByActs() does not contain blockerAct2!");
        Assert.IsTrue(blockedByActs.Count == 2, $"GetBlockedByActs() returned incorrect count! Count={blockedByActs.Count}");

        yield return null;
    }
    [UnityTest]
    public IEnumerator GetActsToBlockValid()
    {
        // Prerequisites
        var targetAct1 = new Act();
        var targetAct2 = new Act();
        var mainAct = new ManualFinishAct();

        targetAct1.Init("Target Act 1");
        targetAct2.Init("Target Act 2");
        mainAct.Init("Main Act");
        mainAct.AddToBlock(new() { targetAct1 }, Act.BlockType.Persistent);
        mainAct.AddToBlock(new() { targetAct2 }, Act.BlockType.Interrupt);

        // Perform Act
        var actsToBlock = mainAct.GetActsToBlock();


        // Assertions
        Assert.IsTrue(actsToBlock.ContainsKey(targetAct1) && actsToBlock[targetAct1] == Act.BlockType.Persistent, "GetActsToBlock() invalid for persistent entry!");
        Assert.IsTrue(actsToBlock.ContainsKey(targetAct2) && actsToBlock[targetAct2] == Act.BlockType.Interrupt, "GetActsToBlock() invalid for interrupt entry!");
        Assert.IsTrue(actsToBlock.Count == 2, $"GetActsToBlock() returned incorrect count! Count={actsToBlock.Count}");

        yield return null;
    }



    [UnityTest]
    public IEnumerator GetStatus()
    {
        // Prerequisites
        var theaterGO = new GameObject("Misc Status Theater");
        var theater = theaterGO.AddComponent<Theater>();

        var prologueAct = new Act();
        prologueAct.Init("Prologue Act");

        var statusBeforePerform = Act.Status.None;
        var statusAfterPerform = Act.Status.None;
        var statusInPreSetup = Act.Status.None;
        var statusInPostSetup = Act.Status.None;
        var statusInPerformStart = Act.Status.None;
        var statusInPrePrologue = Act.Status.None;
        var statusInPrologueComplete = Act.Status.None;
        var statusInPostPrologue = Act.Status.None;
        var statusInPreEnter = Act.Status.None;
        var statusInPostEnter = Act.Status.None;
        var statusInPreTick = Act.Status.None;
        var statusInPostTick = Act.Status.None;
        var statusInPrePhysicsTick = Act.Status.None;
        var statusInPostPhysicsTick = Act.Status.None;
        var statusInPreLateTick = Act.Status.None;
        var statusInPostLateTick = Act.Status.None;
        var statusInPreExit = Act.Status.None;
        var statusInPostExit = Act.Status.None;
        var statusInPerformEnd = Act.Status.None;
        var statusInPreCleanup = Act.Status.None;
        var statusInPostCleanup = Act.Status.None;


        var act = new StatusCheckAct();
        act.OnPreSetup += (a) => { statusInPreSetup = a.GetStatus(); };
        act.OnPostSetup += (a) => { statusInPostSetup = a.GetStatus(); };
        act.OnPerformStart += (a) => { statusInPerformStart = a.GetStatus(); };
        act.OnPrePrologue += (a) => { statusInPrePrologue = a.GetStatus(); };
        act.OnPrologueComplete += (a, pAct, pOutcome) => { statusInPrologueComplete = a.GetStatus(); };
        act.OnPostPrologue += (a) => { statusInPostPrologue = a.GetStatus(); };
        act.OnPreEnter += (a) => { statusInPreEnter = a.GetStatus(); };
        act.OnPostEnter += (a) => { statusInPostEnter = a.GetStatus(); };
        act.OnPreTick += (a) => { statusInPreTick = a.GetStatus(); };
        act.OnPostTick += (a) => { statusInPostTick = a.GetStatus(); };
        act.OnPrePhysicsTick += (a) => { statusInPrePhysicsTick = a.GetStatus(); };
        act.OnPostPhysicsTick += (a) => { statusInPostPhysicsTick = a.GetStatus(); };
        act.OnPreLateTick += (a) => { statusInPreLateTick = a.GetStatus(); };
        act.OnPostLateTick += (a) => { statusInPostLateTick = a.GetStatus(); };
        act.OnPreExit += (a) => { statusInPreExit = a.GetStatus(); };
        act.OnPostExit += (a) => { statusInPostExit = a.GetStatus(); };
        act.OnPerformEnd += (a) => { statusInPerformEnd = a.GetStatus(); };
        act.OnPreCleanup += (a) => { statusInPreCleanup = a.GetStatus(); };
        act.OnPostCleanup += (a) => { statusInPostCleanup = a.GetStatus(); };
        act.prologue += (a) => new() { prologueAct };
        act.Init("Misc Status Act", theater);

        statusBeforePerform = act.GetStatus();
        act.Perform();

        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();
        yield return null;

        act.ForceFinish();

        statusAfterPerform = act.GetStatus();

        act.Deinit();


        // Assertions
        Assert.IsTrue(statusInPreSetup == Act.Status.Initializing, $"Status wrong in OnPreSetup! Status='{statusInPreSetup}'");
        Assert.IsTrue(statusInPostSetup == Act.Status.None, $"Status wrong in OnPostSetup! Status='{statusInPostSetup}'");
        Assert.IsTrue(statusBeforePerform == Act.Status.None, $"Status should be None before Perform() is called. Status='{statusBeforePerform}'");
        Assert.IsTrue(statusInPerformStart == Act.Status.Prologuing, $"Status wrong in OnPerformStart! Status='{statusInPerformStart}'");
        Assert.IsTrue(statusInPrePrologue == Act.Status.Prologuing, $"Status wrong in OnPrePrologue! Status='{statusInPrePrologue}'");
        Assert.IsTrue(statusInPrologueComplete == Act.Status.Prologuing, $"Status wrong in OnPrologueComplete! Status='{statusInPrologueComplete}'");
        Assert.IsTrue(statusInPostPrologue == Act.Status.Prologuing, $"Status wrong in OnPostPrologue! Status='{statusInPostPrologue}'");
        Assert.IsTrue(statusInPreEnter == Act.Status.Entering, $"Status wrong in OnPreEnter! Status='{statusInPreEnter}'");
        Assert.IsTrue(statusInPostEnter == Act.Status.Entering, $"Status wrong in OnPostEnter! Status='{statusInPostEnter}'");
        Assert.IsTrue(statusInPreTick == Act.Status.Ticking, $"Status wrong in OnPreTick! Status='{statusInPreTick}'");
        Assert.IsTrue(statusInPostTick == Act.Status.Ticking, $"Status wrong in OnPostTick! Status='{statusInPostTick}'");
        Assert.IsTrue(statusInPrePhysicsTick == Act.Status.Ticking, $"Status wrong in OnPrePhysicsTick! Status='{statusInPrePhysicsTick}'");
        Assert.IsTrue(statusInPostPhysicsTick == Act.Status.Ticking, $"Status wrong in OnPostPhysicsTick! Status='{statusInPostPhysicsTick}'");
        Assert.IsTrue(statusInPreLateTick == Act.Status.Ticking, $"Status wrong in OnPreLateTick! Status='{statusInPreLateTick}'");
        Assert.IsTrue(statusInPostLateTick == Act.Status.Ticking, $"Status wrong in OnPostLateTick! Status='{statusInPostLateTick}'");
        Assert.IsTrue(statusInPreExit == Act.Status.Exiting, $"Status wrong in OnPreExit! Status='{statusInPreExit}'");
        Assert.IsTrue(statusInPostExit == Act.Status.Exiting, $"Status wrong in OnPostExit! Status='{statusInPostExit}'");
        Assert.IsTrue(statusAfterPerform == Act.Status.None, $"Status should be None after Perform() has ended. Status='{statusAfterPerform}'");
        Assert.IsTrue(statusInPreCleanup == Act.Status.Deinitializing, $"Status wrong in OnPreCleanup! Status='{statusInPreCleanup}'");
        Assert.IsTrue(statusInPostCleanup == Act.Status.None, $"Status wrong in OnPostCleanup! Status='{statusInPostCleanup}'");

        UnityEngine.Object.Destroy(theaterGO);

        yield return null;
    }
    [UnityTest]
    public IEnumerator GetOutcomeAccurateForAllOutcomes()
    {
        // Interrupted outcome
        {
            var act = new ManualFinishAct();
            act.Init("Misc Outcome Act");
            act.Perform();
            act.ManualFinish(Act.Outcome.Interrupted);

            Assert.IsTrue(act.GetOutcome() == Act.Outcome.Interrupted, $"GetOutcome() wrong for Interrupted! Outcome='{act.GetOutcome()}'");
        }

        // Failure outcome
        {
            var act = new ManualFinishAct();
            act.Init("Misc Outcome Act");
            act.Perform();
            act.ManualFinish(Act.Outcome.Failure);

            Assert.IsTrue(act.GetOutcome() == Act.Outcome.Failure, $"GetOutcome() wrong for Failure! Outcome='{act.GetOutcome()}'");
        }

        // Success outcome
        {
            var act = new ManualFinishAct();
            act.Init("Misc Outcome Act");
            act.Perform();
            act.ManualFinish(Act.Outcome.Success);

            Assert.IsTrue(act.GetOutcome() == Act.Outcome.Success, $"GetOutcome() wrong for Success! Outcome='{act.GetOutcome()}'");
        }

        // Retry outcome, captured during exit since retry auto reperforms
        {
            var recivedOutcome = Act.Outcome.Pending;
            var act = new ManualFinishAct();
            act.OnPreExit += (a) => { recivedOutcome = a.GetOutcome(); };
            act.Init("Misc Outcome Act");
            act.Perform();
            act.ManualFinish(Act.Outcome.Retry);

            Assert.IsTrue(recivedOutcome == Act.Outcome.Retry, $"GetOutcome() wrong for Retry! Outcome='{recivedOutcome}'");
        }

        yield return null;
    }



    [UnityTest]
    public IEnumerator GetPerformCountAccurate()
    {
        // Perform Act
        var act = new Act();
        act.Init("Misc Perform Count Act");

        act.Perform();
        act.Perform();
        act.Perform();


        // Assertions
        Assert.IsTrue(act.GetPerformCount() == 3, $"GetPerformCount() inaccurate! Count='{act.GetPerformCount()}'");

        yield return null;
    }
    [UnityTest]
    public IEnumerator GetTickCountAccurate()
    {
        var theaterGO = new GameObject("Misc Tick Count Theater");
        var theater = theaterGO.AddComponent<Theater>();

        var tickEventCount = 0;

        var act = new ManualFinishAct();
        act.overrideTickFlags = Act.TickFlags.Tick;
        act.OnPostTick += (a) => { tickEventCount++; };
        act.Init("Misc Tick Count Act", theater);
        act.Perform();

        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();
        yield return null;

        act.ManualFinish();

        Assert.IsTrue(act.GetTickCount() == tickEventCount && tickEventCount > 0, $"GetTickCount() inaccurate! Count='{act.GetTickCount()}' Expected='{tickEventCount}'");

        UnityEngine.Object.Destroy(theaterGO);
        yield return null;
    }
    [UnityTest]
    public IEnumerator GetPhysicsTickCountAccurate()
    {
        var theaterGO = new GameObject("Misc Tick Count Theater");
        var theater = theaterGO.AddComponent<Theater>();

        var physicsTickEventCount = 0;

        var act = new ManualFinishAct();
        act.overrideTickFlags = Act.TickFlags.PhysicsTick;
        act.OnPostPhysicsTick += (a) => { physicsTickEventCount++; };
        act.Init("Misc Tick Count Act", theater);
        act.Perform();

        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();
        yield return null;

        act.ManualFinish();

        Assert.IsTrue(act.GetPhysicsTickCount() == physicsTickEventCount && physicsTickEventCount > 0, $"GetPhysicsTickCount() inaccurate! Count='{act.GetPhysicsTickCount()}' Expected='{physicsTickEventCount}'");

        UnityEngine.Object.Destroy(theaterGO);
        yield return null;
    }
    [UnityTest]
    public IEnumerator GetLateTickCountAccurate()
    {
        var theaterGO = new GameObject("Misc Tick Count Theater");
        var theater = theaterGO.AddComponent<Theater>();

        var lateTickEventCount = 0;

        var act = new ManualFinishAct();
        act.overrideTickFlags = Act.TickFlags.LateTick;
        act.OnPostLateTick += (a) => { lateTickEventCount++; };
        act.Init("Misc Tick Count Act", theater);
        act.Perform();

        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();
        yield return null;

        act.ManualFinish();

        Assert.IsTrue(act.GetLateTickCount() == lateTickEventCount && lateTickEventCount > 0, $"GetLateTickCount() inaccurate! Count='{act.GetLateTickCount()}' Expected='{lateTickEventCount}'");

        UnityEngine.Object.Destroy(theaterGO);
        yield return null;
    }



    [UnityTest]
    public IEnumerator GetNameAccurate()
    {
        // Perform Act
        var actName = "Misc Name Act";
        var act = new Act();
        act.Init(actName);


        // Assertions
        Assert.IsTrue(act.GetName() == actName, $"GetName() inaccurate! Name='{act.GetName()}'  Expected name='{actName}'");

        yield return null;
    }
}
