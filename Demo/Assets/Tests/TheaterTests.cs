using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;


// 1. Is enabled changed action broadcasted with correct arguments?
// 1. Is perform start action broadcasted with correct arguments?
// 1. Is perform end action broadcasted with correct arguments?
// 1. Is all perform end action broadcasted with correct arguments?

// 1. Does IsEnabled() return correct state?
// 1. Does disabling theater abort all acts?
// 1. Can disabled theater acts perform?

// 1. Does AbortAll() abort all acts?
// 1. Can all acts perform after AbortAll() is invoked?

// 1. Does AreAnyOngoing() return correct state?
// 1. Does GetAllActs() return all acts assigned to theater?


public class TheaterTests
{
    [UnityTest]
    public IEnumerator EnabledChanged()
    {
        // Prerequisites
        bool wasInvoked = false;
        Theater arg1 = null;
        bool arg2 = false;

        // Setup Theater
        var theaterObj = new GameObject("Test Theater");
        var theater = theaterObj.AddComponent<Theater>();
        theater.OnEnableChanged += (t, newIsEnabled) =>
        {
            wasInvoked = true;
            arg1 = t;
            arg2 = newIsEnabled;
        };

        theater.SetEnabled(false);


        // Assertions
        Assert.IsTrue(wasInvoked, "OnEnableChanged not invoked!");
        Assert.IsTrue(arg1 == theater, $"OnEnableChanged first argument is invalid! Arg1='{arg1}'");
        Assert.IsTrue(arg2 == false, $"OnEnableChanged second argument is invalid! Arg2='{arg2}'");

        yield return null;
    }
    [UnityTest]
    public IEnumerator PerformStart()
    {
        // Prerequisites
        bool wasInvoked = false;
        Theater arg1 = null;
        Act arg2 = null;

        // Setup Theater
        var theaterObj = new GameObject("Test Theater");
        var theater = theaterObj.AddComponent<Theater>();
        theater.OnPerformStart += (t, a) =>
        {
            wasInvoked = true;
            arg1 = t;
            arg2 = a;
        };

        // Perform Act
        var act = new ManualFinishAct();
        act.Init("Test Act", theater);
        act.Perform();


        // Assertions
        Assert.IsTrue(wasInvoked, "OnPerformStart not invoked!");
        Assert.IsTrue(arg1 == theater, $"OnPerformStart first argument is invalid! Arg1='{arg1}'");
        Assert.IsTrue(arg2 == act, $"OnPerformStart second argument is invalid! Arg2='{arg2}'");

        yield return null;
    }
    [UnityTest]
    public IEnumerator PerformEnd()
    {
        // Prerequisites
        bool wasInvoked = false;
        Theater arg1 = null;
        Act arg2 = null;

        // Setup Theater
        var theaterObj = new GameObject("Test Theater");
        var theater = theaterObj.AddComponent<Theater>();
        theater.OnPerformEnd += (t, a) =>
        {
            wasInvoked = true;
            arg1 = t;
            arg2 = a;
        };

        // Perform Act
        var act = new ManualFinishAct();
        act.Init("Test Act", theater);
        act.Perform();
        act.ManualFinish();


        // Assertions
        Assert.IsTrue(wasInvoked, "OnPerformEnd not invoked!");
        Assert.IsTrue(arg1 == theater, $"OnPerformEnd first argument is invalid! Arg1='{arg1}'");
        Assert.IsTrue(arg2 == act, $"OnPerformEnd second argument is invalid! Arg2='{arg2}'");

        yield return null;
    }
    [UnityTest]
    public IEnumerator AllPerformEnd()
    {
        // Prerequisites
        int invokeCount = 0;
        Theater arg1 = null;

        // Setup Theater
        var theaterObj = new GameObject("Test Theater");
        var theater = theaterObj.AddComponent<Theater>();
        theater.OnAllPerformEnd += (t) =>
        {
            invokeCount++;
            arg1 = t;
        };

        // Perform Acts
        var actA = new ManualFinishAct();
        actA.Init("Test Act A", theater);
        actA.Perform();
        var actB = new ManualFinishAct();
        actB.Init("Test Act B", theater);
        actB.Perform();

        // Finish first act only
        actA.ManualFinish();


        // Assertions
        Assert.IsTrue(invokeCount == 0, $"OnAllPerformEnd invoked despite one act still ongoing! Invoke Count='{invokeCount}'");

        // Finish second act
        actB.ManualFinish();


        // Assertions
        Assert.IsTrue(invokeCount == 1, $"OnAllPerformEnd not invoked exactly once! Invoke Count='{invokeCount}'");
        Assert.IsTrue(arg1 == theater, $"OnAllPerformEnd argument is invalid! Arg1='{arg1}'");

        yield return null;
    }



