using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;


// 1. Are "pre cleanup" & "post cleanup" actions being broadcasted (with correct arguments)?
// 1. Is Cleanup() being invoked?
// 1. Is theater set to null after deinitialization?
// 1. Does act abort on deinitialization?
// 1. Does prologue act abort on deinitialization?
// 1. Does PerformCount() reset to 0 after deinitialization?
// 1. After Deinit() is the act removed from the theater's allActs & ongoing acts?


public class ActDeinitializeTests
{
    [UnityTest]
    public IEnumerator OnPreAndPostCleanup()
    {
        // Prerequisites
        bool wasPreCleanupInvoked = false;
        Act preCleanupArg1 = null;
        bool wasPostCleanupInvoked = false;
        Act postCleanupArg1 = null;


        // Perform Act
        var act = new Act();
        act.OnPreCleanup += (a) => { wasPreCleanupInvoked = true; preCleanupArg1 = a; };
        act.OnPostCleanup += (a) => { wasPostCleanupInvoked = true; postCleanupArg1 = a; };
        act.Init("Test Act");
        act.Deinit();


        // Assertions
        Assert.IsTrue(wasPreCleanupInvoked, "OnPreCleanup not invoked!");
        Assert.IsTrue(preCleanupArg1 == act, $"OnPreCleanup first argument is invalid! Arg1=`{preCleanupArg1}`");
        Assert.IsTrue(wasPostCleanupInvoked, "OnPostCleanup not invoked!");
        Assert.IsTrue(postCleanupArg1 == act, $"OnPostCleanup first argument is invalid! Arg1=`{preCleanupArg1}`");


        yield return null;
    }
    [UnityTest]
    public IEnumerator Cleanup()
    {
        // Perform Act
        var act = new CleanupAct();
        act.Init("Test Act");
        act.Deinit();


        // Assertions
        Assert.IsTrue(act.callCount == 1, $"Cleanup() not invoked exactly once! Call count={act.callCount}");


        yield return null;
    }
    [UnityTest]
    public IEnumerator TheaterNullAfterDeinit()
    {
        // Perform Act
        var theater = new GameObject().AddComponent<Theater>();
        var act = new Act();
        act.Init("Test Act", theater);
        act.Deinit();


        // Assertions
        Assert.IsTrue(act.GetTheater() == null, $"Theater is not null after Deinit()! Theater='{act.GetTheater()}'");


        yield return null;
    }
    [UnityTest]
    public IEnumerator AbortOnDeinit()
    {
        // Perform Act
        var act = new WaitInfiniAct();
        act.Init("Test Act");
        act.Perform();
        act.Deinit();


        // Assertions
        Assert.IsTrue(!act.IsOngoing(), "Act is still ongoing after Deinit()!");


        yield return null;
    }
    [UnityTest]
    public IEnumerator AbortPrologueOnDeinit()
    {
        // Prerequisites
        var prologueAct = new WaitInfiniAct();
        prologueAct.Init("Prologue Act");


        // Perform Act
        var mainAct = new Act();
        mainAct.Init("Main Act");
        mainAct.prologue = (a) => new() { prologueAct };
        mainAct.Perform();
        prologueAct.Deinit();


        // Assertions
        Assert.IsTrue(!mainAct.IsOngoing(), "Main act is still ongoing after Deinit()!");
        Assert.IsTrue(!prologueAct.IsOngoing(), "Prologue act is still ongoing after Deinit()!");


        yield return null;
    }
    [UnityTest]
    public IEnumerator PerformCountResetAfterDeinit()
    {
        // Perform Act
        var act = new Act();
        act.Init("Test Act");
        act.Perform();
        var performCountBeforeDeinit = act.GetPerformCount();
        act.Deinit();
        var performCountAfterDeinit = act.GetPerformCount();


        // Assertions
        Assert.IsTrue(performCountBeforeDeinit == 1, $"Act did not perform exactly once before deinit, Perform Count={performCountBeforeDeinit}");
        Assert.IsTrue(performCountAfterDeinit == 0, $"Perform count did not reset after deinit, Perform Count={performCountAfterDeinit}");


        yield return null;
    }
    [UnityTest]
    public IEnumerator RemovedFromTheaterAfterDeinit()
    {
        // Prerequisites
        var theaterObj = new GameObject().AddComponent<Theater>();


        // Perform Act
        var act = new WaitInfiniAct();
        act.Init("Test Act", theaterObj);
        act.Perform();
        var wasInAllActs = theaterObj.GetAllActs().Contains(act);
        act.Deinit();


        // Assertions
        Assert.IsTrue(wasInAllActs, "Act was never added to theater's allActs!");
        Assert.IsFalse(theaterObj.GetAllActs().Contains(act), "Act still present in theater's allActs after Deinit()!");
        Assert.IsFalse(theaterObj.AreAnyOngoing(), "Theater still reports act as ongoing after Deinit()!");


        UnityEngine.Object.Destroy(theaterObj.gameObject);
        yield return null;
    }
}
