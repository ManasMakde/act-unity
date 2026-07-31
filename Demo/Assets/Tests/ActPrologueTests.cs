using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;


public class ActPrologueTests
{
    [UnityTest]
    public IEnumerator OnPrePrologue()  // Checks OnPrePrologue broadcasting with correct arguments
    {
        // Prerequisites
        bool wasPrePrologueInvoked = false;
        Act prePrologueArg1 = null;


        // Perform Act
        var act = new Act();
        act.prologue = (a) => new() { new Act() };
        act.OnPrePrologue += (a) => { wasPrePrologueInvoked = true; prePrologueArg1 = a; };
        act.Init("Test Act");
        act.Perform();


        // Assertions
        Assert.IsTrue(wasPrePrologueInvoked, "OnPrePrologue not invoked!");
        Assert.IsTrue(prePrologueArg1 == act, $"OnPrePrologue first argument is invalid! Arg1=`{prePrologueArg1}`");


        yield return null;
    }
    [UnityTest]
    public IEnumerator OnPrePrologueBroadcastWithNoPrologues()  // Checks pre prologue not broadcasting when no prologue acts assigned
    {
        // Prerequisites
        bool wasPrePrologueInvoked = false;


        // Perform Act
        var act = new Act();
        act.OnPrePrologue += (a) => { wasPrePrologueInvoked = true; };
        act.Init("Test Act");
        act.Perform();


        // Assertions
        Assert.IsTrue(!wasPrePrologueInvoked, "OnPrePrologue invoked despite no prologue acts assigned!");


        yield return null;
    }
    [UnityTest]
    public IEnumerator OnPrePrologueBroadcastWithEmptyPrologues()  // Checks pre prologue not broadcasting when empty prologue list assigned
    {
        // Prerequisites
        bool wasPrePrologueInvoked = false;


        // Perform Act
        var act = new Act();
        act.prologue = (a) => new List<Act>();
        act.OnPrePrologue += (a) => { wasPrePrologueInvoked = true; };
        act.Init("Test Act");
        act.Perform();


        // Assertions
        Assert.IsTrue(!wasPrePrologueInvoked, "OnPrePrologue invoked despite empty prologue list assigned!");


        yield return null;
    }



    [UnityTest]
    public IEnumerator OnPostPrologue()  // Checks OnPostPrologue broadcasting with correct arguments
    {
        // Prerequisites
        bool wasPostPrologueInvoked = false;
        Act postPrologueArg1 = null;


        // Perform Act
        var act = new Act();
        act.prologue = (a) => new() { new Act() };
        act.OnPostPrologue += (a) => { wasPostPrologueInvoked = true; postPrologueArg1 = a; };
        act.Init("Test Act");
        act.Perform();


        // Assertions
        Assert.IsTrue(wasPostPrologueInvoked, "OnPostPrologue not invoked!");
        Assert.IsTrue(postPrologueArg1 == act, $"OnPostPrologue first argument is invalid! Arg1=`{postPrologueArg1}`");


        yield return null;
    }
    [UnityTest]
    public IEnumerator OnPostPrologueBroadcastWithNoPrologues()  // Checks post prologue not broadcasting when no prologue acts assigned
    {
        // Prerequisites
        bool wasPostPrologueInvoked = false;


        // Perform Act
        var act = new Act();
        act.OnPostPrologue += (a) => { wasPostPrologueInvoked = true; };
        act.Init("Test Act");
        act.Perform();


        // Assertions
        Assert.IsTrue(!wasPostPrologueInvoked, "OnPostPrologue invoked despite no prologue acts assigned!");


        yield return null;
    }
    [UnityTest]
    public IEnumerator OnPostPrologueBroadcastWithEmptyPrologues()  // Checks post prologue not broadcasting when empty prologue list assigned
    {
        // Prerequisites
        bool wasPostPrologueInvoked = false;


        // Perform Act
        var act = new Act();
        act.prologue = (a) => new List<Act>();
        act.OnPostPrologue += (a) => { wasPostPrologueInvoked = true; };
        act.Init("Test Act");
        act.Perform();


        // Assertions
        Assert.IsTrue(!wasPostPrologueInvoked, "OnPostPrologue invoked despite empty prologue list assigned!");


        yield return null;
    }
    [UnityTest]
    public IEnumerator OnPostPrologueBroadcastWithNullPrologue()  // Checks post prologue not broadcasting when null passed to prologue
    {
        // Prerequisites
        bool wasPostPrologueInvoked = false;


        // Perform Act
        var act = new Act();
        act.prologue = (a) => new() { null };
        act.OnPostPrologue += (a) => { wasPostPrologueInvoked = true; };
        act.Init("Test Act");
        act.Perform();


        // Assertions
        Assert.IsTrue(!wasPostPrologueInvoked, "OnPostPrologue invoked despite null prologue act!");
        Assert.IsTrue(act.GetOutcome() == Act.Outcome.Failure, $"Act outcome is not failure despite null prologue act! Outcome={act.GetOutcome()}");


        yield return null;
    }
    [UnityTest]
    public IEnumerator OnPostPrologueBroadcastWhenPrologueFails()  // Checks post prologue not broadcasting when a prologue act fails
    {
        // Prerequisites
        bool wasPostPrologueInvoked = false;


        // Perform Act
        var act = new Act();
        act.prologue = (a) => new() { new FailingAct() };
        act.OnPostPrologue += (a) => { wasPostPrologueInvoked = true; };
        act.Init("Test Act");
        act.Perform();


        // Assertions
        Assert.IsTrue(!wasPostPrologueInvoked, "OnPostPrologue invoked despite prologue act failing!");
        Assert.IsTrue(act.GetOutcome() == Act.Outcome.Failure, $"Act outcome is not failure despite prologue act failing! Outcome={act.GetOutcome()}");


        yield return null;
    }



    [UnityTest]
    public IEnumerator SelfAsOnlyPrologueSkipped()  // Checks act only passing itself as prologue
    {
        // Perform Act
        var act = new Act();
        act.prologue = (a) => new() { a };
        act.Init("Test Act");
        act.Perform();


        // Assertions
        Assert.IsTrue(act.GetOutcome() == Act.Outcome.Success, $"Act could not perform when passing only itself as prologue!");


        yield return null;
    }
    [UnityTest]
    public IEnumerator SelfAsPrologueSkipped()  // Checks act passing itself as one of the prologues
    {
        // Prologue Act
        var didProloguePerform = false;
        var pAct = new Act();
        pAct.OnPreEnter += (a) =>
        {
            didProloguePerform = true;
        };
        pAct.Init("Prologue Act");


        // Perform Act
        var didPerform = false;
        var act = new Act();
        act.prologue = (a) => new() { a, pAct };
        act.OnPreEnter += (a) =>
        {
            didPerform = true;
        };
        act.Init("Test Act");
        act.Perform();


        // Assertions
        Assert.IsTrue(didPerform, $"Act could not perform when passing itself as one of the prologues!");
        Assert.IsTrue(didProloguePerform, $"Passing self as prologue interfered with other prologue");


        yield return null;
    }



    [UnityTest]
    public IEnumerator SinglePrologue()  // Checks {{actA}}
    {
        // Seq variation
        {
            var executionOrder = new List<string>();
            var actA = new Act();
            actA.OnPreEnter += (a) => executionOrder.Add(a.GetName());
            actA.Init("ActA");

            var mainAct = new Act();
            mainAct.OnPreEnter += (a) => executionOrder.Add(a.GetName());
            mainAct.prologue = (a) => Act.Seq(new() { new() { actA } });
            mainAct.Init("MainAct");

            mainAct.Perform();

            Assert.IsTrue(actA.GetPerformCount() == 1, $"ActA did not perform in Seq() variation, Perform Count={actA.GetPerformCount()}");
            Assert.IsTrue(mainAct.GetPerformCount() == 1, $"MainAct did not perform in Seq() variation, Perform Count={mainAct.GetPerformCount()}");
            Assert.IsTrue(executionOrder.Count == 2 && executionOrder[0] == "ActA" && executionOrder[1] == "MainAct", "Execution order invalid in Seq() variation");
        }


        // Manual variation
        {
            var executionOrder = new List<string>();
            var actA = new Act();
            actA.OnPreEnter += (a) => executionOrder.Add(a.GetName());
            actA.Init("ActA");

            var mainAct = new Act();
            mainAct.OnPreEnter += (a) => executionOrder.Add(a.GetName());
            mainAct.prologue = (a) => new() { actA };
            mainAct.Init("MainAct");

            mainAct.Perform();

            Assert.IsTrue(actA.GetPerformCount() == 1, $"ActA did not perform, Perform Count={actA.GetPerformCount()}");
            Assert.IsTrue(mainAct.GetPerformCount() == 1, $"MainAct did not perform, Perform Count={mainAct.GetPerformCount()}");
            Assert.IsTrue(executionOrder.Count == 2 && executionOrder[0] == "ActA" && executionOrder[1] == "MainAct", "Execution order invalid");
        }


        yield return null;
    }



    [UnityTest]
    public IEnumerator Prologues1x1()  // Checks {{actA}, {actB}}
    {
        // Seq variation
        {
            var executionOrder = new List<string>();
            var actA = new Act();
            actA.OnPreEnter += (a) => executionOrder.Add(a.GetName());
            actA.Init("ActA");

            var actB = new Act();
            actB.OnPreEnter += (a) => executionOrder.Add(a.GetName());
            actB.Init("ActB");

            var mainAct = new Act();
            mainAct.OnPreEnter += (a) => executionOrder.Add(a.GetName());
            mainAct.prologue = (a) => Act.Seq(new() { new() { actA }, new() { actB } });
            mainAct.Init("MainAct");

            mainAct.Perform();

            Assert.IsTrue(actA.GetPerformCount() == 1, $"ActA did not perform in Seq() variation, Perform Count={actA.GetPerformCount()}");
            Assert.IsTrue(actB.GetPerformCount() == 1, $"ActB did not perform in Seq() variation, Perform Count={actB.GetPerformCount()}");
            Assert.IsTrue(mainAct.GetPerformCount() == 1, $"MainAct did not perform in Seq() variation, Perform Count={mainAct.GetPerformCount()}");
            Assert.IsTrue(executionOrder.Count == 3 && executionOrder[0] == "ActA" && executionOrder[1] == "ActB" && executionOrder[2] == "MainAct", "Execution order invalid in Seq() variation");
        }


        // Manual variation
        {
            var executionOrder = new List<string>();
            var actA = new Act();
            actA.OnPreEnter += (a) => executionOrder.Add(a.GetName());
            actA.Init("ActA");

            var actB = new Act();
            actB.OnPreEnter += (a) => executionOrder.Add(a.GetName());
            actB.prologue = (a) => new() { actA };
            actB.Init("ActB");

            var mainAct = new Act();
            mainAct.OnPreEnter += (a) => executionOrder.Add(a.GetName());
            mainAct.prologue = (a) => new() { actB };
            mainAct.Init("MainAct");

            mainAct.Perform();

            Assert.IsTrue(actA.GetPerformCount() == 1, $"ActA did not perform, Perform Count={actA.GetPerformCount()}");
            Assert.IsTrue(actB.GetPerformCount() == 1, $"ActB did not perform, Perform Count={actB.GetPerformCount()}");
            Assert.IsTrue(mainAct.GetPerformCount() == 1, $"MainAct did not perform, Perform Count={mainAct.GetPerformCount()}");
            Assert.IsTrue(executionOrder.Count == 3 && executionOrder[0] == "ActA" && executionOrder[1] == "ActB" && executionOrder[2] == "MainAct", "Execution order invalid");
        }


        yield return null;
    }
    [UnityTest]
    public IEnumerator Prologues1x1x1()  // Checks {{actA}, {actB}, {actC}}
    {
        // Seq variation
        {
            var executionOrder = new List<string>();
            var actA = new Act();
            actA.OnPreEnter += (a) => executionOrder.Add(a.GetName());
            actA.Init("ActA");

            var actB = new Act();
            actB.OnPreEnter += (a) => executionOrder.Add(a.GetName());
            actB.Init("ActB");

            var actC = new Act();
            actC.OnPreEnter += (a) => executionOrder.Add(a.GetName());
            actC.Init("ActC");

            var mainAct = new Act();
            mainAct.OnPreEnter += (a) => executionOrder.Add(a.GetName());
            mainAct.prologue = (a) => Act.Seq(new() { new() { actA }, new() { actB }, new() { actC } });
            mainAct.Init("MainAct");

            mainAct.Perform();

            Assert.IsTrue(actA.GetPerformCount() == 1, $"ActA did not perform exactly once in Seq() variation, Perform Count={actA.GetPerformCount()}");
            Assert.IsTrue(actB.GetPerformCount() == 1, $"ActB did not perform exactly once in Seq() variation, Perform Count={actB.GetPerformCount()}");
            Assert.IsTrue(actC.GetPerformCount() == 1, $"ActC did not perform exactly once in Seq() variation, Perform Count={actC.GetPerformCount()}");
            Assert.IsTrue(mainAct.GetPerformCount() == 1, $"MainAct did not perform exactly once in Seq() variation, Perform Count={mainAct.GetPerformCount()}");
            Assert.IsTrue(executionOrder.Count == 4 && executionOrder[0] == "ActA" && executionOrder[1] == "ActB" && executionOrder[2] == "ActC" && executionOrder[3] == "MainAct", "Execution order invalid in Seq() variation");
        }


        // Manual variation
        {
            var executionOrder = new List<string>();
            var actA = new Act();
            actA.OnPreEnter += (a) => executionOrder.Add(a.GetName());
            actA.Init("ActA");

            var actB = new Act();
            actB.OnPreEnter += (a) => executionOrder.Add(a.GetName());
            actB.prologue = (a) => new() { actA };
            actB.Init("ActB");

            var actC = new Act();
            actC.OnPreEnter += (a) => executionOrder.Add(a.GetName());
            actC.prologue = (a) => new() { actB };
            actC.Init("ActC");

            var mainAct = new Act();
            mainAct.OnPreEnter += (a) => executionOrder.Add(a.GetName());
            mainAct.prologue = (a) => new() { actC };
            mainAct.Init("MainAct");

            mainAct.Perform();

            Assert.IsTrue(actA.GetPerformCount() == 1, $"ActA did not perform exactly once, Perform Count={actA.GetPerformCount()}");
            Assert.IsTrue(actB.GetPerformCount() == 1, $"ActB did not perform exactly once, Perform Count={actB.GetPerformCount()}");
            Assert.IsTrue(actC.GetPerformCount() == 1, $"ActC did not perform exactly once, Perform Count={actC.GetPerformCount()}");
            Assert.IsTrue(mainAct.GetPerformCount() == 1, $"MainAct did not perform exactly once, Perform Count={mainAct.GetPerformCount()}");
            Assert.IsTrue(executionOrder.Count == 4 && executionOrder[0] == "ActA" && executionOrder[1] == "ActB" && executionOrder[2] == "ActC" && executionOrder[3] == "MainAct", "Execution order invalid");
        }


        yield return null;
    }

