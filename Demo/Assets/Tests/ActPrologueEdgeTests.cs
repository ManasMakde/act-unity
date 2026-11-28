using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;



// 1. Does building a prologue chain fail when one of the prologue act in the middle is past prologuing?
// 2. Does building a prologue chain succeed even if one of the prologue act at the end is past prologuing?

// 3. Does failing to build a prologue chain interrupt any of the acts that were going to part of the chain?

// 4. Does adding a duplicate act twice in prologue chain still allow everything perform as intended? and Does the duplicate act only perform once?
// 5. Does adding a duplicate act thrice in prologue chain still allow everything perform as intended? and Does the duplicate act only perform once?
// 6. Does adding multiple duplicate acts in prologue chain still allow everything perform as intended? and Do the duplicate acts only perform once?

// 7. Does the main act blocking the prologue act fail the entire chain?
// 8. Does the prologue act blocking the main act fail the entire chain?

// 9. Does a prologue act performing a sibling act not fail the entire chain?


public class ActPrologueEdgeTests
{
    [UnityTest]
    public IEnumerator BuildFailsWhenMiddleActPastProloguing()  // 1
    {
        // Setup acts
        var actA = new Act();
        actA.Init("ActA");

        var actB = new WaitInfiniAct();
        actB.Init("ActB");

        var actC = new Act();
        actC.Init("ActC");

        var mainAct = new Act();
        mainAct.prologue = (a) => new() { new() { actA }, new() { actB }, new() { actC } };
        mainAct.Init("MainAct");


        // Push middle act past prologuing then perform main
        actB.Perform();
        var statusBeforeMain = actB.GetStatus();

        mainAct.Perform();


        // Assertions
        Assert.IsTrue(statusBeforeMain == Act.Status.Entering, $"ActB is not past prologuing before main performs! Status={statusBeforeMain}");
        Assert.IsTrue(mainAct.GetOutcome() == Act.Outcome.Failure, $"Main act did not fail despite middle prologue act being past prologuing! Outcome={mainAct.GetOutcome()}");
        Assert.IsFalse(mainAct.IsPerforming(), "Main act is still performing despite chain build failing!");
        Assert.IsTrue(actA.GetPerformCount() == 0, $"ActA performed despite chain build failing! Perform Count={actA.GetPerformCount()}");
        Assert.IsTrue(actC.GetPerformCount() == 0, $"ActC performed despite chain build failing! Perform Count={actC.GetPerformCount()}");
        Assert.IsTrue(actB.GetPerformCount() == 1 && actB.IsPerforming(), $"ActB got disturbed by failed chain build! Perform Count={actB.GetPerformCount()}  IsPerforming={actB.IsPerforming()}");


        // Check chain sets cleared after failed build
        Utilities.AssertChainCleared(actA);
        Utilities.AssertChainCleared(actB);
        Utilities.AssertChainCleared(actC);
        Utilities.AssertChainCleared(mainAct);


        yield return null;
    }
    [UnityTest]
    public IEnumerator BuildSucceedsWhenFirstActPastProloguing()  // 2
    {
        // Prerequisites
        var idealOrder = new List<string>() { "ActA", "ActB", "MainAct" };
        var executionOrder = new List<string>();


        // Setup acts
        var actA = new ManualFinishAct();
        actA.OnPreEnter += (a) => executionOrder.Add(a.GetName());
        actA.Init("ActA");

        var actB = new Act();
        actB.OnPreEnter += (a) => executionOrder.Add(a.GetName());
        actB.Init("ActB");

        var mainAct = new Act();
        mainAct.OnPreEnter += (a) => executionOrder.Add(a.GetName());
        mainAct.prologue = (a) => new() { new() { actA }, new() { actB } };
        mainAct.Init("MainAct");


        // Push first act past prologuing then perform main
        actA.Perform();
        var statusABeforeMain = actA.GetStatus();

        mainAct.Perform();

        var statusMainWhileWaiting = mainAct.GetStatus();
        var statusBWhileWaiting = actB.GetStatus();
        var performCountAWhileWaiting = actA.GetPerformCount();


        // Finish first act so chain continues
        actA.ManualFinish();


        // Assertions
        Assert.IsTrue(statusABeforeMain == Act.Status.Entering, $"ActA is not past prologuing before main performs! Status={statusABeforeMain}");
        Assert.IsTrue(statusMainWhileWaiting == Act.Status.Prologuing, $"Main act did not wait on prologue despite chain build succeeding! Status={statusMainWhileWaiting}");
        Assert.IsTrue(statusBWhileWaiting == Act.Status.Prologuing, $"ActB did not wait on ActA! Status={statusBWhileWaiting}");
        Assert.IsTrue(performCountAWhileWaiting == 1, $"ActA got performed again by the chain! Perform Count={performCountAWhileWaiting}");
        Assert.IsTrue(actA.GetPerformCount() == 1, $"ActA did not perform exactly once! Perform Count={actA.GetPerformCount()}");
        Assert.IsTrue(actB.GetPerformCount() == 1, $"ActB did not perform exactly once! Perform Count={actB.GetPerformCount()}");
        Assert.IsTrue(mainAct.GetPerformCount() == 1, $"MainAct did not perform exactly once! Perform Count={mainAct.GetPerformCount()}");
        Assert.IsTrue(mainAct.GetOutcome() == Act.Outcome.Success, $"Main act did not succeed after first act finished! Outcome={mainAct.GetOutcome()}");
        Assert.IsTrue(executionOrder.SequenceEqual(idealOrder), $"Incorrect execution order\nExpected Order=[{string.Join(", ", idealOrder)}]\nOutput Order=[{string.Join(", ", executionOrder)}]");


        // Check chain sets cleared after perform
        Utilities.AssertChainCleared(actA);
        Utilities.AssertChainCleared(actB);
        Utilities.AssertChainCleared(mainAct);


        yield return null;
    }


