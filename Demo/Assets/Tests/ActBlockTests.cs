using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;


// 1. Is "block changed" action being broadcasted (with correct arguments) when blocked/unblocked?
// 2. Is "block changed" action not being broadcasted when enabled/disabled?
// 3. Does blocking abort the act?
// 4. Can a persistantly unblocked act perform?

// 5. Does interrupt block stop the act's performing perform?
// 6. Can a interrupt blocked act perform despite blocker act still performing?

// 7. Does persistent blocking fail when adding self act to block?


public class ActBlockTests
{
    [UnityTest]
    public IEnumerator OnBlockChanged()  // 1
    {
        // Prerequisites
        bool wasBlockChangedInvoked = false;
        Act blockChangedArg1 = null;
        Act blockChangedArg2 = null;
        Act.BlockType blockChangedArg3 = Act.BlockType.Interrupt;
        bool blockChangedArg4 = false;


        // Setup main and target act
        var mainAct = new ManualFinishAct();
        var targetAct = new Act();
        targetAct.OnBlockChanged += (a, bAct, bType, didBlock) =>
        {
            wasBlockChangedInvoked = true;
            blockChangedArg1 = a;
            blockChangedArg2 = bAct;
            blockChangedArg3 = bType;
            blockChangedArg4 = didBlock;
        };
        mainAct.Init("Main Act");
        targetAct.Init("Target Act");
        mainAct.block = (Act a) => new() { { targetAct, Act.BlockType.Persistent } };


        // Perform main act, Should block target act
        mainAct.Perform();


        // Assertions for block
        Assert.IsTrue(wasBlockChangedInvoked, "OnBlockChanged not invoked on block!");
        Assert.IsTrue(blockChangedArg1 == targetAct, $"OnBlockChanged first argument is invalid! Arg1='{blockChangedArg1}'");
        Assert.IsTrue(blockChangedArg2 == mainAct, $"OnBlockChanged second argument is invalid! Arg2='{blockChangedArg2}'");
        Assert.IsTrue(blockChangedArg3 == Act.BlockType.Persistent, $"OnBlockChanged third argument is invalid! Arg3='{blockChangedArg3}'");
        Assert.IsTrue(blockChangedArg4 == true, "OnBlockChanged fourth argument is not true on block!");


        // Reset and finish main act, Should unblock target act
        wasBlockChangedInvoked = false;
        mainAct.ManualFinish();


        // Assertions for unblock
        Assert.IsTrue(wasBlockChangedInvoked, "OnBlockChanged not invoked on unblock!");
        Assert.IsTrue(blockChangedArg1 == targetAct, $"OnBlockChanged first argument is invalid! Arg1='{blockChangedArg1}'");
        Assert.IsTrue(blockChangedArg2 == mainAct, $"OnBlockChanged second argument is invalid! Arg2='{blockChangedArg2}'");
        Assert.IsTrue(blockChangedArg3 == Act.BlockType.Persistent, $"OnBlockChanged third argument is invalid! Arg3='{blockChangedArg3}'");
        Assert.IsTrue(blockChangedArg4 == false, "OnBlockChanged fourth argument is not false on unblock!");


        yield return null;
    }
    [UnityTest]
    public IEnumerator BlockChangeNotOnEnableDisable()  // 2
    {
        // Prerequisites
        bool wasBlockChangedInvoked = false;


        // Setup act
        var act = new Act();
        act.OnBlockChanged += (a, bAct, bType, didBlock) =>
        {
            wasBlockChangedInvoked = true;
        };
        act.Init("Test Act");


        // Disable and enable act
        act.SetEnabled(false);
        act.SetEnabled(true);


        // Assertions
        Assert.IsTrue(!wasBlockChangedInvoked, "OnBlockChanged invoked despite enable disable!");


        yield return null;
    }
    [UnityTest]
    public IEnumerator BlockingAbortsAct()  // 3
    {
        // Prerequisites
        var wasAborted = false;
        Act.Outcome recivedOutcome = Act.Outcome.Pending;


        // Setup main and target act
        var mainAct = new ManualFinishAct();
        var targetAct = new ManualFinishAct();
        targetAct.OnPreExit += (a) =>
        {
            wasAborted = true;
            recivedOutcome = a.GetOutcome();
        };
        mainAct.Init("Main Act");
        targetAct.Init("Target Act");
        mainAct.block = (Act a) => new() { { targetAct, Act.BlockType.Persistent } };


        // Perform target then main, Main should abort target
        targetAct.Perform();
        mainAct.Perform();


        // Assertions
        Assert.IsTrue(wasAborted, "Target act did not go through exit despite getting blocked!");
        Assert.IsTrue(recivedOutcome == Act.Outcome.Interrupted, $"Target act did not recieve 'Interrupted' outcome on block! recivedOutcome={recivedOutcome}");
        Assert.IsTrue(!targetAct.IsPerforming(), "Target act is still performing despite getting blocked!");


        yield return null;
    }
    [UnityTest]
    public IEnumerator UnblockedActCanPerform()  // 4
    {
        // Setup main and target act
        var mainAct = new ManualFinishAct();
        var targetAct = new Act();
        mainAct.Init("Main Act");
        targetAct.Init("Target Act");
        mainAct.block = (Act a) => new() { { targetAct, Act.BlockType.Persistent } };


        // Perform main act, Should block target act
        mainAct.Perform();


        // Finish main act, Should unblock target act
        mainAct.ManualFinish();
        targetAct.Perform();


        // Assertions
        Assert.IsTrue(!targetAct.IsBlocked(), "Target act is still blocked despite main act finishing!");
        Assert.IsTrue(targetAct.GetPerformCount() == 1, $"Target act did not perform despite getting unblocked! Perform Count={targetAct.GetPerformCount()}");


        yield return null;
    }