    [UnityTest]
    public IEnumerator Prologues2()  // Checks {{actA1, actA2}} 
    {
        // Seq variation
        {
            var executionOrder = new List<string>();
            var actA1 = new Act();
            actA1.OnPreEnter += (a) => executionOrder.Add(a.GetName());
            actA1.Init("ActA1");

            var actA2 = new Act();
            actA2.OnPreEnter += (a) => executionOrder.Add(a.GetName());
            actA2.Init("ActA2");

            var mainAct = new Act();
            mainAct.OnPreEnter += (a) => executionOrder.Add(a.GetName());
            mainAct.prologue = (a) => Act.Seq(new() { new() { actA1, actA2 } });
            mainAct.Init("MainAct");

            mainAct.Perform();

            Assert.IsTrue(actA1.GetPerformCount() == 1, $"ActA1 did not perform in Seq() variation, Perform Count={actA1.GetPerformCount()}");
            Assert.IsTrue(actA2.GetPerformCount() == 1, $"ActA2 did not perform in Seq() variation, Perform Count={actA2.GetPerformCount()}");
            Assert.IsTrue(mainAct.GetPerformCount() == 1, $"MainAct did not perform in Seq() variation, Perform Count={mainAct.GetPerformCount()}");
            Assert.IsTrue(executionOrder.IndexOf("MainAct") > executionOrder.IndexOf("ActA1"), "MainAct performed before ActA1 in Seq() variation");
            Assert.IsTrue(executionOrder.IndexOf("MainAct") > executionOrder.IndexOf("ActA2"), "MainAct performed before ActA2 in Seq() variation");
        }


        // Manual variation
        {
            var executionOrder = new List<string>();
            var actA1 = new Act();
            actA1.OnPreEnter += (a) => executionOrder.Add(a.GetName());
            actA1.Init("ActA1");

            var actA2 = new Act();
            actA2.OnPreEnter += (a) => executionOrder.Add(a.GetName());
            actA2.Init("ActA2");

            var mainAct = new Act();
            mainAct.OnPreEnter += (a) => executionOrder.Add(a.GetName());
            mainAct.prologue = (a) => new() { actA1, actA2 };
            mainAct.Init("MainAct");

            mainAct.Perform();

            Assert.IsTrue(actA1.GetPerformCount() == 1, $"ActA1 did not perform, Perform Count={actA1.GetPerformCount()}");
            Assert.IsTrue(actA2.GetPerformCount() == 1, $"ActA2 did not perform, Perform Count={actA2.GetPerformCount()}");
            Assert.IsTrue(mainAct.GetPerformCount() == 1, $"MainAct did not perform, Perform Count={mainAct.GetPerformCount()}");
            Assert.IsTrue(executionOrder.IndexOf("MainAct") > executionOrder.IndexOf("ActA1"), "MainAct performed before ActA1");
            Assert.IsTrue(executionOrder.IndexOf("MainAct") > executionOrder.IndexOf("ActA2"), "MainAct performed before ActA2");
        }


        yield return null;
    }
    [UnityTest]
    public IEnumerator Prologues1x2()  // Checks {{actA}, {actB1, actB2}} 
    {
        // Seq variation instant acts
        {
            var executionOrder = new List<string>();
            var actA = new Act();
            actA.OnPreEnter += (a) => executionOrder.Add(a.GetName());
            actA.Init("ActA");

            var actB1 = new Act();
            actB1.OnPreEnter += (a) => executionOrder.Add(a.GetName());
            actB1.Init("ActB1");

            var actB2 = new Act();
            actB2.OnPreEnter += (a) => executionOrder.Add(a.GetName());
            actB2.Init("ActB2");

            var mainAct = new Act();
            mainAct.OnPreEnter += (a) => executionOrder.Add(a.GetName());
            mainAct.prologue = (a) => Act.Seq(new() { new() { actA }, new() { actB1, actB2 } });
            mainAct.Init("MainAct");

            mainAct.Perform();

            Assert.IsTrue(actA.GetPerformCount() == 1, $"Instant: ActA did not perform exactly once in Seq() variation, Perform Count={actA.GetPerformCount()}");
            Assert.IsTrue(actB1.GetPerformCount() == 1, $"Instant: ActB1 did not perform exactly once in Seq() variation, Perform Count={actB1.GetPerformCount()}");
            Assert.IsTrue(actB2.GetPerformCount() == 1, $"Instant: ActB2 did not perform exactly once in Seq() variation, Perform Count={actB2.GetPerformCount()}");
            Assert.IsTrue(mainAct.GetPerformCount() == 1, $"Instant: MainAct did not perform exactly once in Seq() variation, Perform Count={mainAct.GetPerformCount()}");
            Assert.IsTrue(executionOrder.IndexOf("ActB1") > executionOrder.IndexOf("ActA"), "Instant: ActB1 performed before ActA in Seq() variation");
            Assert.IsTrue(executionOrder.IndexOf("ActB2") > executionOrder.IndexOf("ActA"), "Instant: ActB2 performed before ActA in Seq() variation");
            Assert.IsTrue(executionOrder.IndexOf("MainAct") > executionOrder.IndexOf("ActB1"), "Instant: MainAct performed before ActB1 in Seq() variation");
            Assert.IsTrue(executionOrder.IndexOf("MainAct") > executionOrder.IndexOf("ActB2"), "Instant: MainAct performed before ActB2 in Seq() variation");
        }


        // Seq variation duration acts
        {
            var theater = new GameObject("Theater").AddComponent<Theater>();
            var executionOrder = new List<string>();
            var actA = new SingleTickAct();
            actA.OnPreEnter += (a) => executionOrder.Add(a.GetName());
            actA.Init("ActA", theater);

            var actB1 = new SingleTickAct();
            actB1.OnPreEnter += (a) => executionOrder.Add(a.GetName());
            actB1.Init("ActB1", theater);

            var actB2 = new SingleTickAct();
            actB2.OnPreEnter += (a) => executionOrder.Add(a.GetName());
            actB2.Init("ActB2", theater);

            var mainAct = new Act();
            mainAct.OnPreEnter += (a) => executionOrder.Add(a.GetName());
            mainAct.prologue = (a) => Act.Seq(new() { new() { actA }, new() { actB1, actB2 } });
            mainAct.Init("MainAct", theater);

            mainAct.Perform();

            yield return null;
            yield return null;
            yield return null;

            Assert.IsTrue(actA.GetPerformCount() == 1, $"Timed: ActA did not perform exactly once in Seq() variation, Perform Count={actA.GetPerformCount()}");
            Assert.IsTrue(actB1.GetPerformCount() == 1, $"Timed: ActB1 did not perform exactly once in Seq() variation, Perform Count={actB1.GetPerformCount()}");
            Assert.IsTrue(actB2.GetPerformCount() == 1, $"Timed: ActB2 did not perform exactly once in Seq() variation, Perform Count={actB2.GetPerformCount()}");
            Assert.IsTrue(mainAct.GetPerformCount() == 1, $"Timed: MainAct did not perform exactly once in Seq() variation, Perform Count={mainAct.GetPerformCount()}");
            Assert.IsTrue(executionOrder.IndexOf("ActB1") > executionOrder.IndexOf("ActA"), "Timed: ActB1 performed before ActA in Seq() variation");
            Assert.IsTrue(executionOrder.IndexOf("ActB2") > executionOrder.IndexOf("ActA"), "Timed: ActB2 performed before ActA in Seq() variation");
            Assert.IsTrue(executionOrder.IndexOf("MainAct") > executionOrder.IndexOf("ActB1"), "Timed: MainAct performed before ActB1 in Seq() variation");
            Assert.IsTrue(executionOrder.IndexOf("MainAct") > executionOrder.IndexOf("ActB2"), "Timed: MainAct performed before ActB2 in Seq() variation");

            UnityEngine.Object.Destroy(theater.gameObject);
        }


        // Manual variation instant acts
        {
            var executionOrder = new List<string>();
            var actA = new Act();
            actA.OnPreEnter += (a) => executionOrder.Add(a.GetName());
            actA.Init("ActA");

            var actB1 = new Act();
            actB1.OnPreEnter += (a) => executionOrder.Add(a.GetName());
            actB1.prologue = (a) => new() { actA };
            actB1.Init("ActB1");

            var actB2 = new Act();
            actB2.OnPreEnter += (a) => executionOrder.Add(a.GetName());
            actB2.Init("ActB2");

            var mainAct = new Act();
            mainAct.OnPreEnter += (a) => executionOrder.Add(a.GetName());
            mainAct.prologue = (a) => new() { actB1, actB2 };
            mainAct.Init("MainAct");

            mainAct.Perform();

            Assert.IsTrue(actA.GetPerformCount() == 1, $"Instant: ActA did not perform exactly once, Perform Count={actA.GetPerformCount()}");
            Assert.IsTrue(actB1.GetPerformCount() == 1, $"Instant: ActB1 did not perform exactly once, Perform Count={actB1.GetPerformCount()}");
            Assert.IsTrue(actB2.GetPerformCount() == 1, $"Instant: ActB2 did not perform exactly once, Perform Count={actB2.GetPerformCount()}");
            Assert.IsTrue(mainAct.GetPerformCount() == 1, $"Instant: MainAct did not perform exactly once, Perform Count={mainAct.GetPerformCount()}");
            Assert.IsTrue(executionOrder.IndexOf("ActB1") > executionOrder.IndexOf("ActA"), "Instant: ActB1 performed before ActA");
            Assert.IsTrue(executionOrder.IndexOf("ActB2") > executionOrder.IndexOf("ActA"), "Instant: ActB2 performed before ActA");
            Assert.IsTrue(executionOrder.IndexOf("MainAct") > executionOrder.IndexOf("ActB1"), "Instant: MainAct performed before ActB1");
            Assert.IsTrue(executionOrder.IndexOf("MainAct") > executionOrder.IndexOf("ActB2"), "Instant: MainAct performed before ActB2");
        }


        // Manual variation duration acts
        {
            var theater = new GameObject("Theater").AddComponent<Theater>();
            var executionOrder = new List<string>();
            var actA = new SingleTickAct();
            actA.OnPreEnter += (a) => executionOrder.Add(a.GetName());
            actA.Init("ActA", theater);

            var actB1 = new SingleTickAct();
            actB1.OnPreEnter += (a) => executionOrder.Add(a.GetName());
            actB1.prologue = (a) => new() { actA };
            actB1.Init("ActB1", theater);

            var actB2 = new SingleTickAct();
            actB2.OnPreEnter += (a) => executionOrder.Add(a.GetName());
            actB2.Init("ActB2", theater);

            var mainAct = new Act();
            mainAct.OnPreEnter += (a) => executionOrder.Add(a.GetName());
            mainAct.prologue = (a) => new() { actB1, actB2 };
            mainAct.Init("MainAct", theater);

            mainAct.Perform();

            yield return null;
            yield return null;
            yield return null;

            Assert.IsTrue(actA.GetPerformCount() == 1, $"Timed: ActA did not perform exactly once, Perform Count={actA.GetPerformCount()}");
            Assert.IsTrue(actB1.GetPerformCount() == 1, $"Timed: ActB1 did not perform exactly once, Perform Count={actB1.GetPerformCount()}");
            Assert.IsTrue(actB2.GetPerformCount() == 1, $"Timed: ActB2 did not perform exactly once, Perform Count={actB2.GetPerformCount()}");
            Assert.IsTrue(mainAct.GetPerformCount() == 1, $"Timed: MainAct did not perform exactly once, Perform Count={mainAct.GetPerformCount()}");
            Assert.IsTrue(executionOrder.IndexOf("ActB1") > executionOrder.IndexOf("ActA"), "Timed: ActB1 performed before ActA");
            Assert.IsTrue(executionOrder.IndexOf("ActB2") > executionOrder.IndexOf("ActA"), "Timed: ActB2 performed before ActA");
            Assert.IsTrue(executionOrder.IndexOf("MainAct") > executionOrder.IndexOf("ActB1"), "Timed: MainAct performed before ActB1");
            Assert.IsTrue(executionOrder.IndexOf("MainAct") > executionOrder.IndexOf("ActB2"), "Timed: MainAct performed before ActB2");

            UnityEngine.Object.Destroy(theater.gameObject);
        }


        yield return null;
    }
    [UnityTest]
    public IEnumerator Prologues2x2()  // Checks {{actA1, actA2}, {actB1, actB2}} 
    {
        // Seq variation instant acts
        {
            var executionOrder = new List<string>();
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

            var mainAct = new Act();
            mainAct.OnPreEnter += (a) => executionOrder.Add(a.GetName());
            mainAct.prologue = (a) => Act.Seq(new() { new() { actA1, actA2 }, new() { actB1, actB2 } });
            mainAct.Init("MainAct");

            mainAct.Perform();

            Assert.IsTrue(actA1.GetPerformCount() == 1, $"Instant: ActA1 did not perform exactly once in Seq() variation, Perform Count={actA1.GetPerformCount()}");
            Assert.IsTrue(actA2.GetPerformCount() == 1, $"Instant: ActA2 did not perform exactly once in Seq() variation, Perform Count={actA2.GetPerformCount()}");
            Assert.IsTrue(actB1.GetPerformCount() == 1, $"Instant: ActB1 did not perform exactly once in Seq() variation, Perform Count={actB1.GetPerformCount()}");
            Assert.IsTrue(actB2.GetPerformCount() == 1, $"Instant: ActB2 did not perform exactly once in Seq() variation, Perform Count={actB2.GetPerformCount()}");
            Assert.IsTrue(mainAct.GetPerformCount() == 1, $"Instant: MainAct did not perform exactly once in Seq() variation, Perform Count={mainAct.GetPerformCount()}");
            Assert.IsTrue(executionOrder.IndexOf("ActB1") > executionOrder.IndexOf("ActA1"), "Instant: ActB1 performed before ActA1 in Seq() variation");
            Assert.IsTrue(executionOrder.IndexOf("ActB1") > executionOrder.IndexOf("ActA2"), "Instant: ActB1 performed before ActA2 in Seq() variation");
            Assert.IsTrue(executionOrder.IndexOf("ActB2") > executionOrder.IndexOf("ActA1"), "Instant: ActB2 performed before ActA1 in Seq() variation");
            Assert.IsTrue(executionOrder.IndexOf("ActB2") > executionOrder.IndexOf("ActA2"), "Instant: ActB2 performed before ActA2 in Seq() variation");
            Assert.IsTrue(executionOrder.IndexOf("MainAct") > executionOrder.IndexOf("ActB1"), "Instant: MainAct performed before ActB1 in Seq() variation");
            Assert.IsTrue(executionOrder.IndexOf("MainAct") > executionOrder.IndexOf("ActB2"), "Instant: MainAct performed before ActB2 in Seq() variation");
        }


        // Seq variation duration acts
        {
            var theater = new GameObject("Theater").AddComponent<Theater>();
            var executionOrder = new List<string>();
            var actA1 = new SingleTickAct();
            actA1.OnPreEnter += (a) => executionOrder.Add(a.GetName());
            actA1.Init("ActA1", theater);

            var actA2 = new SingleTickAct();
            actA2.OnPreEnter += (a) => executionOrder.Add(a.GetName());
            actA2.Init("ActA2", theater);

            var actB1 = new SingleTickAct();
            actB1.OnPreEnter += (a) => executionOrder.Add(a.GetName());
            actB1.Init("ActB1", theater);

            var actB2 = new SingleTickAct();
            actB2.OnPreEnter += (a) => executionOrder.Add(a.GetName());
            actB2.Init("ActB2", theater);

            var mainAct = new Act();
            mainAct.OnPreEnter += (a) => executionOrder.Add(a.GetName());
            mainAct.prologue = (a) => Act.Seq(new() { new() { actA1, actA2 }, new() { actB1, actB2 } });
            mainAct.Init("MainAct", theater);

            mainAct.Perform();

            yield return null;
            yield return null;
            yield return null;

            Assert.IsTrue(actA1.GetPerformCount() == 1, $"Timed: ActA1 did not perform exactly once in Seq() variation, Perform Count={actA1.GetPerformCount()}");
            Assert.IsTrue(actA2.GetPerformCount() == 1, $"Timed: ActA2 did not perform exactly once in Seq() variation, Perform Count={actA2.GetPerformCount()}");
            Assert.IsTrue(actB1.GetPerformCount() == 1, $"Timed: ActB1 did not perform exactly once in Seq() variation, Perform Count={actB1.GetPerformCount()}");
            Assert.IsTrue(actB2.GetPerformCount() == 1, $"Timed: ActB2 did not perform exactly once in Seq() variation, Perform Count={actB2.GetPerformCount()}");
            Assert.IsTrue(mainAct.GetPerformCount() == 1, $"Timed: MainAct did not perform exactly once in Seq() variation, Perform Count={mainAct.GetPerformCount()}");
            Assert.IsTrue(executionOrder.IndexOf("ActB1") > executionOrder.IndexOf("ActA1"), "Timed: ActB1 performed before ActA1 in Seq() variation");
            Assert.IsTrue(executionOrder.IndexOf("ActB1") > executionOrder.IndexOf("ActA2"), "Timed: ActB1 performed before ActA2 in Seq() variation");
            Assert.IsTrue(executionOrder.IndexOf("ActB2") > executionOrder.IndexOf("ActA1"), "Timed: ActB2 performed before ActA1 in Seq() variation");
            Assert.IsTrue(executionOrder.IndexOf("ActB2") > executionOrder.IndexOf("ActA2"), "Timed: ActB2 performed before ActA2 in Seq() variation");
            Assert.IsTrue(executionOrder.IndexOf("MainAct") > executionOrder.IndexOf("ActB1"), "Timed: MainAct performed before ActB1 in Seq() variation");
            Assert.IsTrue(executionOrder.IndexOf("MainAct") > executionOrder.IndexOf("ActB2"), "Timed: MainAct performed before ActB2 in Seq() variation");

            UnityEngine.Object.Destroy(theater.gameObject);
        }


        // Manual variation instant acts
        {
            var executionOrder = new List<string>();
            var actA1 = new Act();
            actA1.OnPreEnter += (a) => executionOrder.Add(a.GetName());
            actA1.Init("ActA1");

            var actA2 = new Act();
            actA2.OnPreEnter += (a) => executionOrder.Add(a.GetName());
            actA2.Init("ActA2");

            var actB1 = new Act();
            actB1.OnPreEnter += (a) => executionOrder.Add(a.GetName());
            actB1.prologue = (a) => new() { actA1, actA2 };
            actB1.Init("ActB1");

            var actB2 = new Act();
            actB2.OnPreEnter += (a) => executionOrder.Add(a.GetName());
            actB2.prologue = (a) => new() { actA1, actA2 };
            actB2.Init("ActB2");

            var mainAct = new Act();
            mainAct.OnPreEnter += (a) => executionOrder.Add(a.GetName());
            mainAct.prologue = (a) => new() { actB1, actB2 };
            mainAct.Init("MainAct");

            mainAct.Perform();

            Assert.IsTrue(actA1.GetPerformCount() == 2, $"Instant: ActA1 did not perform exactly once, Perform Count={actA1.GetPerformCount()}");
            Assert.IsTrue(actA2.GetPerformCount() == 2, $"Instant: ActA2 did not perform exactly once, Perform Count={actA2.GetPerformCount()}");
            Assert.IsTrue(actB1.GetPerformCount() == 1, $"Instant: ActB1 did not perform exactly once, Perform Count={actB1.GetPerformCount()}");
            Assert.IsTrue(actB2.GetPerformCount() == 1, $"Instant: ActB2 did not perform exactly once, Perform Count={actB2.GetPerformCount()}");
            Assert.IsTrue(mainAct.GetPerformCount() == 1, $"Instant: MainAct did not perform exactly once, Perform Count={mainAct.GetPerformCount()}");
            Assert.IsTrue(executionOrder.IndexOf("ActB1") > executionOrder.IndexOf("ActA1"), "Instant: ActB1 performed before ActA1");
            Assert.IsTrue(executionOrder.IndexOf("ActB1") > executionOrder.IndexOf("ActA2"), "Instant: ActB1 performed before ActA2");
            Assert.IsTrue(executionOrder.IndexOf("ActB2") > executionOrder.IndexOf("ActA1"), "Instant: ActB2 performed before ActA1");
            Assert.IsTrue(executionOrder.IndexOf("ActB2") > executionOrder.IndexOf("ActA2"), "Instant: ActB2 performed before ActA2");
            Assert.IsTrue(executionOrder.IndexOf("MainAct") > executionOrder.IndexOf("ActB1"), "Instant: MainAct performed before ActB1");
            Assert.IsTrue(executionOrder.IndexOf("MainAct") > executionOrder.IndexOf("ActB2"), "Instant: MainAct performed before ActB2");
        }


        // Manual variation duration acts
        {
            var theater = new GameObject("Theater").AddComponent<Theater>();
            var executionOrder = new List<string>();
            var actA1 = new SingleTickAct();
            actA1.OnPreEnter += (a) => executionOrder.Add(a.GetName());
            actA1.Init("ActA1", theater);

            var actA2 = new SingleTickAct();
            actA2.OnPreEnter += (a) => executionOrder.Add(a.GetName());
            actA2.Init("ActA2", theater);

            var actB1 = new SingleTickAct();
            actB1.OnPreEnter += (a) => executionOrder.Add(a.GetName());
            actB1.prologue = (a) => new() { actA1, actA2 };
            actB1.Init("ActB1", theater);

            var actB2 = new SingleTickAct();
            actB2.OnPreEnter += (a) => executionOrder.Add(a.GetName());
            actB2.prologue = (a) => new() { actA1, actA2 };
            actB2.Init("ActB2", theater);

            var mainAct = new Act();
            mainAct.OnPreEnter += (a) => executionOrder.Add(a.GetName());
            mainAct.prologue = (a) => new() { actB1, actB2 };
            mainAct.Init("MainAct", theater);

            mainAct.Perform();

            yield return null;
            yield return null;
            yield return null;

            Assert.IsTrue(actA1.GetPerformCount() == 1, $"Timed: ActA1 did not perform exactly once, Perform Count={actA1.GetPerformCount()}");
            Assert.IsTrue(actA2.GetPerformCount() == 1, $"Timed: ActA2 did not perform exactly once, Perform Count={actA2.GetPerformCount()}");
            Assert.IsTrue(actB1.GetPerformCount() == 1, $"Timed: ActB1 did not perform exactly once, Perform Count={actB1.GetPerformCount()}");
            Assert.IsTrue(actB2.GetPerformCount() == 1, $"Timed: ActB2 did not perform exactly once, Perform Count={actB2.GetPerformCount()}");
            Assert.IsTrue(mainAct.GetPerformCount() == 1, $"Timed: MainAct did not perform exactly once, Perform Count={mainAct.GetPerformCount()}");
            Assert.IsTrue(executionOrder.IndexOf("ActB1") > executionOrder.IndexOf("ActA1"), "Timed: ActB1 performed before ActA1");
            Assert.IsTrue(executionOrder.IndexOf("ActB1") > executionOrder.IndexOf("ActA2"), "Timed: ActB1 performed before ActA2");
            Assert.IsTrue(executionOrder.IndexOf("ActB2") > executionOrder.IndexOf("ActA1"), "Timed: ActB2 performed before ActA1");
            Assert.IsTrue(executionOrder.IndexOf("ActB2") > executionOrder.IndexOf("ActA2"), "Timed: ActB2 performed before ActA2");
            Assert.IsTrue(executionOrder.IndexOf("MainAct") > executionOrder.IndexOf("ActB1"), "Timed: MainAct performed before ActB1");
            Assert.IsTrue(executionOrder.IndexOf("MainAct") > executionOrder.IndexOf("ActB2"), "Timed: MainAct performed before ActB2");

            UnityEngine.Object.Destroy(theater.gameObject);
        }


        yield return null;
    }
    [UnityTest]
    public IEnumerator Prologues2x2x2()  // Checks {{actA1, actA2}, {actB1, actB2}, {actC1, actC2}}
    {
        // Seq variation instant acts
        {
            var executionOrder = new List<string>();
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
            mainAct.prologue = (a) => Act.Seq(new() { new() { actA1, actA2 }, new() { actB1, actB2 }, new() { actC1, actC2 } });
            mainAct.Init("MainAct");

            mainAct.Perform();

            Assert.IsTrue(actA1.GetPerformCount() == 1, $"Instant: ActA1 did not perform exactly once in Seq() variation, Perform Count={actA1.GetPerformCount()}");
            Assert.IsTrue(actA2.GetPerformCount() == 1, $"Instant: ActA2 did not perform exactly once in Seq() variation, Perform Count={actA2.GetPerformCount()}");
            Assert.IsTrue(actB1.GetPerformCount() == 1, $"Instant: ActB1 did not perform exactly once in Seq() variation, Perform Count={actB1.GetPerformCount()}");
            Assert.IsTrue(actB2.GetPerformCount() == 1, $"Instant: ActB2 did not perform exactly once in Seq() variation, Perform Count={actB2.GetPerformCount()}");
            Assert.IsTrue(actC1.GetPerformCount() == 1, $"Instant: ActC1 did not perform exactly once in Seq() variation, Perform Count={actC1.GetPerformCount()}");
            Assert.IsTrue(actC2.GetPerformCount() == 1, $"Instant: ActC2 did not perform exactly once in Seq() variation, Perform Count={actC2.GetPerformCount()}");
            Assert.IsTrue(mainAct.GetPerformCount() == 1, $"Instant: MainAct did not perform exactly once in Seq() variation, Perform Count={mainAct.GetPerformCount()}");
            Assert.IsTrue(executionOrder.IndexOf("ActB1") > executionOrder.IndexOf("ActA1"), "Instant: ActB1 performed before ActA1 in Seq() variation");
            Assert.IsTrue(executionOrder.IndexOf("ActC1") > executionOrder.IndexOf("ActB1"), "Instant: ActC1 performed before ActB1 in Seq() variation");
            Assert.IsTrue(executionOrder.IndexOf("MainAct") > executionOrder.IndexOf("ActC1"), "Instant: MainAct performed before ActC1 in Seq() variation");
        }


        // Seq variation duration acts
        {
            var theater = new GameObject("Theater").AddComponent<Theater>();
            var executionOrder = new List<string>();
            var actA1 = new SingleTickAct();
            actA1.OnPreEnter += (a) => executionOrder.Add(a.GetName());
            actA1.Init("ActA1", theater);

            var actA2 = new SingleTickAct();
            actA2.OnPreEnter += (a) => executionOrder.Add(a.GetName());
            actA2.Init("ActA2", theater);

            var actB1 = new SingleTickAct();
            actB1.OnPreEnter += (a) => executionOrder.Add(a.GetName());
            actB1.Init("ActB1", theater);

            var actB2 = new SingleTickAct();
            actB2.OnPreEnter += (a) => executionOrder.Add(a.GetName());
            actB2.Init("ActB2", theater);

            var actC1 = new SingleTickAct();
            actC1.OnPreEnter += (a) => executionOrder.Add(a.GetName());
            actC1.Init("ActC1", theater);

            var actC2 = new SingleTickAct();
            actC2.OnPreEnter += (a) => executionOrder.Add(a.GetName());
            actC2.Init("ActC2", theater);

            var mainAct = new Act();
            mainAct.OnPreEnter += (a) => executionOrder.Add(a.GetName());
            mainAct.prologue = (a) => Act.Seq(new() { new() { actA1, actA2 }, new() { actB1, actB2 }, new() { actC1, actC2 } });
            mainAct.Init("MainAct", theater);

            mainAct.Perform();

            yield return null;
            yield return null;
            yield return null;
            yield return null;

            Assert.IsTrue(actA1.GetPerformCount() == 1, $"Timed: ActA1 did not perform exactly once in Seq() variation, Perform Count={actA1.GetPerformCount()}");
            Assert.IsTrue(actA2.GetPerformCount() == 1, $"Timed: ActA2 did not perform exactly once in Seq() variation, Perform Count={actA2.GetPerformCount()}");
            Assert.IsTrue(actB1.GetPerformCount() == 1, $"Timed: ActB1 did not perform exactly once in Seq() variation, Perform Count={actB1.GetPerformCount()}");
            Assert.IsTrue(actB2.GetPerformCount() == 1, $"Timed: ActB2 did not perform exactly once in Seq() variation, Perform Count={actB2.GetPerformCount()}");
            Assert.IsTrue(actC1.GetPerformCount() == 1, $"Timed: ActC1 did not perform exactly once in Seq() variation, Perform Count={actC1.GetPerformCount()}");
            Assert.IsTrue(actC2.GetPerformCount() == 1, $"Timed: ActC2 did not perform exactly once in Seq() variation, Perform Count={actC2.GetPerformCount()}");
            Assert.IsTrue(mainAct.GetPerformCount() == 1, $"Timed: MainAct did not perform exactly once in Seq() variation, Perform Count={mainAct.GetPerformCount()}");
            Assert.IsTrue(executionOrder.IndexOf("ActB1") > executionOrder.IndexOf("ActA1"), "Timed: ActB1 performed before ActA1 in Seq() variation");
            Assert.IsTrue(executionOrder.IndexOf("ActC1") > executionOrder.IndexOf("ActB1"), "Timed: ActC1 performed before ActB1 in Seq() variation");
            Assert.IsTrue(executionOrder.IndexOf("MainAct") > executionOrder.IndexOf("ActC1"), "Timed: MainAct performed before ActC1 in Seq() variation");

            UnityEngine.Object.Destroy(theater.gameObject);
        }


        // Manual variation instant acts
        {
            var executionOrder = new List<string>();
            var actA1 = new Act();
            actA1.OnPreEnter += (a) => executionOrder.Add(a.GetName());
            actA1.Init("ActA1");

            var actA2 = new Act();
            actA2.OnPreEnter += (a) => executionOrder.Add(a.GetName());
            actA2.Init("ActA2");

            var actB1 = new Act();
            actB1.OnPreEnter += (a) => executionOrder.Add(a.GetName());
            actB1.prologue = (a) => new() { actA1, actA2 };
            actB1.Init("ActB1");

            var actB2 = new Act();
            actB2.OnPreEnter += (a) => executionOrder.Add(a.GetName());
            actB2.prologue = (a) => new() { actA1, actA2 };
            actB2.Init("ActB2");

            var actC1 = new Act();
            actC1.OnPreEnter += (a) => executionOrder.Add(a.GetName());
            actC1.prologue = (a) => new() { actB1, actB2 };
            actC1.Init("ActC1");

            var actC2 = new Act();
            actC2.OnPreEnter += (a) => executionOrder.Add(a.GetName());
            actC2.prologue = (a) => new() { actB1, actB2 };
            actC2.Init("ActC2");

            var mainAct = new Act();
            mainAct.OnPreEnter += (a) => executionOrder.Add(a.GetName());
            mainAct.prologue = (a) => new() { actC1, actC2 };
            mainAct.Init("MainAct");

            mainAct.Perform();

            Assert.IsTrue(actA1.GetPerformCount() == 4, $"Instant: ActA1 did not perform exactly once, Perform Count={actA1.GetPerformCount()}");
            Assert.IsTrue(actA2.GetPerformCount() == 4, $"Instant: ActA2 did not perform exactly once, Perform Count={actA2.GetPerformCount()}");
            Assert.IsTrue(actB1.GetPerformCount() == 2, $"Instant: ActB1 did not perform exactly once, Perform Count={actB1.GetPerformCount()}");
            Assert.IsTrue(actB2.GetPerformCount() == 2, $"Instant: ActB2 did not perform exactly once, Perform Count={actB2.GetPerformCount()}");
            Assert.IsTrue(actC1.GetPerformCount() == 1, $"Instant: ActC1 did not perform exactly once, Perform Count={actC1.GetPerformCount()}");
            Assert.IsTrue(actC2.GetPerformCount() == 1, $"Instant: ActC2 did not perform exactly once, Perform Count={actC2.GetPerformCount()}");
            Assert.IsTrue(mainAct.GetPerformCount() == 1, $"Instant: MainAct did not perform exactly once, Perform Count={mainAct.GetPerformCount()}");
            Assert.IsTrue(executionOrder.IndexOf("ActB1") > executionOrder.IndexOf("ActA1"), "Instant: ActB1 performed before ActA1");
            Assert.IsTrue(executionOrder.IndexOf("ActC1") > executionOrder.IndexOf("ActB1"), "Instant: ActC1 performed before ActB1");
            Assert.IsTrue(executionOrder.IndexOf("MainAct") > executionOrder.IndexOf("ActC1"), "Instant: MainAct performed before ActC1");
        }


        // Manual variation duration acts
        {
            var theater = new GameObject("Theater").AddComponent<Theater>();
            var executionOrder = new List<string>();
            var actA1 = new SingleTickAct();
            actA1.OnPreEnter += (a) => executionOrder.Add(a.GetName());
            actA1.Init("ActA1", theater);

            var actA2 = new SingleTickAct();
            actA2.OnPreEnter += (a) => executionOrder.Add(a.GetName());
            actA2.Init("ActA2", theater);

            var actB1 = new SingleTickAct();
            actB1.OnPreEnter += (a) => executionOrder.Add(a.GetName());
            actB1.prologue = (a) => new() { actA1, actA2 };
            actB1.Init("ActB1", theater);

            var actB2 = new SingleTickAct();
            actB2.OnPreEnter += (a) => executionOrder.Add(a.GetName());
            actB2.prologue = (a) => new() { actA1, actA2 };
            actB2.Init("ActB2", theater);

            var actC1 = new SingleTickAct();
            actC1.OnPreEnter += (a) => executionOrder.Add(a.GetName());
            actC1.prologue = (a) => new() { actB1, actB2 };
            actC1.Init("ActC1", theater);

            var actC2 = new SingleTickAct();
            actC2.OnPreEnter += (a) => executionOrder.Add(a.GetName());
            actC2.prologue = (a) => new() { actB1, actB2 };
            actC2.Init("ActC2", theater);

            var mainAct = new Act();
            mainAct.OnPreEnter += (a) => executionOrder.Add(a.GetName());
            mainAct.prologue = (a) => new() { actC1, actC2 };
            mainAct.Init("MainAct", theater);

            mainAct.Perform();

            yield return null;
            yield return null;
            yield return null;
            yield return null;

            Assert.IsTrue(actA1.GetPerformCount() == 1, $"Timed: ActA1 did not perform exactly once, Perform Count={actA1.GetPerformCount()}");
            Assert.IsTrue(actA2.GetPerformCount() == 1, $"Timed: ActA2 did not perform exactly once, Perform Count={actA2.GetPerformCount()}");
            Assert.IsTrue(actB1.GetPerformCount() == 1, $"Timed: ActB1 did not perform exactly once, Perform Count={actB1.GetPerformCount()}");
            Assert.IsTrue(actB2.GetPerformCount() == 1, $"Timed: ActB2 did not perform exactly once, Perform Count={actB2.GetPerformCount()}");
            Assert.IsTrue(actC1.GetPerformCount() == 1, $"Timed: ActC1 did not perform exactly once, Perform Count={actC1.GetPerformCount()}");
            Assert.IsTrue(actC2.GetPerformCount() == 1, $"Timed: ActC2 did not perform exactly once, Perform Count={actC2.GetPerformCount()}");
            Assert.IsTrue(mainAct.GetPerformCount() == 1, $"Timed: MainAct did not perform exactly once, Perform Count={mainAct.GetPerformCount()}");
            Assert.IsTrue(executionOrder.IndexOf("ActB1") > executionOrder.IndexOf("ActA1"), "Timed: ActB1 performed before ActA1");
            Assert.IsTrue(executionOrder.IndexOf("ActC1") > executionOrder.IndexOf("ActB1"), "Timed: ActC1 performed before ActB1");
            Assert.IsTrue(executionOrder.IndexOf("MainAct") > executionOrder.IndexOf("ActC1"), "Timed: MainAct performed before ActC1");

            UnityEngine.Object.Destroy(theater.gameObject);
        }


        yield return null;
    }

