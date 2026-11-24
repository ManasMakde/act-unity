using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;


// 1. Are "pre perform request" & "post perform request" actions being broadcasted (with correct arguments)?
// 2. Are "perform start" & "perform end" actions being broadcasted (with correct arguments)?

// 3. Does perform fail when act disabled?
// 4. Does perform fail when theater disabled?
// 5. Does perform fail when act blocked?
// 6. Does perform fail when already performing and cannot reperform?
// 7. Does perform fail when external condition is false?
// 8. Does perform fail when overriden CanPerform() is false?
// 9. Does perform fail when called while exiting?

// 10. Does perform succeed when act enabled?
// 11. Does perform succeed when theater enabled?
// 12. Does perform succeed when already performing and can reperform?
// 13. Does perform succeed when external condition is true?
// 14. Does perform succeed when overriden CanPerform() is true?

// 15. Does _canReperform work correctly?

// 16. Does perform fail from OnPreSetup?
// 17. Does perform succeed from OnPostSetup?
// 18. Does reperform succeed from OnPerformStart?
// 19. Does reperform succeed from OnPrePrologue?
// 20. Does reperform succeed from OnPrologueComplete?
// 21. Does reperform succeed from OnPostPrologue?
// 22. Does reperform succeed from OnPreEnter?
// 23. Does reperform succeed from OnPostEnter?
// 24. Does reperform succeed from OnPreTick?
// 25. Does reperform succeed from OnPostTick?
// 26. Does reperform succeed from OnPrePhysicsTick?
// 27. Does reperform succeed from OnPostPhysicsTick?
// 28. Does reperform succeed from OnPreLateTick?
// 29. Does reperform succeed from OnPostLateTick?
// 30. Does reperform fail from OnPreExit?
// 31. Does reperform fail from OnPostExit?
// 32. Does perform succeed from OnPerformEnd?
// 33. Does reperform fail from OnPreCleanup?
// 34. Does reperform succeed from OnPostCleanup?
// 35. Does perform succeed from OnEnableChanged?
// 36. Does perform succeed from OnBlockChanged?


