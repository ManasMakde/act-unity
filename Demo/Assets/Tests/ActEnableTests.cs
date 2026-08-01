using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;


// 1. Is enable changed action broadcasted with correct arguments when enabled disabled?
// 1. Is enable changed action not called when blocked unblocked?
// 1. Does disabling abort the act?


public class ActEnableTests
{
    [UnityTest]
    public IEnumerator OnEnableChanged()
    {
        // Prerequisites
        bool wasEnableChangedInvoked = false;
        Act enableChangedArg1 = null;
        bool enableChangedArg2 = true;


        // Perform Act
        var act = new Act();
        act.OnEnableChanged += (a, newIsEnabled) =>
        {
            wasEnableChangedInvoked = true;
            enableChangedArg1 = a;
            enableChangedArg2 = newIsEnabled;
        };
        act.Init("Test Act");
        act.SetEnabled(false);


        // Assertions disable
        Assert.IsTrue(wasEnableChangedInvoked, "OnEnableChanged not invoked on disable!");
        Assert.IsTrue(enableChangedArg1 == act, $"OnEnableChanged first argument is invalid! Arg1='{enableChangedArg1}'");
        Assert.IsTrue(enableChangedArg2 == false, $"OnEnableChanged second argument is invalid on disable! Arg2='{enableChangedArg2}'");


        // Reset and enable back
        wasEnableChangedInvoked = false;
        act.SetEnabled(true);


        // Assertions enable
        Assert.IsTrue(wasEnableChangedInvoked, "OnEnableChanged not invoked on enable!");
        Assert.IsTrue(enableChangedArg2 == true, $"OnEnableChanged second argument is invalid on enable! Arg2='{enableChangedArg2}'");


        yield return null;
    }
    [UnityTest]
    public IEnumerator EnableChangedNotCalledOnBlock()
    {
        // Prerequisites
        bool wasEnableChangedInvoked = false;


        // Perform Act
        var act = new Act();
        var blockerAct = new WaitInfiniAct();
        act.OnEnableChanged += (a, newIsEnabled) =>
        {
            wasEnableChangedInvoked = true;
        };
        act.Init("Test Act");
        blockerAct.Init("Blocker Act");
        blockerAct.Perform();


        // Block act using ongoing blocker
        blockerAct.AddToBlock(new List<Act> { act });


        // Assertions blocked
        Assert.IsTrue(act.IsBlocked(), "Act was not blocked!");
        Assert.IsFalse(wasEnableChangedInvoked, "OnEnableChanged invoked on block!");


        // Unblock act
        blockerAct.RemoveFromBlock(new List<Act> { act });


        // Assertions unblocked
        Assert.IsFalse(act.IsBlocked(), "Act was not unblocked!");
        Assert.IsFalse(wasEnableChangedInvoked, "OnEnableChanged invoked on unblock!");


        yield return null;
    }
    [UnityTest]
    public IEnumerator DisablingAbortsAct()
    {
        // Perform Act
        var act = new WaitInfiniAct();
        act.Init("Test Act");
        act.Perform();

        var wasOngoing = act.IsOngoing();

        act.SetEnabled(false);

        var isOngoingAfterDisable = act.IsOngoing();


        // Assertions
        Assert.IsTrue(wasOngoing, "Act was not ongoing before disable!");
        Assert.IsFalse(isOngoingAfterDisable, "Act was not aborted on disable!");


        yield return null;
    }
}