    [UnityTest]
    public IEnumerator Prologues3()  // Checks {{actA, actA1, actA3}}
    {
        // Seq variation
        {
            var executionOrder = new List<string>();
            var actA1 = new Act();
            actA1.OnPreEnter += (a) => executionOrder.Add(a.GetName());
            actA1.Init("ActA1");

            var actA2 = new Act();
            actA2.OnPreEnter += (a) => executionOrder.Add(a.GetName());
            actA2.Init("ActA2");

            var actA3 = new Act();
            actA3.OnPreEnter += (a) => executionOrder.Add(a.GetName());
            actA3.Init("ActA3");

            var mainAct = new Act();
            mainAct.OnPreEnter += (a) => executionOrder.Add(a.GetName());
            mainAct.prologue = (a) => Act.Seq(new() { new() { actA1, actA2, actA3 } });
            mainAct.Init("MainAct");

            mainAct.Perform();

            Assert.IsTrue(actA1.GetPerformCount() == 1, $"ActA1 did not perform exactly once in Seq() variation, Perform Count={actA1.GetPerformCount()}");
            Assert.IsTrue(actA2.GetPerformCount() == 1, $"ActA2 did not perform exactly once in Seq() variation, Perform Count={actA2.GetPerformCount()}");
            Assert.IsTrue(actA3.GetPerformCount() == 1, $"ActA3 did not perform exactly once in Seq() variation, Perform Count={actA3.GetPerformCount()}");
            Assert.IsTrue(mainAct.GetPerformCount() == 1, $"MainAct did not perform exactly once in Seq() variation, Perform Count={mainAct.GetPerformCount()}");
            Assert.IsTrue(executionOrder.IndexOf("MainAct") > executionOrder.IndexOf("ActA1"), "MainAct performed before ActA1 in Seq() variation");
            Assert.IsTrue(executionOrder.IndexOf("MainAct") > executionOrder.IndexOf("ActA2"), "MainAct performed before ActA2 in Seq() variation");
            Assert.IsTrue(executionOrder.IndexOf("MainAct") > executionOrder.IndexOf("ActA3"), "MainAct performed before ActA3 in Seq() variation");
        }


        // Manual variation
        {
            var executionOrder = new List<string>();
            var actA1 = new Act();
            actA1.OnPreEnter += (a) => executionOrder.Add(a.GetName());
            actA1.Init("ActA1");

            var actA2 = new Act();
            actA2.OnPreEnter += (a) => executionOrder.Add(a.GetName());
            actA2.Init("ActA2");

            var actA3 = new Act();
            actA3.OnPreEnter += (a) => executionOrder.Add(a.GetName());
            actA3.Init("ActA3");

            var mainAct = new Act();
            mainAct.OnPreEnter += (a) => executionOrder.Add(a.GetName());
            mainAct.prologue = (a) => new() { actA1, actA2, actA3 };
            mainAct.Init("MainAct");

            mainAct.Perform();

            Assert.IsTrue(actA1.GetPerformCount() == 1, $"ActA1 did not perform exactly once, Perform Count={actA1.GetPerformCount()}");
            Assert.IsTrue(actA2.GetPerformCount() == 1, $"ActA2 did not perform exactly once, Perform Count={actA2.GetPerformCount()}");
            Assert.IsTrue(actA3.GetPerformCount() == 1, $"ActA3 did not perform exactly once, Perform Count={actA3.GetPerformCount()}");
            Assert.IsTrue(mainAct.GetPerformCount() == 1, $"MainAct did not perform exactly once, Perform Count={mainAct.GetPerformCount()}");
            Assert.IsTrue(executionOrder.IndexOf("MainAct") > executionOrder.IndexOf("ActA1"), "MainAct performed before ActA1");
            Assert.IsTrue(executionOrder.IndexOf("MainAct") > executionOrder.IndexOf("ActA2"), "MainAct performed before ActA2");
            Assert.IsTrue(executionOrder.IndexOf("MainAct") > executionOrder.IndexOf("ActA3"), "MainAct performed before ActA3");
        }


        yield return null;
    }
    [UnityTest]
    public IEnumerator Prologues3x3()  // Checks {{actA, actA1, actA3}, {actB1, actB2, actB3}}
    {
        // Seq variation instant acts
        {
            var executionOrder = new List<string>();
            var actA1 = new Act();
            actA1.OnPreEnter += (a) => executionOrder.Add(a.GetName());
            actA1.Init("ActA1");

            var actA2 = new Act();
            actA2.OnPreEnter += (a) => executionOrder.Add(a.GetName());
            actA2.Init("ActA2");

            var actA3 = new Act();
            actA3.OnPreEnter += (a) => executionOrder.Add(a.GetName());
            actA3.Init("ActA3");

            var actB1 = new Act();
            actB1.OnPreEnter += (a) => executionOrder.Add(a.GetName());
            actB1.Init("ActB1");

            var actB2 = new Act();
            actB2.OnPreEnter += (a) => executionOrder.Add(a.GetName());
            actB2.Init("ActB2");

            var actB3 = new Act();
            actB3.OnPreEnter += (a) => executionOrder.Add(a.GetName());
            actB3.Init("ActB3");

            var mainAct = new Act();
            mainAct.OnPreEnter += (a) => executionOrder.Add(a.GetName());
            mainAct.prologue = (a) => Act.Seq(new() { new() { actA1, actA2, actA3 }, new() { actB1, actB2, actB3 } });
            mainAct.Init("MainAct");

            mainAct.Perform();

            Assert.IsTrue(actA1.GetPerformCount() == 1, $"Instant: ActA1 did not perform exactly once in Seq() variation, Perform Count={actA1.GetPerformCount()}");
            Assert.IsTrue(actA2.GetPerformCount() == 1, $"Instant: ActA2 did not perform exactly once in Seq() variation, Perform Count={actA2.GetPerformCount()}");
            Assert.IsTrue(actA3.GetPerformCount() == 1, $"Instant: ActA3 did not perform exactly once in Seq() variation, Perform Count={actA3.GetPerformCount()}");
            Assert.IsTrue(actB1.GetPerformCount() == 1, $"Instant: ActB1 did not perform exactly once in Seq() variation, Perform Count={actB1.GetPerformCount()}");
            Assert.IsTrue(actB2.GetPerformCount() == 1, $"Instant: ActB2 did not perform exactly once in Seq() variation, Perform Count={actB2.GetPerformCount()}");
            Assert.IsTrue(actB3.GetPerformCount() == 1, $"Instant: ActB3 did not perform exactly once in Seq() variation, Perform Count={actB3.GetPerformCount()}");
            Assert.IsTrue(mainAct.GetPerformCount() == 1, $"Instant: MainAct did not perform exactly once in Seq() variation, Perform Count={mainAct.GetPerformCount()}");
            Assert.IsTrue(executionOrder.IndexOf("ActB1") > executionOrder.IndexOf("ActA1"), "Instant: ActB1 performed before ActA1 in Seq() variation");
            Assert.IsTrue(executionOrder.IndexOf("ActB1") > executionOrder.IndexOf("ActA2"), "Instant: ActB1 performed before ActA2 in Seq() variation");
            Assert.IsTrue(executionOrder.IndexOf("ActB1") > executionOrder.IndexOf("ActA3"), "Instant: ActB1 performed before ActA3 in Seq() variation");
            Assert.IsTrue(executionOrder.IndexOf("ActB2") > executionOrder.IndexOf("ActA1"), "Instant: ActB2 performed before ActA1 in Seq() variation");
            Assert.IsTrue(executionOrder.IndexOf("ActB2") > executionOrder.IndexOf("ActA2"), "Instant: ActB2 performed before ActA2 in Seq() variation");
            Assert.IsTrue(executionOrder.IndexOf("ActB2") > executionOrder.IndexOf("ActA3"), "Instant: ActB2 performed before ActA3 in Seq() variation");
            Assert.IsTrue(executionOrder.IndexOf("ActB3") > executionOrder.IndexOf("ActA1"), "Instant: ActB3 performed before ActA1 in Seq() variation");
            Assert.IsTrue(executionOrder.IndexOf("ActB3") > executionOrder.IndexOf("ActA2"), "Instant: ActB3 performed before ActA2 in Seq() variation");
            Assert.IsTrue(executionOrder.IndexOf("ActB3") > executionOrder.IndexOf("ActA3"), "Instant: ActB3 performed before ActA3 in Seq() variation");
            Assert.IsTrue(executionOrder.IndexOf("MainAct") > executionOrder.IndexOf("ActB1"), "Instant: MainAct performed before ActB1 in Seq() variation");
            Assert.IsTrue(executionOrder.IndexOf("MainAct") > executionOrder.IndexOf("ActB2"), "Instant: MainAct performed before ActB2 in Seq() variation");
            Assert.IsTrue(executionOrder.IndexOf("MainAct") > executionOrder.IndexOf("ActB3"), "Instant: MainAct performed before ActB3 in Seq() variation");
        }


        // Seq variation duration acts
        {
            var theater = new GameObject("Theater").AddComponent<Theater>();
            var executionOrder = new List<string>();
            var actA1 = new SingleTickAct();
            actA1.OnPreEnter += (a) => executionOrder.Add(a.GetName());
            actA1.Init("ActA1", theater);

            var actA2 = new SingleTickAct();
            actA2.OnPreEnter += (a) => executionOrder.Add(a.GetName());
            actA2.Init("ActA2", theater);

            var actA3 = new SingleTickAct();
            actA3.OnPreEnter += (a) => executionOrder.Add(a.GetName());
            actA3.Init("ActA3", theater);

            var actB1 = new SingleTickAct();
            actB1.OnPreEnter += (a) => executionOrder.Add(a.GetName());
            actB1.Init("ActB1", theater);

            var actB2 = new SingleTickAct();
            actB2.OnPreEnter += (a) => executionOrder.Add(a.GetName());
            actB2.Init("ActB2", theater);

            var actB3 = new SingleTickAct();
            actB3.OnPreEnter += (a) => executionOrder.Add(a.GetName());
            actB3.Init("ActB3", theater);

            var mainAct = new Act();
            mainAct.OnPreEnter += (a) => executionOrder.Add(a.GetName());
            mainAct.prologue = (a) => Act.Seq(new() { new() { actA1, actA2, actA3 }, new() { actB1, actB2, actB3 } });
            mainAct.Init("MainAct", theater);

            mainAct.Perform();

            yield return null;
            yield return null;
            yield return null;

            Assert.IsTrue(actA1.GetPerformCount() == 1, $"Timed: ActA1 did not perform exactly once in Seq() variation, Perform Count={actA1.GetPerformCount()}");
            Assert.IsTrue(actA2.GetPerformCount() == 1, $"Timed: ActA2 did not perform exactly once in Seq() variation, Perform Count={actA2.GetPerformCount()}");
            Assert.IsTrue(actA3.GetPerformCount() == 1, $"Timed: ActA3 did not perform exactly once in Seq() variation, Perform Count={actA3.GetPerformCount()}");
            Assert.IsTrue(actB1.GetPerformCount() == 1, $"Timed: ActB1 did not perform exactly once in Seq() variation, Perform Count={actB1.GetPerformCount()}");
            Assert.IsTrue(actB2.GetPerformCount() == 1, $"Timed: ActB2 did not perform exactly once in Seq() variation, Perform Count={actB2.GetPerformCount()}");
            Assert.IsTrue(actB3.GetPerformCount() == 1, $"Timed: ActB3 did not perform exactly once in Seq() variation, Perform Count={actB3.GetPerformCount()}");
            Assert.IsTrue(mainAct.GetPerformCount() == 1, $"Timed: MainAct did not perform exactly once in Seq() variation, Perform Count={mainAct.GetPerformCount()}");
            Assert.IsTrue(executionOrder.IndexOf("ActB1") > executionOrder.IndexOf("ActA1"), "Timed: ActB1 performed before ActA1 in Seq() variation");
            Assert.IsTrue(executionOrder.IndexOf("ActB1") > executionOrder.IndexOf("ActA2"), "Timed: ActB1 performed before ActA2 in Seq() variation");
            Assert.IsTrue(executionOrder.IndexOf("ActB1") > executionOrder.IndexOf("ActA3"), "Timed: ActB1 performed before ActA3 in Seq() variation");
            Assert.IsTrue(executionOrder.IndexOf("ActB2") > executionOrder.IndexOf("ActA1"), "Timed: ActB2 performed before ActA1 in Seq() variation");
            Assert.IsTrue(executionOrder.IndexOf("ActB2") > executionOrder.IndexOf("ActA2"), "Timed: ActB2 performed before ActA2 in Seq() variation");
            Assert.IsTrue(executionOrder.IndexOf("ActB2") > executionOrder.IndexOf("ActA3"), "Timed: ActB2 performed before ActA3 in Seq() variation");
            Assert.IsTrue(executionOrder.IndexOf("ActB3") > executionOrder.IndexOf("ActA1"), "Timed: ActB3 performed before ActA1 in Seq() variation");
            Assert.IsTrue(executionOrder.IndexOf("ActB3") > executionOrder.IndexOf("ActA2"), "Timed: ActB3 performed before ActA2 in Seq() variation");
            Assert.IsTrue(executionOrder.IndexOf("ActB3") > executionOrder.IndexOf("ActA3"), "Timed: ActB3 performed before ActA3 in Seq() variation");
            Assert.IsTrue(executionOrder.IndexOf("MainAct") > executionOrder.IndexOf("ActB1"), "Timed: MainAct performed before ActB1 in Seq() variation");
            Assert.IsTrue(executionOrder.IndexOf("MainAct") > executionOrder.IndexOf("ActB2"), "Timed: MainAct performed before ActB2 in Seq() variation");
            Assert.IsTrue(executionOrder.IndexOf("MainAct") > executionOrder.IndexOf("ActB3"), "Timed: MainAct performed before ActB3 in Seq() variation");

            UnityEngine.Object.Destroy(theater.gameObject);
        }


        // Manual variation instant acts
        {
            var executionOrder = new List<string>();
            var actA1 = new Act();
            actA1.OnPreEnter += (a) => executionOrder.Add(a.GetName());
            actA1.Init("ActA1");

            var actA2 = new Act();
            actA2.OnPreEnter += (a) => executionOrder.Add(a.GetName());
            actA2.Init("ActA2");

            var actA3 = new Act();
            actA3.OnPreEnter += (a) => executionOrder.Add(a.GetName());
            actA3.Init("ActA3");

            var actB1 = new Act();
            actB1.OnPreEnter += (a) => executionOrder.Add(a.GetName());
            actB1.prologue = (a) => new() { actA1, actA2, actA3 };
            actB1.Init("ActB1");

            var actB2 = new Act();
            actB2.OnPreEnter += (a) => executionOrder.Add(a.GetName());
            actB2.prologue = (a) => new() { actA1, actA2, actA3 };
            actB2.Init("ActB2");

            var actB3 = new Act();
            actB3.OnPreEnter += (a) => executionOrder.Add(a.GetName());
            actB3.prologue = (a) => new() { actA1, actA2, actA3 };
            actB3.Init("ActB3");

            var mainAct = new Act();
            mainAct.OnPreEnter += (a) => executionOrder.Add(a.GetName());
            mainAct.prologue = (a) => new() { actB1, actB2, actB3 };
            mainAct.Init("MainAct");

            mainAct.Perform();

            Assert.IsTrue(actA1.GetPerformCount() == 3, $"Instant: ActA1 did not perform exactly once, Perform Count={actA1.GetPerformCount()}");
            Assert.IsTrue(actA2.GetPerformCount() == 3, $"Instant: ActA2 did not perform exactly once, Perform Count={actA2.GetPerformCount()}");
            Assert.IsTrue(actA3.GetPerformCount() == 3, $"Instant: ActA3 did not perform exactly once, Perform Count={actA3.GetPerformCount()}");
            Assert.IsTrue(actB1.GetPerformCount() == 1, $"Instant: ActB1 did not perform exactly once, Perform Count={actB1.GetPerformCount()}");
            Assert.IsTrue(actB2.GetPerformCount() == 1, $"Instant: ActB2 did not perform exactly once, Perform Count={actB2.GetPerformCount()}");
            Assert.IsTrue(actB3.GetPerformCount() == 1, $"Instant: ActB3 did not perform exactly once, Perform Count={actB3.GetPerformCount()}");
            Assert.IsTrue(mainAct.GetPerformCount() == 1, $"Instant: MainAct did not perform exactly once, Perform Count={mainAct.GetPerformCount()}");
            Assert.IsTrue(executionOrder.IndexOf("ActB1") > executionOrder.IndexOf("ActA1"), "Instant: ActB1 performed before ActA1");
            Assert.IsTrue(executionOrder.IndexOf("ActB1") > executionOrder.IndexOf("ActA2"), "Instant: ActB1 performed before ActA2");
            Assert.IsTrue(executionOrder.IndexOf("ActB1") > executionOrder.IndexOf("ActA3"), "Instant: ActB1 performed before ActA3");
            Assert.IsTrue(executionOrder.IndexOf("ActB2") > executionOrder.IndexOf("ActA1"), "Instant: ActB2 performed before ActA1");
            Assert.IsTrue(executionOrder.IndexOf("ActB2") > executionOrder.IndexOf("ActA2"), "Instant: ActB2 performed before ActA2");
            Assert.IsTrue(executionOrder.IndexOf("ActB2") > executionOrder.IndexOf("ActA3"), "Instant: ActB2 performed before ActA3");
            Assert.IsTrue(executionOrder.IndexOf("ActB3") > executionOrder.IndexOf("ActA1"), "Instant: ActB3 performed before ActA1");
            Assert.IsTrue(executionOrder.IndexOf("ActB3") > executionOrder.IndexOf("ActA2"), "Instant: ActB3 performed before ActA2");
            Assert.IsTrue(executionOrder.IndexOf("ActB3") > executionOrder.IndexOf("ActA3"), "Instant: ActB3 performed before ActA3");
            Assert.IsTrue(executionOrder.IndexOf("MainAct") > executionOrder.IndexOf("ActB1"), "Instant: MainAct performed before ActB1");
            Assert.IsTrue(executionOrder.IndexOf("MainAct") > executionOrder.IndexOf("ActB2"), "Instant: MainAct performed before ActB2");
            Assert.IsTrue(executionOrder.IndexOf("MainAct") > executionOrder.IndexOf("ActB3"), "Instant: MainAct performed before ActB3");
        }


        // Manual variation duration acts
        {
            var theater = new GameObject("Theater").AddComponent<Theater>();
            var executionOrder = new List<string>();
            var actA1 = new SingleTickAct();
            actA1.OnPreEnter += (a) => executionOrder.Add(a.GetName());
            actA1.Init("ActA1", theater);

            var actA2 = new SingleTickAct();
            actA2.OnPreEnter += (a) => executionOrder.Add(a.GetName());
            actA2.Init("ActA2", theater);

            var actA3 = new SingleTickAct();
            actA3.OnPreEnter += (a) => executionOrder.Add(a.GetName());
            actA3.Init("ActA3", theater);

            var actB1 = new SingleTickAct();
            actB1.OnPreEnter += (a) => executionOrder.Add(a.GetName());
            actB1.prologue = (a) => new() { actA1, actA2, actA3 };
            actB1.Init("ActB1", theater);

            var actB2 = new SingleTickAct();
            actB2.OnPreEnter += (a) => executionOrder.Add(a.GetName());
            actB2.prologue = (a) => new() { actA1, actA2, actA3 };
            actB2.Init("ActB2", theater);

            var actB3 = new SingleTickAct();
            actB3.OnPreEnter += (a) => executionOrder.Add(a.GetName());
            actB3.prologue = (a) => new() { actA1, actA2, actA3 };
            actB3.Init("ActB3", theater);

            var mainAct = new Act();
            mainAct.OnPreEnter += (a) => executionOrder.Add(a.GetName());
            mainAct.prologue = (a) => new() { actB1, actB2, actB3 };
            mainAct.Init("MainAct", theater);

            mainAct.Perform();

            yield return null;
            yield return null;
            yield return null;

            Assert.IsTrue(actA1.GetPerformCount() == 1, $"Timed: ActA1 did not perform exactly once, Perform Count={actA1.GetPerformCount()}");
            Assert.IsTrue(actA2.GetPerformCount() == 1, $"Timed: ActA2 did not perform exactly once, Perform Count={actA2.GetPerformCount()}");
            Assert.IsTrue(actA3.GetPerformCount() == 1, $"Timed: ActA3 did not perform exactly once, Perform Count={actA3.GetPerformCount()}");
            Assert.IsTrue(actB1.GetPerformCount() == 1, $"Timed: ActB1 did not perform exactly once, Perform Count={actB1.GetPerformCount()}");
            Assert.IsTrue(actB2.GetPerformCount() == 1, $"Timed: ActB2 did not perform exactly once, Perform Count={actB2.GetPerformCount()}");
            Assert.IsTrue(actB3.GetPerformCount() == 1, $"Timed: ActB3 did not perform exactly once, Perform Count={actB3.GetPerformCount()}");
            Assert.IsTrue(mainAct.GetPerformCount() == 1, $"Timed: MainAct did not perform exactly once, Perform Count={mainAct.GetPerformCount()}");
            Assert.IsTrue(executionOrder.IndexOf("ActB1") > executionOrder.IndexOf("ActA1"), "Timed: ActB1 performed before ActA1");
            Assert.IsTrue(executionOrder.IndexOf("ActB1") > executionOrder.IndexOf("ActA2"), "Timed: ActB1 performed before ActA2");
            Assert.IsTrue(executionOrder.IndexOf("ActB1") > executionOrder.IndexOf("ActA3"), "Timed: ActB1 performed before ActA3");
            Assert.IsTrue(executionOrder.IndexOf("ActB2") > executionOrder.IndexOf("ActA1"), "Timed: ActB2 performed before ActA1");
            Assert.IsTrue(executionOrder.IndexOf("ActB2") > executionOrder.IndexOf("ActA2"), "Timed: ActB2 performed before ActA2");
            Assert.IsTrue(executionOrder.IndexOf("ActB2") > executionOrder.IndexOf("ActA3"), "Timed: ActB2 performed before ActA3");
            Assert.IsTrue(executionOrder.IndexOf("ActB3") > executionOrder.IndexOf("ActA1"), "Timed: ActB3 performed before ActA1");
            Assert.IsTrue(executionOrder.IndexOf("ActB3") > executionOrder.IndexOf("ActA2"), "Timed: ActB3 performed before ActA2");
            Assert.IsTrue(executionOrder.IndexOf("ActB3") > executionOrder.IndexOf("ActA3"), "Timed: ActB3 performed before ActA3");
            Assert.IsTrue(executionOrder.IndexOf("MainAct") > executionOrder.IndexOf("ActB1"), "Timed: MainAct performed before ActB1");
            Assert.IsTrue(executionOrder.IndexOf("MainAct") > executionOrder.IndexOf("ActB2"), "Timed: MainAct performed before ActB2");
            Assert.IsTrue(executionOrder.IndexOf("MainAct") > executionOrder.IndexOf("ActB3"), "Timed: MainAct performed before ActB3");

            UnityEngine.Object.Destroy(theater.gameObject);
        }


        yield return null;
    }
    [UnityTest]
    public IEnumerator Prologues3x3x3()  // Checks {{actA, actA1, actA3}, {actB1, actB2, actB3}, {actC1, actC2, actC3}}
    {
        // Seq variation instant acts
        {
            var executionOrder = new List<string>();
            var actA1 = new Act();
            actA1.OnPreEnter += (a) => executionOrder.Add(a.GetName());
            actA1.Init("ActA1");

            var actA2 = new Act();
            actA2.OnPreEnter += (a) => executionOrder.Add(a.GetName());
            actA2.Init("ActA2");

            var actA3 = new Act();
            actA3.OnPreEnter += (a) => executionOrder.Add(a.GetName());
            actA3.Init("ActA3");

            var actB1 = new Act();
            actB1.OnPreEnter += (a) => executionOrder.Add(a.GetName());
            actB1.Init("ActB1");

            var actB2 = new Act();
            actB2.OnPreEnter += (a) => executionOrder.Add(a.GetName());
            actB2.Init("ActB2");

            var actB3 = new Act();
            actB3.OnPreEnter += (a) => executionOrder.Add(a.GetName());
            actB3.Init("ActB3");

            var actC1 = new Act();
            actC1.OnPreEnter += (a) => executionOrder.Add(a.GetName());
            actC1.Init("ActC1");

            var actC2 = new Act();
            actC2.OnPreEnter += (a) => executionOrder.Add(a.GetName());
            actC2.Init("ActC2");

            var actC3 = new Act();
            actC3.OnPreEnter += (a) => executionOrder.Add(a.GetName());
            actC3.Init("ActC3");

            var mainAct = new Act();
            mainAct.OnPreEnter += (a) => executionOrder.Add(a.GetName());
            mainAct.prologue = (a) => Act.Seq(new() { new() { actA1, actA2, actA3 }, new() { actB1, actB2, actB3 }, new() { actC1, actC2, actC3 } });
            mainAct.Init("MainAct");

            mainAct.Perform();

            Assert.IsTrue(actA1.GetPerformCount() == 1, $"Instant: ActA1 did not perform exactly once in Seq() variation, Perform Count={actA1.GetPerformCount()}");
            Assert.IsTrue(actA2.GetPerformCount() == 1, $"Instant: ActA2 did not perform exactly once in Seq() variation, Perform Count={actA2.GetPerformCount()}");
            Assert.IsTrue(actA3.GetPerformCount() == 1, $"Instant: ActA3 did not perform exactly once in Seq() variation, Perform Count={actA3.GetPerformCount()}");
            Assert.IsTrue(actB1.GetPerformCount() == 1, $"Instant: ActB1 did not perform exactly once in Seq() variation, Perform Count={actB1.GetPerformCount()}");
            Assert.IsTrue(actB2.GetPerformCount() == 1, $"Instant: ActB2 did not perform exactly once in Seq() variation, Perform Count={actB2.GetPerformCount()}");
            Assert.IsTrue(actB3.GetPerformCount() == 1, $"Instant: ActB3 did not perform exactly once in Seq() variation, Perform Count={actB3.GetPerformCount()}");
            Assert.IsTrue(actC1.GetPerformCount() == 1, $"Instant: ActC1 did not perform exactly once in Seq() variation, Perform Count={actC1.GetPerformCount()}");
            Assert.IsTrue(actC2.GetPerformCount() == 1, $"Instant: ActC2 did not perform exactly once in Seq() variation, Perform Count={actC2.GetPerformCount()}");
            Assert.IsTrue(actC3.GetPerformCount() == 1, $"Instant: ActC3 did not perform exactly once in Seq() variation, Perform Count={actC3.GetPerformCount()}");
            Assert.IsTrue(mainAct.GetPerformCount() == 1, $"Instant: MainAct did not perform exactly once in Seq() variation, Perform Count={mainAct.GetPerformCount()}");
            Assert.IsTrue(executionOrder.IndexOf("ActB1") > executionOrder.IndexOf("ActA1"), "Instant: ActB1 performed before ActA1 in Seq() variation");
            Assert.IsTrue(executionOrder.IndexOf("ActB1") > executionOrder.IndexOf("ActA2"), "Instant: ActB1 performed before ActA2 in Seq() variation");
            Assert.IsTrue(executionOrder.IndexOf("ActB1") > executionOrder.IndexOf("ActA3"), "Instant: ActB1 performed before ActA3 in Seq() variation");
            Assert.IsTrue(executionOrder.IndexOf("ActB2") > executionOrder.IndexOf("ActA1"), "Instant: ActB2 performed before ActA1 in Seq() variation");
            Assert.IsTrue(executionOrder.IndexOf("ActB2") > executionOrder.IndexOf("ActA2"), "Instant: ActB2 performed before ActA2 in Seq() variation");
            Assert.IsTrue(executionOrder.IndexOf("ActB2") > executionOrder.IndexOf("ActA3"), "Instant: ActB2 performed before ActA3 in Seq() variation");
            Assert.IsTrue(executionOrder.IndexOf("ActB3") > executionOrder.IndexOf("ActA1"), "Instant: ActB3 performed before ActA1 in Seq() variation");
            Assert.IsTrue(executionOrder.IndexOf("ActB3") > executionOrder.IndexOf("ActA2"), "Instant: ActB3 performed before ActA2 in Seq() variation");
            Assert.IsTrue(executionOrder.IndexOf("ActB3") > executionOrder.IndexOf("ActA3"), "Instant: ActB3 performed before ActA3 in Seq() variation");
            Assert.IsTrue(executionOrder.IndexOf("ActC1") > executionOrder.IndexOf("ActB1"), "Instant: ActC1 performed before ActB1 in Seq() variation");
            Assert.IsTrue(executionOrder.IndexOf("ActC1") > executionOrder.IndexOf("ActB2"), "Instant: ActC1 performed before ActB2 in Seq() variation");
            Assert.IsTrue(executionOrder.IndexOf("ActC1") > executionOrder.IndexOf("ActB3"), "Instant: ActC1 performed before ActB3 in Seq() variation");
            Assert.IsTrue(executionOrder.IndexOf("ActC2") > executionOrder.IndexOf("ActB1"), "Instant: ActC2 performed before ActB1 in Seq() variation");
            Assert.IsTrue(executionOrder.IndexOf("ActC2") > executionOrder.IndexOf("ActB2"), "Instant: ActC2 performed before ActB2 in Seq() variation");
            Assert.IsTrue(executionOrder.IndexOf("ActC2") > executionOrder.IndexOf("ActB3"), "Instant: ActC2 performed before ActB3 in Seq() variation");
            Assert.IsTrue(executionOrder.IndexOf("ActC3") > executionOrder.IndexOf("ActB1"), "Instant: ActC3 performed before ActB1 in Seq() variation");
            Assert.IsTrue(executionOrder.IndexOf("ActC3") > executionOrder.IndexOf("ActB2"), "Instant: ActC3 performed before ActB2 in Seq() variation");
            Assert.IsTrue(executionOrder.IndexOf("ActC3") > executionOrder.IndexOf("ActB3"), "Instant: ActC3 performed before ActB3 in Seq() variation");
            Assert.IsTrue(executionOrder.IndexOf("MainAct") > executionOrder.IndexOf("ActC1"), "Instant: MainAct performed before ActC1 in Seq() variation");
            Assert.IsTrue(executionOrder.IndexOf("MainAct") > executionOrder.IndexOf("ActC2"), "Instant: MainAct performed before ActC2 in Seq() variation");
            Assert.IsTrue(executionOrder.IndexOf("MainAct") > executionOrder.IndexOf("ActC3"), "Instant: MainAct performed before ActC3 in Seq() variation");
        }


        // Seq variation duration acts
        {
            var theater = new GameObject("Theater").AddComponent<Theater>();
            var executionOrder = new List<string>();
            var actA1 = new SingleTickAct();
            actA1.OnPreEnter += (a) => executionOrder.Add(a.GetName());
            actA1.Init("ActA1", theater);

            var actA2 = new SingleTickAct();
            actA2.OnPreEnter += (a) => executionOrder.Add(a.GetName());
            actA2.Init("ActA2", theater);

            var actA3 = new SingleTickAct();
            actA3.OnPreEnter += (a) => executionOrder.Add(a.GetName());
            actA3.Init("ActA3", theater);

            var actB1 = new SingleTickAct();
            actB1.OnPreEnter += (a) => executionOrder.Add(a.GetName());
            actB1.Init("ActB1", theater);

            var actB2 = new SingleTickAct();
            actB2.OnPreEnter += (a) => executionOrder.Add(a.GetName());
            actB2.Init("ActB2", theater);

            var actB3 = new SingleTickAct();
            actB3.OnPreEnter += (a) => executionOrder.Add(a.GetName());
            actB3.Init("ActB3", theater);

            var actC1 = new SingleTickAct();
            actC1.OnPreEnter += (a) => executionOrder.Add(a.GetName());
            actC1.Init("ActC1", theater);

            var actC2 = new SingleTickAct();
            actC2.OnPreEnter += (a) => executionOrder.Add(a.GetName());
            actC2.Init("ActC2", theater);

            var actC3 = new SingleTickAct();
            actC3.OnPreEnter += (a) => executionOrder.Add(a.GetName());
            actC3.Init("ActC3", theater);

            var mainAct = new Act();
            mainAct.OnPreEnter += (a) => executionOrder.Add(a.GetName());
            mainAct.prologue = (a) => Act.Seq(new() { new() { actA1, actA2, actA3 }, new() { actB1, actB2, actB3 }, new() { actC1, actC2, actC3 } });
            mainAct.Init("MainAct", theater);

            mainAct.Perform();

            yield return null;
            yield return null;
            yield return null;
            yield return null;

            Assert.IsTrue(actA1.GetPerformCount() == 1, $"Timed: ActA1 did not perform exactly once in Seq() variation, Perform Count={actA1.GetPerformCount()}");
            Assert.IsTrue(actA2.GetPerformCount() == 1, $"Timed: ActA2 did not perform exactly once in Seq() variation, Perform Count={actA2.GetPerformCount()}");
            Assert.IsTrue(actA3.GetPerformCount() == 1, $"Timed: ActA3 did not perform exactly once in Seq() variation, Perform Count={actA3.GetPerformCount()}");
            Assert.IsTrue(actB1.GetPerformCount() == 1, $"Timed: ActB1 did not perform exactly once in Seq() variation, Perform Count={actB1.GetPerformCount()}");
            Assert.IsTrue(actB2.GetPerformCount() == 1, $"Timed: ActB2 did not perform exactly once in Seq() variation, Perform Count={actB2.GetPerformCount()}");
            Assert.IsTrue(actB3.GetPerformCount() == 1, $"Timed: ActB3 did not perform exactly once in Seq() variation, Perform Count={actB3.GetPerformCount()}");
            Assert.IsTrue(actC1.GetPerformCount() == 1, $"Timed: ActC1 did not perform exactly once in Seq() variation, Perform Count={actC1.GetPerformCount()}");
            Assert.IsTrue(actC2.GetPerformCount() == 1, $"Timed: ActC2 did not perform exactly once in Seq() variation, Perform Count={actC2.GetPerformCount()}");
            Assert.IsTrue(actC3.GetPerformCount() == 1, $"Timed: ActC3 did not perform exactly once in Seq() variation, Perform Count={actC3.GetPerformCount()}");
            Assert.IsTrue(mainAct.GetPerformCount() == 1, $"Timed: MainAct did not perform exactly once in Seq() variation, Perform Count={mainAct.GetPerformCount()}");
            Assert.IsTrue(executionOrder.IndexOf("ActB1") > executionOrder.IndexOf("ActA1"), "Timed: ActB1 performed before ActA1 in Seq() variation");
            Assert.IsTrue(executionOrder.IndexOf("ActB1") > executionOrder.IndexOf("ActA2"), "Timed: ActB1 performed before ActA2 in Seq() variation");
            Assert.IsTrue(executionOrder.IndexOf("ActB1") > executionOrder.IndexOf("ActA3"), "Timed: ActB1 performed before ActA3 in Seq() variation");
            Assert.IsTrue(executionOrder.IndexOf("ActB2") > executionOrder.IndexOf("ActA1"), "Timed: ActB2 performed before ActA1 in Seq() variation");
            Assert.IsTrue(executionOrder.IndexOf("ActB2") > executionOrder.IndexOf("ActA2"), "Timed: ActB2 performed before ActA2 in Seq() variation");
            Assert.IsTrue(executionOrder.IndexOf("ActB2") > executionOrder.IndexOf("ActA3"), "Timed: ActB2 performed before ActA3 in Seq() variation");
            Assert.IsTrue(executionOrder.IndexOf("ActB3") > executionOrder.IndexOf("ActA1"), "Timed: ActB3 performed before ActA1 in Seq() variation");
            Assert.IsTrue(executionOrder.IndexOf("ActB3") > executionOrder.IndexOf("ActA2"), "Timed: ActB3 performed before ActA2 in Seq() variation");
            Assert.IsTrue(executionOrder.IndexOf("ActB3") > executionOrder.IndexOf("ActA3"), "Timed: ActB3 performed before ActA3 in Seq() variation");
            Assert.IsTrue(executionOrder.IndexOf("ActC1") > executionOrder.IndexOf("ActB1"), "Timed: ActC1 performed before ActB1 in Seq() variation");
            Assert.IsTrue(executionOrder.IndexOf("ActC1") > executionOrder.IndexOf("ActB2"), "Timed: ActC1 performed before ActB2 in Seq() variation");
            Assert.IsTrue(executionOrder.IndexOf("ActC1") > executionOrder.IndexOf("ActB3"), "Timed: ActC1 performed before ActB3 in Seq() variation");
            Assert.IsTrue(executionOrder.IndexOf("ActC2") > executionOrder.IndexOf("ActB1"), "Timed: ActC2 performed before ActB1 in Seq() variation");
            Assert.IsTrue(executionOrder.IndexOf("ActC2") > executionOrder.IndexOf("ActB2"), "Timed: ActC2 performed before ActB2 in Seq() variation");
            Assert.IsTrue(executionOrder.IndexOf("ActC2") > executionOrder.IndexOf("ActB3"), "Timed: ActC2 performed before ActB3 in Seq() variation");
            Assert.IsTrue(executionOrder.IndexOf("ActC3") > executionOrder.IndexOf("ActB1"), "Timed: ActC3 performed before ActB1 in Seq() variation");
            Assert.IsTrue(executionOrder.IndexOf("ActC3") > executionOrder.IndexOf("ActB2"), "Timed: ActC3 performed before ActB2 in Seq() variation");
            Assert.IsTrue(executionOrder.IndexOf("ActC3") > executionOrder.IndexOf("ActB3"), "Timed: ActC3 performed before ActB3 in Seq() variation");
            Assert.IsTrue(executionOrder.IndexOf("MainAct") > executionOrder.IndexOf("ActC1"), "Timed: MainAct performed before ActC1 in Seq() variation");
            Assert.IsTrue(executionOrder.IndexOf("MainAct") > executionOrder.IndexOf("ActC2"), "Timed: MainAct performed before ActC2 in Seq() variation");
            Assert.IsTrue(executionOrder.IndexOf("MainAct") > executionOrder.IndexOf("ActC3"), "Timed: MainAct performed before ActC3 in Seq() variation");

            UnityEngine.Object.Destroy(theater.gameObject);
        }


        // Manual variation instant acts
        {
            var executionOrder = new List<string>();
            var actA1 = new Act();
            actA1.OnPreEnter += (a) => executionOrder.Add(a.GetName());
            actA1.Init("ActA1");

            var actA2 = new Act();
            actA2.OnPreEnter += (a) => executionOrder.Add(a.GetName());
            actA2.Init("ActA2");

            var actA3 = new Act();
            actA3.OnPreEnter += (a) => executionOrder.Add(a.GetName());
            actA3.Init("ActA3");

            var actB1 = new Act();
            actB1.OnPreEnter += (a) => executionOrder.Add(a.GetName());
            actB1.prologue = (a) => new() { actA1, actA2, actA3 };
            actB1.Init("ActB1");

            var actB2 = new Act();
            actB2.OnPreEnter += (a) => executionOrder.Add(a.GetName());
            actB2.prologue = (a) => new() { actA1, actA2, actA3 };
            actB2.Init("ActB2");

            var actB3 = new Act();
            actB3.OnPreEnter += (a) => executionOrder.Add(a.GetName());
            actB3.prologue = (a) => new() { actA1, actA2, actA3 };
            actB3.Init("ActB3");

            var actC1 = new Act();
            actC1.OnPreEnter += (a) => executionOrder.Add(a.GetName());
            actC1.prologue = (a) => new() { actB1, actB2, actB3 };
            actC1.Init("ActC1");

            var actC2 = new Act();
            actC2.OnPreEnter += (a) => executionOrder.Add(a.GetName());
            actC2.prologue = (a) => new() { actB1, actB2, actB3 };
            actC2.Init("ActC2");

            var actC3 = new Act();
            actC3.OnPreEnter += (a) => executionOrder.Add(a.GetName());
            actC3.prologue = (a) => new() { actB1, actB2, actB3 };
            actC3.Init("ActC3");

            var mainAct = new Act();
            mainAct.OnPreEnter += (a) => executionOrder.Add(a.GetName());
            mainAct.prologue = (a) => new() { actC1, actC2, actC3 };
            mainAct.Init("MainAct");

            mainAct.Perform();

            Assert.IsTrue(actA1.GetPerformCount() == 9, $"Instant: ActA1 did not perform exactly once, Perform Count={actA1.GetPerformCount()}");
            Assert.IsTrue(actA2.GetPerformCount() == 9, $"Instant: ActA2 did not perform exactly once, Perform Count={actA2.GetPerformCount()}");
            Assert.IsTrue(actA3.GetPerformCount() == 9, $"Instant: ActA3 did not perform exactly once, Perform Count={actA3.GetPerformCount()}");
            Assert.IsTrue(actB1.GetPerformCount() == 3, $"Instant: ActB1 did not perform exactly once, Perform Count={actB1.GetPerformCount()}");
            Assert.IsTrue(actB2.GetPerformCount() == 3, $"Instant: ActB2 did not perform exactly once, Perform Count={actB2.GetPerformCount()}");
            Assert.IsTrue(actB3.GetPerformCount() == 3, $"Instant: ActB3 did not perform exactly once, Perform Count={actB3.GetPerformCount()}");
            Assert.IsTrue(actC1.GetPerformCount() == 1, $"Instant: ActC1 did not perform exactly once, Perform Count={actC1.GetPerformCount()}");
            Assert.IsTrue(actC2.GetPerformCount() == 1, $"Instant: ActC2 did not perform exactly once, Perform Count={actC2.GetPerformCount()}");
            Assert.IsTrue(actC3.GetPerformCount() == 1, $"Instant: ActC3 did not perform exactly once, Perform Count={actC3.GetPerformCount()}");
            Assert.IsTrue(mainAct.GetPerformCount() == 1, $"Instant: MainAct did not perform exactly once, Perform Count={mainAct.GetPerformCount()}");
            Assert.IsTrue(executionOrder.IndexOf("ActB1") > executionOrder.IndexOf("ActA1"), "Instant: ActB1 performed before ActA1");
            Assert.IsTrue(executionOrder.IndexOf("ActB1") > executionOrder.IndexOf("ActA2"), "Instant: ActB1 performed before ActA2");
            Assert.IsTrue(executionOrder.IndexOf("ActB1") > executionOrder.IndexOf("ActA3"), "Instant: ActB1 performed before ActA3");
            Assert.IsTrue(executionOrder.IndexOf("ActB2") > executionOrder.IndexOf("ActA1"), "Instant: ActB2 performed before ActA1");
            Assert.IsTrue(executionOrder.IndexOf("ActB2") > executionOrder.IndexOf("ActA2"), "Instant: ActB2 performed before ActA2");
            Assert.IsTrue(executionOrder.IndexOf("ActB2") > executionOrder.IndexOf("ActA3"), "Instant: ActB2 performed before ActA3");
            Assert.IsTrue(executionOrder.IndexOf("ActB3") > executionOrder.IndexOf("ActA1"), "Instant: ActB3 performed before ActA1");
            Assert.IsTrue(executionOrder.IndexOf("ActB3") > executionOrder.IndexOf("ActA2"), "Instant: ActB3 performed before ActA2");
            Assert.IsTrue(executionOrder.IndexOf("ActB3") > executionOrder.IndexOf("ActA3"), "Instant: ActB3 performed before ActA3");
            Assert.IsTrue(executionOrder.IndexOf("ActC1") > executionOrder.IndexOf("ActB1"), "Instant: ActC1 performed before ActB1");
            Assert.IsTrue(executionOrder.IndexOf("ActC1") > executionOrder.IndexOf("ActB2"), "Instant: ActC1 performed before ActB2");
            Assert.IsTrue(executionOrder.IndexOf("ActC1") > executionOrder.IndexOf("ActB3"), "Instant: ActC1 performed before ActB3");
            Assert.IsTrue(executionOrder.IndexOf("ActC2") > executionOrder.IndexOf("ActB1"), "Instant: ActC2 performed before ActB1");
            Assert.IsTrue(executionOrder.IndexOf("ActC2") > executionOrder.IndexOf("ActB2"), "Instant: ActC2 performed before ActB2");
            Assert.IsTrue(executionOrder.IndexOf("ActC2") > executionOrder.IndexOf("ActB3"), "Instant: ActC2 performed before ActB3");
            Assert.IsTrue(executionOrder.IndexOf("ActC3") > executionOrder.IndexOf("ActB1"), "Instant: ActC3 performed before ActB1");
            Assert.IsTrue(executionOrder.IndexOf("ActC3") > executionOrder.IndexOf("ActB2"), "Instant: ActC3 performed before ActB2");
            Assert.IsTrue(executionOrder.IndexOf("ActC3") > executionOrder.IndexOf("ActB3"), "Instant: ActC3 performed before ActB3");
            Assert.IsTrue(executionOrder.IndexOf("MainAct") > executionOrder.IndexOf("ActC1"), "Instant: MainAct performed before ActC1");
            Assert.IsTrue(executionOrder.IndexOf("MainAct") > executionOrder.IndexOf("ActC2"), "Instant: MainAct performed before ActC2");
            Assert.IsTrue(executionOrder.IndexOf("MainAct") > executionOrder.IndexOf("ActC3"), "Instant: MainAct performed before ActC3");
        }


        // Manual variation duration acts
        {
            var theater = new GameObject("Theater").AddComponent<Theater>();
            var executionOrder = new List<string>();
            var actA1 = new SingleTickAct();
            actA1.OnPreEnter += (a) => executionOrder.Add(a.GetName());
            actA1.Init("ActA1", theater);

            var actA2 = new SingleTickAct();
            actA2.OnPreEnter += (a) => executionOrder.Add(a.GetName());
            actA2.Init("ActA2", theater);

            var actA3 = new SingleTickAct();
            actA3.OnPreEnter += (a) => executionOrder.Add(a.GetName());
            actA3.Init("ActA3", theater);

            var actB1 = new SingleTickAct();
            actB1.OnPreEnter += (a) => executionOrder.Add(a.GetName());
            actB1.prologue = (a) => new() { actA1, actA2, actA3 };
            actB1.Init("ActB1", theater);

            var actB2 = new SingleTickAct();
            actB2.OnPreEnter += (a) => executionOrder.Add(a.GetName());
            actB2.prologue = (a) => new() { actA1, actA2, actA3 };
            actB2.Init("ActB2", theater);

            var actB3 = new SingleTickAct();
            actB3.OnPreEnter += (a) => executionOrder.Add(a.GetName());
            actB3.prologue = (a) => new() { actA1, actA2, actA3 };
            actB3.Init("ActB3", theater);

            var actC1 = new SingleTickAct();
            actC1.OnPreEnter += (a) => executionOrder.Add(a.GetName());
            actC1.prologue = (a) => new() { actB1, actB2, actB3 };
            actC1.Init("ActC1", theater);

            var actC2 = new SingleTickAct();
            actC2.OnPreEnter += (a) => executionOrder.Add(a.GetName());
            actC2.prologue = (a) => new() { actB1, actB2, actB3 };
            actC2.Init("ActC2", theater);

            var actC3 = new SingleTickAct();
            actC3.OnPreEnter += (a) => executionOrder.Add(a.GetName());
            actC3.prologue = (a) => new() { actB1, actB2, actB3 };
            actC3.Init("ActC3", theater);

            var mainAct = new Act();
            mainAct.OnPreEnter += (a) => executionOrder.Add(a.GetName());
            mainAct.prologue = (a) => new() { actC1, actC2, actC3 };
            mainAct.Init("MainAct", theater);

            mainAct.Perform();

            yield return null;
            yield return null;
            yield return null;
            yield return null;

            Assert.IsTrue(actA1.GetPerformCount() == 1, $"Timed: ActA1 did not perform exactly once, Perform Count={actA1.GetPerformCount()}");
            Assert.IsTrue(actA2.GetPerformCount() == 1, $"Timed: ActA2 did not perform exactly once, Perform Count={actA2.GetPerformCount()}");
            Assert.IsTrue(actA3.GetPerformCount() == 1, $"Timed: ActA3 did not perform exactly once, Perform Count={actA3.GetPerformCount()}");
            Assert.IsTrue(actB1.GetPerformCount() == 1, $"Timed: ActB1 did not perform exactly once, Perform Count={actB1.GetPerformCount()}");
            Assert.IsTrue(actB2.GetPerformCount() == 1, $"Timed: ActB2 did not perform exactly once, Perform Count={actB2.GetPerformCount()}");
            Assert.IsTrue(actB3.GetPerformCount() == 1, $"Timed: ActB3 did not perform exactly once, Perform Count={actB3.GetPerformCount()}");
            Assert.IsTrue(actC1.GetPerformCount() == 1, $"Timed: ActC1 did not perform exactly once, Perform Count={actC1.GetPerformCount()}");
            Assert.IsTrue(actC2.GetPerformCount() == 1, $"Timed: ActC2 did not perform exactly once, Perform Count={actC2.GetPerformCount()}");
            Assert.IsTrue(actC3.GetPerformCount() == 1, $"Timed: ActC3 did not perform exactly once, Perform Count={actC3.GetPerformCount()}");
            Assert.IsTrue(mainAct.GetPerformCount() == 1, $"Timed: MainAct did not perform exactly once, Perform Count={mainAct.GetPerformCount()}");
            Assert.IsTrue(executionOrder.IndexOf("ActB1") > executionOrder.IndexOf("ActA1"), "Timed: ActB1 performed before ActA1");
            Assert.IsTrue(executionOrder.IndexOf("ActB1") > executionOrder.IndexOf("ActA2"), "Timed: ActB1 performed before ActA2");
            Assert.IsTrue(executionOrder.IndexOf("ActB1") > executionOrder.IndexOf("ActA3"), "Timed: ActB1 performed before ActA3");
            Assert.IsTrue(executionOrder.IndexOf("ActB2") > executionOrder.IndexOf("ActA1"), "Timed: ActB2 performed before ActA1");
            Assert.IsTrue(executionOrder.IndexOf("ActB2") > executionOrder.IndexOf("ActA2"), "Timed: ActB2 performed before ActA2");
            Assert.IsTrue(executionOrder.IndexOf("ActB2") > executionOrder.IndexOf("ActA3"), "Timed: ActB2 performed before ActA3");
            Assert.IsTrue(executionOrder.IndexOf("ActB3") > executionOrder.IndexOf("ActA1"), "Timed: ActB3 performed before ActA1");
            Assert.IsTrue(executionOrder.IndexOf("ActB3") > executionOrder.IndexOf("ActA2"), "Timed: ActB3 performed before ActA2");
            Assert.IsTrue(executionOrder.IndexOf("ActB3") > executionOrder.IndexOf("ActA3"), "Timed: ActB3 performed before ActA3");
            Assert.IsTrue(executionOrder.IndexOf("ActC1") > executionOrder.IndexOf("ActB1"), "Timed: ActC1 performed before ActB1");
            Assert.IsTrue(executionOrder.IndexOf("ActC1") > executionOrder.IndexOf("ActB2"), "Timed: ActC1 performed before ActB2");
            Assert.IsTrue(executionOrder.IndexOf("ActC1") > executionOrder.IndexOf("ActB3"), "Timed: ActC1 performed before ActB3");
            Assert.IsTrue(executionOrder.IndexOf("ActC2") > executionOrder.IndexOf("ActB1"), "Timed: ActC2 performed before ActB1");
            Assert.IsTrue(executionOrder.IndexOf("ActC2") > executionOrder.IndexOf("ActB2"), "Timed: ActC2 performed before ActB2");
            Assert.IsTrue(executionOrder.IndexOf("ActC2") > executionOrder.IndexOf("ActB3"), "Timed: ActC2 performed before ActB3");
            Assert.IsTrue(executionOrder.IndexOf("ActC3") > executionOrder.IndexOf("ActB1"), "Timed: ActC3 performed before ActB1");
            Assert.IsTrue(executionOrder.IndexOf("ActC3") > executionOrder.IndexOf("ActB2"), "Timed: ActC3 performed before ActB2");
            Assert.IsTrue(executionOrder.IndexOf("ActC3") > executionOrder.IndexOf("ActB3"), "Timed: ActC3 performed before ActB3");
            Assert.IsTrue(executionOrder.IndexOf("MainAct") > executionOrder.IndexOf("ActC1"), "Timed: MainAct performed before ActC1");
            Assert.IsTrue(executionOrder.IndexOf("MainAct") > executionOrder.IndexOf("ActC2"), "Timed: MainAct performed before ActC2");
            Assert.IsTrue(executionOrder.IndexOf("MainAct") > executionOrder.IndexOf("ActC3"), "Timed: MainAct performed before ActC3");

            UnityEngine.Object.Destroy(theater.gameObject);
        }


        yield return null;
    }