    [UnityTest]
    public IEnumerator IsEnabled()
    {
        // Setup Theater
        var theaterObj = new GameObject("Test Theater");
        var theater = theaterObj.AddComponent<Theater>();

        var isEnabled0 = theater.IsEnabled();
        theater.SetEnabled(true);
        var isEnabled1 = theater.IsEnabled();
        theater.SetEnabled(false);
        var isEnabled2 = theater.IsEnabled();
        theater.SetEnabled(true);
        var isEnabled3 = theater.IsEnabled();


        // Assertions
        Assert.IsTrue(isEnabled0, "IsEnabled() is false despite theater being enabled by default!");
        Assert.IsTrue(isEnabled1, "IsEnabled() is false despite theater being set enabled!");
        Assert.IsTrue(!isEnabled2, "IsEnabled() is true despite theater being set disabled!");
        Assert.IsTrue(isEnabled3, "IsEnabled() is false despite theater being set enabled again!");

        yield return null;
    }
    [UnityTest]
    public IEnumerator DisablingTheaterAbortsAllActs()
    {
        // Setup Theater
        var theaterObj = new GameObject("Test Theater");
        var theater = theaterObj.AddComponent<Theater>();

        // Perform Acts
        var actA = new WaitInfiniAct();
        actA.Init("Test Act A", theater);

        var actB = new WaitInfiniAct();
        actB.Init("Test Act B", theater);

        var actC = new WaitInfiniAct();
        actC.Init("Test Act C", theater);


        actA.Perform();
        actB.Perform();
        actC.Perform();

        // Disable Theater
        theater.SetEnabled(false);


        // Assertions
        Assert.IsTrue(!actA.IsOngoing(), "Act A still ongoing despite theater being disabled!");
        Assert.IsTrue(!actB.IsOngoing(), "Act B still ongoing despite theater being disabled!");
        Assert.IsTrue(!actC.IsOngoing(), "Act C still ongoing despite theater being disabled!");

        yield return null;
    }
    [UnityTest]
    public IEnumerator DisabledTheaterActsCannotPerform()
    {
        // Setup Theater
        var theaterObj = new GameObject("Test Theater");
        var theater = theaterObj.AddComponent<Theater>();
        theater.SetEnabled(false);

        // Perform Acts
        var actA = new WaitInfiniAct();
        actA.Init("Test Act A", theater);

        var actB = new WaitInfiniAct();
        actB.Init("Test Act B", theater);

        var actC = new WaitInfiniAct();
        actC.Init("Test Act C", theater);


        actA.Perform();
        actB.Perform();
        actC.Perform();


        // Assertions
        Assert.IsTrue(!actA.IsOngoing(), "Act A is ongoing despite theater being disabled!");
        Assert.IsTrue(!actB.IsOngoing(), "Act B is ongoing despite theater being disabled!");
        Assert.IsTrue(!actC.IsOngoing(), "Act C is ongoing despite theater being disabled!");

        yield return null;
    }



    [UnityTest]
    public IEnumerator AbortAll()
    {
        // Setup Theater
        var theaterObj = new GameObject("Test Theater");
        var theater = theaterObj.AddComponent<Theater>();

        // Perform Acts
        var actA = new WaitInfiniAct();
        actA.Init("Test Act A", theater);


        var actB = new WaitInfiniAct();
        actB.Init("Test Act B", theater);


        var actC = new WaitInfiniAct();
        actC.Init("Test Act C", theater);


        actA.Perform();
        actB.Perform();
        actC.Perform();


        // Abort All
        theater.AbortAll();


        // Assertions
        Assert.IsTrue(!actA.IsOngoing(), "Act A still ongoing despite AbortAll() being invoked!");
        Assert.IsTrue(!actB.IsOngoing(), "Act B still ongoing despite AbortAll() being invoked!");
        Assert.IsTrue(!actC.IsOngoing(), "Act C still ongoing despite AbortAll() being invoked!");

        yield return null;
    }
    [UnityTest]
    public IEnumerator PerformAfterAbortAll()
    {
        // Setup Theater
        var theaterObj = new GameObject("Test Theater");
        var theater = theaterObj.AddComponent<Theater>();

        // Perform Acts
        var actA = new WaitInfiniAct();
        actA.Init("Test Act A", theater);
        var actB = new WaitInfiniAct();
        actB.Init("Test Act B", theater);
        var actC = new WaitInfiniAct();
        actC.Init("Test Act C", theater);

        actA.Perform();
        actB.Perform();
        actC.Perform();


        // Abort All
        theater.AbortAll();


        // Reperform Acts
        actA.Perform();
        actB.Perform();
        actC.Perform();


        // Assertions
        Assert.IsTrue(actA.IsOngoing(), "Act A did not perform after AbortAll() was invoked!");
        Assert.IsTrue(actB.IsOngoing(), "Act B did not perform after AbortAll() was invoked!");
        Assert.IsTrue(actC.IsOngoing(), "Act C did not perform after AbortAll() was invoked!");

        yield return null;
    }



