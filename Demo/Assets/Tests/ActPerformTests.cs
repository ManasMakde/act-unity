using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;


// 1. Are "perform start" & "perform end" actions being broadcasted (with correct arguments)?

// 1. Does perform fail when act disabled?
// 1. Does perform fail when theater disabled?
// 1. Does perform fail when act blocked?
// 1. Does perform fail when already ongoing and cannot reperform?
// 1. Does perform fail when external condition is false?
// 1. Does perform fail when overriden CanPerform() is false?
// 1. Does perform fail when called while exiting?

// 1. Does perform succeed when act enabled?
// 1. Does perform succeed when theater enabled?
// 1. Does perform succeed when already ongoing and can reperform?
// 1. Does perform succeed when external condition is true?
// 1. Does perform succeed when overriden CanPerform() is true?

// 1. Does _canReperform work correctly?

// 1. Does perform fail from OnPreSetup?
// 1. Does perform fail from OnPostSetup?
// 1. Does reperform succeed from OnPerformStart?
// 1. Does perform succeed from OnPerformEnd?
// 1. Does reperform succeed from OnPrePrologue?
// 1. Does reperform fail from OnPostPrologue?
// 1. Does reperform succeed from OnPreEnter?
// 1. Does reperform succeed from OnPostEnter?
// 1. Does reperform succeed from OnPreTick?
// 1. Does reperform succeed from OnPostTick?
// 1. Does reperform succeed from OnPrePhysicsTick?
// 1. Does reperform succeed from OnPostPhysicsTick?
// 1. Does reperform succeed from OnPreLateTick?
// 1. Does reperform succeed from OnPostLateTick?
// 1. Does reperform fail from OnPreExit?
// 1. Does reperform fail from OnPostExit?
// 1. Does reperform fail from OnPreCleanup?
// 1. Does reperform fail from OnPostCleanup?
// 1. Does perform succeed from OnEnableChanged?
// 1. Does perform succeed from OnBlockChanged?


public class ActPerformTests
{
    [UnityTest]
    public IEnumerator OnPerformStartAndEnd()
    {
        // Prerequisites
        bool wasStartInvoked = false;
        Act startArg1 = null;
        bool wasEndInvoked = false;
        Act endArg1 = null;


        // Perform Act
        var act = new Act();
        act.OnPerformStart += (a) => { wasStartInvoked = true; startArg1 = a; };
        act.OnPerformEnd += (a) => { wasEndInvoked = true; endArg1 = a; };
        act.Init("Test Act");
        act.Perform();


        // Assertions
        Assert.IsTrue(wasStartInvoked, "OnPerformStart not invoked!");
        Assert.IsTrue(startArg1 == act, $"OnPerformStart first argument is invalid! Arg1=`{startArg1}`");
        Assert.IsTrue(wasEndInvoked, "OnPerformEnd not invoked!");
        Assert.IsTrue(endArg1 == act, $"OnPerformEnd first argument is invalid! Arg1=`{endArg1}`");


        yield return null;
    }