    [UnityTest]
    public IEnumerator Prologues1x2x1()  // Checks {{actA} , {actB1, actB2}, {actC}}
    {
        // Seq variation instant acts
        {
            var executionOrder = new List<string>();
            var actA = new Act();
            actA.OnPreEnter += (a) => executionOrder.Add(a.GetName());
            actA.Init("ActA");

            var actB1 = new Act();
            actB1.OnPreEnter += (a) => executionOrder.Add(a.GetName());
            actB1.Init("ActB1");

            var actB2 = new Act();
            actB2.OnPreEnter += (a) => executionOrder.Add(a.GetName());
            actB2.Init("ActB2");

            var actC = new Act();
            actC.OnPreEnter += (a) => executionOrder.Add(a.GetName());
            actC.Init("ActC");

            var mainAct = new Act();
            mainAct.OnPreEnter += (a) => executionOrder.Add(a.GetName());
            mainAct.prologue = (a) => Act.Seq(new() { new() { actA }, new() { actB1, actB2 }, new() { actC } });
            mainAct.Init("MainAct");

            mainAct.Perform();

            Assert.IsTrue(actA.GetPerformCount() == 1, $"Instant: ActA did not perform exactly once in Seq() variation, Perform Count={actA.GetPerformCount()}");
            Assert.IsTrue(actB1.GetPerformCount() == 1, $"Instant: ActB1 did not perform exactly once in Seq() variation, Perform Count={actB1.GetPerformCount()}");
            Assert.IsTrue(actB2.GetPerformCount() == 1, $"Instant: ActB2 did not perform exactly once in Seq() variation, Perform Count={actB2.GetPerformCount()}");
            Assert.IsTrue(actC.GetPerformCount() == 1, $"Instant: ActC did not perform exactly once in Seq() variation, Perform Count={actC.GetPerformCount()}");
            Assert.IsTrue(mainAct.GetPerformCount() == 1, $"Instant: MainAct did not perform exactly once in Seq() variation, Perform Count={mainAct.GetPerformCount()}");
            Assert.IsTrue(executionOrder.IndexOf("ActB1") > executionOrder.IndexOf("ActA"), "Instant: ActB1 performed before ActA in Seq() variation");
            Assert.IsTrue(executionOrder.IndexOf("ActB2") > executionOrder.IndexOf("ActA"), "Instant: ActB2 performed before ActA in Seq() variation");
            Assert.IsTrue(executionOrder.IndexOf("ActC") > executionOrder.IndexOf("ActB1"), "Instant: ActC performed before ActB1 in Seq() variation");
            Assert.IsTrue(executionOrder.IndexOf("ActC") > executionOrder.IndexOf("ActB2"), "Instant: ActC performed before ActB2 in Seq() variation");
            Assert.IsTrue(executionOrder.IndexOf("MainAct") > executionOrder.IndexOf("ActC"), "Instant: MainAct performed before ActC in Seq() variation");
        }


        // Seq variation duration acts
        {
            var theater = new GameObject("Theater").AddComponent<Theater>();
            var executionOrder = new List<string>();
            var actA = new SingleTickAct();
            actA.OnPreEnter += (a) => executionOrder.Add(a.GetName());
            actA.Init("ActA", theater);

            var actB1 = new SingleTickAct();
            actB1.OnPreEnter += (a) => executionOrder.Add(a.GetName());
            actB1.Init("ActB1", theater);

            var actB2 = new SingleTickAct();
            actB2.OnPreEnter += (a) => executionOrder.Add(a.GetName());
            actB2.Init("ActB2", theater);

            var actC = new SingleTickAct();
            actC.OnPreEnter += (a) => executionOrder.Add(a.GetName());
            actC.Init("ActC", theater);

            var mainAct = new Act();
            mainAct.OnPreEnter += (a) => executionOrder.Add(a.GetName());
            mainAct.prologue = (a) => Act.Seq(new() { new() { actA }, new() { actB1, actB2 }, new() { actC } });
            mainAct.Init("MainAct", theater);

            mainAct.Perform();

            yield return null;
            yield return null;
            yield return null;
            yield return null;

            Assert.IsTrue(actA.GetPerformCount() == 1, $"Timed: ActA did not perform exactly once in Seq() variation, Perform Count={actA.GetPerformCount()}");
            Assert.IsTrue(actB1.GetPerformCount() == 1, $"Timed: ActB1 did not perform exactly once in Seq() variation, Perform Count={actB1.GetPerformCount()}");
            Assert.IsTrue(actB2.GetPerformCount() == 1, $"Timed: ActB2 did not perform exactly once in Seq() variation, Perform Count={actB2.GetPerformCount()}");
            Assert.IsTrue(actC.GetPerformCount() == 1, $"Timed: ActC did not perform exactly once in Seq() variation, Perform Count={actC.GetPerformCount()}");
            Assert.IsTrue(mainAct.GetPerformCount() == 1, $"Timed: MainAct did not perform exactly once in Seq() variation, Perform Count={mainAct.GetPerformCount()}");
            Assert.IsTrue(executionOrder.IndexOf("ActB1") > executionOrder.IndexOf("ActA"), "Timed: ActB1 performed before ActA in Seq() variation");
            Assert.IsTrue(executionOrder.IndexOf("ActB2") > executionOrder.IndexOf("ActA"), "Timed: ActB2 performed before ActA in Seq() variation");
            Assert.IsTrue(executionOrder.IndexOf("ActC") > executionOrder.IndexOf("ActB1"), "Timed: ActC performed before ActB1 in Seq() variation");
            Assert.IsTrue(executionOrder.IndexOf("ActC") > executionOrder.IndexOf("ActB2"), "Timed: ActC performed before ActB2 in Seq() variation");
            Assert.IsTrue(executionOrder.IndexOf("MainAct") > executionOrder.IndexOf("ActC"), "Timed: MainAct performed before ActC in Seq() variation");

            UnityEngine.Object.Destroy(theater.gameObject);
        }


        // Manual variation instant acts
        {
            var executionOrder = new List<string>();
            var actA = new Act();
            actA.OnPreEnter += (a) => executionOrder.Add(a.GetName());
            actA.Init("ActA");

            var actB1 = new Act();
            actB1.OnPreEnter += (a) => executionOrder.Add(a.GetName());
            actB1.prologue = (a) => new() { actA };
            actB1.Init("ActB1");

            var actB2 = new Act();
            actB2.OnPreEnter += (a) => executionOrder.Add(a.GetName());
            actB2.prologue = (a) => new() { actA };
            actB2.Init("ActB2");

            var actC = new Act();
            actC.OnPreEnter += (a) => executionOrder.Add(a.GetName());
            actC.prologue = (a) => new() { actB1, actB2 };
            actC.Init("ActC");

            var mainAct = new Act();
            mainAct.OnPreEnter += (a) => executionOrder.Add(a.GetName());
            mainAct.prologue = (a) => new() { actC };
            mainAct.Init("MainAct");

            mainAct.Perform();

            Assert.IsTrue(actA.GetPerformCount() == 2, $"Instant: ActA did not perform exactly once, Perform Count={actA.GetPerformCount()}");
            Assert.IsTrue(actB1.GetPerformCount() == 1, $"Instant: ActB1 did not perform exactly once, Perform Count={actB1.GetPerformCount()}");
            Assert.IsTrue(actB2.GetPerformCount() == 1, $"Instant: ActB2 did not perform exactly once, Perform Count={actB2.GetPerformCount()}");
            Assert.IsTrue(actC.GetPerformCount() == 1, $"Instant: ActC did not perform exactly once, Perform Count={actC.GetPerformCount()}");
            Assert.IsTrue(mainAct.GetPerformCount() == 1, $"Instant: MainAct did not perform exactly once, Perform Count={mainAct.GetPerformCount()}");
            Assert.IsTrue(executionOrder.IndexOf("ActB1") > executionOrder.IndexOf("ActA"), "Instant: ActB1 performed before ActA");
            Assert.IsTrue(executionOrder.IndexOf("ActB2") > executionOrder.IndexOf("ActA"), "Instant: ActB2 performed before ActA");
            Assert.IsTrue(executionOrder.IndexOf("ActC") > executionOrder.IndexOf("ActB1"), "Instant: ActC performed before ActB1");
            Assert.IsTrue(executionOrder.IndexOf("ActC") > executionOrder.IndexOf("ActB2"), "Instant: ActC performed before ActB2");
            Assert.IsTrue(executionOrder.IndexOf("MainAct") > executionOrder.IndexOf("ActC"), "Instant: MainAct performed before ActC");
        }


        // Manual variation duration acts
        {
            var theater = new GameObject("Theater").AddComponent<Theater>();
            var executionOrder = new List<string>();
            var actA = new SingleTickAct();
            actA.OnPreEnter += (a) => executionOrder.Add(a.GetName());
            actA.Init("ActA", theater);

            var actB1 = new SingleTickAct();
            actB1.OnPreEnter += (a) => executionOrder.Add(a.GetName());
            actB1.prologue = (a) => new() { actA };
            actB1.Init("ActB1", theater);

            var actB2 = new SingleTickAct();
            actB2.OnPreEnter += (a) => executionOrder.Add(a.GetName());
            actB2.prologue = (a) => new() { actA };
            actB2.Init("ActB2", theater);

            var actC = new SingleTickAct();
            actC.OnPreEnter += (a) => executionOrder.Add(a.GetName());
            actC.prologue = (a) => new() { actB1, actB2 };
            actC.Init("ActC", theater);

            var mainAct = new Act();
            mainAct.OnPreEnter += (a) => executionOrder.Add(a.GetName());
            mainAct.prologue = (a) => new() { actC };
            mainAct.Init("MainAct", theater);

            mainAct.Perform();

            yield return null;
            yield return null;
            yield return null;
            yield return null;

            Assert.IsTrue(actA.GetPerformCount() == 1, $"Timed: ActA did not perform exactly once, Perform Count={actA.GetPerformCount()}");
            Assert.IsTrue(actB1.GetPerformCount() == 1, $"Timed: ActB1 did not perform exactly once, Perform Count={actB1.GetPerformCount()}");
            Assert.IsTrue(actB2.GetPerformCount() == 1, $"Timed: ActB2 did not perform exactly once, Perform Count={actB2.GetPerformCount()}");
            Assert.IsTrue(actC.GetPerformCount() == 1, $"Timed: ActC did not perform exactly once, Perform Count={actC.GetPerformCount()}");
            Assert.IsTrue(mainAct.GetPerformCount() == 1, $"Timed: MainAct did not perform exactly once, Perform Count={mainAct.GetPerformCount()}");
            Assert.IsTrue(executionOrder.IndexOf("ActB1") > executionOrder.IndexOf("ActA"), "Timed: ActB1 performed before ActA");
            Assert.IsTrue(executionOrder.IndexOf("ActB2") > executionOrder.IndexOf("ActA"), "Timed: ActB2 performed before ActA");
            Assert.IsTrue(executionOrder.IndexOf("ActC") > executionOrder.IndexOf("ActB1"), "Timed: ActC performed before ActB1");
            Assert.IsTrue(executionOrder.IndexOf("ActC") > executionOrder.IndexOf("ActB2"), "Timed: ActC performed before ActB2");
            Assert.IsTrue(executionOrder.IndexOf("MainAct") > executionOrder.IndexOf("ActC"), "Timed: MainAct performed before ActC");

            UnityEngine.Object.Destroy(theater.gameObject);
        }


        yield return null;
    }
    [UnityTest]
    public IEnumerator Prologues2x1x2()  // Checks {{actA1, actA2} , {actB}, {actC1, actC2}}
    {
        // Seq variation instant acts
        {
            var executionOrder = new List<string>();
            var actA1 = new Act();
            actA1.OnPreEnter += (a) => executionOrder.Add(a.GetName());
            actA1.Init("ActA1");

            var actA2 = new Act();
            actA2.OnPreEnter += (a) => executionOrder.Add(a.GetName());
            actA2.Init("ActA2");

            var actB = new Act();
            actB.OnPreEnter += (a) => executionOrder.Add(a.GetName());
            actB.Init("ActB");

            var actC1 = new Act();
            actC1.OnPreEnter += (a) => executionOrder.Add(a.GetName());
            actC1.Init("ActC1");

            var actC2 = new Act();
            actC2.OnPreEnter += (a) => executionOrder.Add(a.GetName());
            actC2.Init("ActC2");

            var mainAct = new Act();
            mainAct.OnPreEnter += (a) => executionOrder.Add(a.GetName());
            mainAct.prologue = (a) => Act.Seq(new() { new() { actA1, actA2 }, new() { actB }, new() { actC1, actC2 } });
            mainAct.Init("MainAct");

            mainAct.Perform();

            Assert.IsTrue(actA1.GetPerformCount() == 1, $"Instant: ActA1 did not perform exactly once in Seq() variation, Perform Count={actA1.GetPerformCount()}");
            Assert.IsTrue(actA2.GetPerformCount() == 1, $"Instant: ActA2 did not perform exactly once in Seq() variation, Perform Count={actA2.GetPerformCount()}");
            Assert.IsTrue(actB.GetPerformCount() == 1, $"Instant: ActB did not perform exactly once in Seq() variation, Perform Count={actB.GetPerformCount()}");
            Assert.IsTrue(actC1.GetPerformCount() == 1, $"Instant: ActC1 did not perform exactly once in Seq() variation, Perform Count={actC1.GetPerformCount()}");
            Assert.IsTrue(actC2.GetPerformCount() == 1, $"Instant: ActC2 did not perform exactly once in Seq() variation, Perform Count={actC2.GetPerformCount()}");
            Assert.IsTrue(mainAct.GetPerformCount() == 1, $"Instant: MainAct did not perform exactly once in Seq() variation, Perform Count={mainAct.GetPerformCount()}");
            Assert.IsTrue(executionOrder.IndexOf("ActB") > executionOrder.IndexOf("ActA1"), "Instant: ActB performed before ActA1 in Seq() variation");
            Assert.IsTrue(executionOrder.IndexOf("ActB") > executionOrder.IndexOf("ActA2"), "Instant: ActB performed before ActA2 in Seq() variation");
            Assert.IsTrue(executionOrder.IndexOf("ActC1") > executionOrder.IndexOf("ActB"), "Instant: ActC1 performed before ActB in Seq() variation");
            Assert.IsTrue(executionOrder.IndexOf("ActC2") > executionOrder.IndexOf("ActB"), "Instant: ActC2 performed before ActB in Seq() variation");
            Assert.IsTrue(executionOrder.IndexOf("MainAct") > executionOrder.IndexOf("ActC1"), "Instant: MainAct performed before ActC1 in Seq() variation");
            Assert.IsTrue(executionOrder.IndexOf("MainAct") > executionOrder.IndexOf("ActC2"), "Instant: MainAct performed before ActC2 in Seq() variation");
        }


        // Seq variation duration acts
        {
            var theater = new GameObject("Theater").AddComponent<Theater>();
            var executionOrder = new List<string>();
            var actA1 = new SingleTickAct();
            actA1.OnPreEnter += (a) => executionOrder.Add(a.GetName());
            actA1.Init("ActA1", theater);

            var actA2 = new SingleTickAct();
            actA2.OnPreEnter += (a) => executionOrder.Add(a.GetName());
            actA2.Init("ActA2", theater);

            var actB = new SingleTickAct();
            actB.OnPreEnter += (a) => executionOrder.Add(a.GetName());
            actB.Init("ActB", theater);

            var actC1 = new SingleTickAct();
            actC1.OnPreEnter += (a) => executionOrder.Add(a.GetName());
            actC1.Init("ActC1", theater);

            var actC2 = new SingleTickAct();
            actC2.OnPreEnter += (a) => executionOrder.Add(a.GetName());
            actC2.Init("ActC2", theater);

            var mainAct = new Act();
            mainAct.OnPreEnter += (a) => executionOrder.Add(a.GetName());
            mainAct.prologue = (a) => Act.Seq(new() { new() { actA1, actA2 }, new() { actB }, new() { actC1, actC2 } });
            mainAct.Init("MainAct", theater);

            mainAct.Perform();

            yield return null;
            yield return null;
            yield return null;
            yield return null;

            Assert.IsTrue(actA1.GetPerformCount() == 1, $"Timed: ActA1 did not perform exactly once in Seq() variation, Perform Count={actA1.GetPerformCount()}");
            Assert.IsTrue(actA2.GetPerformCount() == 1, $"Timed: ActA2 did not perform exactly once in Seq() variation, Perform Count={actA2.GetPerformCount()}");
            Assert.IsTrue(actB.GetPerformCount() == 1, $"Timed: ActB did not perform exactly once in Seq() variation, Perform Count={actB.GetPerformCount()}");
            Assert.IsTrue(actC1.GetPerformCount() == 1, $"Timed: ActC1 did not perform exactly once in Seq() variation, Perform Count={actC1.GetPerformCount()}");
            Assert.IsTrue(actC2.GetPerformCount() == 1, $"Timed: ActC2 did not perform exactly once in Seq() variation, Perform Count={actC2.GetPerformCount()}");
            Assert.IsTrue(mainAct.GetPerformCount() == 1, $"Timed: MainAct did not perform exactly once in Seq() variation, Perform Count={mainAct.GetPerformCount()}");
            Assert.IsTrue(executionOrder.IndexOf("ActB") > executionOrder.IndexOf("ActA1"), "Timed: ActB performed before ActA1 in Seq() variation");
            Assert.IsTrue(executionOrder.IndexOf("ActB") > executionOrder.IndexOf("ActA2"), "Timed: ActB performed before ActA2 in Seq() variation");
            Assert.IsTrue(executionOrder.IndexOf("ActC1") > executionOrder.IndexOf("ActB"), "Timed: ActC1 performed before ActB in Seq() variation");
            Assert.IsTrue(executionOrder.IndexOf("ActC2") > executionOrder.IndexOf("ActB"), "Timed: ActC2 performed before ActB in Seq() variation");
            Assert.IsTrue(executionOrder.IndexOf("MainAct") > executionOrder.IndexOf("ActC1"), "Timed: MainAct performed before ActC1 in Seq() variation");
            Assert.IsTrue(executionOrder.IndexOf("MainAct") > executionOrder.IndexOf("ActC2"), "Timed: MainAct performed before ActC2 in Seq() variation");

            UnityEngine.Object.Destroy(theater.gameObject);
        }


        // Manual variation instant acts
        {
            var executionOrder = new List<string>();
            var actA1 = new Act();
            actA1.OnPreEnter += (a) => executionOrder.Add(a.GetName());
            actA1.Init("ActA1");

            var actA2 = new Act();
            actA2.OnPreEnter += (a) => executionOrder.Add(a.GetName());
            actA2.Init("ActA2");

            var actB = new Act();
            actB.OnPreEnter += (a) => executionOrder.Add(a.GetName());
            actB.prologue = (a) => new() { actA1, actA2 };
            actB.Init("ActB");

            var actC1 = new Act();
            actC1.OnPreEnter += (a) => executionOrder.Add(a.GetName());
            actC1.prologue = (a) => new() { actB };
            actC1.Init("ActC1");

            var actC2 = new Act();
            actC2.OnPreEnter += (a) => executionOrder.Add(a.GetName());
            actC2.prologue = (a) => new() { actB };
            actC2.Init("ActC2");

            var mainAct = new Act();
            mainAct.OnPreEnter += (a) => executionOrder.Add(a.GetName());
            mainAct.prologue = (a) => new() { actC1, actC2 };
            mainAct.Init("MainAct");

            mainAct.Perform();

            Assert.IsTrue(actA1.GetPerformCount() == 2, $"Instant: ActA1 did not perform exactly once, Perform Count={actA1.GetPerformCount()}");
            Assert.IsTrue(actA2.GetPerformCount() == 2, $"Instant: ActA2 did not perform exactly once, Perform Count={actA2.GetPerformCount()}");
            Assert.IsTrue(actB.GetPerformCount() == 2, $"Instant: ActB did not perform exactly once, Perform Count={actB.GetPerformCount()}");
            Assert.IsTrue(actC1.GetPerformCount() == 1, $"Instant: ActC1 did not perform exactly once, Perform Count={actC1.GetPerformCount()}");
            Assert.IsTrue(actC2.GetPerformCount() == 1, $"Instant: ActC2 did not perform exactly once, Perform Count={actC2.GetPerformCount()}");
            Assert.IsTrue(mainAct.GetPerformCount() == 1, $"Instant: MainAct did not perform exactly once, Perform Count={mainAct.GetPerformCount()}");
            Assert.IsTrue(executionOrder.IndexOf("ActB") > executionOrder.IndexOf("ActA1"), "Instant: ActB performed before ActA1");
            Assert.IsTrue(executionOrder.IndexOf("ActB") > executionOrder.IndexOf("ActA2"), "Instant: ActB performed before ActA2");
            Assert.IsTrue(executionOrder.IndexOf("ActC1") > executionOrder.IndexOf("ActB"), "Instant: ActC1 performed before ActB");
            Assert.IsTrue(executionOrder.IndexOf("ActC2") > executionOrder.IndexOf("ActB"), "Instant: ActC2 performed before ActB");
            Assert.IsTrue(executionOrder.IndexOf("MainAct") > executionOrder.IndexOf("ActC1"), "Instant: MainAct performed before ActC1");
            Assert.IsTrue(executionOrder.IndexOf("MainAct") > executionOrder.IndexOf("ActC2"), "Instant: MainAct performed before ActC2");
        }


        // Manual variation duration acts
        {
            var theater = new GameObject("Theater").AddComponent<Theater>();
            var executionOrder = new List<string>();
            var actA1 = new SingleTickAct();
            actA1.OnPreEnter += (a) => executionOrder.Add(a.GetName());
            actA1.Init("ActA1", theater);

            var actA2 = new SingleTickAct();
            actA2.OnPreEnter += (a) => executionOrder.Add(a.GetName());
            actA2.Init("ActA2", theater);

            var actB = new SingleTickAct();
            actB.OnPreEnter += (a) => executionOrder.Add(a.GetName());
            actB.prologue = (a) => new() { actA1, actA2 };
            actB.Init("ActB", theater);

            var actC1 = new SingleTickAct();
            actC1.OnPreEnter += (a) => executionOrder.Add(a.GetName());
            actC1.prologue = (a) => new() { actB };
            actC1.Init("ActC1", theater);

            var actC2 = new SingleTickAct();
            actC2.OnPreEnter += (a) => executionOrder.Add(a.GetName());
            actC2.prologue = (a) => new() { actB };
            actC2.Init("ActC2", theater);

            var mainAct = new Act();
            mainAct.OnPreEnter += (a) => executionOrder.Add(a.GetName());
            mainAct.prologue = (a) => new() { actC1, actC2 };
            mainAct.Init("MainAct", theater);

            mainAct.Perform();

            yield return null;
            yield return null;
            yield return null;
            yield return null;

            Assert.IsTrue(actA1.GetPerformCount() == 1, $"Timed: ActA1 did not perform exactly once, Perform Count={actA1.GetPerformCount()}");
            Assert.IsTrue(actA2.GetPerformCount() == 1, $"Timed: ActA2 did not perform exactly once, Perform Count={actA2.GetPerformCount()}");
            Assert.IsTrue(actB.GetPerformCount() == 1, $"Timed: ActB did not perform exactly once, Perform Count={actB.GetPerformCount()}");
            Assert.IsTrue(actC1.GetPerformCount() == 1, $"Timed: ActC1 did not perform exactly once, Perform Count={actC1.GetPerformCount()}");
            Assert.IsTrue(actC2.GetPerformCount() == 1, $"Timed: ActC2 did not perform exactly once, Perform Count={actC2.GetPerformCount()}");
            Assert.IsTrue(mainAct.GetPerformCount() == 1, $"Timed: MainAct did not perform exactly once, Perform Count={mainAct.GetPerformCount()}");
            Assert.IsTrue(executionOrder.IndexOf("ActB") > executionOrder.IndexOf("ActA1"), "Timed: ActB performed before ActA1");
            Assert.IsTrue(executionOrder.IndexOf("ActB") > executionOrder.IndexOf("ActA2"), "Timed: ActB performed before ActA2");
            Assert.IsTrue(executionOrder.IndexOf("ActC1") > executionOrder.IndexOf("ActB"), "Timed: ActC1 performed before ActB");
            Assert.IsTrue(executionOrder.IndexOf("ActC2") > executionOrder.IndexOf("ActB"), "Timed: ActC2 performed before ActB");
            Assert.IsTrue(executionOrder.IndexOf("MainAct") > executionOrder.IndexOf("ActC1"), "Timed: MainAct performed before ActC1");
            Assert.IsTrue(executionOrder.IndexOf("MainAct") > executionOrder.IndexOf("ActC2"), "Timed: MainAct performed before ActC2");

            UnityEngine.Object.Destroy(theater.gameObject);
        }


        yield return null;
    }