    [UnityTest]
    public IEnumerator AreAnyOngoing()
    {
        // All acts ongoing
        {
            // Setup Theater
            var theaterObj = new GameObject("Test Theater");
            var theater = theaterObj.AddComponent<Theater>();

            // Perform Acts
            var actA = new WaitInfiniAct();
            actA.Init("Test Act A", theater);
            actA.Perform();

            var actB = new WaitInfiniAct();
            actB.Init("Test Act B", theater);
            actB.Perform();

            var actC = new WaitInfiniAct();
            actC.Init("Test Act C", theater);
            actC.Perform();


            // Assertions
            Assert.IsTrue(theater.AreAnyOngoing(), "AreAnyOngoing() is false despite all acts ongoing!");
        }

        // One act ongoing rest not
        {
            // Setup Theater
            var theaterObj = new GameObject("Test Theater");
            var theater = theaterObj.AddComponent<Theater>();

            // Perform Acts
            var actA = new WaitInfiniAct();
            actA.Init("Test Act A", theater);
            actA.Perform();

            var actB = new ManualFinishAct();
            actB.Init("Test Act B", theater);
            actB.Perform();
            actB.ManualFinish();

            var actC = new ManualFinishAct();
            actC.Init("Test Act C", theater);
            actC.Perform();
            actC.ManualFinish();


            // Assertions
            Assert.IsTrue(theater.AreAnyOngoing(), "AreAnyOngoing() is false despite one act still ongoing!");
        }

        // No acts ongoing
        {
            // Setup Theater
            var theaterObj = new GameObject("Test Theater");
            var theater = theaterObj.AddComponent<Theater>();

            // Perform Acts
            var actA = new ManualFinishAct();
            actA.Init("Test Act A", theater);
            actA.Perform();
            actA.ManualFinish();

            var actB = new ManualFinishAct();
            actB.Init("Test Act B", theater);
            actB.Perform();
            actB.ManualFinish();

            var actC = new ManualFinishAct();
            actC.Init("Test Act C", theater);
            actC.Perform();
            actC.ManualFinish();


            // Assertions
            Assert.IsFalse(theater.AreAnyOngoing(), "AreAnyOngoing() is true despite no acts ongoing!");
        }

        yield return null;
    }
    [UnityTest]
    public IEnumerator GetAllActsReturnsAllActs()
    {
        // Only 1 act passed
        {
            // Setup Theater
            var theaterObj = new GameObject("Test Theater");
            var theater = theaterObj.AddComponent<Theater>();

            // Init Acts
            var actA = new ManualFinishAct();
            actA.Init("Test Act A", theater);

            // Get All Acts
            var allActs = theater.GetAllActs();


            // Assertions
            Assert.IsTrue(allActs.Contains(actA), "GetAllActs() does not contain Act A!");
            Assert.IsTrue(allActs.Count == 1, $"GetAllActs() returned incorrect count! Count='{allActs.Count}'");
        }

        // 2 acts passed
        {
            // Setup Theater
            var theaterObj = new GameObject("Test Theater");
            var theater = theaterObj.AddComponent<Theater>();

            // Init Acts
            var actA = new ManualFinishAct();
            actA.Init("Test Act A", theater);

            var actB = new ManualFinishAct();
            actB.Init("Test Act B", theater);

            // Get All Acts
            var allActs = theater.GetAllActs();


            // Assertions
            Assert.IsTrue(allActs.Contains(actA), "GetAllActs() does not contain Act A!");
            Assert.IsTrue(allActs.Contains(actB), "GetAllActs() does not contain Act B!");
            Assert.IsTrue(allActs.Count == 2, $"GetAllActs() returned incorrect count! Count='{allActs.Count}'");
        }

        // 3 acts passed
        {
            // Setup Theater
            var theaterObj = new GameObject("Test Theater");
            var theater = theaterObj.AddComponent<Theater>();

            // Init Acts
            var actA = new ManualFinishAct();
            actA.Init("Test Act A", theater);

            var actB = new ManualFinishAct();
            actB.Init("Test Act B", theater);

            var actC = new ManualFinishAct();
            actC.Init("Test Act C", theater);

            // Get All Acts
            var allActs = theater.GetAllActs();


            // Assertions
            Assert.IsTrue(allActs.Contains(actA), "GetAllActs() does not contain Act A!");
            Assert.IsTrue(allActs.Contains(actB), "GetAllActs() does not contain Act B!");
            Assert.IsTrue(allActs.Contains(actC), "GetAllActs() does not contain Act C!");
            Assert.IsTrue(allActs.Count == 3, $"GetAllActs() returned incorrect count! Count='{allActs.Count}'");
        }

        yield return null;
    }
}
