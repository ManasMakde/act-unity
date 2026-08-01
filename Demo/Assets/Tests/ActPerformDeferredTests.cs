using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;


// 1. Does perform deferred work?
// 1. Does perform deferred fail without theater?
// 1. Does perform deferred not immediately perform the act?
// 1. Does act perform once when deferred twice?
// 1. Does perform deferred with tick flag as none do nothing?
// 1. Is perform deferred cleared upon performing immediately?
// 1. Is perform deferred cleared on aborting?

// 1. Does perform deferred succeed from OnPreSetup?
// 1. Does perform deferred succeed from OnPostSetup?
// 1. Does reperform deferred succeed from OnPerformStart?
// 1. Does reperform deferred succeed from OnPrePrologue?
// 1. Does reperform deferred succeed from OnPrologueComplete?
// 1. Does reperform deferred succeed from OnPostPrologue?
// 1. Does reperform deferred succeed from OnPreEnter?
// 1. Does reperform deferred succeed from OnPostEnter?
// 1. Does reperform deferred succeed from OnPreTick?
// 1. Does reperform deferred succeed from OnPostTick?
// 1. Does reperform deferred succeed from OnPrePhysicsTick?
// 1. Does reperform deferred succeed from OnPostPhysicsTick?
// 1. Does reperform deferred succeed from OnPreLateTick?
// 1. Does reperform deferred succeed from OnPostLateTick?
// 1. Does reperform deferred succeed from OnPreExit?
// 1. Does reperform deferred succeed from OnPostExit?
// 1. Does perform deferred succeed from OnPerformEnd?
// 1. Does reperform deferred fail from OnPreCleanup?
// 1. Does reperform deferred fail from OnPostCleanup?
// 1. Does perform deferred succeed from OnEnableChanged?
// 1. Does perform deferred succeed from OnBlockChanged?


