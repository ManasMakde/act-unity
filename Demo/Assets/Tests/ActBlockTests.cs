using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;


// 1. Is "block changed" action being broadcasted (with correct arguments) when blocked/unblocked?
// 1. Is "block changed" action not being broadcasted when enabled/disabled?
// 1. Does blocking abort the act?
// 1. Can a persistantly unblocked act perform?

// 1. Does oneshot block stop the act's ongoing perform?
// 1. Can a oneshot blocked act perform despite blocker act still ongoing?

// 1. Does adding an act to the main act (which is ongoing) block the act?
// 1. Does removing a blocked act from the main act (which is ongoing) unblock that act?
// 1. Does persistent blocking fail when adding self act to block?


public class ActBlockTests
{
    [UnityTest]
    public IEnumerator OnBlockUnblock()
    {
        // Prerequisites
        bool wasBlockChangedInvoked = false;
        Act blockChangedArg1 = null;
        Act blockChangedArg2 = null;
        Act.BlockType blockChangedArg3 = Act.BlockType.Oneshot;
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
        mainAct.AddToBlock(new List<Act> { targetAct });


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
    public IEnumerator BlockChangeNotOnEnableDisable()
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
    public IEnumerator BlockingAbortsAct()
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
        mainAct.AddToBlock(new List<Act> { targetAct });


        // Perform target then main, Main should abort target
        targetAct.Perform();
        mainAct.Perform();


        // Assertions
        Assert.IsTrue(wasAborted, "Target act did not go through exit despite getting blocked!");
        Assert.IsTrue(recivedOutcome == Act.Outcome.Interrupted, $"Target act did not recieve 'Interrupted' outcome on block! recivedOutcome={recivedOutcome}");
        Assert.IsTrue(!targetAct.IsOngoing(), "Target act is still ongoing despite getting blocked!");


        yield return null;
    }
    [UnityTest]
    public IEnumerator UnblockedActCanPerform()
    {
        // Setup main and target act
        var mainAct = new ManualFinishAct();
        var targetAct = new Act();
        mainAct.Init("Main Act");
        targetAct.Init("Target Act");
        mainAct.AddToBlock(new List<Act> { targetAct });


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
    public IEnumerator OneshotBlockedActEndsPerform()
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
        mainAct.AddToBlock(new List<Act> { targetAct }, Act.BlockType.Oneshot);


        // Perform target then main, Main should end target via oneshot block
        targetAct.Perform();
        mainAct.Perform();


        // Assertions
        Assert.IsTrue(wasAborted, "Target act did not go through exit despite oneshot block!");
        Assert.IsTrue(!targetAct.IsOngoing(), "Target act is still ongoing despite oneshot block!");


        yield return null;
    }
    [UnityTest]
    public IEnumerator OneshotBlockedActCanReperform()
    {
        // Setup main and target act
        var mainAct = new ManualFinishAct();
        var targetAct = new Act();
        mainAct.Init("Main Act");
        targetAct.Init("Target Act");
        mainAct.AddToBlock(new List<Act> { targetAct }, Act.BlockType.Oneshot);


        // Perform main act, Should oneshot block target act once
        mainAct.Perform();
        targetAct.Perform();


        // Assertions
        Assert.IsTrue(mainAct.IsOngoing(), "Main act is not ongoing!");
        Assert.IsTrue(!targetAct.IsBlocked(), "Target act is still blocked despite oneshot block!");
        Assert.IsTrue(targetAct.GetPerformCount() == 1, $"Target act did not perform despite oneshot block being one time only! Perform Count={targetAct.GetPerformCount()}");


        yield return null;
    }


    [UnityTest]
    public IEnumerator AddToBlockWhileOngoing()
    {
        // Setup main and target act
        var mainAct = new ManualFinishAct();
        var targetAct = new Act();
        mainAct.Init("Main Act");
        targetAct.Init("Target Act");


        // Perform main act first, Then add target to block
        mainAct.Perform();
        mainAct.AddToBlock(new List<Act> { targetAct });


        // Assertions
        Assert.IsTrue(targetAct.IsBlocked(), "Target act is not blocked despite being added while main act is ongoing!");


        yield return null;
    }
    [UnityTest]
    public IEnumerator RemoveFromBlockWhileOngoing()
    {
        // Setup main and target act
        var mainAct = new ManualFinishAct();
        var targetAct = new Act();
        mainAct.Init("Main Act");
        targetAct.Init("Target Act");
        mainAct.AddToBlock(new List<Act> { targetAct });


        // Perform main act, Should block target act
        mainAct.Perform();


        // Remove target from block while main act still ongoing
        mainAct.RemoveFromBlock(new List<Act> { targetAct });


        // Assertions
        Assert.IsTrue(mainAct.IsOngoing(), "Main act is not ongoing!");
        Assert.IsTrue(!targetAct.IsBlocked(), "Target act is still blocked despite being removed while main act is ongoing!");


        yield return null;
    }
    [UnityTest]
    public IEnumerator AddSelfToBlock()
    {
        // Setup main act
        var mainAct = new ManualFinishAct();
        mainAct.Init("Main Act");


        // Try adding self to block list
        mainAct.AddToBlock(new List<Act> { mainAct });
        mainAct.Perform();


        // Assertions
        Assert.IsTrue(mainAct.IsOngoing(), "Main act did not perform despite trying to add self to block being a no op!");
        Assert.IsTrue(!mainAct.IsBlocked(), "Main act got blocked despite trying to add self to block being reserved for enable disable!");


        yield return null;
    }
}
