using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;


// 1. Does an ongoing act stop when Abort() is invoked?
// 1. Does abort fail from OnPreSetup?
// 1. Does abort fail from OnPostSetup?
// 1. Does abort succeed from OnPerformStart?
// 1. Does abort succeed from OnPrePrologue?
// 1. Does abort succeed from OnPrologueComplete?
// 1. Does abort succeed from OnPostPrologue?
// 1. Does abort succeed from OnPreEnter?
// 1. Does abort succeed from OnPostEnter?
// 1. Does abort succeed from OnPreTick?
// 1. Does abort succeed from OnPostTick?
// 1. Does abort succeed from OnPrePhysicsTick?
// 1. Does abort succeed from OnPostPhysicsTick?
// 1. Does abort succeed from OnPreLateTick?
// 1. Does abort succeed from OnPostLateTick?
// 1. Does abort fail from OnPreExit?
// 1. Does abort fail from OnPostExit?
// 1. Does abort succeed from OnPerformEnd?
// 1. Does abort fail from OnPreCleanup?
// 1. Does abort fail from OnPostCleanup?
// 1. Does abort succeed from OnEnableChanged?
// 1. Does abort succeed from OnBlockChanged?


public class ActAbortTests
{
    [UnityTest]
    public IEnumerator AbortStopsOngoingAct()
    {
        // Perform Act
        var act = new ManualFinishAct();
        act.Init("Test Act");
        act.Perform();

        var wasOngoing = act.IsOngoing();

        act.Abort();


        // Assertions
        Assert.IsTrue(wasOngoing, "Act was not ongoing before Abort()!");
        Assert.IsTrue(!act.IsOngoing() && act.GetOutcome() == Act.Outcome.Interrupted, $"Act did not stop on Abort()! IsOngoing={act.IsOngoing()}  Outcome={act.GetOutcome()}");

        yield return null;
    }
    [UnityTest]
    public IEnumerator AbortFailsFromOnPreSetup()
    {
        // Perform Act
        var act = new ExitAct();
        act.OnPreSetup += (a) => { act.Abort(); };
        act.Init("Test Act");


        // Assertions
        Assert.IsTrue(act.callCount == 0 && !act.IsOngoing(), $"Abort from OnPreSetup calling Exit()! callCount={act.callCount}  Outcome={act.GetOutcome()}");

        yield return null;
    }
    [UnityTest]
    public IEnumerator AbortFailsFromOnPostSetup()
    {
        // Perform Act
        var act = new ExitAct();
        act.OnPostSetup += (a) => { act.Abort(); };
        act.Init("Test Act");


        // Assertions
        Assert.IsTrue(act.callCount == 0 && !act.IsOngoing(), $"Abort from OnPostSetup calling Exit()! callCount={act.callCount}  Outcome={act.GetOutcome()}");

        yield return null;
    }
    [UnityTest]
    public IEnumerator AbortSucceedsFromOnPerformStart()
    {
        // Perform Act
        var act = new EnterAct();
        act.OnPerformStart += (a) => { act.Abort(); };
        act.Init("Test Act");
        act.Perform();


        // Assertions
        Assert.IsTrue(act.callCount == 0 && act.GetOutcome() == Act.Outcome.Interrupted, $"Abort from OnPerformStart did not cut perform short! callCount={act.callCount}  Outcome={act.GetOutcome()}");

        yield return null;
    }
    [UnityTest]
    public IEnumerator AbortSucceedsFromOnPrePrologue()
    {
        // Perform Act
        var prologueAct = new Act();
        prologueAct.Init("Prologue Act");

        var mainAct = new EnterAct();
        mainAct.OnPrePrologue += (a) => { mainAct.Abort(); };
        mainAct.prologue = (a) => new List<Act> { prologueAct };
        mainAct.Init("Main Act");

        mainAct.Perform();


        // Assertions
        Assert.IsTrue(prologueAct.GetPerformCount() == 0 && mainAct.callCount == 0 && mainAct.GetOutcome() == Act.Outcome.Interrupted, $"Abort from OnPrePrologue did not cut prologue short! prologueCount={prologueAct.GetPerformCount()}  callCount={mainAct.callCount}  Outcome={mainAct.GetOutcome()}");

        yield return null;
    }
    [UnityTest]
    public IEnumerator AbortSucceedsFromOnPrologueComplete()
    {
        // Perform Act
        var prologueAct = new Act();
        prologueAct.Init("Prologue Act");

        var mainAct = new EnterAct();
        mainAct.prologue = (a) => new List<Act> { prologueAct };
        mainAct.OnPrologueComplete += (a, pAct, outcome) => { mainAct.Abort(); };
        mainAct.Init("Main Act");

        mainAct.Perform();


        // Assertions
        Assert.IsTrue(mainAct.callCount == 0 && mainAct.GetOutcome() == Act.Outcome.Interrupted, $"Abort from OnPrologueComplete did not cut perform short! callCount={mainAct.callCount}  Outcome={mainAct.GetOutcome()}");

        yield return null;
    }
    [UnityTest]
    public IEnumerator AbortSucceedsFromOnPostPrologue()
    {
        // Perform Act
        var prologueAct = new Act();
        prologueAct.Init("Prologue Act");

        var mainAct = new EnterAct();
        mainAct.prologue = (a) => new List<Act> { prologueAct };
        mainAct.OnPostPrologue += (a) => { mainAct.Abort(); };
        mainAct.Init("Main Act");

        mainAct.Perform();


        // Assertions
        Assert.IsTrue(mainAct.callCount == 0 && mainAct.GetOutcome() == Act.Outcome.Interrupted, $"Abort from OnPostPrologue did not cut perform short! callCount={mainAct.callCount}  Outcome={mainAct.GetOutcome()}");

        yield return null;
    }
    [UnityTest]
    public IEnumerator AbortSucceedsFromOnPreEnter()
    {
        // Perform Act
        var act = new EnterAct();
        act.OnPreEnter += (a) => { act.Abort(); };
        act.Init("Test Act");
        act.Perform();


        // Assertions
        Assert.IsTrue(act.callCount == 0 && act.GetOutcome() == Act.Outcome.Interrupted, $"Abort from OnPreEnter did not cut Enter() short! callCount={act.callCount}  Outcome={act.GetOutcome()}");

        yield return null;
    }
    [UnityTest]
    public IEnumerator AbortSucceedsFromOnPostEnter()
    {
        // Perform Act
        var act = new EnterAct();
        act.OnPostEnter += (a) => { act.Abort(); };
        act.Init("Test Act");
        act.Perform();


        // Assertions
        Assert.IsTrue(act.callCount == 1 && act.GetOutcome() == Act.Outcome.Interrupted, $"Abort from OnPostEnter did not override natural outcome! callCount={act.callCount}  Outcome={act.GetOutcome()}");

        yield return null;
    }
    [UnityTest]
    public IEnumerator AbortSucceedsFromOnPreTick()
    {
        // Perform Act
        var theater = new GameObject("Test Theater").AddComponent<Theater>();
        var act = new TickAct();
        act.OnPreTick += (a) => { act.Abort(); };
        act.Init("Test Act", theater);

        act.Perform();

        yield return null;
        yield return null;


        // Assertions
        Assert.IsTrue(act.callCount == 0 && act.GetOutcome() == Act.Outcome.Interrupted, $"Abort from OnPreTick did not cut Tick() short! callCount={act.callCount}  Outcome={act.GetOutcome()}");

        yield return null;
    }
    [UnityTest]
    public IEnumerator AbortSucceedsFromOnPostTick()
    {
        // Perform Act
        var theater = new GameObject("Test Theater").AddComponent<Theater>();
        var act = new TickAct();
        act.OnPostTick += (a) => { act.Abort(); };
        act.Init("Test Act", theater);
        act.Perform();

        yield return null;
        yield return null;


        // Assertions
        Assert.IsTrue(act.callCount == 1 && act.GetOutcome() == Act.Outcome.Interrupted, $"Abort from OnPostTick did not override natural outcome! callCount={act.callCount}  Outcome={act.GetOutcome()}");

        yield return null;
    }
    [UnityTest]
    public IEnumerator AbortSucceedsFromOnPrePhysicsTick()
    {
        // Perform Act
        var theater = new GameObject("Test Theater").AddComponent<Theater>();
        var act = new PhysicsTickAct();
        act.OnPrePhysicsTick += (a) => { act.Abort(); };
        act.Init("Test Act", theater);
        act.Perform();

        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();


        // Assertions
        Assert.IsTrue(act.callCount == 0 && act.GetOutcome() == Act.Outcome.Interrupted, $"Abort from OnPrePhysicsTick did not cut PhysicsTick() short! callCount={act.callCount}  Outcome={act.GetOutcome()}");

        yield return null;
    }
    [UnityTest]
    public IEnumerator AbortSucceedsFromOnPostPhysicsTick()
    {
        // Perform Act
        var theater = new GameObject("Test Theater").AddComponent<Theater>();
        var act = new PhysicsTickAct();
        act.OnPostPhysicsTick += (a) => { act.Abort(); };
        act.Init("Test Act", theater);
        act.Perform();

        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();


        // Assertions
        Assert.IsTrue(act.callCount == 1 && act.GetOutcome() == Act.Outcome.Interrupted, $"Abort from OnPostPhysicsTick did not override natural outcome! callCount={act.callCount}  Outcome={act.GetOutcome()}");

        yield return null;
    }
    [UnityTest]
    public IEnumerator AbortSucceedsFromOnPreLateTick()
    {
        // Perform Act
        var theater = new GameObject("Test Theater").AddComponent<Theater>();
        var act = new LateTickAct();
        act.OnPreLateTick += (a) => { act.Abort(); };
        act.Init("Test Act", theater);
        act.Perform();

        yield return null;
        yield return null;


        // Assertions
        Assert.IsTrue(act.callCount == 0 && act.GetOutcome() == Act.Outcome.Interrupted, $"Abort from OnPreLateTick did not cut LateTick() short! callCount={act.callCount}  Outcome={act.GetOutcome()}");

        yield return null;
    }
    [UnityTest]
    public IEnumerator AbortSucceedsFromOnPostLateTick()
    {
        // Perform Act
        var theater = new GameObject("Test Theater").AddComponent<Theater>();
        var act = new LateTickAct();
        act.OnPostLateTick += (a) => { act.Abort(); };
        act.Init("Test Act", theater);
        act.Perform();

        yield return null;
        yield return null;


        // Assertions
        Assert.IsTrue(act.callCount == 1 && act.GetOutcome() == Act.Outcome.Interrupted, $"Abort from OnPostLateTick did not override natural outcome! callCount={act.callCount}  Outcome={act.GetOutcome()}");

        yield return null;
    }
    [UnityTest]
    public IEnumerator AbortFailsFromOnPreExit()
    {
        // Perform Act
        var act = new ManualFinishAct();
        act.OnPreExit += (a) => { act.Abort(); };
        act.Init("Test Act");
        act.Perform();

        act.ManualFinish(Act.Outcome.Success);


        // Assertions
        Assert.IsTrue(!act.IsOngoing() && act.GetOutcome() == Act.Outcome.Success, $"Abort from OnPreExit wrongly overrode outcome! Outcome={act.GetOutcome()}");

        yield return null;
    }
    [UnityTest]
    public IEnumerator AbortFailsFromOnPostExit()
    {
        // Perform Act
        var act = new ManualFinishAct();
        act.OnPostExit += (a) => { act.Abort(); };
        act.Init("Test Act");
        act.Perform();

        act.ManualFinish(Act.Outcome.Success);


        // Assertions
        Assert.IsTrue(!act.IsOngoing() && act.GetOutcome() == Act.Outcome.Success, $"Abort from OnPostExit wrongly overrode outcome! Outcome={act.GetOutcome()}");

        yield return null;
    }
    [UnityTest]
    public IEnumerator AbortSucceedsFromOnPerformEnd()
    {
        // Perform Act
        var mainAct = new Act();
        mainAct.OnPerformEnd += (a) => { mainAct.Abort(); };
        mainAct.Init("Main Act");

        mainAct.Perform();


        // Assertions
        Assert.IsTrue(!mainAct.IsOngoing() && mainAct.GetOutcome() == Act.Outcome.Success, $"Abort from OnPerformEnd overrode outcome! IsOngoing={mainAct.IsOngoing()}  Outcome={mainAct.GetOutcome()}");

        yield return null;
    }
    [UnityTest]
    public IEnumerator AbortFailsFromOnPreCleanup()
    {
        // Perform Act
        var act = new Act();
        act.OnPreCleanup += (a) => { act.Abort(); };
        act.Init("Test Act");
        act.Deinit();


        // Assertions
        Assert.IsTrue(!act.IsOngoing(), $"Abort from OnPreCleanup wronglymade act ongoing! Ongoing={act.IsOngoing()}");

        yield return null;
    }
    [UnityTest]
    public IEnumerator AbortFailsFromOnPostCleanup()
    {
        // Perform Act
        var act = new Act();
        act.OnPostCleanup += (a) => { act.Abort(); };
        act.Init("Test Act");
        act.Deinit();


        // Assertions
        Assert.IsTrue(!act.IsOngoing(), $"Abort from OnPostCleanup wrongly made act ongoing! Ongoing={act.IsOngoing()}");

        yield return null;
    }
    [UnityTest]
    public IEnumerator AbortSucceedsFromOnEnableChanged()
    {
        // Perform Act
        var act = new Act();
        act.OnEnableChanged += (a, e) => { act.Abort(); };
        act.Init("Test Act");

        act.SetEnabled(false);
        bool IsOngoingAfterDisable = act.IsOngoing();
        act.SetEnabled(true);
        bool IsOngoingAfterEnable = act.IsOngoing();


        // Assertions
        Assert.IsTrue(!IsOngoingAfterDisable, $"Abort from OnEnableChanged false made act ongoing! IsOngoingAfterDisable={IsOngoingAfterDisable}");
        Assert.IsTrue(!IsOngoingAfterEnable, $"Abort from OnEnableChanged true made act ongoing! IsOngoingAfterEnable={IsOngoingAfterEnable}");


        yield return null;
    }
    [UnityTest]
    public IEnumerator AbortSucceedsFromOnBlockChanged()
    {
        var blockedAct = new WaitInfiniAct();
        blockedAct.OnBlockChanged += (a, byAct, blockType, didBlock) => { blockedAct.Abort(); };
        blockedAct.Init("Blocked Act");
        blockedAct.Perform();

        var blockerAct = new WaitInfiniAct();
        blockerAct.AddToBlock(new List<Act> { blockedAct });
        blockerAct.Init("Blocker Act");

        blockerAct.Perform();


        // Assertions
        Assert.IsTrue(!blockedAct.IsOngoing(), $"Abort from OnBlockChanged made act ongoing! IsOngoing={blockedAct.IsOngoing()}");

        yield return null;
    }
}
