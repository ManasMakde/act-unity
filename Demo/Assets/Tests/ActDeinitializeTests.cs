using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;


// 1. Are "pre cleanup" & "post cleanup" actions being broadcasted (with correct arguments)?
// 2. Is Cleanup() being invoked?
// 3. Does calling Deinit() twice fail?
// 4. Is theater set to null after deinitialization?
// 5. Does act abort on deinitialization?
// 6. Does prologue act abort on deinitialization?
// 7. Does PerformCount() reset to 0 after deinitialization?
// 8. After Deinit() is the act removed from the theater's allActs & performing acts?


public class ActDeinitializeTests
{
    [UnityTest]
    public IEnumerator OnPreAndPostCleanup()  // 1
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
    public IEnumerator Cleanup()  // 2
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
    public IEnumerator DeinitCalledTwiceFails()  // 3
    {
        // Prerequisites
        var preCleanupCount = 0;
        var postCleanupCount = 0;

        // Perform Act
        var act = new Act();
        act.OnPreCleanup += (a) => { preCleanupCount++; };
        act.OnPostCleanup += (a) => { postCleanupCount++; };
        act.Init("Test Act");
        act.Deinit();
        act.Deinit();


        // Assertions
        Assert.IsTrue(preCleanupCount == 1, $"OnPreCleanup invoked more than once despite calling Deinit() twice! Count={preCleanupCount}");
        Assert.IsTrue(postCleanupCount == 1, $"OnPostCleanup invoked more than once despite calling Deinit() twice! Count={postCleanupCount}");

        yield return null;
    }
    [UnityTest]
    public IEnumerator TheaterNullAfterDeinit()  // 4
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
    public IEnumerator AbortOnDeinit()  // 5
    {
        // Perform Act
        var act = new WaitInfiniAct();
        act.Init("Test Act");
        act.Perform();
        act.Deinit();


        // Assertions
        Assert.IsTrue(!act.IsPerforming(), "Act is still performing after Deinit()!");


        yield return null;
    }
    [UnityTest]
    public IEnumerator AbortPrologueOnDeinit()  // 6
    {
        // Prerequisites
        var prologueAct = new WaitInfiniAct();
        prologueAct.Init("Prologue Act");


        // Perform Act
        var mainAct = new Act();
        mainAct.Init("Main Act");
        mainAct.prologue = (a) => new() { new() { prologueAct } };
        mainAct.Perform();
        prologueAct.Deinit();


        // Assertions
        Assert.IsTrue(!mainAct.IsPerforming(), "Main act is still performing after Deinit()!");
        Assert.IsTrue(!prologueAct.IsPerforming(), "Prologue act is still performing after Deinit()!");


        yield return null;
    }
    [UnityTest]
    public IEnumerator PerformCountResetAfterDeinit()  // 7
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
    public IEnumerator RemovedFromTheaterAfterDeinit()  // 8
    {
        // Prerequisites
        var theaterObj = new GameObject().AddComponent<Theater>();


        // Perform Act
        var act = new WaitInfiniAct();
        act.Init("Test Act", theaterObj);
        act.Perform();
        var wasInAllActs = theaterObj.HasAct(act);
        act.Deinit();


        // Assertions
        Assert.IsTrue(wasInAllActs, "Act was never added to theater's allActs!");
        Assert.IsFalse(theaterObj.HasAct(act), "Act still present in theater's allActs after Deinit()!");
        Assert.IsFalse(theaterObj.AreAnyPerforming(), "Theater still reports act as performing after Deinit()!");


        UnityEngine.Object.Destroy(theaterObj.gameObject);
        yield return null;
    }
}
