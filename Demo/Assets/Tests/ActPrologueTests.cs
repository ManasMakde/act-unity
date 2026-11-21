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

// 1. Is "prologue complete" action being broadcasted (with correct arguments)?
// 1. Is "prologue complete" action not being broadcasted when no prologue acts assigned?
// 1. Is "prologue complete" action not being broadcasted when assigned empty prologue acts list?
// 1. Is "prologue complete" action not being broadcasted when null passed to prologue?
// 1. Is "prologue complete" action not being broadcasted when any prologue act fails?

// 1. Does an act calling itself as one of the prologues get skipped?
// 1. Does an act only calling itself as prologue get skipped?

// 1. Does prologue {null} & null both fail the act?
// 1. Does Seq() {{null}}, {null} & null all fail the act?


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
        act.prologue = (a) => new() { new() { new Act() } };
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
        act.prologue = (a) => new() { new() { } };
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
        act.prologue = (a) => new() { new() { new Act() } };
        act.OnPostPrologue += (a) => { wasPostPrologueInvoked = true; postPrologueArg1 = a; };
        act.Init("Test Act");
        act.Perform();


        // Assertions
        Assert.IsTrue(wasPostPrologueInvoked, "OnPostPrologue not invoked!");
        Assert.IsTrue(postPrologueArg1 == act, $"OnPostPrologue first argument is invalid! Arg1=`{postPrologueArg1}`");


        yield return null;
    }
    [UnityTest]
    public IEnumerator OnPostPrologueWithNoPrologues()
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
    public IEnumerator OnPostPrologueWithEmptyPrologues()
    {
        // Prerequisites
        bool wasPostPrologueInvoked = false;


        // Perform Act
        var act = new Act();
        act.prologue = (a) => new();
        act.OnPostPrologue += (a) => { wasPostPrologueInvoked = true; };
        act.Init("Test Act");
        act.Perform();


        // Assertions
        Assert.IsTrue(!wasPostPrologueInvoked, "OnPostPrologue invoked despite empty prologue list assigned!");


        yield return null;
    }
    [UnityTest]
    public IEnumerator OnPostPrologueWithNullPrologue()
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
    public IEnumerator OnPostPrologueWhenPrologueFails()
    {
        // Prerequisites
        bool wasPostPrologueInvoked = false;


        // Perform Act
        var act = new Act();
        act.prologue = (a) => new() { new() { new FailingAct() } };
        act.OnPostPrologue += (a) => { wasPostPrologueInvoked = true; };
        act.Init("Test Act");
        act.Perform();


        // Assertions
        Assert.IsTrue(!wasPostPrologueInvoked, "OnPostPrologue invoked despite prologue act failing!");
        Assert.IsTrue(act.GetOutcome() == Act.Outcome.Failure, $"Act outcome is not failure despite prologue act failing! Outcome={act.GetOutcome()}");


        yield return null;
    }



    [UnityTest]
    public IEnumerator OnPrologueComplete()
    {
        // Prerequisites
        bool wasPrologueCompleteInvoked = false;
        Act prologueCompleteArg1 = null;
        Act prologueCompleteArg2 = null;
        Act.Outcome prologueCompleteArg3 = Act.Outcome.Pending;


        // Prologue Act
        var pAct = new Act();
        pAct.Init("Prologue Act");


        // Perform Act
        var act = new Act();
        act.prologue = (a) => new() { new() { pAct } };
        act.OnPrologueComplete += (a, p, o) => { wasPrologueCompleteInvoked = true; prologueCompleteArg1 = a; prologueCompleteArg2 = p; prologueCompleteArg3 = o; };
        act.Init("Test Act");
        act.Perform();


        // Assertions
        Assert.IsTrue(wasPrologueCompleteInvoked, "OnPrologueComplete not invoked!");
        Assert.IsTrue(prologueCompleteArg1 == act, $"OnPrologueComplete first argument is invalid! Arg1=`{prologueCompleteArg1}`");
        Assert.IsTrue(prologueCompleteArg2 == pAct, $"OnPrologueComplete second argument is invalid! Arg2=`{prologueCompleteArg2}`");
        Assert.IsTrue(prologueCompleteArg3 == Act.Outcome.Success, $"OnPrologueComplete third argument is invalid! Arg3=`{prologueCompleteArg3}`");


        yield return null;
    }
    [UnityTest]
    public IEnumerator OnPrologueCompleteWithNoPrologues()
    {
        // Prerequisites
        bool wasPrologueCompleteInvoked = false;


        // Perform Act
        var act = new Act();
        act.OnPrologueComplete += (a, p, o) => { wasPrologueCompleteInvoked = true; };
        act.Init("Test Act");
        act.Perform();


        // Assertions
        Assert.IsTrue(!wasPrologueCompleteInvoked, "OnPrologueComplete invoked despite no prologue acts assigned!");


        yield return null;
    }
    [UnityTest]
    public IEnumerator OnPrologueCompleteWithEmptyPrologues()
    {
        // Prerequisites
        bool wasPrologueCompleteInvoked = false;


        // Perform Act
        var act = new Act();
        act.prologue = (a) => new();
        act.OnPrologueComplete += (a, p, o) => { wasPrologueCompleteInvoked = true; };
        act.Init("Test Act");
        act.Perform();


        // Assertions
        Assert.IsTrue(!wasPrologueCompleteInvoked, "OnPrologueComplete invoked despite empty prologue list assigned!");


        yield return null;
    }
    [UnityTest]
    public IEnumerator OnPrologueCompleteWithNullPrologue()
    {
        // Prerequisites
        bool wasPrologueCompleteInvoked = false;


        // Perform Act
        var act = new Act();
        act.prologue = (a) => new() { null };
        act.OnPrologueComplete += (a, p, o) => { wasPrologueCompleteInvoked = true; };
        act.Init("Test Act");
        act.Perform();


        // Assertions
        Assert.IsTrue(!wasPrologueCompleteInvoked, "OnPrologueComplete invoked despite null prologue act!");
        Assert.IsTrue(act.GetOutcome() == Act.Outcome.Failure, $"Act outcome is not failure despite null prologue act! Outcome={act.GetOutcome()}");


        yield return null;
    }
    [UnityTest]
    public IEnumerator OnPrologueCompleteWhenPrologueFails()
    {
        // Prerequisites
        bool wasPrologueCompleteInvoked = false;
        Act.Outcome prologueCompleteOutcome = Act.Outcome.Pending;


        // Perform Act
        var act = new Act();
        act.prologue = (a) => new() { new() { new FailingAct() } };
        act.OnPrologueComplete += (a, p, o) => { wasPrologueCompleteInvoked = true; prologueCompleteOutcome = o; };
        act.Init("Test Act");
        act.Perform();


        // Assertions
        Assert.IsTrue(wasPrologueCompleteInvoked, "OnPrologueComplete not invoked despite prologue act failing!");
        Assert.IsTrue(prologueCompleteOutcome == Act.Outcome.Failure, $"OnPrologueComplete outcome is not failure! Outcome={prologueCompleteOutcome}");
        Assert.IsTrue(act.GetOutcome() == Act.Outcome.Failure, $"Act outcome is not failure despite prologue act failing! Outcome={act.GetOutcome()}");


        yield return null;
    }


    // [UnityTest]
    // public IEnumerator SelfAsOnlyPrologueSkipped()
    // {
    //     // Perform Act
    //     var act = new Act();
    //     act.prologue = (a) => new() { new() { a } };
    //     act.Init("Test Act");
    //     act.Perform();


    //     // Assertions
    //     Assert.IsTrue(act.GetOutcome() == Act.Outcome.Success, $"Act could not perform when passing only itself as prologue!");


    //     yield return null;
    // }
    // [UnityTest]
    // public IEnumerator SelfAsPrologueSkipped()
    // {
    //     // Prologue Act
    //     var didProloguePerform = false;
    //     var pAct = new Act();
    //     pAct.OnPreEnter += (a) =>
    //     {
    //         didProloguePerform = true;
    //     };
    //     pAct.Init("Prologue Act");


    //     // Perform Act
    //     var didEnter = false;
    //     var act = new Act();
    //     act.prologue = (a) => new() { new() { a, pAct } };
    //     act.OnPreEnter += (a) =>
    //     {
    //         didEnter = true;
    //     };
    //     act.Init("Test Act");
    //     act.Perform();


    //     // Assertions
    //     Assert.IsTrue(didEnter, $"Act could not perform when passing itself as one of the prologues!");
    //     Assert.IsTrue(didProloguePerform, $"Passing self as prologue interfered with other prologue");


    //     yield return null;
    // }



    [UnityTest]
    public IEnumerator PrologueNull()
    {
        // Null prologue list
        {
            var act = new Act();
            act.prologue = (a) => null;
            act.Init("Test Act");
            act.Perform();

            // Assertions
            Assert.IsTrue(act.GetOutcome() == Act.Outcome.Failure, $"Act did not fail when prologue returned null! Outcome={act.GetOutcome()}");
        }

        // Prologue list containing null
        {
            var act = new Act();
            act.prologue = (a) => new() { null };
            act.Init("Test Act");
            act.Perform();

            // Assertions
            Assert.IsTrue(act.GetOutcome() == Act.Outcome.Failure, $"Act did not fail when prologue list contained null! Outcome={act.GetOutcome()}");
        }

        yield return null;
    }
    [UnityTest]
    public IEnumerator SeqPrologueNull()
    {
        // Seq with inner list containing null
        {
            var act = new Act();
            act.prologue = (a) => new() { new() { null } };
            act.Init("Test Act");
            act.Perform();

            // Assertions
            Assert.IsTrue(act.GetOutcome() == Act.Outcome.Failure, $"Act did not fail for Seq() with inner null! Outcome={act.GetOutcome()}");
        }

        // Seq with null inner list
        {
            var act = new Act();
            act.prologue = (a) => new() { null };
            act.Init("Test Act");
            act.Perform();

            // Assertions
            Assert.IsTrue(act.GetOutcome() == Act.Outcome.Failure, $"Act did not fail for Seq() with null inner list! Outcome={act.GetOutcome()}");
        }

        // Seq with null passed directly
        {
            var act = new Act();
            act.prologue = (a) => null;
            act.Init("Test Act");
            act.Perform();

            // Assertions
            Assert.IsTrue(act.GetOutcome() == Act.Outcome.Failure, $"Act did not fail for Seq(null)! Outcome={act.GetOutcome()}");
        }

        yield return null;
    }
}