    [UnityTest]
    public IEnumerator BuildFailureDoesNotInterruptChainActs()  // 3
    {
        // Setup acts
        var actA = new WaitInfiniAct();
        actA.Init("ActA");

        var actB = new WaitInfiniAct();
        actB.Init("ActB");

        var actC = new WaitInfiniAct();
        actC.Init("ActC");

        var mainAct = new Act();
        mainAct.prologue = (a) => new() { new() { actA }, new() { actB }, new() { actC } };
        mainAct.Init("MainAct");


        // Push middle and last act past prologuing then perform main
        actB.Perform();
        actC.Perform();

        mainAct.Perform();


        // Assertions for main act
        Assert.IsTrue(mainAct.GetOutcome() == Act.Outcome.Failure, $"Main act did not fail despite chain build failing! Outcome={mainAct.GetOutcome()}");
        Assert.IsFalse(mainAct.IsPerforming(), "Main act is still performing despite chain build failing!");


        // Assertions for idle chain act
        Assert.IsTrue(actA.GetPerformCount() == 0, $"ActA performed despite chain build failing! Perform Count={actA.GetPerformCount()}");
        Assert.IsTrue(actA.GetOutcome() == Act.Outcome.None, $"ActA got interrupted by failed chain build! Outcome={actA.GetOutcome()}");


        // Assertions for already performing chain acts
        Assert.IsTrue(actB.IsPerforming() && actB.GetOutcome() == Act.Outcome.Pending, $"ActB got interrupted by failed chain build! IsPerforming={actB.IsPerforming()}  Outcome={actB.GetOutcome()}");
        Assert.IsTrue(actC.IsPerforming() && actC.GetOutcome() == Act.Outcome.Pending, $"ActC got interrupted by failed chain build! IsPerforming={actC.IsPerforming()}  Outcome={actC.GetOutcome()}");
        Assert.IsTrue(actB.GetPerformCount() == 1, $"ActB performed again due to failed chain build! Perform Count={actB.GetPerformCount()}");
        Assert.IsTrue(actC.GetPerformCount() == 1, $"ActC performed again due to failed chain build! Perform Count={actC.GetPerformCount()}");


        // Check chain sets cleared after failed build
        Utilities.AssertChainCleared(actA);
        Utilities.AssertChainCleared(actB);
        Utilities.AssertChainCleared(actC);
        Utilities.AssertChainCleared(mainAct);


        yield return null;
    }