    [UnityTest]
    public IEnumerator PerformWhenDisabled()
    {
        // Perform Act
        var act = new Act();
        act.Init("Test Act", null);

        bool wasEnabled = act.IsEnabled();
        act.SetEnabled(false);
        bool WasDisabled = !act.IsEnabled();
        act.SetEnabled(true);
        bool wasReEnabled = act.IsEnabled();


        // Assertions
        Assert.IsTrue(wasEnabled, "Act not enabled by default!");
        Assert.IsTrue(WasDisabled, "Act disabling failed!");
        Assert.IsTrue(wasReEnabled, "Act re-enabling after disabling failed!");


        yield return null;
    }
    [UnityTest]
    public IEnumerator PerformWhenTheaterDisabled()
    {
        // Prerequisites
        var theater = new GameObject().AddComponent<Theater>();
        theater.SetEnabled(false);


        // Perform Act
        var act = new WaitInfiniAct();
        act.Init("Test Act", theater);
        act.Perform();


        // Assertions
        Assert.IsFalse(act.IsOngoing(), "Act performed despite theater being disabled!");


        UnityEngine.Object.Destroy(theater.gameObject);
        yield return null;
    }
    [UnityTest]
    public IEnumerator PerformWhenBlocked()
    {
        // Perform First Block Later
        bool isOngoing_PFBL = false;
        bool isBlocked_PFBL = false;
        {
            var act = new WaitInfiniAct();
            act.Init("Test Act");
            act.Perform();

            var blockingAct = new WaitInfiniAct();
            blockingAct.Init("Blocking Act");
            blockingAct.AddToBlock(new() { act });
            blockingAct.Perform();

            isOngoing_PFBL = act.IsOngoing();
            isBlocked_PFBL = act.IsBlocked();
        }


        // Block First Perform Later
        bool isOngoing_BFPL = false;
        bool isBlocked_BFPL = false;
        {
            var act = new WaitInfiniAct();
            act.Init("Test Act");

            var blockingAct = new WaitInfiniAct();
            blockingAct.Init("Blocking Act");
            blockingAct.AddToBlock(new List<Act> { act });
            blockingAct.Perform();

            act.Perform();


            isOngoing_BFPL = act.IsOngoing();
            isBlocked_BFPL = act.IsBlocked();
        }


        // Assertions
        Assert.IsTrue(!isOngoing_PFBL, "Act is ongoing despite being blocked (Perform First Block Later)!");
        Assert.IsTrue(isBlocked_PFBL, "Act is unblocked despite being blocked (Perform First Block Later)!");
        Assert.IsTrue(!isOngoing_BFPL, "Act is ongoing despite being blocked (Block First Perform Later)!");
        Assert.IsTrue(isBlocked_BFPL, "Act is unblocked despite being blocked (Block First Perform Later)!");

        yield return null;
    }
    [UnityTest]
    public IEnumerator PerformWhenCannotReperform()
    {
        // Perform Act
        var act = new NonReperformableInfiAct();
        var enterCount = 0;
        act.OnPreEnter += (a) =>
        {
            enterCount++;
        };
        act.Init("Test Act");
        act.Perform();
        act.Perform();


        // Assertions
        Assert.IsTrue(enterCount == 1, "Act reperformed despite _canReperform being false!");


        yield return null;
    }
    [UnityTest]
    public IEnumerator PerformWhenExternalConditionFalse()
    {
        // Perform Act
        var act = new WaitInfiniAct();
        act.performConditions.Add((a) => false);
        act.Init("Test Act");
        act.Perform();


        // Assertions
        Assert.IsTrue(!act.IsOngoing(), "Act performed despite external condition being false!");


        yield return null;
    }
    [UnityTest]
    public IEnumerator PerformWhenCanPerformFalse()
    {
        // Perform Act
        var act = new FalseCanPerformAct();
        act.Init("Test Act");
        act.Perform();


        // Assertions
        Assert.IsTrue(!act.IsOngoing(), "Act performed despite CanPerform() being false!");


        yield return null;
    }



    [UnityTest]
    public IEnumerator PerformWhenEnabled()
    {
        // Perform Act
        var act = new WaitInfiniAct();
        act.Init("Test Act");
        act.SetEnabled(true);
        act.Perform();


        // Assertions
        Assert.IsTrue(act.IsOngoing(), "Act did not perform despite being enabled!");


        yield return null;
    }
    [UnityTest]
    public IEnumerator PerformWhenTheaterEnabled()
    {
        // Prerequisites
        var theater = new GameObject().AddComponent<Theater>();


        // Perform Act
        var act = new WaitInfiniAct();
        act.Init("Test Act", theater);
        theater.SetEnabled(true);
        act.Perform();


        // Assertions
        Assert.IsTrue(act.IsOngoing(), "Act did not perform despite theater being enabled!");


        UnityEngine.Object.Destroy(theater.gameObject);
        yield return null;
    }
    [UnityTest]
    public IEnumerator PerformWhenCanReperform()
    {
        // Perform Act
        var act = new ReperformableInfiAct();
        var enterCount = 0;
        act.OnPreEnter += (a) =>
        {
            enterCount++;
        };
        act.Init("Test Act");
        act.Perform();
        act.Perform();


        // Assertions
        Assert.IsTrue(enterCount == 2, "Act did not reperform despite _canReperform being true!");


        yield return null;
    }
    [UnityTest]
    public IEnumerator PerformWhenExternalConditionTrue()
    {
        // Perform Act
        var act = new WaitInfiniAct();
        act.performConditions.Add((a) => true);
        act.Init("Test Act");
        act.Perform();


        // Assertions
        Assert.IsTrue(act.IsOngoing(), "Act did not perform despite external condition being true!");


        yield return null;
    }
    [UnityTest]
    public IEnumerator PerformWhenCanPerformTrue()
    {
        // Perform Act
        var act = new CanPerformAct();
        act.Init("Test Act");
        act.Perform();


        // Assertions
        Assert.IsTrue(act.callCount == 1, "CanPerform() not invoked despite performing!");
        Assert.IsTrue(act.GetPerformCount() == 1, "Act did not perform despite CanPerform() being true!");


        yield return null;
    }



