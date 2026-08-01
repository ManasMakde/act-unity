using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;


// 1. Are pre enter and post enter actions being broadcasted with correct arguments?
// 1. Is Enter() being invoked?
// 1. Does returning outcome pending make the act wait until Finish() is invoked?
// 1. Does using Finish() pass accurate outcomes to Exit()?


public class ActEnterTests
{
    [UnityTest]
    public IEnumerator OnPreAndPostEnter()
    {
        // Prerequisites
        bool wasPreEnterInvoked = false;
        Act preEnterArg1 = null;
        bool wasPostEnterInvoked = false;
        Act postEnterArg1 = null;


        // Perform Act
        var act = new Act();
        act.OnPreEnter += (a) => { wasPreEnterInvoked = true; preEnterArg1 = a; };
        act.OnPostEnter += (a) => { wasPostEnterInvoked = true; postEnterArg1 = a; };
        act.Init("Test Act");
        act.Perform();


        // Assertions
        Assert.IsTrue(wasPreEnterInvoked, "OnPreEnter not invoked!");
        Assert.IsTrue(preEnterArg1 == act, $"OnPreEnter first argument is invalid! Arg1='{preEnterArg1}'");
        Assert.IsTrue(wasPostEnterInvoked, "OnPostEnter not invoked!");
        Assert.IsTrue(postEnterArg1 == act, $"OnPostEnter first argument is invalid! Arg1='{postEnterArg1}'");


        yield return null;
    }
    [UnityTest]
    public IEnumerator Enter()
    {
        // Perform Act
        var act = new EnterAct();
        act.Init("Test Act");
        act.Perform();


        // Assertions
        Assert.IsTrue(act.callCount == 1, $"Enter() not invoked exactly once! Call count='{act.callCount}'");


        yield return null;
    }
    [UnityTest]
    public IEnumerator EnterFinish()
    {
        // Perform Act
        var wentThroughExit = false;
        var act = new ManualFinishAct();
        act.OnPreExit += (a) =>
        {
            wentThroughExit = true;
        };
        act.Init("Test Act");
        act.Perform();

        var actStatus = act.GetStatus();
        var wasOngoing = act.IsOngoing();

        act.ManualFinish();

        var didComplete = !act.IsOngoing();


        // Assertions
        Assert.IsTrue(actStatus == Act.Status.Entering, $"Act did not have status 'Entering' despite pending outcome! Status='{actStatus}'");
        Assert.IsTrue(wasOngoing, "Act is not ongoing despite pending outcome!");
        Assert.IsTrue(didComplete && act.GetPerformCount() == 1, $"Act is not exit despite calling Finish()! Perform Count={act.GetPerformCount()}");
        Assert.IsTrue(wentThroughExit, $"Act is not go through exit on calling Finish()!");

        yield return null;
    }
    [UnityTest]
    public IEnumerator FinishPassesAccurateOutcome()
    {
        // Interrupted outcome
        {
            Act.Outcome givenOutcome = Act.Outcome.Interrupted;
            Act.Outcome recivedOutcome = Act.Outcome.Pending;
            var act = new ManualFinishAct();
            act.OnPreExit += (a) =>
            {
                recivedOutcome = a.GetOutcome();
            };
            act.Init("Test Act");
            act.Perform();
            act.ManualFinish(givenOutcome);


            // Assertions
            Assert.IsTrue(recivedOutcome == givenOutcome, $"Failed to pass 'Interrupted' outcome to Exit()! givenOutcome={givenOutcome}  recivedOutcome={recivedOutcome}");
        }

        // Failure outcome
        {
            Act.Outcome givenOutcome = Act.Outcome.Failure;
            Act.Outcome recivedOutcome = Act.Outcome.Pending;
            var act = new ManualFinishAct();
            act.OnPreExit += (a) =>
            {
                recivedOutcome = a.GetOutcome();
            };
            act.Init("Test Act");
            act.Perform();
            act.ManualFinish(givenOutcome);


            // Assertions
            Assert.IsTrue(recivedOutcome == givenOutcome, $"Failed to pass 'Failure' outcome to Exit()! givenOutcome={givenOutcome}  recivedOutcome={recivedOutcome}");
        }

        // Success outcome
        {
            Act.Outcome givenOutcome = Act.Outcome.Success;
            Act.Outcome recivedOutcome = Act.Outcome.Pending;
            var act = new ManualFinishAct();
            act.OnPreExit += (a) =>
            {
                recivedOutcome = a.GetOutcome();
            };
            act.Init("Test Act");
            act.Perform();
            act.ManualFinish(givenOutcome);


            // Assertions
            Assert.IsTrue(recivedOutcome == givenOutcome, $"Failed to pass 'Success' outcome to Exit()! givenOutcome={givenOutcome}  recivedOutcome={recivedOutcome}");
        }

        // Retry outcome
        {
            Act.Outcome givenOutcome = Act.Outcome.Retry;
            Act.Outcome recivedOutcome = Act.Outcome.Pending;
            var act = new ManualFinishAct();
            act.OnPreExit += (a) =>
            {
                recivedOutcome = a.GetOutcome();
            };
            act.Init("Test Act");
            act.Perform();
            act.ManualFinish(givenOutcome);


            // Assertions
            Assert.IsTrue(recivedOutcome == givenOutcome, $"Failed to pass 'Retry' outcome to Exit()! givenOutcome={givenOutcome}  recivedOutcome={recivedOutcome}");
        }

        yield return null;
    }
}
