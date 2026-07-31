using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;


// 1. Is correct status exiting applied?
// 1. Are pre exit and post exit actions being broadcasted with correct arguments?
// 1. Is Exit() being invoked?
// 1. Does invoking Abort() while exiting not exit the act again?
// 1. Is the status reset to None after exiting?
// 1. Is perform end action being invoked after exiting?


public class ActExitTests
{
    [UnityTest]
    public IEnumerator CorrectStatusExiting()
    {
        // Perform Act
        var preExitStatus = Act.Status.None;
        var postExitStatus = Act.Status.None;
        var act = new ManualFinishAct();
        act.OnPreExit += (a) =>
        {
            preExitStatus = act.GetStatus();
        };
        act.OnPostExit += (a) =>
        {
            postExitStatus = act.GetStatus();
        };
        act.Init("Test Act");
        act.Perform();
        act.ManualFinish();


        // Assertions
        Assert.IsTrue(preExitStatus == Act.Status.Exiting && postExitStatus == Act.Status.Exiting, $"Status is not 'Exiting' during Exit()! preExitStatus={preExitStatus}  postExitStatus={postExitStatus}");


        yield return null;
    }
    [UnityTest]
    public IEnumerator OnPreAndPostExit()
    {
        // Prerequisites
        bool wasPreExitInvoked = false;
        Act preExitArg1 = null;
        bool wasPostExitInvoked = false;
        Act postExitArg1 = null;


        // Perform Act
        var act = new ManualFinishAct();
        act.OnPreExit += (a) => { wasPreExitInvoked = true; preExitArg1 = a; };
        act.OnPostExit += (a) => { wasPostExitInvoked = true; postExitArg1 = a; };
        act.Init("Test Act");
        act.Perform();
        act.ManualFinish();


        // Assertions
        Assert.IsTrue(wasPreExitInvoked, "OnPreExit not invoked!");
        Assert.IsTrue(preExitArg1 == act, $"OnPreExit first argument is invalid! Arg1='{preExitArg1}'");
        Assert.IsTrue(wasPostExitInvoked, "OnPostExit not invoked!");
        Assert.IsTrue(postExitArg1 == act, $"OnPostExit first argument is invalid! Arg1='{postExitArg1}'");


        yield return null;
    }
    [UnityTest]
    public IEnumerator Exit()
    {
        // Perform Act
        var act = new ExitAct();
        act.Init("Test Act");
        act.Perform();


        // Assertions
        Assert.IsTrue(act.callCount == 1, $"Exit() not invoked exactly once! Call count='{act.callCount}'");


        yield return null;
    }
    [UnityTest]
    public IEnumerator PerformWhileExitingReperformable()
    {
        // Perform Act
        var act = new ManualFinishAct();
        act.canReperformOverride = true;
        act.OnPreExit += (a) =>
        {
            act.Perform();
        };
        act.Init("Test Act");
        act.Perform();
        act.ManualFinish();


        // Assertions
        Assert.IsTrue(act.GetPerformCount() == 1, $"Act reperformed while exiting despite canReperform true! Perform Count={act.GetPerformCount()}");


        yield return null;
    }
    [UnityTest]
    public IEnumerator AbortWhileExiting()
    {
        // Perform Act
        var preExitCallCount = 0;
        var act = new ManualFinishAct();
        act.OnPreExit += (a) =>
        {
            preExitCallCount++;
            act.Abort();
        };
        act.Init("Test Act");
        act.Perform();
        act.ManualFinish();


        // Assertions
        Assert.IsTrue(preExitCallCount == 1, $"Act exited more than once from calling Abort() while exiting! Call count={preExitCallCount}");


        yield return null;
    }
    [UnityTest]
    public IEnumerator StatusResetAfterExit()
    {
        // Perform Act
        var act = new ManualFinishAct();
        act.Init("Test Act");
        act.Perform();
        act.ManualFinish();

        var statusAfterExit = act.GetStatus();


        // Assertions
        Assert.IsTrue(statusAfterExit == Act.Status.None, $"Status did not reset to 'None' after exiting! Status='{statusAfterExit}'");


        yield return null;
    }
    [UnityTest]
    public IEnumerator OnPerformEndAfterExit()
    {
        // Prerequisites
        bool wasPerformEndInvoked = false;
        Act performEndArg1 = null;
        Act.Status statusDuringOnPerformEnd = Act.Status.Entering;


        // Perform Act
        var act = new ManualFinishAct();
        act.OnPerformEnd += (a) =>
        {
            statusDuringOnPerformEnd = a.GetStatus();
            wasPerformEndInvoked = true;
            performEndArg1 = a;
        };
        act.Init("Test Act");
        act.Perform();
        act.ManualFinish();


        // Assertions
        Assert.IsTrue(wasPerformEndInvoked, "OnPerformEnd not invoked after exiting!");
        Assert.IsTrue(statusDuringOnPerformEnd == Act.Status.None, $"Incorrect status in OnPerformEnd! status={statusDuringOnPerformEnd}  expected status={Act.Status.None}");
        Assert.IsTrue(performEndArg1 == act, $"OnPerformEnd first argument is invalid! Arg1='{performEndArg1}'");


        yield return null;
    }
}