public class ActPerformTests
{
    [UnityTest]
    public IEnumerator OnPerformPreAndPostReq()  // 1
    {
        // Prerequisites
        var preReqTag = "PreReq";
        var postReqTag = "PostReq";
        var startTag = "Start";
        var eventOrder = new List<string>();


        // Successful perform
        bool wasPreReqInvoked = false;
        Act preReqArg1 = null;
        bool wasPostReqInvoked = false;
        Act postReqArg1 = null;
        bool postReqArg2 = false;

        var act = new Act();
        act.OnPrePerformReq += (a) => { wasPreReqInvoked = true; preReqArg1 = a; eventOrder.Add(preReqTag); };
        act.OnPostPerformReq += (a, willPerform) => { wasPostReqInvoked = true; postReqArg1 = a; postReqArg2 = willPerform; eventOrder.Add(postReqTag); };
        act.OnPerformStart += (a) => eventOrder.Add(startTag);
        act.Init("Test Act");
        act.Perform();


        // Assertions for successful perform
        Assert.IsTrue(wasPreReqInvoked, "OnPrePerformReq not invoked!");
        Assert.IsTrue(preReqArg1 == act, $"OnPrePerformReq first argument is invalid! Arg1='{preReqArg1}'");
        Assert.IsTrue(wasPostReqInvoked, "OnPostPerformReq not invoked!");
        Assert.IsTrue(postReqArg1 == act, $"OnPostPerformReq first argument is invalid! Arg1='{postReqArg1}'");
        Assert.IsTrue(postReqArg2 == true, $"OnPostPerformReq second argument is not true despite act performing! Arg2='{postReqArg2}'");
        Assert.IsTrue(eventOrder.Count == 3 && eventOrder[0] == preReqTag && eventOrder[1] == postReqTag && eventOrder[2] == startTag, $"Perform request events invalid order! Order='{string.Join(",", eventOrder)}'");


        // Failed perform
        wasPreReqInvoked = false;
        wasPostReqInvoked = false;
        postReqArg2 = true;

        var failAct = new FalseCanPerformAct();
        failAct.OnPrePerformReq += (a) => { wasPreReqInvoked = true; };
        failAct.OnPostPerformReq += (a, willPerform) => { wasPostReqInvoked = true; postReqArg2 = willPerform; };
        failAct.Init("Fail Act");
        failAct.Perform();


        // Assertions for failed perform
        Assert.IsTrue(wasPreReqInvoked, "OnPrePerformReq not invoked despite perform failing!");
        Assert.IsTrue(wasPostReqInvoked, "OnPostPerformReq not invoked despite perform failing!");
        Assert.IsTrue(postReqArg2 == false, $"OnPostPerformReq second argument is not false despite perform failing! Arg2='{postReqArg2}'");
        Assert.IsTrue(failAct.GetPerformCount() == 0, $"Act performed despite CanPerform() being false! Perform Count={failAct.GetPerformCount()}");


        yield return null;
    }
    [UnityTest]
    public IEnumerator OnPerformStartAndEnd()  // 2
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
    public IEnumerator PerformWhenDisabled()  // 3
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
    public IEnumerator PerformWhenTheaterDisabled()  // 4
    {
        // Prerequisites
        var theater = new GameObject().AddComponent<Theater>();
        theater.SetEnabled(false);


        // Perform Act
        var act = new WaitInfiniAct();
        act.Init("Test Act", theater);
        act.Perform();


        // Assertions
        Assert.IsFalse(act.IsPerforming(), "Act performed despite theater being disabled!");


        UnityEngine.Object.Destroy(theater.gameObject);
        yield return null;
    }
    [UnityTest]
    public IEnumerator PerformWhenBlocked()  // 5
    {
        // Perform First Block Later
        bool isPerforming_PFBL = false;
        bool isBlocked_PFBL = false;
        {
            var act = new WaitInfiniAct();
            act.Init("Test Act");
            act.Perform();

            var blockingAct = new WaitInfiniAct();
            blockingAct.Init("Blocking Act");
            blockingAct.block = (Act a) => new() { { act, Act.BlockType.Persistent } };
            blockingAct.Perform();

            isPerforming_PFBL = act.IsPerforming();
            isBlocked_PFBL = act.IsBlocked();
        }


        // Block First Perform Later
        bool isPerforming_BFPL = false;
        bool isBlocked_BFPL = false;
        {
            var act = new WaitInfiniAct();
            act.Init("Test Act");

            var blockingAct = new WaitInfiniAct();
            blockingAct.Init("Blocking Act");
            blockingAct.block = (Act a) => new() { { act, Act.BlockType.Persistent } };
            blockingAct.Perform();

            act.Perform();


            isPerforming_BFPL = act.IsPerforming();
            isBlocked_BFPL = act.IsBlocked();
        }


        // Assertions
        Assert.IsTrue(!isPerforming_PFBL, "Act is performing despite being blocked (Perform First Block Later)!");
        Assert.IsTrue(isBlocked_PFBL, "Act is unblocked despite being blocked (Perform First Block Later)!");
        Assert.IsTrue(!isPerforming_BFPL, "Act is performing despite being blocked (Block First Perform Later)!");
        Assert.IsTrue(isBlocked_BFPL, "Act is unblocked despite being blocked (Block First Perform Later)!");

        yield return null;
    }
    [UnityTest]
    public IEnumerator PerformWhenCannotReperform()  // 6
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
    public IEnumerator PerformWhenExternalConditionFalse()  // 7
    {
        // Perform Act
        var act = new WaitInfiniAct();
        act.condition = (a) => false;
        act.Init("Test Act");
        act.Perform();


        // Assertions
        Assert.IsTrue(!act.IsPerforming(), "Act performed despite external condition being false!");


        yield return null;
    }
    [UnityTest]
    public IEnumerator PerformWhenCanPerformFalse()  // 8
    {
        // Perform Act
        var act = new FalseCanPerformAct();
        act.Init("Test Act");
        act.Perform();


        // Assertions
        Assert.IsTrue(!act.IsPerforming(), "Act performed despite CanPerform() being false!");


        yield return null;
    }
    [UnityTest]
    public IEnumerator PerformWhenExiting()  // 9
    {
        // Prerequisites
        var preExitResult = true;
        var postExitResult = true;
        var statusInPreExit = Act.Status.None;
        var statusInPostExit = Act.Status.None;


        // Perform Act
        var act = new ManualFinishAct();
        act.canReperformOverride = true;
        act.OnPreExit += (a) => { statusInPreExit = a.GetStatus(); preExitResult = a.Perform(); };
        act.OnPostExit += (a) => { statusInPostExit = a.GetStatus(); postExitResult = a.Perform(); };
        act.Init("Test Act");
        act.Perform();
        act.ManualFinish();


        // Assertions
        Assert.IsTrue(statusInPreExit == Act.Status.Exiting, $"Act not exiting in OnPreExit! Status='{statusInPreExit}'");
        Assert.IsTrue(statusInPostExit == Act.Status.Exiting, $"Act not exiting in OnPostExit! Status='{statusInPostExit}'");
        Assert.IsFalse(preExitResult, "Perform() succeeded from OnPreExit while exiting!");
        Assert.IsFalse(postExitResult, "Perform() succeeded from OnPostExit while exiting!");
        Assert.IsTrue(act.GetPerformCount() == 1, $"Act reperformed while exiting! Perform Count={act.GetPerformCount()}");


        yield return null;
    }