    [UnityTest]
    public IEnumerator MainActBlockingPrologue()  // Checks main act blocking prologue
    {
        // Prerequisites
        var prologueAct = new Act();
        prologueAct.Init("Prologue Act");


        // Perform Act
        var mainAct = new Act();
        mainAct.prologue = (a) => new() { prologueAct };
        mainAct.AddToBlock(new() { prologueAct });
        mainAct.Init("Main Act");

        mainAct.Perform();


        // Assertions
        Assert.IsTrue(prologueAct.GetPerformCount() == 1, $"prologueAct did not perform exactly once, Perform Count={prologueAct.GetPerformCount()}");
        Assert.IsTrue(mainAct.GetPerformCount() == 1, $"mainAct did not perform exactly once, Perform Count={mainAct.GetPerformCount()}");


        yield return null;
    }
    [UnityTest]
    public IEnumerator PrologueBlockingMainChain()  // Checks prologue blocking main act
    {
        // Prerequisites
        var mainAct = new Act();


        // Perform Act
        var prologueAct = new Act();
        prologueAct.AddToBlock(new() { mainAct });
        prologueAct.Init("Prologue Act");

        mainAct.prologue = (a) => new() { prologueAct };
        mainAct.Init("Main Act");
        mainAct.Perform();


        // Assertions
        Assert.IsTrue(prologueAct.GetPerformCount() == 1, $"prologueAct did not perform exactly once, Perform Count={prologueAct.GetPerformCount()}");
        Assert.IsTrue(mainAct.GetPerformCount() == 1, $"mainAct did not perform exactly once, Perform Count={mainAct.GetPerformCount()}");


        yield return null;
    }
    [UnityTest]
    public IEnumerator MainActBlockingGrandchildPrologue()  // Checks main act blocking deep prologue
    {
        var grandchildAct = new Act();
        grandchildAct.Init("Grandchild Act");

        var childAct = new Act();
        childAct.prologue = (a) => new() { grandchildAct };
        childAct.Init("Child Act");

        var mainAct = new Act();
        mainAct.prologue = (a) => new() { childAct };
        mainAct.AddToBlock(new() { grandchildAct });
        mainAct.Init("Main Act");
        
        mainAct.Perform();


        // Assertions
        Assert.IsTrue(grandchildAct.GetPerformCount() == 1, $"grandchildAct did not perform exactly once, Perform Count={grandchildAct.GetPerformCount()}");
        Assert.IsTrue(childAct.GetPerformCount() == 1, $"childAct did not perform exactly once, Perform Count={childAct.GetPerformCount()}");
        Assert.IsTrue(mainAct.GetPerformCount() == 1, $"mainAct did not perform exactly once, Perform Count={mainAct.GetPerformCount()}");


        yield return null;
    }
    // [UnityTest]
    // public IEnumerator GrandchildPrologueBlockingMainAct()  // Checks deep prologue blocking main act
    // {
    //     // Prerequisites
    //     var mainAct = new Act();