public class ActPerformDeferredTests
{
    [UnityTest]
    public IEnumerator PerformDeferredWorks()
    {
        // Prerequisites
        var theater = new GameObject().AddComponent<Theater>();


        // Physics Tick Deferr Perform Act
        var physicsAct = new Act();
        physicsAct.Init("Test Physics Act", theater);
        physicsAct.PerformDeferred(Act.TickFlags.PhysicsTick);
        yield return new WaitForFixedUpdate();
        yield return null;


        // Tick Deferr Perform Act
        var tickAct = new Act();
        tickAct.Init("Test Tick Act", theater);
        tickAct.PerformDeferred(Act.TickFlags.Tick);
        yield return null;
        yield return null;


        // Late Tick Deferr Perform Act
        var lateTickAct = new Act();
        lateTickAct.Init("Test Late Tick Act", theater);
        lateTickAct.PerformDeferred(Act.TickFlags.LateTick);
        yield return null;
        yield return null;


        // Assertions
        Assert.IsTrue(physicsAct.GetPerformCount() == 1, "Deferred act did not perform after physics tick!");
        Assert.IsTrue(tickAct.GetPerformCount() == 1, "Deferred act did not perform after tick!");
        Assert.IsTrue(lateTickAct.GetPerformCount() == 1, "Deferred act did not perform after late tick!");


        UnityEngine.Object.Destroy(theater.gameObject);
    }
    [UnityTest]
    public IEnumerator PerformDeferWithoutTheater()
    {
        // Perform Act
        var act = new WaitInfiniAct();
        act.Init("Test Act");
        act.PerformDeferred();

        yield return null;
        yield return null;
        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();


        // Assertions
        Assert.IsTrue(!act.IsOngoing(), "Act is ongoing despite deferred performing without theater!");
        Assert.IsTrue(act.GetPerformCount() == 0, $"Act performed despite missing theater! Perform Count='{act.GetPerformCount()}'");

        yield return null;
    }
    [UnityTest]
    public IEnumerator PerformDeferredDoesNotImmediatelyPerform()
    {
        // Prerequisites
        var theater = new GameObject().AddComponent<Theater>();


        // Perform Act
        var act = new Act();
        act.Init("Test Act", theater);
        act.PerformDeferred(Act.TickFlags.PhysicsTick);


        // Assertions
        Assert.IsTrue(act.GetPerformCount() == 0, "Act performed immediately despite being deferred!");


        UnityEngine.Object.Destroy(theater.gameObject);
        yield return null;
    }
    [UnityTest]
    public IEnumerator PerformsOnceWhenDeferredTwice()
    {
        // Prerequisites
        var theater = new GameObject().AddComponent<Theater>();


        // Tick Deferred Perform
        int tickDeferredCount = 0;
        {
            var act = new ReperformableInfiAct();
            act.Init("Test Act", theater);
            act.PerformDeferred(Act.TickFlags.Tick);
            act.PerformDeferred(Act.TickFlags.Tick);
            yield return null;
            yield return null;

            tickDeferredCount = act.GetPerformCount();
        }


        // Physics Deferred Perform
        int physicsTickDeferredCount = 0;
        {
            var act = new ReperformableInfiAct();
            act.Init("Test Act", theater);
            act.PerformDeferred(Act.TickFlags.PhysicsTick);
            act.PerformDeferred(Act.TickFlags.PhysicsTick);
            yield return new WaitForFixedUpdate();
            yield return null;

            physicsTickDeferredCount = act.GetPerformCount();
        }


        // Late Tick Deferred Perform
        int lateTickDeferredCount = 0;
        {
            var act = new ReperformableInfiAct();
            act.Init("Test Act", theater);
            act.PerformDeferred(Act.TickFlags.LateTick);
            act.PerformDeferred(Act.TickFlags.LateTick);
            yield return null;
            yield return null;

            lateTickDeferredCount = act.GetPerformCount();
        }


        // Assertions
        Assert.IsTrue(tickDeferredCount == 1, $"Act did not perform once despite being deferred twice! Count={tickDeferredCount}");
        Assert.IsTrue(lateTickDeferredCount == 1, $"Act did not perform once despite being deferred twice! Count={lateTickDeferredCount}");
        Assert.IsTrue(physicsTickDeferredCount == 1, $"Act did not perform once despite being deferred twice! Count={physicsTickDeferredCount}");


        UnityEngine.Object.Destroy(theater.gameObject);
    }
    [UnityTest]
    public IEnumerator PerformDeferredWithNoneFlagDoesNothing()
    {
        // Prerequisites
        var theater = new GameObject().AddComponent<Theater>();


        // Perform Act
        var act = new Act();
        act.Init("Test Act", theater);
        act.PerformDeferred(Act.TickFlags.None);
        yield return new WaitForFixedUpdate();
        yield return null;


        // Assertions
        Assert.IsTrue(act.GetPerformCount() == 0, "Act performed despite tick flag being none!");


        UnityEngine.Object.Destroy(theater.gameObject);
    }
    [UnityTest]
    public IEnumerator PerformDeferredClearedUponPerformingImmediately()
    {
        // Prerequisites
        var theater = new GameObject().AddComponent<Theater>();


        // Perform Act
        var act = new Act();
        act.Init("Test Act", theater);
        act.PerformDeferred(Act.TickFlags.PhysicsTick);
        act.Perform();
        var performCountAfterImmediate = act.GetPerformCount();
        yield return new WaitForFixedUpdate();
        yield return null;
        var performCountAfterTick = act.GetPerformCount();


        // Assertions
        Assert.IsTrue(performCountAfterImmediate == 1, "Act did not perform immediately!");
        Assert.IsTrue(performCountAfterTick == 1, "Deferred perform was not cleared after performing immediately!");


        UnityEngine.Object.Destroy(theater.gameObject);
    }
    [UnityTest]
    public IEnumerator PerformDeferredClearedOnAbort()
    {
        // Prerequisites
        var theater = new GameObject().AddComponent<Theater>();


        // Perform Act
        var act = new Act();
        act.Init("Test Act", theater);
        act.PerformDeferred();
        act.Abort();
        yield return new WaitForFixedUpdate();
        yield return null;


        // Assertions
        Assert.IsTrue(act.GetPerformCount() == 0, "Act deferred performed despite being aborted!");


        UnityEngine.Object.Destroy(theater.gameObject);
    }