    [UnityTest]
    public IEnumerator PerformWhenEnabled()  // 10
    {
        // Perform Act
        var act = new WaitInfiniAct();
        act.Init("Test Act");
        act.SetEnabled(true);
        act.Perform();


        // Assertions
        Assert.IsTrue(act.IsPerforming(), "Act did not perform despite being enabled!");


        yield return null;
    }
    [UnityTest]
    public IEnumerator PerformWhenTheaterEnabled()  // 11
    {
        // Prerequisites
        var theater = new GameObject().AddComponent<Theater>();


        // Perform Act
        var act = new WaitInfiniAct();
        act.Init("Test Act", theater);
        theater.SetEnabled(true);
        act.Perform();


        // Assertions
        Assert.IsTrue(act.IsPerforming(), "Act did not perform despite theater being enabled!");


        UnityEngine.Object.Destroy(theater.gameObject);
        yield return null;
    }
    [UnityTest]
    public IEnumerator PerformWhenCanReperform()  // 12
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
    public IEnumerator PerformWhenExternalConditionTrue()  // 13
    {
        // Perform Act
        var act = new WaitInfiniAct();
        act.condition = (a) => true;
        act.Init("Test Act");
        act.Perform();


        // Assertions
        Assert.IsTrue(act.IsPerforming(), "Act did not perform despite external condition being true!");


        yield return null;
    }
    [UnityTest]
    public IEnumerator PerformWhenCanPerformTrue()  // 14
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
    public IEnumerator CanReperformWorksCorrectly()  // 15
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
    public IEnumerator PerformFromOnPreSetup()  // 16
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
    public IEnumerator PerformFromOnPostSetup()  // 17
    {
        // Perform Act
        var act = new Act();
        act.OnPostSetup += (a) =>
        {
            a.Perform();
        };
        act.Init("Test Act");


        // Assertions
        Assert.IsTrue(act.GetPerformCount() == 1, $"Act did not perform exactly once from OnPostSetup! Perform Count={act.GetPerformCount()}");


        yield return null;
    }
    [UnityTest]
    public IEnumerator PerformFromOnPerformStart()  // 18
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
    public IEnumerator PerformFromOnPrePrologue()  // 19
    {
        // Prologue act so the prologue signal actually fires
        var prologueAct = new ReperformableAct();
        prologueAct.Init("Prologue Act");


        // Perform Act
        var act = new ReperformableAct();
        act.prologue = (a) => new() { new() { prologueAct } };
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
    public IEnumerator PerformFromOnPrologueComplete()  // 20
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
        act.prologue = (a) => new() { new() { prologue1Act, prologue2Act } };
        act.OnPrologueComplete += (a, pA, pO) =>
        {
            if (pO != Act.Outcome.Success)
            {
                return;
            }

            if (act.GetPerformCount() <= 2)
            {
                act.Perform();
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
        Assert.IsTrue(act.GetPerformCount() == 3, $"Act did not reperform thrice from OnPrePrologue! Perform Count={act.GetPerformCount()}");


        UnityEngine.Object.Destroy(theater.gameObject);
    }
    [UnityTest]
    public IEnumerator PerformFromOnPostPrologue()  // 21
    {
        // Prologue act so the prologue signal actually fires
        var prologueAct = new ReperformableAct();
        prologueAct.Init("Prologue Act");


        // Perform Act
        var act = new ReperformableAct();
        act.prologue = (a) => new() { new() { prologueAct } };
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
        Assert.IsTrue(act.GetPerformCount() == 3, $"Act did not reperform thrice from OnPostPrologue! Perform Count={act.GetPerformCount()}");


        yield return null;
    }
    [UnityTest]
    public IEnumerator PerformFromOnPreEnter()  // 22
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
    public IEnumerator PerformFromOnPostEnter()  // 23
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
    public IEnumerator PerformFromOnPreTick()  // 24
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
    public IEnumerator PerformFromOnPostTick()  // 25
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
    public IEnumerator PerformFromOnPrePhysicsTick()  // 26
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
    public IEnumerator PerformFromOnPostPhysicsTick()  // 27
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
    public IEnumerator PerformFromOnPreLateTick()  // 28
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
    public IEnumerator PerformFromOnPostLateTick()  // 29
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
    public IEnumerator PerformFromOnPreExit()  // 30
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
    public IEnumerator PerformFromOnPostExit()  // 31
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
    public IEnumerator PerformFromOnPerformEnd()  // 32
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
    public IEnumerator PerformFromOnPreCleanup()  // 33
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
    public IEnumerator PerformFromOnPostCleanup()  // 34
    {
        // Perform Act
        var act = new Act();
        act.OnPostCleanup += (a) =>
        {
            a.Perform();
        };
        act.Init("Test Act");
        act.Deinit();


        // Assertions
        Assert.IsTrue(act.GetPerformCount() == 1, $"Act performed not exactly once from OnPostCleanup! Perform Count={act.GetPerformCount()}");


        yield return null;
    }
    [UnityTest]
    public IEnumerator PerformFromOnEnableChanged()  // 35
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
    public IEnumerator PerformFromOnBlockChanged()  // 36
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
        blocker.block = (Act a) => new() { { act, Act.BlockType.Persistent } };
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