    [UnityTest]
    public IEnumerator CanReperformWorksCorrectly()
    {
        // Perform Act
        var cannotReperformAct = new NonReperformableInfiAct();
        cannotReperformAct.Init("Cannot Reperform Act");
        cannotReperformAct.Perform();
        cannotReperformAct.Perform();

        var canReperformAct = new ReperformableInfiAct();
        canReperformAct.Init("Can Reperform Act");
        canReperformAct.Perform();
        canReperformAct.Perform();


        // Assertions
        Assert.IsTrue(cannotReperformAct.GetPerformCount() == 1, $"Act reperformed despite _canReperform being false! Count={cannotReperformAct.GetPerformCount()}");
        Assert.IsTrue(canReperformAct.GetPerformCount() == 2, $"Act did not reperform despite _canReperform being true! Count={canReperformAct.GetPerformCount()}");


        yield return null;
    }



    [UnityTest]
    public IEnumerator PerformFromOnPreSetup()
    {
        // Perform Act
        var act = new Act();
        act.OnPreSetup += (a) =>
        {
            a.Perform();
        };
        act.Init("Test Act");


        // Assertions
        Assert.IsTrue(act.GetPerformCount() == 0, $"Act performed from OnPreSetup! Perform Count={act.GetPerformCount()}");


        yield return null;
    }
    [UnityTest]
    public IEnumerator PerformFromOnPostSetup()
    {
        // Perform Act
        var act = new Act();
        act.OnPostSetup += (a) =>
        {
            a.Perform();
        };
        act.Init("Test Act");


        // Assertions
        Assert.IsTrue(act.GetPerformCount() == 0, $"Act performed from OnPostSetup! Perform Count={act.GetPerformCount()}");


        yield return null;
    }
    [UnityTest]
    public IEnumerator PerformFromOnPerformStart()
    {

        // Perform Act
        var act = new ReperformableAct();
        act.OnPerformStart += (a) =>
        {
            if (act.GetPerformCount() <= 2)
            {
                a.Perform();
            }
        };
        act.Init("Test Act");
        act.Perform();


        // Assertions
        Assert.IsTrue(act.GetPerformCount() == 3, $"Act did not reperform from OnPerformStart! Perform Count={act.GetPerformCount()}");

        yield return null;
    }
    [UnityTest]
    public IEnumerator PerformFromOnPerformEnd()
    {
        // Perform Act
        var act = new ReperformableAct();
        act.OnPerformEnd += (a) =>
        {
            if (act.GetPerformCount() <= 2)
            {
                a.Perform();
            }
        };
        act.Init("Test Act");
        act.Perform();


        // Assertions
        Assert.IsTrue(act.GetPerformCount() == 3, $"Act did not reperform thrice from OnPerformEnd! Perform Count={act.GetPerformCount()}");


        yield return null;
    }
    [UnityTest]
    public IEnumerator PerformFromOnPrePrologue()
    {
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
                a.Perform();
            }
        };
        act.Init("Test Act");
        act.Perform();


        // Assertions
        Assert.IsTrue(act.GetPerformCount() == 3, $"Act did not reperform thrice from OnPrePrologue! Perform Count={act.GetPerformCount()}");


        yield return null;
    }
    [UnityTest]
    public IEnumerator PerformFromOnPostPrologue()
    {
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
                act.Perform();
            }
        };
        act.Init("Test Act");
        act.Perform();


        // Assertions
        Assert.IsTrue(act.GetPerformCount() != 3, $"Act reperform thrice from OnPostPrologue! Perform Count={act.GetPerformCount()}");


        yield return null;

    }
    [UnityTest]
    public IEnumerator PerformFromOnPreEnter()
    {
        // Perform Act
        var act = new ReperformableAct();
        act.OnPreEnter += (a) =>
        {
            if (act.GetPerformCount() <= 2)
            {
                a.Perform();
            }
        };
        act.Init("Test Act");
        act.Perform();


        // Assertions
        Assert.IsTrue(act.GetPerformCount() == 3, $"Act did not reperform thrice from OnPreEnter! Perform Count={act.GetPerformCount()}");


        yield return null;
    }
    [UnityTest]
    public IEnumerator PerformFromOnPostEnter()
    {
        // Perform Act
        var act = new ReperformableAct();
        act.OnPostEnter += (a) =>
        {
            if (act.GetPerformCount() <= 2)
            {
                a.Perform();
            }
        };
        act.Init("Test Act");
        act.Perform();


        // Assertions
        Assert.IsTrue(act.GetPerformCount() == 3, $"Act did not reperform thrice from OnPostEnter! Perform Count={act.GetPerformCount()}");


        yield return null;

    }
    [UnityTest]
    public IEnumerator PerformFromOnPreTick()
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
                a.Perform();
            }
        };
        act.Init("Test Act", theater);
        act.Perform();


        // Wait for tick cascade
        int frame = 0;
        while (act.GetPerformCount() < 3 && frame < 10)
        {
            yield return null;
            frame++;
        }


        // Assertions
        Assert.IsTrue(act.GetPerformCount() == 3, $"Act did not reperform thrice from OnPreTick! Perform Count={act.GetPerformCount()}");


        // Cleanup
        Object.Destroy(theaterGO);

    }
    [UnityTest]
    public IEnumerator PerformFromOnPostTick()
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
                a.Perform();
            }
        };
        act.Init("Test Act", theater);
        act.Perform();


        // Wait for tick cascade
        int frame = 0;
        while (act.GetPerformCount() < 3 && frame < 10)
        {
            yield return null;
            frame++;
        }


        // Assertions
        Assert.IsTrue(act.GetPerformCount() == 3, $"Act did not reperform thrice from OnPostTick! Perform Count={act.GetPerformCount()}");


        // Cleanup
        Object.Destroy(theaterGO);
    }
    [UnityTest]
    public IEnumerator PerformFromOnPrePhysicsTick()
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
                a.Perform();
            }
        };
        act.Init("Test Act", theater);
        act.Perform();


        // Wait for physics tick cascade
        int frame = 0;
        while (act.GetPerformCount() < 3 && frame < 10)
        {
            yield return new WaitForFixedUpdate();
            frame++;
        }


        // Assertions
        Assert.IsTrue(act.GetPerformCount() == 3, $"Act did not reperform thrice from OnPrePhysicsTick! Perform Count={act.GetPerformCount()}");


        // Cleanup
        Object.Destroy(theaterGO);
    }
    [UnityTest]
    public IEnumerator PerformFromOnPostPhysicsTick()
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
                a.Perform();
            }
        };
        act.Init("Test Act", theater);
        act.Perform();


        // Wait for physics tick cascade
        int frame = 0;
        while (act.GetPerformCount() < 3 && frame < 10)
        {
            yield return new WaitForFixedUpdate();
            frame++;
        }


        // Assertions
        Assert.IsTrue(act.GetPerformCount() == 3, $"Act did not reperform thrice from OnPostPhysicsTick! Perform Count={act.GetPerformCount()}");


        // Cleanup
        Object.Destroy(theaterGO);
    }
    [UnityTest]
    public IEnumerator PerformFromOnPreLateTick()
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
                a.Perform();
            }
        };
        act.Init("Test Act", theater);
        act.Perform();


        // Wait for late tick cascade
        int frame = 0;
        while (act.GetPerformCount() < 3 && frame < 10)
        {
            yield return null;
            frame++;
        }


        // Assertions
        Assert.IsTrue(act.GetPerformCount() == 3, $"Act did not reperform thrice from OnPreLateTick! Perform Count={act.GetPerformCount()}");


        // Cleanup
        Object.Destroy(theaterGO);
    }
    [UnityTest]
    public IEnumerator PerformFromOnPostLateTick()
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
                a.Perform();
            }
        };
        act.Init("Test Act", theater);
        act.Perform();


        // Wait for late tick cascade
        int frame = 0;
        while (act.GetPerformCount() < 3 && frame < 10)
        {
            yield return null;
            frame++;
        }


        // Assertions
        Assert.IsTrue(act.GetPerformCount() == 3, $"Act did not reperform thrice from OnPostLateTick! Perform Count={act.GetPerformCount()}");


        // Cleanup
        Object.Destroy(theaterGO);

    }
    [UnityTest]
    public IEnumerator PerformFromOnPreExit()
    {
        // Perform Act
        var act = new ReperformableAct();
        act.OnPreExit += (a) =>
        {
            if (act.GetPerformCount() <= 2)
            {
                a.Perform();
            }
        };
        act.Init("Test Act");
        act.Perform();


        // Assertions
        Assert.IsTrue(act.GetPerformCount() != 3, $"Act reperformed from OnPreExit! Perform Count={act.GetPerformCount()}");


        yield return null;
    }
    [UnityTest]
    public IEnumerator PerformFromOnPostExit()
    {
        // Perform Act
        var act = new ReperformableAct();
        act.OnPostExit += (a) =>
        {
            if (act.GetPerformCount() <= 2)
            {
                a.Perform();
            }
        };
        act.Init("Test Act");
        act.Perform();


        // Assertions
        Assert.IsTrue(act.GetPerformCount() != 3, $"Act reperformed from OnPostExit! Perform Count={act.GetPerformCount()}");


        yield return null;
    }
    [UnityTest]
    public IEnumerator PerformFromOnPreCleanup()
    {
        // Perform Act
        var act = new ReperformableAct();
        act.OnPreCleanup += (a) =>
        {
            a.Perform();
        };
        act.Init("Test Act");
        act.Deinit();


        // Assertions
        Assert.IsTrue(act.GetPerformCount() == 0, $"Act performed from OnPreCleanup! Perform Count={act.GetPerformCount()}");


        yield return null;
    }
    [UnityTest]
    public IEnumerator PerformsFromOnPostCleanup()
    {
        // Perform Act
        var act = new ReperformableAct();
        act.OnPostCleanup += (a) =>
        {
            a.Perform();
        };
        act.Init("Test Act");
        act.Deinit();


        // Assertions
        Assert.IsTrue(act.GetPerformCount() == 0, $"Act performed from OnPostCleanup! Perform Count={act.GetPerformCount()}");


        yield return null;
    }
    [UnityTest]
    public IEnumerator PerformFromOnEnableChanged()
    {
        // Perform Act
        var act = new Act();
        act.OnEnableChanged += (a, newEnabled) =>
        {
            if (act.GetPerformCount() <= 2)
            {
                a.Perform();
            }
        };
        act.Init("Test Act");
        act.SetEnabled(false);
        act.SetEnabled(true);
        act.SetEnabled(false);
        act.SetEnabled(true);
        act.SetEnabled(false);
        act.SetEnabled(true);


        // Assertions
        Assert.IsTrue(act.GetPerformCount() == 3, $"Act did not reperform thrice from OnEnableChanged! Perform Count={act.GetPerformCount()}");


        yield return null;
    }
    [UnityTest]
    public IEnumerator PerformFromOnBlockChanged()
    {
        // Perform Act
        var act = new Act();
        act.OnBlockChanged += (a, byAct, blockType, didBlock) =>
        {
            if (act.GetPerformCount() <= 2)
            {
                a.Perform();
            }
        };
        act.Init("Test Act");


        var blocker = new ReperformableAct();
        blocker.Init("Blocker Act");
        blocker.AddToBlock(new List<Act> { act });
        blocker.Perform();
        blocker.Abort();
        blocker.Perform();
        blocker.Abort();
        blocker.Perform();
        blocker.Abort();


        // Assertions
        Assert.IsTrue(act.GetPerformCount() == 3, $"Act did not reperform thrice from OnBlockChanged! Perform Count={act.GetPerformCount()}");


        yield return null;
    }
}