    [UnityTest]
    public IEnumerator PerformDeferredFromOnPreSetup()
    {
        // Prerequisites
        var theater = new GameObject().AddComponent<Theater>();


        // Perform Act
        var act = new Act();
        act.OnPreSetup += (a) =>
        {
            a.PerformDeferred();
        };
        act.Init("Test Act", theater);

        yield return null;
        yield return new WaitForFixedUpdate();


        // Assertions
        Assert.IsTrue(act.GetPerformCount() == 1, $"Act did not perform deferred from OnPreSetup! Perform Count={act.GetPerformCount()}");


        UnityEngine.Object.Destroy(theater.gameObject);
        yield return null;
    }
    [UnityTest]
    public IEnumerator PerformDeferredFromOnPostSetup()
    {
        // Prerequisites
        var theater = new GameObject().AddComponent<Theater>();


        // Perform Act
        var act = new Act();
        act.OnPostSetup += (a) =>
        {
            a.PerformDeferred();
        };
        act.Init("Test Act", theater);

        yield return null;
        yield return new WaitForFixedUpdate();


        // Assertions
        Assert.IsTrue(act.GetPerformCount() == 1, $"Act did not perform deferred from OnPostSetup! Perform Count={act.GetPerformCount()}");


        UnityEngine.Object.Destroy(theater.gameObject);
        yield return null;
    }
    [UnityTest]
    public IEnumerator PerformDeferredFromOnPerformStart()
    {
        // Prerequisites
        var theater = new GameObject().AddComponent<Theater>();


        // Perform Act
        var act = new Act();
        act.OnPerformStart += (a) =>
        {
            if (act.GetPerformCount() <= 2)
            {
                a.PerformDeferred();
            }
        };
        act.Init("Test Act", theater);
        act.Perform();


        yield return new WaitForFixedUpdate();
        yield return null;

        yield return new WaitForFixedUpdate();
        yield return null;

        yield return new WaitForFixedUpdate();
        yield return null;


        // Assertions
        Assert.IsTrue(act.GetPerformCount() == 3, $"Act did not reperform deferred thrice from OnPerformStart! Perform Count={act.GetPerformCount()}");

        UnityEngine.Object.Destroy(theater.gameObject);
    }
    [UnityTest]
    public IEnumerator PerformDeferredFromOnPrePrologue()
    {
        // Prerequisites
        var theater = new GameObject().AddComponent<Theater>();


        // Prologue act so the prologue signal actually fires
        var prologueAct = new ReperformableAct();
        prologueAct.Init("Prologue Act");


        // Perform Act
        var act = new ReperformableAct();
        act.prologue = (a) => new List<Act> { prologueAct };
        act.OnPrePrologue += (a) =>
        {
            if (act.GetPerformCount() <= 2)
            {
                a.PerformDeferred();
            }
        };
        act.Init("Test Act", theater);
        act.Perform();


        yield return new WaitForFixedUpdate();
        yield return null;

        yield return new WaitForFixedUpdate();
        yield return null;

        yield return new WaitForFixedUpdate();
        yield return null;


        // Assertions
        Assert.IsTrue(act.GetPerformCount() == 3, $"Act did not reperform deferred thrice from OnPrePrologue! Perform Count={act.GetPerformCount()}");


        UnityEngine.Object.Destroy(theater.gameObject);
    }
    [UnityTest]
    public IEnumerator PerformDeferredFromOnPrologueComplete()
    {
        // Prerequisites
        var theater = new GameObject().AddComponent<Theater>();


        // Prologue act
        var prologue1Act = new ReperformableAct();
        prologue1Act.Init("Prologue 1 Act");

        var prologue2Act = new ReperformableAct();
        prologue2Act.Init("Prologue 2 Act");


        // Perform Act
        var act = new ReperformableAct();
        act.prologue = (a) => new List<Act> { prologue1Act, prologue2Act };
        act.OnPrologueComplete += (a, pA, pO) =>
        {
            if (pO != Act.Outcome.Success)
            {
                return;
            }

            if (act.GetPerformCount() <= 2)
            {
                act.PerformDeferred();
            }
        };
        act.Init("Test Act", theater);
        act.Perform();


        yield return new WaitForFixedUpdate();
        yield return null;

        yield return new WaitForFixedUpdate();
        yield return null;

        yield return new WaitForFixedUpdate();
        yield return null;


        // Assertions
        Assert.IsTrue(act.GetPerformCount() == 3, $"Act did not reperform deferred thrice from OnPrePrologue! Perform Count={act.GetPerformCount()}");


        UnityEngine.Object.Destroy(theater.gameObject);
    }
    [UnityTest]
    public IEnumerator PerformDeferredFromOnPostPrologue()
    {
        // Prerequisites
        var theater = new GameObject().AddComponent<Theater>();


        // Prologue act so the prologue signal actually fires
        var prologueAct = new ReperformableAct();
        prologueAct.Init("Prologue Act");


        // Perform Act
        var act = new ReperformableAct();
        act.prologue = (a) => new List<Act> { prologueAct };
        act.OnPostPrologue += (a) =>
        {
            if (act.GetPerformCount() <= 2)
            {
                act.PerformDeferred();
            }
        };
        act.Init("Test Act", theater);
        act.Perform();


        yield return new WaitForFixedUpdate();
        yield return null;

        yield return new WaitForFixedUpdate();
        yield return null;

        yield return new WaitForFixedUpdate();
        yield return null;


        // Assertions
        Assert.IsTrue(act.GetPerformCount() == 3, $"Act did not reperform deferred thrice from OnPostPrologue! Perform Count={act.GetPerformCount()}");


        UnityEngine.Object.Destroy(theater.gameObject);
    }
    [UnityTest]
    public IEnumerator PerformDeferredFromOnPreEnter()
    {
        // Prerequisites
        var theater = new GameObject().AddComponent<Theater>();


        // Perform Act
        var act = new ReperformableAct();
        act.OnPreEnter += (a) =>
        {
            if (act.GetPerformCount() <= 2)
            {
                a.PerformDeferred();
            }
        };
        act.Init("Test Act", theater);
        act.Perform();


        yield return new WaitForFixedUpdate();
        yield return null;

        yield return new WaitForFixedUpdate();
        yield return null;

        yield return new WaitForFixedUpdate();
        yield return null;


        // Assertions
        Assert.IsTrue(act.GetPerformCount() == 3, $"Act did not reperform deferred thrice from OnPreEnter! Perform Count={act.GetPerformCount()}");


        UnityEngine.Object.Destroy(theater.gameObject);
    }
    [UnityTest]
    public IEnumerator PerformDeferredFromOnPostEnter()
    {
        // Prerequisites
        var theater = new GameObject().AddComponent<Theater>();


        // Perform Act
        var act = new ReperformableAct();
        act.OnPostEnter += (a) =>
        {
            if (act.GetPerformCount() <= 2)
            {
                a.PerformDeferred();
            }
        };
        act.Init("Test Act", theater);
        act.Perform();


        yield return new WaitForFixedUpdate();
        yield return null;

        yield return new WaitForFixedUpdate();
        yield return null;

        yield return new WaitForFixedUpdate();
        yield return null;


        // Assertions
        Assert.IsTrue(act.GetPerformCount() == 3, $"Act did not reperform deferred thrice from OnPostEnter! Perform Count={act.GetPerformCount()}");


        UnityEngine.Object.Destroy(theater.gameObject);
    }
    [UnityTest]
    public IEnumerator PerformDeferredFromOnPreTick()
    {
        // Real theater needed to drive ticks
        var theaterGO = new GameObject("TestTheater");
        var theater = theaterGO.AddComponent<Theater>();


        // Perform Act
        var act = new ReperformableInfiAct();
        act.overrideTickFlag = Act.TickFlags.Tick;
        act.OnPreTick += (a) =>
        {
            if (act.GetPerformCount() <= 2)
            {
                a.PerformDeferred(Act.TickFlags.Tick);
            }
        };
        act.Init("Test Act", theater);
        act.Perform();


        yield return new WaitForFixedUpdate();
        yield return null;

        yield return new WaitForFixedUpdate();
        yield return null;

        yield return new WaitForFixedUpdate();
        yield return null;


        // Assertions
        Assert.IsTrue(act.GetPerformCount() == 3, $"Act did not reperform deferred thrice from OnPreTick! Perform Count={act.GetPerformCount()}");


        // Cleanup
        Object.Destroy(theaterGO);
    }
    [UnityTest]
    public IEnumerator PerformDeferredFromOnPostTick()
    {
        // Real theater needed to drive ticks
        var theaterGO = new GameObject("TestTheater");
        var theater = theaterGO.AddComponent<Theater>();


        // Perform Act
        var act = new ReperformableInfiAct();
        act.overrideTickFlag = Act.TickFlags.Tick;
        act.OnPostTick += (a) =>
        {
            if (act.GetPerformCount() <= 2)
            {
                a.PerformDeferred(Act.TickFlags.Tick);
            }
        };
        act.Init("Test Act", theater);
        act.Perform();


        yield return new WaitForFixedUpdate();
        yield return null;

        yield return new WaitForFixedUpdate();
        yield return null;

        yield return new WaitForFixedUpdate();
        yield return null;


        // Assertions
        Assert.IsTrue(act.GetPerformCount() == 3, $"Act did not reperform deferred thrice from OnPostTick! Perform Count={act.GetPerformCount()}");


        // Cleanup
        Object.Destroy(theaterGO);
    }
    [UnityTest]
    public IEnumerator PerformDeferredFromOnPrePhysicsTick()
    {
        // Real theater needed to drive ticks
        var theaterGO = new GameObject("TestTheater");
        var theater = theaterGO.AddComponent<Theater>();


        // Perform Act
        var act = new ReperformableInfiAct();
        act.overrideTickFlag = Act.TickFlags.PhysicsTick;
        act.OnPrePhysicsTick += (a) =>
        {
            if (act.GetPerformCount() <= 2)
            {
                a.PerformDeferred(Act.TickFlags.PhysicsTick);
            }
        };
        act.Init("Test Act", theater);
        act.Perform();


        yield return new WaitForFixedUpdate();
        yield return null;

        yield return new WaitForFixedUpdate();
        yield return null;

        yield return new WaitForFixedUpdate();
        yield return null;


        // Assertions
        Assert.IsTrue(act.GetPerformCount() == 3, $"Act did not reperform deferred thrice from OnPrePhysicsTick! Perform Count={act.GetPerformCount()}");


        // Cleanup
        Object.Destroy(theaterGO);
    }
    [UnityTest]
    public IEnumerator PerformDeferredFromOnPostPhysicsTick()
    {
        // Real theater needed to drive ticks
        var theaterGO = new GameObject("TestTheater");
        var theater = theaterGO.AddComponent<Theater>();


        // Perform Act
        var act = new ReperformableInfiAct();
        act.overrideTickFlag = Act.TickFlags.PhysicsTick;
        act.OnPostPhysicsTick += (a) =>
        {
            if (act.GetPerformCount() <= 2)
            {
                a.PerformDeferred(Act.TickFlags.PhysicsTick);
            }
        };
        act.Init("Test Act", theater);
        act.Perform();


        yield return new WaitForFixedUpdate();
        yield return null;

        yield return new WaitForFixedUpdate();
        yield return null;

        yield return new WaitForFixedUpdate();
        yield return null;


        // Assertions
        Assert.IsTrue(act.GetPerformCount() == 3, $"Act did not reperform deferred thrice from OnPostPhysicsTick! Perform Count={act.GetPerformCount()}");


        // Cleanup
        Object.Destroy(theaterGO);
    }
    [UnityTest]
    public IEnumerator PerformDeferredFromOnPreLateTick()
    {
        // Real theater needed to drive ticks
        var theaterGO = new GameObject("TestTheater");
        var theater = theaterGO.AddComponent<Theater>();


        // Perform Act
        var act = new ReperformableInfiAct();
        act.overrideTickFlag = Act.TickFlags.LateTick;
        act.OnPreLateTick += (a) =>
        {
            if (act.GetPerformCount() <= 2)
            {
                a.PerformDeferred(Act.TickFlags.LateTick);
            }
        };
        act.Init("Test Act", theater);
        act.Perform();


        yield return new WaitForFixedUpdate();
        yield return null;

        yield return new WaitForFixedUpdate();
        yield return null;

        yield return new WaitForFixedUpdate();
        yield return null;


        // Assertions
        Assert.IsTrue(act.GetPerformCount() == 3, $"Act did not reperform deferred thrice from OnPreLateTick! Perform Count={act.GetPerformCount()}");


        // Cleanup
        Object.Destroy(theaterGO);
    }
    [UnityTest]
    public IEnumerator PerformDeferredFromOnPostLateTick()
    {
        // Real theater needed to drive ticks
        var theaterGO = new GameObject("TestTheater");
        var theater = theaterGO.AddComponent<Theater>();


        // Perform Act
        var act = new ReperformableInfiAct();
        act.overrideTickFlag = Act.TickFlags.LateTick;
        act.OnPostLateTick += (a) =>
        {
            if (act.GetPerformCount() <= 2)
            {
                a.PerformDeferred(Act.TickFlags.LateTick);
            }
        };
        act.Init("Test Act", theater);
        act.Perform();


        yield return new WaitForFixedUpdate();
        yield return null;

        yield return new WaitForFixedUpdate();
        yield return null;

        yield return new WaitForFixedUpdate();
        yield return null;


        // Assertions
        Assert.IsTrue(act.GetPerformCount() == 3, $"Act did not reperform deferred thrice from OnPostLateTick! Perform Count={act.GetPerformCount()}");


        // Cleanup
        Object.Destroy(theaterGO);
    }
    [UnityTest]
    public IEnumerator PerformDeferredFromOnPreExit()
    {
        // Prerequisites
        var theater = new GameObject().AddComponent<Theater>();


        // Perform Act
        var act = new ReperformableAct();
        act.OnPreExit += (a) =>
        {
            if (act.GetPerformCount() <= 2)
            {
                a.PerformDeferred();
            }
        };
        act.Init("Test Act", theater);
        act.Perform();


        yield return new WaitForFixedUpdate();
        yield return null;

        yield return new WaitForFixedUpdate();
        yield return null;

        yield return new WaitForFixedUpdate();
        yield return null;


        // Assertions
        Assert.IsTrue(act.GetPerformCount() == 3, $"Act did not reperform deferred thrice from OnPreExit! Perform Count={act.GetPerformCount()}");


        UnityEngine.Object.Destroy(theater.gameObject);
    }
    [UnityTest]
    public IEnumerator PerformDeferredFromOnPostExit()
    {
        // Prerequisites
        var theater = new GameObject().AddComponent<Theater>();


        // Perform Act
        var act = new ReperformableAct();
        act.OnPostExit += (a) =>
        {
            if (act.GetPerformCount() <= 2)
            {
                a.PerformDeferred();
            }
        };
        act.Init("Test Act", theater);
        act.Perform();


        yield return new WaitForFixedUpdate();
        yield return null;

        yield return new WaitForFixedUpdate();
        yield return null;

        yield return new WaitForFixedUpdate();
        yield return null;


        // Assertions
        Assert.IsTrue(act.GetPerformCount() == 3, $"Act did not reperform deferred thrice from OnPostExit! Perform Count={act.GetPerformCount()}");


        UnityEngine.Object.Destroy(theater.gameObject);
    }
    [UnityTest]
    public IEnumerator PerformDeferredFromOnPerformEnd()
    {
        // Prerequisites
        var theater = new GameObject().AddComponent<Theater>();


        // Perform Act
        var act = new ReperformableAct();
        act.OnPerformEnd += (a) =>
        {
            if (act.GetPerformCount() <= 2)
            {
                a.PerformDeferred();
            }
        };
        act.Init("Test Act", theater);
        act.Perform();


        yield return new WaitForFixedUpdate();
        yield return null;

        yield return new WaitForFixedUpdate();
        yield return null;

        yield return new WaitForFixedUpdate();
        yield return null;


        // Assertions
        Assert.IsTrue(act.GetPerformCount() == 3, $"Act did not reperform deferred thrice from OnPerformEnd! Perform Count={act.GetPerformCount()}");

        UnityEngine.Object.Destroy(theater.gameObject);
    }
    [UnityTest]
    public IEnumerator PerformDeferredFromOnPreCleanup()
    {
        // Prerequisites
        var theater = new GameObject().AddComponent<Theater>();


        // Perform Act
        var act = new ReperformableAct();
        act.OnPreCleanup += (a) =>
        {
            a.PerformDeferred();
        };
        act.Init("Test Act", theater);
        act.Deinit();

        yield return new WaitForFixedUpdate();
        yield return null;


        // Assertions
        Assert.IsTrue(act.GetPerformCount() == 0, $"Act performed from OnPreCleanup! Perform Count={act.GetPerformCount()}");

        UnityEngine.Object.Destroy(theater.gameObject);
    }
    [UnityTest]
    public IEnumerator PerformDeferredFromOnPostCleanup()
    {
        // Prerequisites
        var theater = new GameObject().AddComponent<Theater>();


        // Perform Act
        var act = new ReperformableAct();
        act.OnPostCleanup += (a) =>
        {
            a.PerformDeferred();
        };
        act.Init("Test Act", theater);
        act.Deinit();


        yield return new WaitForFixedUpdate();
        yield return null;


        // Assertions
        Assert.IsTrue(act.GetPerformCount() == 0, $"Act performed from OnPostCleanup! Perform Count={act.GetPerformCount()}");

        UnityEngine.Object.Destroy(theater.gameObject);
    }
    [UnityTest]
    public IEnumerator PerformDeferredFromOnEnableChanged()
    {
        // Prerequisites
        var theater = new GameObject().AddComponent<Theater>();


        // Perform Act
        var act = new Act();
        act.OnEnableChanged += (a, newEnabled) =>
        {
            if (act.GetPerformCount() <= 2)
            {
                a.PerformDeferred();
            }
        };
        act.Init("Test Act", theater);


        // Toggle with a wait between each, so all deferred performs should happen before next toggle
        act.SetEnabled(false);
        yield return new WaitForFixedUpdate();
        yield return null;
        act.SetEnabled(true);
        yield return new WaitForFixedUpdate();
        yield return null;
        act.SetEnabled(false);
        yield return new WaitForFixedUpdate();
        yield return null;
        act.SetEnabled(true);
        yield return new WaitForFixedUpdate();
        yield return null;
        act.SetEnabled(false);
        yield return new WaitForFixedUpdate();
        yield return null;
        act.SetEnabled(true);
        yield return new WaitForFixedUpdate();
        yield return null;


        // Assertions
        Assert.IsTrue(act.GetPerformCount() == 3, $"Act did not reperform deferred thrice from OnEnableChanged! Perform Count={act.GetPerformCount()}");


        UnityEngine.Object.Destroy(theater.gameObject);
    }
    [UnityTest]
    public IEnumerator PerformDeferredFromOnBlockChanged()
    {
        // Prerequisites
        var theater = new GameObject().AddComponent<Theater>();


        // Perform Act
        var act = new Act();
        act.OnBlockChanged += (a, byAct, blockType, didBlock) =>
        {
            if (act.GetPerformCount() <= 2)
            {
                a.PerformDeferred();
            }
        };
        act.Init("Test Act", theater);


        var blocker = new ReperformableAct();
        blocker.Init("Blocker Act");
        blocker.AddToBlock(new List<Act> { act });


        // Toggle with a wait between each, so all deferred performs should happen before next toggle
        blocker.Perform();
        yield return new WaitForFixedUpdate();
        yield return null;
        blocker.Abort();
        yield return new WaitForFixedUpdate();
        yield return null;
        blocker.Perform();
        yield return new WaitForFixedUpdate();
        yield return null;
        blocker.Abort();
        yield return new WaitForFixedUpdate();
        yield return null;
        blocker.Perform();
        yield return new WaitForFixedUpdate();
        yield return null;
        blocker.Abort();
        yield return new WaitForFixedUpdate();
        yield return null;


        // Assertions
        Assert.IsTrue(act.GetPerformCount() == 3, $"Act did not reperform deferred thrice from OnBlockChanged! Perform Count={act.GetPerformCount()}");


        UnityEngine.Object.Destroy(theater.gameObject);
    }
}