    [UnityTest]
    public IEnumerator InterruptBlockedActEndsPerform()  // 5
    {
        // Prerequisites
        var wasAborted = false;


        // Setup main and target act
        var mainAct = new ManualFinishAct();
        var targetAct = new ManualFinishAct();
        targetAct.OnPreExit += (a) =>
        {
            wasAborted = true;
        };
        mainAct.Init("Main Act");
        targetAct.Init("Target Act");
        mainAct.block = (Act a) => new() { { targetAct, Act.BlockType.Interrupt } };


        // Perform target then main, Main should end target via interrupt block
        targetAct.Perform();
        mainAct.Perform();


        // Assertions
        Assert.IsTrue(wasAborted, "Target act did not go through exit despite interrupt block!");
        Assert.IsTrue(!targetAct.IsPerforming(), "Target act is still performing despite interrupt block!");


        yield return null;
    }
    [UnityTest]
    public IEnumerator InterruptBlockedActCanReperform()  // 6
    {
        // Setup main and target act
        var mainAct = new ManualFinishAct();
        var targetAct = new Act();
        mainAct.Init("Main Act");
        targetAct.Init("Target Act");
        mainAct.block = (Act a) => new() { { targetAct, Act.BlockType.Interrupt } };


        // Perform main act, Should interrupt block target act once
        mainAct.Perform();
        targetAct.Perform();


        // Assertions
        Assert.IsTrue(mainAct.IsPerforming(), "Main act is not performing!");
        Assert.IsTrue(!targetAct.IsBlocked(), "Target act is still blocked despite interrupt block!");
        Assert.IsTrue(targetAct.GetPerformCount() == 1, $"Target act did not perform despite interrupt block being one time only! Perform Count={targetAct.GetPerformCount()}");


        yield return null;
    }


    [UnityTest]
    public IEnumerator AddSelfToBlock()  // 7
    {
        // Setup main act
        var mainAct = new ManualFinishAct();
        mainAct.Init("Main Act");


        // Try adding self to block list
        mainAct.block = (Act a) => new() { { mainAct, Act.BlockType.Persistent } };
        mainAct.Perform();


        // Assertions
        Assert.IsTrue(mainAct.IsPerforming(), "Main act did not perform despite trying to add self to block being a no op!");
        Assert.IsTrue(!mainAct.IsBlocked(), "Main act got blocked despite trying to add self to block being reserved for enable disable!");


        yield return null;
    }
}
