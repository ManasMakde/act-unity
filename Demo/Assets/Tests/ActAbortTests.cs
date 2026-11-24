using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;


// 1. Does a performing act stop when Abort() is invoked?
// 2. Does abort fail from OnPreSetup?
// 3. Does abort fail from OnPostSetup?
// 4. Does abort succeed from OnPerformStart?
// 5. Does abort succeed from OnPrePrologue?
// 6. Does abort succeed from OnPrologueComplete?
// 7. Does abort succeed from OnPostPrologue?
// 8. Does abort succeed from OnPreEnter?
// 9. Does abort succeed from OnPostEnter?
// 10. Does abort succeed from OnPreTick?
// 11. Does abort succeed from OnPostTick?
// 12. Does abort succeed from OnPrePhysicsTick?
// 13. Does abort succeed from OnPostPhysicsTick?
// 14. Does abort succeed from OnPreLateTick?
// 15. Does abort succeed from OnPostLateTick?
// 16. Does abort fail from OnPreExit?
// 17. Does abort fail from OnPostExit?
// 18. Does abort succeed from OnPerformEnd?
// 19. Does abort fail from OnPreCleanup?
// 20. Does abort fail from OnPostCleanup?
// 21. Does abort succeed from OnEnableChanged?
// 22. Does abort succeed from OnBlockChanged?