    [UnityTest]
    public IEnumerator DuplicateActTwice()  // 4
    {
        // Prerequisites
        var idealOrder = new List<string>() { "ActA", "ActB", "MainAct" };
        var executionOrder = new List<string>();


        // Setup acts
        var actA = new Act();
        actA.OnPreEnter += (a) => executionOrder.Add(a.GetName());
        actA.Init("ActA");

        var actB = new Act();
        actB.OnPreEnter += (a) => executionOrder.Add(a.GetName());
        actB.Init("ActB");

        var mainAct = new Act();
        mainAct.OnPreEnter += (a) => executionOrder.Add(a.GetName());
        mainAct.prologue = (a) => new() { new() { actA, actA, actB } };
        mainAct.Init("MainAct");


        // Perform main
        mainAct.Perform();


        // Assertions
        Assert.IsTrue(actA.GetPerformCount() == 1, $"Duplicate ActA did not perform exactly once! Perform Count={actA.GetPerformCount()}");
        Assert.IsTrue(actB.GetPerformCount() == 1, $"ActB did not perform exactly once! Perform Count={actB.GetPerformCount()}");
        Assert.IsTrue(mainAct.GetPerformCount() == 1, $"MainAct did not perform exactly once! Perform Count={mainAct.GetPerformCount()}");
        Assert.IsTrue(mainAct.GetOutcome() == Act.Outcome.Success, $"MainAct did not succeed! Outcome={mainAct.GetOutcome()}");
        Assert.IsTrue(executionOrder.SequenceEqual(idealOrder), $"Incorrect execution order\nExpected Order=[{string.Join(", ", idealOrder)}]\nOutput Order=[{string.Join(", ", executionOrder)}]");


        // Check chain sets cleared after perform
        Utilities.AssertChainCleared(actA);
        Utilities.AssertChainCleared(actB);
        Utilities.AssertChainCleared(mainAct);


        yield return null;
    }
    [UnityTest]
    public IEnumerator DuplicateActThrice()  // 5
    {
        // Prerequisites
        var idealOrder = new List<string>() { "ActA", "ActB", "MainAct" };
        var executionOrder = new List<string>();


        // Setup acts
        var actA = new Act();
        actA.OnPreEnter += (a) => executionOrder.Add(a.GetName());
        actA.Init("ActA");

        var actB = new Act();
        actB.OnPreEnter += (a) => executionOrder.Add(a.GetName());
        actB.Init("ActB");

        var mainAct = new Act();
        mainAct.OnPreEnter += (a) => executionOrder.Add(a.GetName());
        mainAct.prologue = (a) => new() { new() { actA, actA, actA, actB } };
        mainAct.Init("MainAct");


        // Perform main
        mainAct.Perform();


        // Assertions
        Assert.IsTrue(actA.GetPerformCount() == 1, $"Triplicate ActA did not perform exactly once! Perform Count={actA.GetPerformCount()}");
        Assert.IsTrue(actB.GetPerformCount() == 1, $"ActB did not perform exactly once! Perform Count={actB.GetPerformCount()}");
        Assert.IsTrue(mainAct.GetPerformCount() == 1, $"MainAct did not perform exactly once! Perform Count={mainAct.GetPerformCount()}");
        Assert.IsTrue(mainAct.GetOutcome() == Act.Outcome.Success, $"MainAct did not succeed! Outcome={mainAct.GetOutcome()}");
        Assert.IsTrue(executionOrder.SequenceEqual(idealOrder), $"Incorrect execution order\nExpected Order=[{string.Join(", ", idealOrder)}]\nOutput Order=[{string.Join(", ", executionOrder)}]");


        // Check chain sets cleared after perform
        Utilities.AssertChainCleared(actA);
        Utilities.AssertChainCleared(actB);
        Utilities.AssertChainCleared(mainAct);


        yield return null;
    }
    [UnityTest]
    public IEnumerator MultipleDuplicateActs()  // 6
    {
        // Prerequisites
        var idealOrder = new List<string>() { "ActA1", "ActA2", "ActB1", "ActB2", "ActC1", "ActC2", "MainAct" };
        var executionOrder = new List<string>();


        // Setup acts
        var actA1 = new Act();
        actA1.OnPreEnter += (a) => executionOrder.Add(a.GetName());
        actA1.Init("ActA1");

        var actA2 = new Act();
        actA2.OnPreEnter += (a) => executionOrder.Add(a.GetName());
        actA2.Init("ActA2");

        var actB1 = new Act();
        actB1.OnPreEnter += (a) => executionOrder.Add(a.GetName());
        actB1.Init("ActB1");

        var actB2 = new Act();
        actB2.OnPreEnter += (a) => executionOrder.Add(a.GetName());
        actB2.Init("ActB2");

        var actC1 = new Act();
        actC1.OnPreEnter += (a) => executionOrder.Add(a.GetName());
        actC1.Init("ActC1");

        var actC2 = new Act();
        actC2.OnPreEnter += (a) => executionOrder.Add(a.GetName());
        actC2.Init("ActC2");

        var mainAct = new Act();
        mainAct.OnPreEnter += (a) => executionOrder.Add(a.GetName());
        mainAct.prologue = (a) => new()
        {
            new() { actA1, actA1, actA2, actA1 },
            new() { actB1, actB2, actB2, actB1 },
            new() { actC1, actC1, actC2, actC2, actC1 }
        };
        mainAct.Init("MainAct");


        // Perform main
        mainAct.Perform();


        // Assertions
        Assert.IsTrue(actA1.GetPerformCount() == 1, $"Duplicate ActA1 did not perform exactly once! Perform Count={actA1.GetPerformCount()}");
        Assert.IsTrue(actA2.GetPerformCount() == 1, $"ActA2 did not perform exactly once! Perform Count={actA2.GetPerformCount()}");
        Assert.IsTrue(actB1.GetPerformCount() == 1, $"Duplicate ActB1 did not perform exactly once! Perform Count={actB1.GetPerformCount()}");
        Assert.IsTrue(actB2.GetPerformCount() == 1, $"Duplicate ActB2 did not perform exactly once! Perform Count={actB2.GetPerformCount()}");
        Assert.IsTrue(actC1.GetPerformCount() == 1, $"Duplicate ActC1 did not perform exactly once! Perform Count={actC1.GetPerformCount()}");
        Assert.IsTrue(actC2.GetPerformCount() == 1, $"Duplicate ActC2 did not perform exactly once! Perform Count={actC2.GetPerformCount()}");
        Assert.IsTrue(mainAct.GetPerformCount() == 1, $"MainAct did not perform exactly once! Perform Count={mainAct.GetPerformCount()}");
        Assert.IsTrue(mainAct.GetOutcome() == Act.Outcome.Success, $"MainAct did not succeed! Outcome={mainAct.GetOutcome()}");
        Assert.IsTrue(executionOrder.SequenceEqual(idealOrder), $"Incorrect execution order\nExpected Order=[{string.Join(", ", idealOrder)}]\nOutput Order=[{string.Join(", ", executionOrder)}]");


        // Check chain sets cleared after perform
        Utilities.AssertChainCleared(actA1);
        Utilities.AssertChainCleared(actA2);
        Utilities.AssertChainCleared(actB1);
        Utilities.AssertChainCleared(actB2);
        Utilities.AssertChainCleared(actC1);
        Utilities.AssertChainCleared(actC2);
        Utilities.AssertChainCleared(mainAct);


        yield return null;
    }


