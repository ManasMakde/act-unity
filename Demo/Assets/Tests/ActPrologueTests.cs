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
    [UnityTest]
    public IEnumerator GrandchildPrologueBlockingMainAct()  // Checks deep prologue blocking main act
    {
        // Prerequisites
        var mainAct = new Act();
        var childAct = new Act();
        var grandchildAct = new Act();


        mainAct.prologue = (a) => new() { childAct };
        mainAct.Init("Main Act");

        childAct.prologue = (a) => new() { grandchildAct };
        childAct.Init("Child Act");

        grandchildAct.AddToBlock(new() { mainAct });
        grandchildAct.Init("Grandchild Act");

        mainAct.Perform();


        // Assertions
        Assert.IsTrue(grandchildAct.GetPerformCount() == 1, $"grandchildAct did not perform exactly once, Perform Count={grandchildAct.GetPerformCount()}");
        Assert.IsTrue(childAct.GetPerformCount() == 1, $"childAct did not perform exactly once, Perform Count={childAct.GetPerformCount()}");
        Assert.IsTrue(mainAct.GetPerformCount() == 1, $"mainAct did not perform exactly once, Perform Count={mainAct.GetPerformCount()}");

        yield return null;
    }
    [UnityTest]
    public IEnumerator ProloguesBlockingSiblings()  // Checks prologue blocking sibling prologue of the same chain
    {
        var siblingActB = new Act();
        siblingActB.Init("Sibling Act B");

        var siblingActA = new Act();
        siblingActA.AddToBlock(new() { siblingActB });
        siblingActA.Init("Sibling Act A");

        var mainAct = new Act();
        mainAct.prologue = (a) => new() { siblingActA, siblingActB };
        mainAct.Init("Main Act");

        mainAct.Perform();


        // Assertions
        Assert.IsTrue(siblingActA.GetPerformCount() == 1, $"siblingActA did not perform exactly once, Perform Count={siblingActA.GetPerformCount()}");
        Assert.IsTrue(siblingActB.GetPerformCount() == 1, $"siblingActB did not perform exactly once, Perform Count={siblingActB.GetPerformCount()}");
        Assert.IsTrue(mainAct.GetPerformCount() == 1, $"mainAct did not perform exactly once, Perform Count={mainAct.GetPerformCount()}");

        yield return null;
    }
}