public class ActAbortTests
{
    [UnityTest]
    public IEnumerator AbortStopsPerformingAct()  // 1
    {
        // Perform Act
        var act = new ManualFinishAct();
        act.Init("Test Act");
        act.Perform();

        var wasPerforming = act.IsPerforming();

        act.Abort();


        // Assertions
        Assert.IsTrue(wasPerforming, "Act was not performing before Abort()!");
        Assert.IsTrue(!act.IsPerforming() && act.GetOutcome() == Act.Outcome.Interrupted, $"Act did not stop on Abort()! IsPerforming={act.IsPerforming()}  Outcome={act.GetOutcome()}");

        yield return null;
    }
    [UnityTest]
    public IEnumerator AbortFailsFromOnPreSetup()  // 2
    {
        // Perform Act
        var act = new ExitAct();
        act.OnPreSetup += (a) => { act.Abort(); };
        act.Init("Test Act");


        // Assertions
        Assert.IsTrue(act.callCount == 0 && !act.IsPerforming(), $"Abort from OnPreSetup calling Exit()! callCount={act.callCount}  Outcome={act.GetOutcome()}");

        yield return null;
    }
    [UnityTest]
    public IEnumerator AbortFailsFromOnPostSetup()  // 3
    {
        // Perform Act
        var act = new ExitAct();
        act.OnPostSetup += (a) => { act.Abort(); };
        act.Init("Test Act");


        // Assertions
        Assert.IsTrue(act.callCount == 0 && !act.IsPerforming(), $"Abort from OnPostSetup calling Exit()! callCount={act.callCount}  Outcome={act.GetOutcome()}");

        yield return null;
    }
    [UnityTest]
    public IEnumerator AbortSucceedsFromOnPerformStart()  // 4
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
    public IEnumerator AbortSucceedsFromOnPrePrologue()  // 5
    {
        // Perform Act
        var prologueAct = new Act();
        prologueAct.Init("Prologue Act");

        var mainAct = new EnterAct();
        mainAct.OnPrePrologue += (a) => { mainAct.Abort(); };
        mainAct.prologue = (a) => new() { new() { prologueAct } };
        mainAct.Init("Main Act");

        mainAct.Perform();


        // Assertions
        Assert.IsTrue(prologueAct.GetPerformCount() == 0 && mainAct.callCount == 0 && mainAct.GetOutcome() == Act.Outcome.Interrupted, $"Abort from OnPrePrologue did not cut prologue short! prologueCount={prologueAct.GetPerformCount()}  callCount={mainAct.callCount}  Outcome={mainAct.GetOutcome()}");

        yield return null;
    }
    [UnityTest]
    public IEnumerator AbortSucceedsFromOnPrologueComplete()  // 6
    {
        // Perform Act
        var prologueAct = new Act();
        prologueAct.Init("Prologue Act");

        var mainAct = new EnterAct();
        mainAct.prologue = (a) => new() { new() { prologueAct } };
        mainAct.OnPrologueComplete += (a, pAct, outcome) => { mainAct.Abort(); };
        mainAct.Init("Main Act");

        mainAct.Perform();


        // Assertions
        Assert.IsTrue(mainAct.callCount == 0 && mainAct.GetOutcome() == Act.Outcome.Interrupted, $"Abort from OnPrologueComplete did not cut perform short! callCount={mainAct.callCount}  Outcome={mainAct.GetOutcome()}");

        yield return null;
    }
    [UnityTest]
    public IEnumerator AbortSucceedsFromOnPostPrologue()  // 7
    {
        // Perform Act
        var prologueAct = new Act();
        prologueAct.Init("Prologue Act");

        var mainAct = new EnterAct();
        mainAct.prologue = (a) => new() { new() { prologueAct } };
        mainAct.OnPostPrologue += (a) => { mainAct.Abort(); };
        mainAct.Init("Main Act");

        mainAct.Perform();


        // Assertions
        Assert.IsTrue(mainAct.callCount == 0 && mainAct.GetOutcome() == Act.Outcome.Interrupted, $"Abort from OnPostPrologue did not cut perform short! callCount={mainAct.callCount}  Outcome={mainAct.GetOutcome()}");

        yield return null;
    }
    [UnityTest]
    public IEnumerator AbortSucceedsFromOnPreEnter()  // 8
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
    public IEnumerator AbortSucceedsFromOnPostEnter()  // 9
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
    public IEnumerator AbortSucceedsFromOnPreTick()  // 10
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
    public IEnumerator AbortSucceedsFromOnPostTick()  // 11
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
    public IEnumerator AbortSucceedsFromOnPrePhysicsTick()  // 12
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
    public IEnumerator AbortSucceedsFromOnPostPhysicsTick()  // 13
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
    public IEnumerator AbortSucceedsFromOnPreLateTick()  // 14
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
    public IEnumerator AbortSucceedsFromOnPostLateTick()  // 15
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
    public IEnumerator AbortFailsFromOnPreExit()  // 16
    {
        // Perform Act
        var act = new ManualFinishAct();
        act.OnPreExit += (a) => { act.Abort(); };
        act.Init("Test Act");
        act.Perform();

        act.ManualFinish(Act.Outcome.Success);


        // Assertions
        Assert.IsTrue(!act.IsPerforming() && act.GetOutcome() == Act.Outcome.Success, $"Abort from OnPreExit wrongly overrode outcome! Outcome={act.GetOutcome()}");

        yield return null;
    }
    [UnityTest]
    public IEnumerator AbortFailsFromOnPostExit()  // 17
    {
        // Perform Act
        var act = new ManualFinishAct();
        act.OnPostExit += (a) => { act.Abort(); };
        act.Init("Test Act");
        act.Perform();

        act.ManualFinish(Act.Outcome.Success);


        // Assertions
        Assert.IsTrue(!act.IsPerforming() && act.GetOutcome() == Act.Outcome.Success, $"Abort from OnPostExit wrongly overrode outcome! Outcome={act.GetOutcome()}");

        yield return null;
    }
    [UnityTest]
    public IEnumerator AbortSucceedsFromOnPerformEnd()  // 18
    {
        // Perform Act
        var mainAct = new Act();
        mainAct.OnPerformEnd += (a) => { mainAct.Abort(); };
        mainAct.Init("Main Act");

        mainAct.Perform();


        // Assertions
        Assert.IsTrue(!mainAct.IsPerforming() && mainAct.GetOutcome() == Act.Outcome.Success, $"Abort from OnPerformEnd overrode outcome! IsPerforming={mainAct.IsPerforming()}  Outcome={mainAct.GetOutcome()}");

        yield return null;
    }
    [UnityTest]
    public IEnumerator AbortFailsFromOnPreCleanup()  // 19
    {
        // Perform Act
        var act = new Act();
        act.OnPreCleanup += (a) => { act.Abort(); };
        act.Init("Test Act");
        act.Deinit();


        // Assertions
        Assert.IsTrue(!act.IsPerforming(), $"Abort from OnPreCleanup wronglymade act performing! Performing={act.IsPerforming()}");

        yield return null;
    }
    [UnityTest]
    public IEnumerator AbortFailsFromOnPostCleanup()  // 20
    {
        // Perform Act
        var act = new Act();
        act.OnPostCleanup += (a) => { act.Abort(); };
        act.Init("Test Act");
        act.Deinit();


        // Assertions
        Assert.IsTrue(!act.IsPerforming(), $"Abort from OnPostCleanup wrongly made act performing! Performing={act.IsPerforming()}");

        yield return null;
    }
    [UnityTest]
    public IEnumerator AbortSucceedsFromOnEnableChanged()  // 21
    {
        // Perform Act
        var act = new Act();
        act.OnEnableChanged += (a, e) => { act.Abort(); };
        act.Init("Test Act");

        act.SetEnabled(false);
        bool IsPerformingAfterDisable = act.IsPerforming();
        act.SetEnabled(true);
        bool IsPerformingAfterEnable = act.IsPerforming();


        // Assertions
        Assert.IsTrue(!IsPerformingAfterDisable, $"Abort from OnEnableChanged false made act performing! IsPerformingAfterDisable={IsPerformingAfterDisable}");
        Assert.IsTrue(!IsPerformingAfterEnable, $"Abort from OnEnableChanged true made act performing! IsPerformingAfterEnable={IsPerformingAfterEnable}");


        yield return null;
    }
    [UnityTest]
    public IEnumerator AbortSucceedsFromOnBlockChanged()  // 22
    {
        var blockedAct = new WaitInfiniAct();
        blockedAct.OnBlockChanged += (a, byAct, blockType, didBlock) => { blockedAct.Abort(); };
        blockedAct.Init("Blocked Act");
        blockedAct.Perform();

        var blockerAct = new WaitInfiniAct();
        blockerAct.block = (Act a) => new() { { blockedAct, Act.BlockType.Persistent } };
        blockerAct.Init("Blocker Act");

        blockerAct.Perform();


        // Assertions
        Assert.IsTrue(!blockedAct.IsPerforming(), $"Abort from OnBlockChanged made act performing! IsPerforming={blockedAct.IsPerforming()}");

        yield return null;
    }
}
