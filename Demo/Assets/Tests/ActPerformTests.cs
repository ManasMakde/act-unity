using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;


public class ActPerformTests
{
    [UnityTest]
    public IEnumerator OnPerformStartAndEnd()  // Checks OnPerformStart & OnPerformEnd
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
    public IEnumerator PerformFailsWhenDisabled()  // Checks perform fails when act disabled
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
    public IEnumerator PerformFailsWhenTheaterDisabled()  // Checks perform fails when theater disabled
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
    public IEnumerator PerformFailsWhenBlocked()  // Checks perform fails when act blocked
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
    public IEnumerator PerformFailsWhenCannotReperform()  // Checks perform fails when already ongoing & cannot reperform
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
    public IEnumerator PerformFailsWhenExternalConditionFalse()  // Checks perform fails when external condition is false
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
    public IEnumerator PerformFailsWhenCanPerformFalse()  // Checks perform fails when overriden CanPerform() is false
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
    public IEnumerator PerformFailsWhenCalledWhileExiting()  // Checks perform does not reperform act when called during exiting
    {
        // Perform Act
        var act = new ExitAct();
        var performCountDuringExit = 0;
        act.OnPreExit += (a) =>
        {
            a.Perform();
            performCountDuringExit = a.GetPerformCount();
        };
        act.Init("Test Act");
        act.Perform();


        // Assertions
        Assert.IsTrue(performCountDuringExit == 1, "Act reperformed despite being called during exiting!");


        yield return null;
    }



