using System;
using System.Collections;
using System.Linq;
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

// 1. Is IsBlockedBy() true only for the acts actually blocking?
// 1. Is IsBlockedBy() false after unblock?
// 1. Is IsBlockedBy(self) false when act is disabled?

// 1. Is HasInitialized() false before Init(), true after Init(), false after Deinit()?

// 1. Is GetPrevStatus() valid everywhere?

// 1. Is GetTickFlags() accurate for every flag combination?

// 1. Is GetLifetimePerformCount() accurate?
// 1. Is GetLifetimeTickCount() accurate?
// 1. Is GetLifetimePhysicsTickCount() accurate?
// 1. Is GetLifetimeLateTickCount() accurate?
// 1. Do lifetime counts persist after Deinit() while normal counts reset?


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
        act.prologue += (a) => new() { new() { prologueAct } };
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
        act.prologue += (a) => new() { new() { prologueAct } };
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
        act.prologue += (a) => new() { new() { prologueAct } };
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
        blocker.block = (Act a) => new() { { blockedAct, Act.BlockType.Persistent } };
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
    public IEnumerator GetBlockedByActsValid()
    {
        // Prerequisites
        var blockedAct = new Act();
        var blockerAct1 = new ManualFinishAct();
        var blockerAct2 = new ManualFinishAct();

        blockedAct.Init("Blocked Act");
        blockerAct1.Init("Blocker Act 1");
        blockerAct2.Init("Blocker Act 2");
        blockerAct1.block = (Act a) => new() { { blockedAct, Act.BlockType.Persistent } };
        blockerAct2.block = (Act a) => new() { { blockedAct, Act.BlockType.Persistent } };

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
        mainAct.block = (Act a) => new() {
            { targetAct1, Act.BlockType.Persistent },
            { targetAct2, Act.BlockType.Interrupt }
        };

        // Perform Act
        mainAct.Perform();
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
        act.prologue += (a) => new() { new() { prologueAct } };
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


    [UnityTest]
    public IEnumerator IsBlockedByOnlyActualBlockers()
    {
        // Prerequisites
        var targetName = "Misc Target Act";
        var otherTargetName = "Misc Other Target Act";
        var blockerName = "Misc Blocker Act";
        var otherBlockerName = "Misc Other Blocker Act";
        var idleBlockerName = "Misc Idle Blocker Act";

        var target = new Act();
        var otherTarget = new Act();
        var blocker = new ManualFinishAct();
        var otherBlocker = new ManualFinishAct();
        var idleBlocker = new ManualFinishAct();

        target.Init(targetName);
        otherTarget.Init(otherTargetName);
        blocker.Init(blockerName);
        otherBlocker.Init(otherBlockerName);
        idleBlocker.Init(idleBlockerName);

        blocker.block = (Act a) => new() { { target, Act.BlockType.Persistent } };
        otherBlocker.block = (Act a) => new() { { otherTarget, Act.BlockType.Persistent } };
        idleBlocker.block = (Act a) => new() { { target, Act.BlockType.Persistent } };


        // Perform blockers, idle blocker never performs
        blocker.Perform();
        otherBlocker.Perform();


        // Assertions
        Assert.IsTrue(target.IsBlockedBy(blocker), "IsBlockedBy() false for the act actually blocking!");
        Assert.IsFalse(target.IsBlockedBy(otherBlocker), "IsBlockedBy() true for an act blocking someone else!");
        Assert.IsFalse(target.IsBlockedBy(idleBlocker), "IsBlockedBy() true for an act that is not performing!");
        Assert.IsFalse(target.IsBlockedBy(target), "IsBlockedBy() true for self while enabled!");
        Assert.IsFalse(otherTarget.IsBlockedBy(blocker), "IsBlockedBy() true for blocker of another target!");
        Assert.IsTrue(otherTarget.IsBlockedBy(otherBlocker), "IsBlockedBy() false for other target's actual blocker!");


        yield return null;
    }
    // [UnityTest]
    // public IEnumerator IsBlockedByFalseAfterUnblock()
    // {
    //     // Prerequisites
    //     var targetName = "Misc Target Act";
    //     var blockerName = "Misc Blocker Act";


    //     // Unblock by blocker finishing
    //     {
    //         var target = new Act();
    //         var blocker = new ManualFinishAct();
    //         target.Init(targetName);
    //         blocker.Init(blockerName);
    //         blocker.block = (Act a) => new() { target, Act.BlockType.Persistent };

    //         blocker.Perform();
    //         var wasBlockedBy = target.IsBlockedBy(blocker);
    //         blocker.ManualFinish();
    //         var isBlockedByAfterFinish = target.IsBlockedBy(blocker);


    //         // Assertions
    //         Assert.IsTrue(wasBlockedBy, "IsBlockedBy() false while blocker is performing!");
    //         Assert.IsFalse(isBlockedByAfterFinish, "IsBlockedBy() true after blocker finished!");
    //     }


    //     // Unblock by removing from block list
    //     {
    //         var target = new Act();
    //         var blocker = new ManualFinishAct();
    //         target.Init(targetName);
    //         blocker.Init(blockerName);
    //         blocker.block = (Act a) => new() { target, Act.BlockType.Persistent };

    //         blocker.Perform();
    //         var wasBlockedBy = target.IsBlockedBy(blocker);
    //         blocker.RemoveFromBlock(new List<Act> { target });
    //         var isBlockedByAfterRemove = target.IsBlockedBy(blocker);


    //         // Assertions
    //         Assert.IsTrue(wasBlockedBy, "IsBlockedBy() false while blocker is performing! Remove variation");
    //         Assert.IsFalse(isBlockedByAfterRemove, "IsBlockedBy() true after removed from block list!");
    //     }

    //     yield return null;
    // }
    [UnityTest]
    public IEnumerator IsBlockedBySelfFalseWhenDisabled()
    {
        // Prerequisites
        var actName = "Misc Disabled Act";
        var blockerName = "Misc Blocker Act";


        // Only disabled
        {
            var act = new Act();
            act.Init(actName);
            act.SetEnabled(false);


            // Assertions
            Assert.IsFalse(act.IsEnabled(), "IsEnabled() true despite being disabled!");
            Assert.IsFalse(act.IsBlockedBy(act), "IsBlockedBy(self) true when act is disabled!");
        }


        // Disabled and blocked by another act
        {
            var act = new Act();
            var blocker = new ManualFinishAct();
            act.Init(actName);
            blocker.Init(blockerName);
            blocker.block = (Act a) => new() { { act, Act.BlockType.Persistent } };

            act.SetEnabled(false);
            blocker.Perform();


            // Assertions
            Assert.IsFalse(act.IsBlockedBy(act), "IsBlockedBy(self) true when act is disabled and blocked!");
            Assert.IsTrue(act.IsBlockedBy(blocker), "IsBlockedBy() false for blocker when act is also disabled!");
        }

        yield return null;
    }



    [UnityTest]
    public IEnumerator HasInitializedValid()
    {
        // Prerequisites
        var actName = "Misc Initialized Act";
        var act = new Act();


        // Check at every stage
        var beforeInit = act.HasInitialized();
        act.Init(actName);
        var afterInit = act.HasInitialized();
        act.Deinit();
        var afterDeinit = act.HasInitialized();


        // Assertions
        Assert.IsFalse(beforeInit, "HasInitialized() true before Init()!");
        Assert.IsTrue(afterInit, "HasInitialized() false after Init()!");
        Assert.IsFalse(afterDeinit, "HasInitialized() true after Deinit()!");

        yield return null;
    }



    [UnityTest]
    public IEnumerator GetPrevStatus()
    {
        // Prerequisites
        var theaterName = "Misc Prev Status Theater";
        var prologueName = "Prologue Act";
        var actName = "Misc Prev Status Act";
        var waitFrames = 2;

        var theaterGO = new GameObject(theaterName);
        var theater = theaterGO.AddComponent<Theater>();

        var prologueAct = new Act();
        prologueAct.Init(prologueName);

        var prevInPreSetup = Act.Status.Entering;
        var prevInPostSetup = Act.Status.Entering;
        var prevInPerformStart = Act.Status.Entering;
        var prevInPrePrologue = Act.Status.Entering;
        var prevInPrologueComplete = Act.Status.Entering;
        var prevInPostPrologue = Act.Status.Entering;
        var prevInPreEnter = Act.Status.Entering;
        var prevInPostEnter = Act.Status.Entering;
        var prevInPreTick = Act.Status.None;
        var prevInPostTick = Act.Status.None;
        var prevInPrePhysicsTick = Act.Status.None;
        var prevInPostPhysicsTick = Act.Status.None;
        var prevInPreLateTick = Act.Status.None;
        var prevInPostLateTick = Act.Status.None;
        var prevInPreExit = Act.Status.None;
        var prevInPostExit = Act.Status.None;
        var prevInPerformEnd = Act.Status.None;
        var prevInPreCleanup = Act.Status.Entering;
        var prevInPostCleanup = Act.Status.Entering;


        // Setup act
        var act = new PrevStatusCheckAct();
        act.OnPreSetup += (a) => { prevInPreSetup = a.GetPrevStatus(); };
        act.OnPostSetup += (a) => { prevInPostSetup = a.GetPrevStatus(); };
        act.OnPerformStart += (a) => { prevInPerformStart = a.GetPrevStatus(); };
        act.OnPrePrologue += (a) => { prevInPrePrologue = a.GetPrevStatus(); };
        act.OnPrologueComplete += (a, pAct, pOutcome) => { prevInPrologueComplete = a.GetPrevStatus(); };
        act.OnPostPrologue += (a) => { prevInPostPrologue = a.GetPrevStatus(); };
        act.OnPreEnter += (a) => { prevInPreEnter = a.GetPrevStatus(); };
        act.OnPostEnter += (a) => { prevInPostEnter = a.GetPrevStatus(); };
        act.OnPreTick += (a) => { prevInPreTick = a.GetPrevStatus(); };
        act.OnPostTick += (a) => { prevInPostTick = a.GetPrevStatus(); };
        act.OnPrePhysicsTick += (a) => { prevInPrePhysicsTick = a.GetPrevStatus(); };
        act.OnPostPhysicsTick += (a) => { prevInPostPhysicsTick = a.GetPrevStatus(); };
        act.OnPreLateTick += (a) => { prevInPreLateTick = a.GetPrevStatus(); };
        act.OnPostLateTick += (a) => { prevInPostLateTick = a.GetPrevStatus(); };
        act.OnPreExit += (a) => { prevInPreExit = a.GetPrevStatus(); };
        act.OnPostExit += (a) => { prevInPostExit = a.GetPrevStatus(); };
        act.OnPerformEnd += (a) => { prevInPerformEnd = a.GetPrevStatus(); };
        act.OnPreCleanup += (a) => { prevInPreCleanup = a.GetPrevStatus(); };
        act.OnPostCleanup += (a) => { prevInPostCleanup = a.GetPrevStatus(); };
        act.prologue += (a) => new() { new() { prologueAct } };
        act.Init(actName, theater);

        var prevAfterInit = act.GetPrevStatus();

        act.Perform();

        for (var i = 0; i < waitFrames; i++)
        {
            yield return new WaitForFixedUpdate();
            yield return null;
        }

        act.ForceFinish();

        var prevAfterPerform = act.GetPrevStatus();

        act.Deinit();


        // Assertions for init
        Assert.IsTrue(prevInPreSetup == Act.Status.None, $"PrevStatus wrong in OnPreSetup! Prev='{prevInPreSetup}'");
        Assert.IsTrue(act.prevStatusInSetup == Act.Status.None, $"PrevStatus wrong in Setup()! Prev='{act.prevStatusInSetup}'");
        Assert.IsTrue(prevInPostSetup == Act.Status.Initializing, $"PrevStatus wrong in OnPostSetup! Prev='{prevInPostSetup}'");
        Assert.IsTrue(prevAfterInit == Act.Status.Initializing, $"PrevStatus wrong after Init()! Prev='{prevAfterInit}'");


        // Assertions for prologue
        Assert.IsTrue(prevInPerformStart == Act.Status.None, $"PrevStatus wrong in OnPerformStart! Prev='{prevInPerformStart}'");
        Assert.IsTrue(prevInPrePrologue == Act.Status.None, $"PrevStatus wrong in OnPrePrologue! Prev='{prevInPrePrologue}'");
        Assert.IsTrue(prevInPrologueComplete == Act.Status.None, $"PrevStatus wrong in OnPrologueComplete! Prev='{prevInPrologueComplete}'");
        Assert.IsTrue(prevInPostPrologue == Act.Status.None, $"PrevStatus wrong in OnPostPrologue! Prev='{prevInPostPrologue}'");


        // Assertions for enter
        Assert.IsTrue(prevInPreEnter == Act.Status.Prologuing, $"PrevStatus wrong in OnPreEnter! Prev='{prevInPreEnter}'");
        Assert.IsTrue(act.prevStatusInEnter == Act.Status.Prologuing, $"PrevStatus wrong in Enter()! Prev='{act.prevStatusInEnter}'");
        Assert.IsTrue(prevInPostEnter == Act.Status.Prologuing, $"PrevStatus wrong in OnPostEnter! Prev='{prevInPostEnter}'");


        // Assertions for ticks
        Assert.IsTrue(prevInPreTick == Act.Status.Entering, $"PrevStatus wrong in OnPreTick! Prev='{prevInPreTick}'");
        Assert.IsTrue(act.prevStatusInTick == Act.Status.Entering, $"PrevStatus wrong in Tick()! Prev='{act.prevStatusInTick}'");
        Assert.IsTrue(prevInPostTick == Act.Status.Entering, $"PrevStatus wrong in OnPostTick! Prev='{prevInPostTick}'");
        Assert.IsTrue(prevInPrePhysicsTick == Act.Status.Entering, $"PrevStatus wrong in OnPrePhysicsTick! Prev='{prevInPrePhysicsTick}'");
        Assert.IsTrue(act.prevStatusInPhysicsTick == Act.Status.Entering, $"PrevStatus wrong in PhysicsTick()! Prev='{act.prevStatusInPhysicsTick}'");
        Assert.IsTrue(prevInPostPhysicsTick == Act.Status.Entering, $"PrevStatus wrong in OnPostPhysicsTick! Prev='{prevInPostPhysicsTick}'");
        Assert.IsTrue(prevInPreLateTick == Act.Status.Entering, $"PrevStatus wrong in OnPreLateTick! Prev='{prevInPreLateTick}'");
        Assert.IsTrue(act.prevStatusInLateTick == Act.Status.Entering, $"PrevStatus wrong in LateTick()! Prev='{act.prevStatusInLateTick}'");
        Assert.IsTrue(prevInPostLateTick == Act.Status.Entering, $"PrevStatus wrong in OnPostLateTick! Prev='{prevInPostLateTick}'");


        // Assertions for exit
        Assert.IsTrue(prevInPreExit == Act.Status.Ticking, $"PrevStatus wrong in OnPreExit! Prev='{prevInPreExit}'");
        Assert.IsTrue(act.prevStatusInExit == Act.Status.Ticking, $"PrevStatus wrong in Exit()! Prev='{act.prevStatusInExit}'");
        Assert.IsTrue(prevInPostExit == Act.Status.Ticking, $"PrevStatus wrong in OnPostExit! Prev='{prevInPostExit}'");
        Assert.IsTrue(prevInPerformEnd == Act.Status.Concluding, $"PrevStatus wrong in OnPerformEnd! Prev='{prevInPerformEnd}'");
        Assert.IsTrue(prevAfterPerform == Act.Status.Concluding, $"PrevStatus wrong after perform ended! Prev='{prevAfterPerform}'");


        // Assertions for cleanup
        Assert.IsTrue(prevInPreCleanup == Act.Status.None, $"PrevStatus wrong in OnPreCleanup! Prev='{prevInPreCleanup}'");
        Assert.IsTrue(act.prevStatusInCleanup == Act.Status.None, $"PrevStatus wrong in Cleanup()! Prev='{act.prevStatusInCleanup}'");
        Assert.IsTrue(prevInPostCleanup == Act.Status.Deinitializing, $"PrevStatus wrong in OnPostCleanup! Prev='{prevInPostCleanup}'");

        UnityEngine.Object.Destroy(theaterGO);

        yield return null;
    }



    [UnityTest]
    public IEnumerator GetTickFlagsAccurate()
    {
        // Prerequisites
        var actName = "Misc Tick Flags Act";
        var allCombos = new Act.TickFlags[]
        {
            Act.TickFlags.None,
            Act.TickFlags.Tick,
            Act.TickFlags.PhysicsTick,
            Act.TickFlags.LateTick,
            Act.TickFlags.Tick | Act.TickFlags.PhysicsTick,
            Act.TickFlags.Tick | Act.TickFlags.LateTick,
            Act.TickFlags.PhysicsTick | Act.TickFlags.LateTick,
            Act.TickFlags.Tick | Act.TickFlags.PhysicsTick | Act.TickFlags.LateTick,
        };


        // Check every combo
        foreach (var combo in allCombos)
        {
            var act = new ReperformableInfiAct();
            act.overrideTickFlag = combo;
            act.Init(actName);


            // Assertions
            Assert.IsTrue(act.GetTickFlags() == combo, $"GetTickFlags() wrong for combo='{combo}'! Flags='{act.GetTickFlags()}'");
        }

        yield return null;
    }



    [UnityTest]
    public IEnumerator GetLifetimePerformCountAccurate()
    {
        // Prerequisites
        var actName = "Misc Lifetime Perform Act";
        var failActName = "Misc Fail Perform Act";
        var performAttempts = 3;
        var extraAttempts = 1;


        // Successful performs
        var act = new Act();
        act.Init(actName);

        for (var i = 0; i < performAttempts; i++)
        {
            act.Perform();
        }

        var lifetimeBeforeDeinit = act.GetLifetimePerformCount();
        var countBeforeDeinit = act.GetPerformCount();


        // Reinit and perform again
        act.Deinit();
        act.Init(actName);

        for (var i = 0; i < extraAttempts; i++)
        {
            act.Perform();
        }


        // Failed performs
        var failAct = new FalseCanPerformAct();
        failAct.Init(failActName);

        for (var i = 0; i < performAttempts; i++)
        {
            failAct.Perform();
        }


        // Assertions
        Assert.IsTrue(lifetimeBeforeDeinit == performAttempts, $"GetLifetimePerformCount() inaccurate! Count={lifetimeBeforeDeinit}  Expected={performAttempts}");
        Assert.IsTrue(lifetimeBeforeDeinit == countBeforeDeinit, $"GetLifetimePerformCount() differs from GetPerformCount() before deinit! Lifetime={lifetimeBeforeDeinit}  Normal={countBeforeDeinit}");
        Assert.IsTrue(act.GetLifetimePerformCount() == performAttempts + extraAttempts, $"GetLifetimePerformCount() did not accumulate after reinit! Count={act.GetLifetimePerformCount()}");
        Assert.IsTrue(failAct.GetLifetimePerformCount() == 0, $"GetLifetimePerformCount() counted failed performs! Count={failAct.GetLifetimePerformCount()}");

        yield return null;
    }
    [UnityTest]
    public IEnumerator GetLifetimeTickCountAccurate()
    {
        // Prerequisites
        var theaterName = "Misc Lifetime Tick Theater";
        var actName = "Misc Lifetime Tick Act";
        var waitFrames = 3;

        var theaterGO = new GameObject(theaterName);
        var theater = theaterGO.AddComponent<Theater>();

        var tickEventCount = 0;


        // Perform Act
        var act = new TickAct();
        act.OnPostTick += (a) => { tickEventCount++; };
        act.Init(actName, theater);
        act.Perform();

        for (var i = 0; i < waitFrames; i++)
        {
            yield return null;
        }

        act.Abort();


        // Assertions
        Assert.IsTrue(act.GetLifetimeTickCount() == tickEventCount && tickEventCount > 0, $"GetLifetimeTickCount() inaccurate! Count='{act.GetLifetimeTickCount()}' Expected='{tickEventCount}'");
        Assert.IsTrue(act.GetLifetimeTickCount() == act.GetTickCount(), $"GetLifetimeTickCount() differs from GetTickCount() before deinit! Lifetime='{act.GetLifetimeTickCount()}' Normal='{act.GetTickCount()}'");

        UnityEngine.Object.Destroy(theaterGO);
        yield return null;
    }
    [UnityTest]
    public IEnumerator GetLifetimePhysicsTickCountAccurate()
    {
        // Prerequisites
        var theaterName = "Misc Lifetime Physics Tick Theater";
        var actName = "Misc Lifetime Physics Tick Act";
        var waitFrames = 3;

        var theaterGO = new GameObject(theaterName);
        var theater = theaterGO.AddComponent<Theater>();

        var physicsTickEventCount = 0;


        // Perform Act
        var act = new PhysicsTickAct();
        act.OnPostPhysicsTick += (a) => { physicsTickEventCount++; };
        act.Init(actName, theater);
        act.Perform();

        for (var i = 0; i < waitFrames; i++)
        {
            yield return new WaitForFixedUpdate();
        }

        act.Abort();


        // Assertions
        Assert.IsTrue(act.GetLifetimePhysicsTickCount() == physicsTickEventCount && physicsTickEventCount > 0, $"GetLifetimePhysicsTickCount() inaccurate! Count='{act.GetLifetimePhysicsTickCount()}' Expected='{physicsTickEventCount}'");
        Assert.IsTrue(act.GetLifetimePhysicsTickCount() == act.GetPhysicsTickCount(), $"GetLifetimePhysicsTickCount() differs from GetPhysicsTickCount() before deinit! Lifetime='{act.GetLifetimePhysicsTickCount()}' Normal='{act.GetPhysicsTickCount()}'");

        UnityEngine.Object.Destroy(theaterGO);
        yield return null;
    }
    [UnityTest]
    public IEnumerator GetLifetimeLateTickCountAccurate()
    {
        // Prerequisites
        var theaterName = "Misc Lifetime Late Tick Theater";
        var actName = "Misc Lifetime Late Tick Act";
        var waitFrames = 3;

        var theaterGO = new GameObject(theaterName);
        var theater = theaterGO.AddComponent<Theater>();

        var lateTickEventCount = 0;


        // Perform Act
        var act = new LateTickAct();
        act.OnPostLateTick += (a) => { lateTickEventCount++; };
        act.Init(actName, theater);
        act.Perform();

        for (var i = 0; i < waitFrames; i++)
        {
            yield return null;
        }

        act.Abort();


        // Assertions
        Assert.IsTrue(act.GetLifetimeLateTickCount() == lateTickEventCount && lateTickEventCount > 0, $"GetLifetimeLateTickCount() inaccurate! Count='{act.GetLifetimeLateTickCount()}' Expected='{lateTickEventCount}'");
        Assert.IsTrue(act.GetLifetimeLateTickCount() == act.GetLateTickCount(), $"GetLifetimeLateTickCount() differs from GetLateTickCount() before deinit! Lifetime='{act.GetLifetimeLateTickCount()}' Normal='{act.GetLateTickCount()}'");

        UnityEngine.Object.Destroy(theaterGO);
        yield return null;
    }
    [UnityTest]
    public IEnumerator LifetimeCountsPersistAfterDeinit()
    {
        // Prerequisites
        var theaterName = "Misc Lifetime Deinit Theater";
        var actName = "Misc Lifetime Deinit Act";
        var waitFrames = 3;
        var resetCount = 0;

        var theaterGO = new GameObject(theaterName);
        var theater = theaterGO.AddComponent<Theater>();


        // Perform Act
        var act = new PerformingCheckAct();
        act.Init(actName, theater);
        act.Perform();

        for (var i = 0; i < waitFrames; i++)
        {
            yield return new WaitForFixedUpdate();
            yield return null;
        }


        // Store counts before deinit
        var lifetimePerformBefore = act.GetLifetimePerformCount();
        var lifetimeTickBefore = act.GetLifetimeTickCount();
        var lifetimePhysicsTickBefore = act.GetLifetimePhysicsTickCount();
        var lifetimeLateTickBefore = act.GetLifetimeLateTickCount();

        act.Deinit();


        // Assertions for lifetime counts
        Assert.IsTrue(lifetimePerformBefore > 0, $"Act did not perform before deinit! Lifetime Perform Count={lifetimePerformBefore}");
        Assert.IsTrue(lifetimeTickBefore > 0, $"Act did not tick before deinit! Lifetime Tick Count={lifetimeTickBefore}");
        Assert.IsTrue(lifetimePhysicsTickBefore > 0, $"Act did not physics tick before deinit! Lifetime Physics Tick Count={lifetimePhysicsTickBefore}");
        Assert.IsTrue(lifetimeLateTickBefore > 0, $"Act did not late tick before deinit! Lifetime Late Tick Count={lifetimeLateTickBefore}");
        Assert.IsTrue(act.GetLifetimePerformCount() == lifetimePerformBefore, $"Lifetime perform count changed after Deinit()! Count={act.GetLifetimePerformCount()}  Expected={lifetimePerformBefore}");
        Assert.IsTrue(act.GetLifetimeTickCount() == lifetimeTickBefore, $"Lifetime tick count changed after Deinit()! Count={act.GetLifetimeTickCount()}  Expected={lifetimeTickBefore}");
        Assert.IsTrue(act.GetLifetimePhysicsTickCount() == lifetimePhysicsTickBefore, $"Lifetime physics tick count changed after Deinit()! Count={act.GetLifetimePhysicsTickCount()}  Expected={lifetimePhysicsTickBefore}");
        Assert.IsTrue(act.GetLifetimeLateTickCount() == lifetimeLateTickBefore, $"Lifetime late tick count changed after Deinit()! Count={act.GetLifetimeLateTickCount()}  Expected={lifetimeLateTickBefore}");


        // Assertions for normal counts
        Assert.IsTrue(act.GetPerformCount() == resetCount, $"Perform count did not reset after Deinit()! Count={act.GetPerformCount()}");
        Assert.IsTrue(act.GetTickCount() == resetCount, $"Tick count did not reset after Deinit()! Count={act.GetTickCount()}");
        Assert.IsTrue(act.GetPhysicsTickCount() == resetCount, $"Physics tick count did not reset after Deinit()! Count={act.GetPhysicsTickCount()}");
        Assert.IsTrue(act.GetLateTickCount() == resetCount, $"Late tick count did not reset after Deinit()! Count={act.GetLateTickCount()}");

        UnityEngine.Object.Destroy(theaterGO);
        yield return null;
    }
}
