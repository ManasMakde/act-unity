using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;


// 1. Does prologue chain {{actA}} work?
// 1. Does prologue chain {{actA}, {actB}} work?
// 1. Does prologue chain {{actA}, {actB}, {actC}} work?


public class ActPrologue1Tests
{
    [UnityTest]
    public IEnumerator PrologueInstant1()
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

        // Check chain sets cleared after perform
        Utilities.AssertChainCleared(actA);
        Utilities.AssertChainCleared(mainAct);


        yield return null;
    }

    [UnityTest]
    public IEnumerator PrologueSeqInstant1()
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

        // Check chain sets cleared after perform
        Utilities.AssertChainCleared(actA, suffix: " in Seq() variation");
        Utilities.AssertChainCleared(mainAct, suffix: " in Seq() variation");


        yield return null;
    }

    [UnityTest]
    public IEnumerator PrologueInstant1x1()
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

        // Check chain sets cleared after perform
        Utilities.AssertChainCleared(actA);
        Utilities.AssertChainCleared(actB);
        Utilities.AssertChainCleared(mainAct);


        yield return null;
    }

    [UnityTest]
    public IEnumerator PrologueSeqInstant1x1()
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

        // Check chain sets cleared after perform
        Utilities.AssertChainCleared(actA, suffix: " in Seq() variation");
        Utilities.AssertChainCleared(actB, suffix: " in Seq() variation");
        Utilities.AssertChainCleared(mainAct, suffix: " in Seq() variation");


        yield return null;
    }

    [UnityTest]
    public IEnumerator PrologueInstant1x1x1()
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

        // Check chain sets cleared after perform
        Utilities.AssertChainCleared(actA);
        Utilities.AssertChainCleared(actB);
        Utilities.AssertChainCleared(actC);
        Utilities.AssertChainCleared(mainAct);


        yield return null;
    }

    [UnityTest]
    public IEnumerator PrologueSeqInstant1x1x1()
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

        // Check chain sets cleared after perform
        Utilities.AssertChainCleared(actA, suffix: " in Seq() variation");
        Utilities.AssertChainCleared(actB, suffix: " in Seq() variation");
        Utilities.AssertChainCleared(actC, suffix: " in Seq() variation");
        Utilities.AssertChainCleared(mainAct, suffix: " in Seq() variation");


        yield return null;
    }
}
