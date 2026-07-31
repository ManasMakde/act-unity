using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;


// 1. Is "pre prologue" action being broadcasted (with correct arguments)?
// 1. Is "pre prologue" action not being broadcasted when no prologue acts assigned?
// 1. Is "pre prologue" action not being broadcasted when assigned empty prologue acts list?

// 1. Is "post prologue" action being broadcasted (with correct arguments)?
// 1. Is "post prologue" action not being broadcasted when no prologue acts assigned?
// 1. Is "post prologue" action not being broadcasted when assigned empty prologue acts list?
// 1. Is "post prologue" action not being broadcasted when null passed to prologue?
// 1. Is "post prologue" action not being broadcasted when any prologue act fails?

// 1. Does an act calling itself as one of the prologues get skipped?
// 1. Does an act only calling itself as prologue get skipped?

// 1. Does main act perform when a prologue act blocks it?
// 1. Does prologue act perform when main act blocks it?
// 1. Does grandchild prologue act perform when main act blocks it?
// 1. Does main act perform when grandchild prologue act blocks it?
// 1. Do sibling acts perform when they block each other?


public class ActPrologueTests
{
    [UnityTest]
    public IEnumerator OnPrePrologue()
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
    public IEnumerator OnPrePrologueBroadcastWithNoPrologues()
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
    public IEnumerator OnPrePrologueBroadcastWithEmptyPrologues()
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
    public IEnumerator OnPostPrologue()
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
    public IEnumerator OnPostPrologueBroadcastWithNoPrologues()
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
    public IEnumerator OnPostPrologueBroadcastWithEmptyPrologues()
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
    public IEnumerator OnPostPrologueBroadcastWithNullPrologue()
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
    public IEnumerator OnPostPrologueBroadcastWhenPrologueFails()
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
    public IEnumerator SelfAsOnlyPrologueSkipped()
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
    public IEnumerator SelfAsPrologueSkipped()
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
    public IEnumerator MainActBlockingPrologue()
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
    public IEnumerator PrologueBlockingMainChain()
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
    public IEnumerator MainActBlockingGrandchildPrologue()
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
    public IEnumerator GrandchildPrologueBlockingMainAct()
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
    public IEnumerator ProloguesBlockingSiblings()
    {
        // One sibling blocks other
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


        // Both siblings blocks each other
        {
            var siblingActA = new Act();
            var siblingActB = new Act();
            siblingActB.AddToBlock(new() { siblingActA });
            siblingActB.Init("Sibling Act B");

            siblingActA.AddToBlock(new() { siblingActB });
            siblingActA.Init("Sibling Act A");

            var mainAct = new Act();
            mainAct.prologue = (a) => new() { siblingActA, siblingActB };
            mainAct.Init("Main Act");

            mainAct.Perform();


            // Assertions
            Assert.IsTrue(siblingActA.GetPerformCount() == 1, $"siblingActA did not perform exactly once (both siblings block each other), Perform Count={siblingActA.GetPerformCount()}");
            Assert.IsTrue(siblingActB.GetPerformCount() == 1, $"siblingActB did not perform exactly once (both siblings block each other), Perform Count={siblingActB.GetPerformCount()}");
            Assert.IsTrue(mainAct.GetPerformCount() == 1, $"mainAct did not perform exactly once, Perform Count={mainAct.GetPerformCount()}");

            yield return null;
        }
    }
}