    //     var childAct = new Act();
    //     mainAct.prologue = (a) => new() { childAct };


    //     // Perform Act
    //     var grandchildAct = new Act();
    //     grandchildAct.AddToBlock(new() { mainAct });
    //     grandchildAct.Init("Grandchild Act");
    //     childAct.prologue = (a) => new() { grandchildAct };
    //     childAct.Init("Child Act");
    //     mainAct.Init("Main Act");
    //     mainAct.Perform();


    //     // Assertions
    //     Assert.IsTrue(!mainAct.IsBlocked(), "Main act was blocked despite being in the same prologue chain as grandchild act!");
    //     Assert.IsTrue(mainAct.GetOutcome() == Act.Outcome.Success, $"Main act did not succeed despite prologue chain block being skipped! Outcome={mainAct.GetOutcome()}");


    //     yield return null;
    // }



    // [UnityTest]
    // public IEnumerator ProloguesBlockingSiblings()  // Checks prologues blocking parallel sibling prologue of the same chain
    // {
    //     // Prerequisites
    //     var siblingActB = new Act();
    //     siblingActB.Init("Sibling Act B");

    //     var siblingActA = new Act();
    //     siblingActA.AddToBlock(new() { siblingActB });
    //     siblingActA.Init("Sibling Act A");