    [UnityTest]
    public IEnumerator MainBlockingPrologueFailsChain()  // 7
    {
        // Prerequisites
        var enteredNames = new List<string>();


        // Setup acts
        var prologueAct = new Act();
        prologueAct.OnPreEnter += (a) => enteredNames.Add(a.GetName());
        prologueAct.Init("PrologueAct");

        var mainAct = new Act();
        mainAct.OnPreEnter += (a) => enteredNames.Add(a.GetName());
        mainAct.prologue = (a) => new() { new() { prologueAct } };
        mainAct.block = (Act a) => new() { { prologueAct, Act.BlockType.Persistent } };
        mainAct.Init("MainAct");


        // Perform main
        mainAct.Perform();


        // Assertions
        Assert.IsTrue(mainAct.GetOutcome() == Act.Outcome.Failure, $"Main act did not fail despite blocking its own prologue act! Outcome={mainAct.GetOutcome()}");
        Assert.IsFalse(mainAct.IsPerforming(), "Main act is still performing despite blocking its own prologue act!");
        Assert.IsFalse(prologueAct.IsPerforming(), "Prologue act is performing despite being blocked by main act!");
        Assert.IsTrue(prologueAct.GetPerformCount() == 0, $"Prologue act performed despite being blocked! Perform Count={prologueAct.GetPerformCount()}");
        Assert.IsTrue(enteredNames.Count == 0, $"Some act entered despite chain failing! Entered=[{string.Join(", ", enteredNames)}]");
        Assert.IsFalse(prologueAct.IsBlocked(), "Prologue act is still blocked after main act concluded!");


        // Check chain sets cleared after failed chain
        Utilities.AssertChainCleared(prologueAct);
        Utilities.AssertChainCleared(mainAct);


        yield return null;
    }
    [UnityTest]
    public IEnumerator PrologueBlockingMainFailsChain()  // 8
    {
        // Prerequisites
        var enteredNames = new List<string>();


        // Setup acts
        var prologueAct = new Act();
        var mainAct = new Act();

        prologueAct.OnPreEnter += (a) => enteredNames.Add(a.GetName());
        prologueAct.block = (Act a) => new() { { mainAct, Act.BlockType.Persistent } };
        prologueAct.Init("PrologueAct");

        mainAct.OnPreEnter += (a) => enteredNames.Add(a.GetName());
        mainAct.prologue = (a) => new() { new() { prologueAct } };
        mainAct.Init("MainAct");


        // Perform main
        mainAct.Perform();


        // Assertions
        Assert.IsTrue(mainAct.GetOutcome() != Act.Outcome.Success, $"Main act succeeded despite being blocked by its prologue act! Outcome={mainAct.GetOutcome()}");
        Assert.IsFalse(mainAct.IsPerforming(), "Main act is still performing despite being blocked by its prologue act!");
        Assert.IsFalse(prologueAct.IsPerforming(), "Prologue act is still performing despite chain failing!");
        Assert.IsTrue(enteredNames.Count == 0, $"Some act entered despite chain failing! Entered=[{string.Join(", ", enteredNames)}]");
        Assert.IsFalse(mainAct.IsBlocked(), "Main act is still blocked after prologue act concluded!");


        // Check chain sets cleared after failed chain
        Utilities.AssertChainCleared(prologueAct);
        Utilities.AssertChainCleared(mainAct);


        yield return null;
    }


