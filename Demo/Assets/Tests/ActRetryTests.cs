using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;


// 1. Does retry outcome make the act retry perform?
// 1. Does Retry() work externally?
// 1. Does retry cancel prologues?
// 1. Does retry not cancel epilogues?
// 1. Does Retry() perform the act even if not ongoing?
// 1. Does failing to retry change the outcome to failure?


public class ActRetryTests
{
    [UnityTest]
    public IEnumerator RetryOutcome()
    {
        // Perform Act
        var act = new RetryAct();
        act.isVerbose = true;
        act.retryLimit = 1;
        act.Init("Test Act");
        act.Perform();


        // Assertions
        Assert.IsTrue(act.enterCallCount == 2, $"Enter() did not run again after internal retry! Call count={act.enterCallCount}");
        Assert.IsTrue(act.GetPerformCount() == 2, $"Act did not perform again after internal retry! Perform Count={act.GetPerformCount()}");
        Assert.IsTrue(act.GetOutcome() == Act.Outcome.Success, $"Act did not end with success after retrying! Outcome={act.GetOutcome()}");

        yield return null;
    }
    [UnityTest]
    public IEnumerator RetryExternal()
    {
        // Perform Act
        var act = new WaitInfiniAct();
        act.Init("Test Act");
        act.Perform();
        act.Retry();


        // Assertions
        Assert.IsTrue(act.GetPerformCount() == 2, $"Act did not perform again after calling Retry() externally! Perform Count={act.GetPerformCount()}");
        Assert.IsTrue(act.GetStatus() == Act.Status.Entering, $"Act did not re enter after retrying! Status={act.GetStatus()}");

        yield return null;
    }
    [UnityTest]
    public IEnumerator RetryCancelsPrologues()
    {
        // Perform Act
        var didPrologue = false;
        var prologueAct = new ManualFinishAct();
        prologueAct.Init("Prologue Act");
        var act = new WaitInfiniAct();
        act.prologue = (a) =>
        {
            if (didPrologue)
            {
                return new List<Act>();
            }
            didPrologue = true;
            return new List<Act> { prologueAct };
        };
        act.Init("Test Act");
        act.Perform();
        act.Retry();


        // Assertions
        Assert.IsTrue(prologueAct.GetPerformCount() == 1, $"Prologue act did not exactly once! Perform Count={prologueAct.GetPerformCount()}");
        Assert.IsTrue(prologueAct.GetOutcome() == Act.Outcome.Interrupted, $"Prologue act was not interrupted after retry! Outcome={prologueAct.GetOutcome()}");
        Assert.IsFalse(prologueAct.IsOngoing(), $"Prologue act is still ongoing after retry cancelled it!");
        Assert.IsTrue(act.GetStatus() == Act.Status.Entering, $"Act did not skip cancelled prologue and enter! Status={act.GetStatus()}");

        yield return null;
    }
    [UnityTest]
    public IEnumerator RetryDoesNotCancelEpilogues()
    {
        // Perform Act
        var prologueAct = new WaitInfiniAct();
        prologueAct.Init("Prologue Act");
        var act = new WaitInfiniAct();
        act.prologue = (a) => new List<Act> { prologueAct };
        act.Init("Test Act");
        act.Perform();

        var statusBeforeRetry = act.GetStatus();
        prologueAct.Retry();
        var statusAfterRetry = act.GetStatus();


        // Assertions
        Assert.IsTrue(statusBeforeRetry == Act.Status.Prologuing, $"Act did not wait on prologue before retry! Status={statusBeforeRetry}");
        Assert.IsTrue(statusAfterRetry == Act.Status.Prologuing, $"Act got cancelled after prologue retried! Status={statusAfterRetry}");
        Assert.IsTrue(act.IsOngoing(), "Act stopped being ongoing after prologue retried!");

        yield return null;
    }
    [UnityTest]
    public IEnumerator RetryPerformsWhenNotOngoing()
    {
        // Perform Act
        var act = new EnterAct();
        act.Init("Test Act");
        act.Retry();


        // Assertions
        Assert.IsTrue(act.GetPerformCount() == 1, $"Act did not perform after calling Retry() while not ongoing! Perform Count={act.GetPerformCount()}");
        Assert.IsTrue(act.callCount == 1, $"Enter() was not invoked after Retry() while not ongoing! Call count={act.callCount}");

        yield return null;
    }
    [UnityTest]
    public IEnumerator RetryFailureChangesOutcome()
    {
        // Perform Act
        var act = new RetryOnceThenFailAct();
        act.Init("Test Act");
        act.Perform();


        // Assertions
        Assert.IsTrue(act.GetOutcome() == Act.Outcome.Failure, $"Outcome did not change to failure after failing to retry! Outcome={act.GetOutcome()}");
        Assert.IsFalse(act.IsOngoing(), "Act is still ongoing despite failing to retry!");

        yield return null;
    }
}