    [UnityTest]
    public IEnumerator PerformSucceedsWhenEnabled()  // Checks perform succeeds when act enabled
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
    public IEnumerator PerformSucceedsWhenTheaterEnabled()  // Checks perform succeeds when theater enabled
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
    public IEnumerator PerformSucceedsWhenCanReperform()  // Checks perform succeeds again when ongoing and can reperform
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
    public IEnumerator PerformSucceedsWhenExternalConditionTrue()  // Checks perform succeeds when external condition true
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
    public IEnumerator PerformSucceedsWhenCanPerformTrue()  // Checks perform succeeds when overriden CanPerform() true
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
    public IEnumerator PerformDeferredWorks()  // Checks perform deferred performs act on next tick
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
    public IEnumerator PerformDeferredDoesNotImmediatelyPerform()  // Checks perform deferred does not perform immediately
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
    public IEnumerator PerformsOnceWhenDeferredTwice()  // Checks act performs once even when deferred twice
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
    public IEnumerator PerformDeferredWithNoneFlagDoesNothing()  // Checks perform deferred with tick flag none does nothing
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
    public IEnumerator PerformDeferredClearedUponPerformingImmediately()  // Checks deferred perform cleared when performed immediately
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
    public IEnumerator PerformDeferredClearedOnAbort()  // Checks deferred perform cleared on abort
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
    public IEnumerator PerformFromOnPerformStart()  // Checks act reperforms when Perform() called from OnPerformStart
    {
        // Perform Act
        var performStartCount = 0;
        var act = new ReperformableInfiAct();
        act.OnPerformStart += (a) =>
        {
            performStartCount++;
            if (performStartCount == 1)
            {
                a.Perform();
            }
        };
        act.Init("Test Act");
        act.Perform();


        // Assertions
        Assert.IsTrue(performStartCount == 2, $"Act did not reperform when Perform() called from OnPerformStart! Perform Count={performStartCount}");


        yield return null;
    }
    [UnityTest]
    public IEnumerator PerformFromOnPrePrologue()  // Checks act reperforms when Perform() called from OnPrePrologue
    {
        // Perform Act
        var prePrologueCount = 0;
        var act = new ReperformableInfiAct();
        act.prologue = (a) => new List<Act> { new Act() };
        act.OnPrePrologue += (a) =>
        {
            prePrologueCount++;
            if (prePrologueCount == 1)
            {
                a.Perform();
            }
        };
        act.Init("Test Act");
        act.Perform();


        // Assertions
        Assert.IsTrue(prePrologueCount == 2, $"Act did not reperform when Perform() called from OnPrePrologue! Perform Count={prePrologueCount}");


        yield return null;
    }
    [UnityTest]
    public IEnumerator PerformFromOnPostPrologue()  // Checks act reperforms when Perform() called from OnPostPrologue
    {
        // Perform Act
        var postPrologueCount = 0;
        var act = new ReperformableInfiAct();
        act.prologue = (a) => new List<Act> { new Act() };
        act.OnPostPrologue += (a) =>
        {
            postPrologueCount++;
            if (postPrologueCount == 1)
            {
                a.Perform();
            }
        };
        act.Init("Test Act");
        act.Perform();


        // Assertions
        Assert.IsTrue(postPrologueCount == 2, $"Act did not reperform when Perform() called from OnPostPrologue! Perform Count={postPrologueCount}");


        yield return null;
    }
    [UnityTest]
    public IEnumerator PerformFromOnPreEnter()  // Checks act reperforms when Perform() called from OnPreEnter
    {
        // Perform Act
        var preEnterCount = 0;
        var act = new ReperformableInfiAct();
        act.OnPreEnter += (a) =>
        {
            preEnterCount++;
            if (preEnterCount == 1)
            {
                a.Perform();
            }
        };
        act.Init("Test Act");
        act.Perform();


        // Assertions
        Assert.IsTrue(preEnterCount == 2, $"Act did not reperform when Perform() called from OnPreEnter! Perform Count={preEnterCount}");


        yield return null;
    }
    [UnityTest]
    public IEnumerator PerformFromOnPostEnter()  // Checks act reperforms when Perform() called from OnPostEnter
    {
        // Perform Act
        var postEnterCount = 0;
        var act = new ReperformableInfiAct();
        act.OnPostEnter += (a) =>
        {
            postEnterCount++;
            if (postEnterCount == 1)
            {
                a.Perform();
            }
        };
        act.Init("Test Act");
        act.Perform();


        // Assertions
        Assert.IsTrue(postEnterCount == 2, $"Act did not reperform when Perform() called from OnPostEnter! Perform Count={postEnterCount}");


        yield return null;
    }
    [UnityTest]
    public IEnumerator PerformFromOnPreTick()  // Checks act reperforms when Perform() called from OnPreTick
    {
        // Prerequisites
        var theaterObj = new GameObject("Test Theater");
        var theater = theaterObj.AddComponent<Theater>();


        // Perform Act
        var preTickCount = 0;
        var act = new ReperformableInfiAct();
        act.isVerbose = true;
        act.overrideTickFlag = Act.TickFlags.Tick;
        act.OnPreTick += (a) =>
        {
            preTickCount++;
            if (preTickCount == 1)
            {
                a.Perform();
            }
        };
        act.Init("Test Act", theater);
        act.Perform();
        yield return null;
        yield return null;


        // Assertions
        Assert.IsTrue(preTickCount == 2, $"Act did not reperform when Perform() called from OnPreTick! Perform Count={preTickCount}");


        UnityEngine.Object.Destroy(theaterObj);
    }
    [UnityTest]
    public IEnumerator PerformFromOnPostTick()  // Checks act reperforms when Perform() called from OnPostTick
    {
        // Prerequisites
        var theaterObj = new GameObject("Test Theater");
        var theater = theaterObj.AddComponent<Theater>();


        // Perform Act
        var postTickCount = 0;
        var act = new ReperformableInfiAct();
        act.overrideTickFlag = Act.TickFlags.Tick;
        act.OnPostTick += (a) =>
        {
            postTickCount++;
            if (postTickCount == 1)
            {
                a.Perform();
            }
        };
        act.Init("Test Act", theater);
        act.Perform();
        yield return null;
        yield return null;


        // Assertions
        Assert.IsTrue(postTickCount == 2, $"Act did not reperform when Perform() called from OnPostTick! Perform Count={postTickCount}");


        UnityEngine.Object.Destroy(theaterObj);
    }
    [UnityTest]
    public IEnumerator PerformFromOnPrePhysicsTick()  // Checks act reperforms when Perform() called from OnPrePhysicsTick
    {
        // Prerequisites
        var theaterObj = new GameObject("Test Theater");
        var theater = theaterObj.AddComponent<Theater>();


        // Perform Act
        var prePhysicsTickCount = 0;
        var act = new ReperformableInfiAct();
        act.overrideTickFlag = Act.TickFlags.PhysicsTick;
        act.OnPrePhysicsTick += (a) =>
        {
            prePhysicsTickCount++;
            if (prePhysicsTickCount == 1)
            {
                a.Perform();
            }
        };
        act.Init("Test Act", theater);
        act.Perform();
        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();


        // Assertions
        Assert.IsTrue(prePhysicsTickCount == 2, $"Act did not reperform when Perform() called from OnPrePhysicsTick! Perform Count={prePhysicsTickCount}");


        UnityEngine.Object.Destroy(theaterObj);
    }
    [UnityTest]
    public IEnumerator PerformFromOnPostPhysicsTick()  // Checks act reperforms when Perform() called from OnPostPhysicsTick
    {
        // Prerequisites
        var theaterObj = new GameObject("Test Theater");
        var theater = theaterObj.AddComponent<Theater>();


        // Perform Act
        var postPhysicsTickCount = 0;
        var act = new ReperformableInfiAct();
        act.overrideTickFlag = Act.TickFlags.PhysicsTick;
        act.OnPostPhysicsTick += (a) =>
        {
            postPhysicsTickCount++;
            if (postPhysicsTickCount == 1)
            {
                a.Perform();
            }
        };
        act.Init("Test Act", theater);
        act.Perform();
        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();


        // Assertions
        Assert.IsTrue(postPhysicsTickCount == 2, $"Act did not reperform when Perform() called from OnPostPhysicsTick! Perform Count={postPhysicsTickCount}");


        UnityEngine.Object.Destroy(theaterObj);
    }
    [UnityTest]
    public IEnumerator PerformFromOnPreLateTick()  // Checks act reperforms when Perform() called from OnPreLateTick
    {
        // Prerequisites
        var theaterObj = new GameObject("Test Theater");
        var theater = theaterObj.AddComponent<Theater>();


        // Perform Act
        var preLateTickCount = 0;
        var act = new ReperformableInfiAct();
        act.overrideTickFlag = Act.TickFlags.LateTick;
        act.OnPreLateTick += (a) =>
        {
            preLateTickCount++;
            if (preLateTickCount == 1)
            {
                a.Perform();
            }
        };
        act.Init("Test Act", theater);
        act.Perform();
        yield return null;
        yield return null;


        // Assertions
        Assert.IsTrue(preLateTickCount == 2, $"Act did not reperform when Perform() called from OnPreLateTick! Perform Count={preLateTickCount}");


        UnityEngine.Object.Destroy(theaterObj);
    }
    [UnityTest]
    public IEnumerator PerformFromOnPostLateTick()  // Checks act reperforms when Perform() called from OnPostLateTick
    {
        // Prerequisites
        var theaterObj = new GameObject("Test Theater");
        var theater = theaterObj.AddComponent<Theater>();


        // Perform Act
        var postLateTickCount = 0;
        var act = new ReperformableInfiAct();
        act.overrideTickFlag = Act.TickFlags.LateTick;
        act.OnPostLateTick += (a) =>
        {
            postLateTickCount++;
            if (postLateTickCount == 1)
            {
                a.Perform();
            }
        };
        act.Init("Test Act", theater);
        act.Perform();
        yield return null;
        yield return null;


        // Assertions
        Assert.IsTrue(postLateTickCount == 2, $"Act did not reperform when Perform() called from OnPostLateTick! Perform Count={postLateTickCount}");


        UnityEngine.Object.Destroy(theaterObj);
    }
    [UnityTest]
    public IEnumerator PerformFromOnPerformEnd()  // Checks act reperforms when Perform() called from OnPerformEnd
    {
        // Perform Act
        var performEndCount = 0;
        var act = new ReperformableAct();
        act.isVerbose = true;
        act.OnPerformEnd += (a) =>
        {
            performEndCount++;
            if (performEndCount == 1)
            {
                a.Perform();
            }
        };
        act.Init("Test Act");
        act.Perform();


        // Assertions
        Assert.IsTrue(performEndCount == 2, $"Act did not reperform when Perform() called from OnPerformEnd! Perform Count={performEndCount}");


        yield return null;
    }
    [UnityTest]
    public IEnumerator PerformsFromOnEnableChanged()  // Checks act performs when Perform() called from OnEnableChanged
    {
        // Perform Act
        var act = new WaitInfiniAct();
        act.OnEnableChanged += (a, isEnabled) =>
        {
            if (isEnabled)
            {
                a.Perform();
            }
        };
        act.Init("Test Act");
        act.SetEnabled(false);
        act.SetEnabled(true);


        // Assertions
        Assert.IsTrue(act.IsOngoing(), "Act did not perform when Perform() called from OnEnableChanged!");


        yield return null;
    }
    [UnityTest]
    public IEnumerator PerformsFromOnBlockChanged()  // Checks act performs when Perform() called from OnBlockChanged
    {
        // Perform Act
        var act = new WaitInfiniAct();
        act.OnBlockChanged += (a, blockingAct, blockType, didBlock) =>
        {
            if (!didBlock)
            {
                a.Perform();
            }
        };
        act.Init("Test Act");

        var blockerAct = new Act();
        blockerAct.Init("Blocker Act");
        blockerAct.AddToBlock(new List<Act> { act });
        blockerAct.Perform();


        // Assertions
        Assert.IsTrue(act.IsOngoing(), "Act did not perform when Perform() called from OnBlockChanged!");


        yield return null;
    }



    [UnityTest]
    public IEnumerator CanReperformWorksCorrectly()  // Checks _canReperform works correctly
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
}