    [UnityTest]
    public IEnumerator PrologueActPerformingSiblingKeepsChain()  // 9
    {
        // Setup acts
        var actB = new ManualFinishAct();
        actB.Init("ActB");

        var actA = new Act();
        actA.OnPreEnter += (a) => actB.Perform();
        actA.Init("ActA");

        var mainAct = new Act();
        mainAct.prologue = (a) => new() { new() { actA, actB } };
        mainAct.Init("MainAct");


        // Perform main, sibling should keep chain waiting
        mainAct.Perform();

        var statusMainWhileWaiting = mainAct.GetStatus();
        var isBPerformingWhileWaiting = actB.IsPerforming();


        // Finish sibling so chain continues
        actB.ManualFinish();


        // Assertions
        Assert.IsTrue(statusMainWhileWaiting == Act.Status.Prologuing, $"Main act did not wait on sibling prologue act! Status={statusMainWhileWaiting}");
        Assert.IsTrue(isBPerformingWhileWaiting, "ActB is not performing despite ActA performing it!");
        Assert.IsTrue(actA.GetPerformCount() == 1, $"ActA did not perform exactly once! Perform Count={actA.GetPerformCount()}");
        Assert.IsTrue(actB.GetPerformCount() == 1, $"ActB did not perform exactly once! Perform Count={actB.GetPerformCount()}");
        Assert.IsTrue(mainAct.GetPerformCount() == 1, $"MainAct did not perform exactly once! Perform Count={mainAct.GetPerformCount()}");
        Assert.IsTrue(mainAct.GetOutcome() == Act.Outcome.Success, $"Main act did not succeed despite sibling performing! Outcome={mainAct.GetOutcome()}");


        // Check chain sets cleared after perform
        Utilities.AssertChainCleared(actA);
        Utilities.AssertChainCleared(actB);
        Utilities.AssertChainCleared(mainAct);


        yield return null;
    }
}