    //     // Perform Act
    //     var mainAct = new Act();
    //     mainAct.prologue = (a) => new() { siblingActA, siblingActB };
    //     mainAct.Init("Main Act");
    //     mainAct.Perform();


    //     // Assertions
    //     Assert.IsTrue(!siblingActB.IsBlocked(), "Sibling act was blocked despite being in the same prologue chain!");
    //     Assert.IsTrue(mainAct.GetOutcome() == Act.Outcome.Success, $"Main act did not succeed despite sibling block being skipped! Outcome={mainAct.GetOutcome()}");


    //     yield return null;
    // }



    // [UnityTest]
    // public IEnumerator PrologueClearedWhenActFails()  // Checks failing an act clears stale prologues further down the sequence
    // {
    //     // Prerequisites
    //     var failingAct = new FailingAct();
    //     failingAct.Init("Failing Act");

    //     var staleAct = new Act();
    //     staleAct.Init("Stale Act");


    //     // Perform Act
    //     var mainAct = new Act();
    //     mainAct.prologue = (a) => new() { failingAct, staleAct };
    //     mainAct.Init("Main Act");
    //     mainAct.Perform();


    //     // Assertions
    //     Assert.IsTrue(mainAct.GetOutcome() == Act.Outcome.Failure, $"Main act did not fail despite a prologue act failing! Outcome={mainAct.GetOutcome()}");
    //     Assert.IsTrue(staleAct.GetPerformCount() == 0, $"Stale sibling prologue act performed despite chain failing before it! Perform Count={staleAct.GetPerformCount()}");
    //     Assert.IsTrue(!staleAct.IsOngoing(), "Stale sibling prologue act still ongoing despite chain failing!");


    //     yield return null;
    // }
}
