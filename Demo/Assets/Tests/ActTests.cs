using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;


// // Waits pending on Enter, exposes Finish() publicly for tests
// public class FinishableAct : Act
// {
//     protected override Outcome Enter()
//     {
//         return Outcome.Pending;
//     }
//     public void CallFinish(Outcome outcome = Outcome.Success)
//     {
//         Finish(outcome);
//     }
// }
// // Waits pending on Enter but allows reperforming

// // Retries once internally then succeeds
// public class RetryOnceAct : Act
// {
//     public int enterCount = 0;
//     protected override Outcome Enter()
//     {
//         enterCount++;
//         return enterCount == 1 ? Outcome.Retry : Outcome.Success;
//     }
// }
// // Retries internally but fails to reperform since CanPerform only true once
// public class FailRetryAct : Act
// {
//     private int _canPerformCount = 0;
//     protected override bool CanPerform()
//     {
//         _canPerformCount++;
//         return _canPerformCount == 1;
//     }
//     protected override Outcome Enter()
//     {
//         return Outcome.Retry;
//     }
// }
// // Fails immediately on entering
// public class FailOnEnterAct : Act
// {
//     protected override Outcome Enter()
//     {
//         return Outcome.Failure;
//     }
// }
// // Ticks on all 3 tick types
// public class TestAllTicksAct : Act
// {
//     public int tickCount = 0;
//     public int physicsTickCount = 0;
//     public int lateTickCount = 0;
//     protected override void Setup()
//     {
//         _tickFlags = TickFlags.Tick | TickFlags.PhysicsTick | TickFlags.LateTick;
//     }
//     protected override Outcome Tick()
//     {
//         tickCount++;
//         return Outcome.Pending;
//     }
//     protected override Outcome PhysicsTick()
//     {
//         physicsTickCount++;
//         return Outcome.Pending;
//     }
//     protected override Outcome LateTick()
//     {
//         lateTickCount++;
//         return Outcome.Pending;
//     }
// }


// Tests
// public class ActPrologueTests
// {
//     [UnityTest]
//     public IEnumerator PrePrologueNotBroadcastWhenNoPrologueAssigned()  // Checks pre prologue not broadcast when no prologue acts assigned
//     {
//         // Prerequisites
//         bool wasPrePrologueInvoked = false;


//         // Perform Act
//         var act = new Act();
//         act.OnPrePrologue += (a) => { wasPrePrologueInvoked = true; };
//         act.Init("Test Act");
//         act.Perform();


//         // Assertions
//         Assert.IsFalse(wasPrePrologueInvoked, "OnPrePrologue invoked despite no prologue acts assigned!");


//         yield return null;
//     }
//     [UnityTest]
//     public IEnumerator PrePrologueNotBroadcastWhenEmptyPrologueAssigned()  // Checks pre prologue not broadcast when empty prologue list assigned
//     {
//         // Prerequisites
//         bool wasPrePrologueInvoked = false;


//         // Perform Act
//         var act = new Act();
//         act.prologue = (a) => new List<Act>();
//         act.OnPrePrologue += (a) => { wasPrePrologueInvoked = true; };
//         act.Init("Test Act");
//         act.Perform();


//         // Assertions
//         Assert.IsFalse(wasPrePrologueInvoked, "OnPrePrologue invoked despite empty prologue list assigned!");


//         yield return null;
//     }
//     [UnityTest]
//     public IEnumerator OnPostPrologue()  // Checks OnPostPrologue broadcast with correct arguments
//     {
//         // Prerequisites
//         var prologueAct = new Act();
//         prologueAct.Init("Prologue Act");
//         bool wasPostPrologueInvoked = false;
//         Act postPrologueArg1 = null;


//         // Perform Act
//         var mainAct = new Act();
//         mainAct.prologue = (a) => new() { prologueAct };
//         mainAct.OnPostPrologue += (a) => { wasPostPrologueInvoked = true; postPrologueArg1 = a; };
//         mainAct.Init("Main Act");
//         mainAct.Perform();


//         // Assertions
//         Assert.IsTrue(wasPostPrologueInvoked, "OnPostPrologue not invoked!");
//         Assert.IsTrue(postPrologueArg1 == mainAct, $"OnPostPrologue first argument is invalid! Arg1=`{postPrologueArg1}`");


//         yield return null;
//     }
//     [UnityTest]
//     public IEnumerator PostPrologueNotBroadcastWhenNoPrologueAssigned()  // Checks post prologue not broadcast when no prologue acts assigned
//     {
//         // Prerequisites
//         bool wasPostPrologueInvoked = false;


//         // Perform Act
//         var act = new Act();
//         act.OnPostPrologue += (a) => { wasPostPrologueInvoked = true; };
//         act.Init("Test Act");
//         act.Perform();


//         // Assertions
//         Assert.IsFalse(wasPostPrologueInvoked, "OnPostPrologue invoked despite no prologue acts assigned!");


//         yield return null;
//     }
//     [UnityTest]
//     public IEnumerator PostPrologueNotBroadcastWhenEmptyPrologueAssigned()  // Checks post prologue not broadcast when empty prologue list assigned
//     {
//         // Prerequisites
//         bool wasPostPrologueInvoked = false;


//         // Perform Act
//         var act = new Act();
//         act.prologue = (a) => new List<Act>();
//         act.OnPostPrologue += (a) => { wasPostPrologueInvoked = true; };
//         act.Init("Test Act");
//         act.Perform();


//         // Assertions
//         Assert.IsFalse(wasPostPrologueInvoked, "OnPostPrologue invoked despite empty prologue list assigned!");


//         yield return null;
//     }
//     [UnityTest]
//     public IEnumerator PostPrologueNotBroadcastWhenNullPassed()  // Checks post prologue not broadcast when null passed to prologue
//     {
//         // Prerequisites
//         bool wasPostPrologueInvoked = false;


//         // Perform Act
//         var act = new Act();
//         act.prologue = (a) => new() { null };
//         act.OnPostPrologue += (a) => { wasPostPrologueInvoked = true; };
//         act.Init("Test Act");
//         act.Perform();


//         // Assertions
//         Assert.IsFalse(wasPostPrologueInvoked, "OnPostPrologue invoked despite null being passed to prologue!");


//         yield return null;
//     }
//     [UnityTest]
//     public IEnumerator PostPrologueNotBroadcastWhenPrologueActFails()  // Checks post prologue not broadcast when a prologue act fails
//     {
//         // Prerequisites
//         var prologueAct = new FalseCanPerformAct();
//         prologueAct.Init("Prologue Act");
//         bool wasPostPrologueInvoked = false;


//         // Perform Act
//         var mainAct = new Act();
//         mainAct.prologue = (a) => new() { prologueAct };
//         mainAct.OnPostPrologue += (a) => { wasPostPrologueInvoked = true; };
//         mainAct.Init("Main Act");
//         mainAct.Perform();


//         // Assertions
//         Assert.IsFalse(wasPostPrologueInvoked, "OnPostPrologue invoked despite prologue act failing!");


//         yield return null;
//     }
//     [UnityTest]
//     public IEnumerator SelfInPrologueSkipped()  // Checks act calling itself in prologue is skipped
//     {
//         // Perform Act
//         Act act = null;
//         act = new Act();
//         act.prologue = (a) => new() { act };
//         act.Init("Test Act");
//         act.Perform();


//         // Assertions
//         Assert.IsTrue(act.GetOutcome() == Act.Outcome.Success, $"Act did not succeed despite self being skipped in prologue! Outcome={act.GetOutcome()}");


//         yield return null;
//     }
//     [UnityTest]
//     public IEnumerator SingleProloguePerformsBeforeMain()  // Checks single prologue act performs before main act
//     {
//         // Prerequisites
//         var prologueAct = new Act();
//         prologueAct.Init("Prologue Act");
//         var performOrder = new List<string>();
//         prologueAct.OnPerformStart += (a) => { performOrder.Add("Prologue"); };


//         // Perform Act
//         var mainAct = new Act();
//         mainAct.prologue = (a) => new() { prologueAct };
//         mainAct.OnPerformStart += (a) => { performOrder.Add("Main"); };
//         mainAct.Init("Main Act");
//         mainAct.Perform();


//         // Assertions
//         Assert.IsTrue(performOrder.Count == 2, $"Both acts did not perform! Count={performOrder.Count}");
//         Assert.IsTrue(performOrder[0] == "Prologue", "Prologue act did not perform before main act!");


//         yield return null;
//     }
//     [UnityTest]
//     public IEnumerator ChainOfTwoProloguesPerformsBeforeMain()  // Checks chain of 2 prologue acts performs before main act
//     {
//         // Prerequisites
//         var prologueAct1 = new Act();
//         prologueAct1.Init("Prologue Act 1");
//         var prologueAct2 = new Act();
//         prologueAct2.prologue = (a) => new() { prologueAct1 };
//         prologueAct2.Init("Prologue Act 2");
//         var performOrder = new List<string>();
//         prologueAct1.OnPerformStart += (a) => { performOrder.Add("Prologue1"); };
//         prologueAct2.OnPerformStart += (a) => { performOrder.Add("Prologue2"); };


//         // Perform Act
//         var mainAct = new Act();
//         mainAct.prologue = (a) => new() { prologueAct2 };
//         mainAct.OnPerformStart += (a) => { performOrder.Add("Main"); };
//         mainAct.Init("Main Act");
//         mainAct.Perform();


//         // Assertions
//         Assert.IsTrue(performOrder.Count == 3, $"Not all acts performed! Count={performOrder.Count}");
//         Assert.IsTrue(performOrder[0] == "Prologue1" && performOrder[1] == "Prologue2" && performOrder[2] == "Main", $"Chain of 2 prologues did not perform in correct order! Order={string.Join(",", performOrder)}");


//         yield return null;
//     }
//     [UnityTest]
//     public IEnumerator ChainOfThreeProloguesPerformsBeforeMain()  // Checks chain of 3 prologue acts performs before main act
//     {
//         // Prerequisites
//         var prologueAct1 = new Act();
//         prologueAct1.Init("Prologue Act 1");
//         var prologueAct2 = new Act();
//         prologueAct2.prologue = (a) => new() { prologueAct1 };
//         prologueAct2.Init("Prologue Act 2");
//         var prologueAct3 = new Act();
//         prologueAct3.prologue = (a) => new() { prologueAct2 };
//         prologueAct3.Init("Prologue Act 3");
//         var performOrder = new List<string>();
//         prologueAct1.OnPerformStart += (a) => { performOrder.Add("Prologue1"); };
//         prologueAct2.OnPerformStart += (a) => { performOrder.Add("Prologue2"); };
//         prologueAct3.OnPerformStart += (a) => { performOrder.Add("Prologue3"); };


//         // Perform Act
//         var mainAct = new Act();
//         mainAct.prologue = (a) => new() { prologueAct3 };
//         mainAct.OnPerformStart += (a) => { performOrder.Add("Main"); };
//         mainAct.Init("Main Act");
//         mainAct.Perform();


//         // Assertions
//         Assert.IsTrue(performOrder.Count == 4, $"Not all acts performed! Count={performOrder.Count}");
//         Assert.IsTrue(string.Join(",", performOrder) == "Prologue1,Prologue2,Prologue3,Main", $"Chain of 3 prologues did not perform in correct order! Order={string.Join(",", performOrder)}");


//         yield return null;
//     }
//     [UnityTest]
//     public IEnumerator TwoParallelProloguesPerformBeforeMain()  // Checks 2 parallel prologue acts perform before main act
//     {
//         // Prerequisites
//         var prologueAct1 = new Act();
//         prologueAct1.Init("Prologue Act 1");
//         var prologueAct2 = new Act();
//         prologueAct2.Init("Prologue Act 2");


//         // Perform Act
//         var mainAct = new Act();
//         mainAct.prologue = (a) => new() { prologueAct1, prologueAct2 };
//         mainAct.Init("Main Act");
//         mainAct.Perform();


//         // Assertions
//         Assert.IsTrue(mainAct.GetOutcome() == Act.Outcome.Success, $"Main act did not succeed after parallel prologues! Outcome={mainAct.GetOutcome()}");


//         yield return null;
//     }
//     [UnityTest]
//     public IEnumerator ThreeParallelProloguesPerformBeforeMain()  // Checks 3 parallel prologue acts perform before main act
//     {
//         // Prerequisites
//         var prologueAct1 = new Act();
//         prologueAct1.Init("Prologue Act 1");
//         var prologueAct2 = new Act();
//         prologueAct2.Init("Prologue Act 2");
//         var prologueAct3 = new Act();
//         prologueAct3.Init("Prologue Act 3");


//         // Perform Act
//         var mainAct = new Act();
//         mainAct.prologue = (a) => new() { prologueAct1, prologueAct2, prologueAct3 };
//         mainAct.Init("Main Act");
//         mainAct.Perform();


//         // Assertions
//         Assert.IsTrue(mainAct.GetOutcome() == Act.Outcome.Success, $"Main act did not succeed after 3 parallel prologues! Outcome={mainAct.GetOutcome()}");


//         yield return null;
//     }
//     [UnityTest]
//     public IEnumerator SeqOfTwoWorks()  // Checks Seq of 2 prologues works
//     {
//         // Prerequisites
//         var actA = new Act();
//         actA.Init("Act A");
//         var actB = new Act();
//         actB.Init("Act B");


//         // Perform Act
//         var mainAct = new Act();
//         mainAct.prologue = (a) => Act.Seq(new() { new() { actA }, new() { actB } });
//         mainAct.Init("Main Act");
//         mainAct.Perform();


//         // Assertions
//         Assert.IsTrue(mainAct.GetOutcome() == Act.Outcome.Success, $"Main act did not succeed after Seq of 2! Outcome={mainAct.GetOutcome()}");


//         yield return null;
//     }
//     [UnityTest]
//     public IEnumerator SeqOfThreeWorks()  // Checks Seq of 3 prologues works
//     {
//         // Prerequisites
//         var actA = new Act();
//         actA.Init("Act A");
//         var actB = new Act();
//         actB.Init("Act B");
//         var actC = new Act();
//         actC.Init("Act C");


//         // Perform Act
//         var mainAct = new Act();
//         mainAct.prologue = (a) => Act.Seq(new() { new() { actA }, new() { actB }, new() { actC } });
//         mainAct.Init("Main Act");
//         mainAct.Perform();


//         // Assertions
//         Assert.IsTrue(mainAct.GetOutcome() == Act.Outcome.Success, $"Main act did not succeed after Seq of 3! Outcome={mainAct.GetOutcome()}");


//         yield return null;
//     }
//     [UnityTest]
//     public IEnumerator SeqOfTwoParallelWorks()  // Checks Seq of 2 parallel prologues works
//     {
//         // Prerequisites
//         var actA1 = new Act();
//         actA1.Init("Act A1");
//         var actA2 = new Act();
//         actA2.Init("Act A2");
//         var actB1 = new Act();
//         actB1.Init("Act B1");
//         var actB2 = new Act();
//         actB2.Init("Act B2");


//         // Perform Act
//         var mainAct = new Act();
//         mainAct.prologue = (a) => Act.Seq(new() { new() { actA1, actA2 }, new() { actB1, actB2 } });
//         mainAct.Init("Main Act");
//         mainAct.Perform();


//         // Assertions
//         Assert.IsTrue(mainAct.GetOutcome() == Act.Outcome.Success, $"Main act did not succeed after Seq of 2 parallel! Outcome={mainAct.GetOutcome()}");


//         yield return null;
//     }
//     [UnityTest]
//     public IEnumerator SeqOfThreeParallelWorks()  // Checks Seq of 3 parallel prologues works
//     {
//         // Prerequisites
//         var actA1 = new Act();
//         actA1.Init("Act A1");
//         var actA2 = new Act();
//         actA2.Init("Act A2");
//         var actA3 = new Act();
//         actA3.Init("Act A3");
//         var actB1 = new Act();
//         actB1.Init("Act B1");
//         var actB2 = new Act();
//         actB2.Init("Act B2");
//         var actB3 = new Act();
//         actB3.Init("Act B3");


//         // Perform Act
//         var mainAct = new Act();
//         mainAct.prologue = (a) => Act.Seq(new() { new() { actA1, actA2, actA3 }, new() { actB1, actB2, actB3 } });
//         mainAct.Init("Main Act");
//         mainAct.Perform();


//         // Assertions
//         Assert.IsTrue(mainAct.GetOutcome() == Act.Outcome.Success, $"Main act did not succeed after Seq of 3 parallel! Outcome={mainAct.GetOutcome()}");


//         yield return null;
//     }
//     [UnityTest]
//     public IEnumerator SeqMixedGroupsWorks()  // Checks Seq {actA} {actB actC} {actD} works
//     {
//         // Prerequisites
//         var actA = new Act();
//         actA.Init("Act A");
//         var actB = new Act();
//         actB.Init("Act B");
//         var actC = new Act();
//         actC.Init("Act C");
//         var actD = new Act();
//         actD.Init("Act D");


//         // Perform Act
//         var mainAct = new Act();
//         mainAct.prologue = (a) => Act.Seq(new() { new() { actA }, new() { actB, actC }, new() { actD } });
//         mainAct.Init("Main Act");
//         mainAct.Perform();


//         // Assertions
//         Assert.IsTrue(mainAct.GetOutcome() == Act.Outcome.Success, $"Main act did not succeed after mixed Seq groups! Outcome={mainAct.GetOutcome()}");


//         yield return null;
//     }
//     [UnityTest]
//     public IEnumerator SeqTripleGroupsWorks()  // Checks Seq of 3 groups with 2 acts each works
//     {
//         // Prerequisites
//         var actA1 = new Act();
//         actA1.Init("Act A1");
//         var actA2 = new Act();
//         actA2.Init("Act A2");
//         var actB1 = new Act();
//         actB1.Init("Act B1");
//         var actB2 = new Act();
//         actB2.Init("Act B2");
//         var actC1 = new Act();
//         actC1.Init("Act C1");
//         var actC2 = new Act();
//         actC2.Init("Act C2");


//         // Perform Act
//         var mainAct = new Act();
//         mainAct.prologue = (a) => Act.Seq(new() { new() { actA1, actA2 }, new() { actB1, actB2 }, new() { actC1, actC2 } });
//         mainAct.Init("Main Act");
//         mainAct.Perform();


//         // Assertions
//         Assert.IsTrue(mainAct.GetOutcome() == Act.Outcome.Success, $"Main act did not succeed after Seq of 3 groups! Outcome={mainAct.GetOutcome()}");


//         yield return null;
//     }
//     [UnityTest]
//     public IEnumerator PrologueCompletesWhenPrologueActBlocksMain()  // Checks prologue completes even when a prologue act blocks main act
//     {
//         // Prerequisites
//         var prologueAct = new WaitInfiniAct();
//         prologueAct.Init("Prologue Act");


//         // Perform Act
//         var mainAct = new WaitInfiniAct();
//         mainAct.prologue = (a) => new() { prologueAct };
//         prologueAct.AddToBlock(new() { mainAct }, Act.BlockType.Persistent);
//         mainAct.Init("Main Act");
//         mainAct.Perform();


//         // Assertions
//         Assert.IsFalse(mainAct.IsOngoing(), "Main act still ongoing despite being blocked by its own prologue act!");


//         yield return null;
//     }
//     [UnityTest]
//     public IEnumerator PrologueCompletesWhenMainBlocksPrologueAct()  // Checks prologue completes even when main act blocks prologue act
//     {
//         // Prerequisites
//         var prologueAct = new WaitInfiniAct();
//         prologueAct.Init("Prologue Act");


//         // Perform Act
//         var mainAct = new WaitInfiniAct();
//         mainAct.prologue = (a) => new() { prologueAct };
//         mainAct.AddToBlock(new() { prologueAct }, Act.BlockType.Persistent);
//         mainAct.Init("Main Act");
//         mainAct.Perform();


//         // Assertions
//         Assert.IsFalse(mainAct.IsOngoing(), "Main act still ongoing despite blocking its own prologue act!");
//         Assert.IsTrue(mainAct.GetOutcome() == Act.Outcome.Failure, $"Main act outcome incorrect when blocking its own prologue act! Outcome={mainAct.GetOutcome()}");


//         yield return null;
//     }
//     [UnityTest]
//     public IEnumerator ProloguesCompleteWhenSiblingBlocksSibling()  // Checks prologue completes even when a prologue act blocks a sibling act
//     {
//         // Prerequisites
//         var prologueAct1 = new Act();
//         prologueAct1.Init("Prologue Act 1");
//         var prologueAct2 = new WaitInfiniAct();
//         prologueAct2.Init("Prologue Act 2");
//         prologueAct1.AddToBlock(new() { prologueAct2 }, Act.BlockType.Persistent);


//         // Perform Act
//         var mainAct = new Act();
//         mainAct.prologue = (a) => new() { prologueAct1, prologueAct2 };
//         mainAct.Init("Main Act");
//         mainAct.Perform();


//         // Assertions
//         Assert.IsFalse(mainAct.IsOngoing(), "Main act did not complete despite a sibling prologue act blocking another sibling!");


//         yield return null;
//     }
//     [UnityTest]
//     public IEnumerator PrologueClearedWhenActFails()  // Checks failing an act clears prologues further down the Seq
//     {
//         // Prerequisites
//         var actA = new FalseCanPerformAct();
//         actA.Init("Act A");
//         var actB = new Act();
//         actB.Init("Act B");


//         // Perform Act
//         var mainAct = new Act();
//         mainAct.prologue = (a) => Act.Seq(new() { new() { actA }, new() { actB } });
//         mainAct.Init("Main Act");
//         mainAct.Perform();


//         // Assertions
//         Assert.IsTrue(mainAct.GetOutcome() == Act.Outcome.Failure, $"Main act outcome incorrect when a Seq act fails! Outcome={mainAct.GetOutcome()}");
//         Assert.IsFalse(actB.IsOngoing(), "Act B still ongoing despite Act A in the same Seq chain failing!");


//         yield return null;
//     }
// }
// public class ActEnteringTests
// {
//     [UnityTest]
//     public IEnumerator CorrectEnteringStatusApplied()  // Checks correct entering status applied
//     {
//         // Prerequisites
//         Act.Status statusDuringEnter = Act.Status.None;


//         // Perform Act
//         var act = new FinishableAct();
//         act.OnPreEnter += (a) => { statusDuringEnter = a.GetStatus(); };
//         act.Init("Test Act");
//         act.Perform();


//         // Assertions
//         Assert.IsTrue(statusDuringEnter == Act.Status.Entering, $"Status is not Entering during enter! Status={statusDuringEnter}");


//         yield return null;
//     }
//     [UnityTest]
//     public IEnumerator OnPreAndPostEnter()  // Checks OnPreEnter & OnPostEnter broadcast with correct arguments
//     {
//         // Prerequisites
//         bool wasPreEnterInvoked = false;
//         Act preEnterArg1 = null;
//         bool wasPostEnterInvoked = false;
//         Act postEnterArg1 = null;


//         // Perform Act
//         var act = new Act();
//         act.OnPreEnter += (a) => { wasPreEnterInvoked = true; preEnterArg1 = a; };
//         act.OnPostEnter += (a) => { wasPostEnterInvoked = true; postEnterArg1 = a; };
//         act.Init("Test Act");
//         act.Perform();


//         // Assertions
//         Assert.IsTrue(wasPreEnterInvoked, "OnPreEnter not invoked!");
//         Assert.IsTrue(preEnterArg1 == act, $"OnPreEnter first argument is invalid! Arg1=`{preEnterArg1}`");
//         Assert.IsTrue(wasPostEnterInvoked, "OnPostEnter not invoked!");
//         Assert.IsTrue(postEnterArg1 == act, $"OnPostEnter first argument is invalid! Arg1=`{postEnterArg1}`");


//         yield return null;
//     }
//     [UnityTest]
//     public IEnumerator EnterIsInvoked()  // Checks Enter() is invoked
//     {
//         // Perform Act
//         var act = new EnterAct();
//         act.Init("Test Act");
//         act.Perform();


//         // Assertions
//         Assert.IsTrue(act.callCount == 1, $"Enter() not invoked exactly once! Call count={act.callCount}");


//         yield return null;
//     }
//     [UnityTest]
//     public IEnumerator PendingOutcomeWaitsForFinish()  // Checks pending outcome does not exit immediately
//     {
//         // Perform Act
//         var act = new WaitInfiniAct();
//         act.Init("Test Act");
//         act.Perform();


//         // Assertions
//         Assert.IsTrue(act.IsOngoing(), "Act exited immediately despite Enter() returning pending!");


//         yield return null;
//     }
//     [UnityTest]
//     public IEnumerator FinishContinuesToExit()  // Checks Finish() continues act to exit
//     {
//         // Perform Act
//         var act = new FinishableAct();
//         act.Init("Test Act");
//         act.Perform();
//         act.CallFinish(Act.Outcome.Success);


//         // Assertions
//         Assert.IsFalse(act.IsOngoing(), "Act did not exit after Finish() was called!");


//         yield return null;
//     }
//     [UnityTest]
//     public IEnumerator FinishPassesAccurateOutcome()  // Checks Finish() passes accurate outcome to exit
//     {
//         // Perform Act
//         var act = new FinishableAct();
//         act.Init("Test Act");
//         act.Perform();
//         act.CallFinish(Act.Outcome.Failure);


//         // Assertions
//         Assert.IsTrue(act.GetOutcome() == Act.Outcome.Failure, $"Outcome after Finish() is inaccurate! Outcome={act.GetOutcome()}");


//         yield return null;
//     }
// }
// public class ActTickingTests
// {
//     [UnityTest]
//     public IEnumerator CorrectTickingStatusApplied()  // Checks correct ticking status applied
//     {
//         // Prerequisites
//         var theater = new GameObject().AddComponent<Theater>();
//         Act.Status statusDuringTick = Act.Status.None;


//         // Perform Act
//         var act = new TickAct();
//         act.OnPreTick += (a) => { statusDuringTick = a.GetStatus(); };
//         act.Init("Test Act", theater);
//         act.Perform();
//         yield return null;


//         // Assertions
//         Assert.IsTrue(statusDuringTick == Act.Status.Ticking, $"Status is not Ticking during tick! Status={statusDuringTick}");


//         UnityEngine.Object.Destroy(theater.gameObject);
//     }
//     [UnityTest]
//     public IEnumerator OnPreAndPostTick()  // Checks pre tick & post tick broadcast with correct arguments
//     {
//         // Prerequisites
//         var theater = new GameObject().AddComponent<Theater>();
//         bool wasPreTickInvoked = false;
//         Act preTickArg1 = null;
//         bool wasPostTickInvoked = false;
//         Act postTickArg1 = null;


//         // Perform Act
//         var act = new TickAct();
//         act.OnPreTick += (a) => { wasPreTickInvoked = true; preTickArg1 = a; };
//         act.OnPostTick += (a) => { wasPostTickInvoked = true; postTickArg1 = a; };
//         act.Init("Test Act", theater);
//         act.Perform();
//         yield return null;


//         // Assertions
//         Assert.IsTrue(wasPreTickInvoked, "OnPreTick not invoked!");
//         Assert.IsTrue(preTickArg1 == act, $"OnPreTick first argument is invalid! Arg1=`{preTickArg1}`");
//         Assert.IsTrue(wasPostTickInvoked, "OnPostTick not invoked!");
//         Assert.IsTrue(postTickArg1 == act, $"OnPostTick first argument is invalid! Arg1=`{postTickArg1}`");


//         UnityEngine.Object.Destroy(theater.gameObject);
//     }
//     [UnityTest]
//     public IEnumerator AllTickMethodsInvoked()  // Checks all tick methods being invoked
//     {
//         // Prerequisites
//         var theater = new GameObject().AddComponent<Theater>();


//         // Perform Act
//         var act = new TestAllTicksAct();
//         act.Init("Test Act", theater);
//         act.Perform();
//         yield return new WaitForFixedUpdate();
//         yield return null;


//         // Assertions
//         Assert.IsTrue(act.tickCount >= 1, $"Tick() not invoked! Call count={act.tickCount}");
//         Assert.IsTrue(act.physicsTickCount >= 1, $"PhysicsTick() not invoked! Call count={act.physicsTickCount}");
//         Assert.IsTrue(act.lateTickCount >= 1, $"LateTick() not invoked! Call count={act.lateTickCount}");


//         UnityEngine.Object.Destroy(theater.gameObject);
//     }
//     [UnityTest]
//     public IEnumerator TickingWorksForAllTickTypes()  // Checks ticking works for tick physics tick and late tick individually
//     {
//         // Prerequisites
//         var theater = new GameObject().AddComponent<Theater>();


//         // Perform Act
//         var tickAct = new TickAct();
//         tickAct.Init("Tick Act", theater);
//         tickAct.Perform();

//         var physicsAct = new PhysicsTickAct();
//         physicsAct.Init("Physics Act", theater);
//         physicsAct.Perform();

//         var lateAct = new LateTickAct();
//         lateAct.Init("Late Act", theater);
//         lateAct.Perform();

//         yield return new WaitForFixedUpdate();
//         yield return null;


//         // Assertions
//         Assert.IsTrue(tickAct.callCount >= 1, $"Tick() not invoked! Call count={tickAct.callCount}");
//         Assert.IsTrue(physicsAct.callCount >= 1, $"PhysicsTick() not invoked! Call count={physicsAct.callCount}");
//         Assert.IsTrue(lateAct.callCount >= 1, $"LateTick() not invoked! Call count={lateAct.callCount}");


//         UnityEngine.Object.Destroy(theater.gameObject);
//     }
//     [UnityTest]
//     public IEnumerator TickingNotInvokedWhenFlagNone()  // Checks ticking not invoked when tick flag is none
//     {
//         // Prerequisites
//         var theater = new GameObject().AddComponent<Theater>();


//         // Perform Act
//         var act = new Act();
//         act.Init("Test Act", theater);
//         act.Perform();
//         yield return new WaitForFixedUpdate();
//         yield return null;


//         // Assertions
//         Assert.IsFalse(act.CanTick(Act.TickFlags.Tick), "Act reports ticking despite no tick flag assigned!");
//         Assert.IsFalse(act.IsOngoing(), "Act still ongoing despite no tick flag assigned!");


//         UnityEngine.Object.Destroy(theater.gameObject);
//     }
//     [UnityTest]
//     public IEnumerator DeltaValuesAccurate()  // Checks GetDelta and GetPhysicsDelta return accurate values
//     {
//         // Assertions
//         Assert.IsTrue(Act.GetDelta() == Time.deltaTime, $"GetDelta() is inaccurate! Value={Act.GetDelta()}");
//         Assert.IsTrue(Act.GetPhysicsDelta() == Time.fixedDeltaTime, $"GetPhysicsDelta() is inaccurate! Value={Act.GetPhysicsDelta()}");


//         yield return null;
//     }
// }
// public class ActExitingTests
// {
//     [UnityTest]
//     public IEnumerator CorrectExitingStatusApplied()  // Checks correct exiting status applied
//     {
//         // Prerequisites
//         Act.Status statusDuringExit = Act.Status.None;


//         // Perform Act
//         var act = new Act();
//         act.OnPreExit += (a) => { statusDuringExit = a.GetStatus(); };
//         act.Init("Test Act");
//         act.Perform();


//         // Assertions
//         Assert.IsTrue(statusDuringExit == Act.Status.Exiting, $"Status is not Exiting during exit! Status={statusDuringExit}");


//         yield return null;
//     }
//     [UnityTest]
//     public IEnumerator OnPreAndPostExit()  // Checks pre exit & post exit broadcast with correct arguments
//     {
//         // Prerequisites
//         bool wasPreExitInvoked = false;
//         Act preExitArg1 = null;
//         bool wasPostExitInvoked = false;
//         Act postExitArg1 = null;


//         // Perform Act
//         var act = new Act();
//         act.OnPreExit += (a) => { wasPreExitInvoked = true; preExitArg1 = a; };
//         act.OnPostExit += (a) => { wasPostExitInvoked = true; postExitArg1 = a; };
//         act.Init("Test Act");
//         act.Perform();


//         // Assertions
//         Assert.IsTrue(wasPreExitInvoked, "OnPreExit not invoked!");
//         Assert.IsTrue(preExitArg1 == act, $"OnPreExit first argument is invalid! Arg1=`{preExitArg1}`");
//         Assert.IsTrue(wasPostExitInvoked, "OnPostExit not invoked!");
//         Assert.IsTrue(postExitArg1 == act, $"OnPostExit first argument is invalid! Arg1=`{postExitArg1}`");


//         yield return null;
//     }
//     [UnityTest]
//     public IEnumerator ExitIsInvoked()  // Checks Exit() is invoked
//     {
//         // Perform Act
//         var act = new ExitAct();
//         act.Init("Test Act");
//         act.Perform();


//         // Assertions
//         Assert.IsTrue(act.callCount == 1, $"Exit() not invoked exactly once! Call count={act.callCount}");


//         yield return null;
//     }
//     [UnityTest]
//     public IEnumerator PerformDuringExitDoesNotReperform()  // Checks perform while exiting does not reperform with canReperform true or false
//     {
//         // Perform Act
//         var nonReperformAct = new ExitAct();
//         nonReperformAct.OnPreExit += (a) => { a.Perform(); };
//         nonReperformAct.Init("Non Reperform Act");
//         nonReperformAct.Perform();

//         var reperformAct = new ReperformableInfiAct();
//         var exitCount = 0;
//         reperformAct.OnPreExit += (a) =>
//         {
//             exitCount++;
//             a.Perform();
//         };
//         reperformAct.Init("Reperform Act");
//         reperformAct.Perform();
//         reperformAct.Abort();


//         // Assertions
//         Assert.IsTrue(nonReperformAct.callCount == 1, "Non reperformable act exited more than once during exit!");
//         Assert.IsTrue(exitCount == 1, "Reperformable act reperformed despite being called during exit!");


//         yield return null;
//     }
//     [UnityTest]
//     public IEnumerator AbortDuringExitDoesNotExitAgain()  // Checks abort while exiting does not exit act again
//     {
//         // Perform Act
//         var act = new ExitAct();
//         act.OnPreExit += (a) => { a.Abort(); };
//         act.Init("Test Act");
//         act.Perform();


//         // Assertions
//         Assert.IsTrue(act.callCount == 1, $"Exit() invoked more than once despite Abort() called during exit! Call count={act.callCount}");


//         yield return null;
//     }
//     [UnityTest]
//     public IEnumerator StatusResetAfterExit()  // Checks status reset to none after exiting
//     {
//         // Perform Act
//         var act = new Act();
//         act.Init("Test Act");
//         act.Perform();


//         // Assertions
//         Assert.IsTrue(act.GetStatus() == Act.Status.None, $"Status not reset to None after exiting! Status={act.GetStatus()}");


//         yield return null;
//     }
//     [UnityTest]
//     public IEnumerator PerformEndInvokedAfterExit()  // Checks perform end invoked after exiting
//     {
//         // Prerequisites
//         bool wasPerformEndInvoked = false;


//         // Perform Act
//         var act = new Act();
//         act.OnPerformEnd += (a) => { wasPerformEndInvoked = true; };
//         act.Init("Test Act");
//         act.Perform();


//         // Assertions
//         Assert.IsTrue(wasPerformEndInvoked, "OnPerformEnd not invoked after exiting!");


//         yield return null;
//     }
//     [UnityTest]
//     public IEnumerator ProloguesAndEpiloguesClearedAfterExit()  // Checks prologues and epilogues cleared after exiting
//     {
//         // Prerequisites
//         var prologueAct = new Act();
//         prologueAct.Init("Prologue Act");


//         // Perform Act
//         var mainAct = new Act();
//         mainAct.prologue = (a) => new() { prologueAct };
//         mainAct.Init("Main Act");
//         mainAct.Perform();


//         // Assertions
//         Assert.IsFalse(prologueAct.IsOngoing(), "Prologue act still ongoing after main act exited!");
//         Assert.IsFalse(mainAct.IsOngoing(), "Main act still ongoing after exiting!");


//         yield return null;
//     }
// }
// public class ActBlockingTests
// {
//     [UnityTest]
//     public IEnumerator OnBlockChangedBroadcast()  // Checks block changed broadcast with correct arguments when blocked and unblocked
//     {
//         // Prerequisites
//         var blockedAct = new WaitInfiniAct();
//         blockedAct.Init("Blocked Act");
//         bool wasBlockInvoked = false;
//         bool didBlockArg = false;


//         // Perform Act
//         blockedAct.OnBlockChanged += (a, byAct, blockType, didBlock) => { wasBlockInvoked = true; didBlockArg = didBlock; };
//         var blockingAct = new WaitInfiniAct();
//         blockingAct.AddToBlock(new() { blockedAct });
//         blockingAct.Init("Blocking Act");
//         blockingAct.Perform();


//         // Assertions
//         Assert.IsTrue(wasBlockInvoked, "OnBlockChanged not invoked when blocked!");
//         Assert.IsTrue(didBlockArg, "OnBlockChanged didBlock argument is false despite blocking!");


//         // Unblock
//         wasBlockInvoked = false;
//         blockingAct.RemoveFromBlock(new() { blockedAct });


//         // Assertions
//         Assert.IsTrue(wasBlockInvoked, "OnBlockChanged not invoked when unblocked!");


//         yield return null;
//     }
//     [UnityTest]
//     public IEnumerator OnBlockChangedNotBroadcastOnEnableChange()  // Checks block changed not broadcast when enabled or disabled
//     {
//         // Prerequisites
//         bool wasBlockInvoked = false;


//         // Perform Act
//         var act = new Act();
//         act.OnBlockChanged += (a, byAct, blockType, didBlock) => { wasBlockInvoked = true; };
//         act.Init("Test Act");
//         act.SetEnabled(false);
//         act.SetEnabled(true);


//         // Assertions
//         Assert.IsFalse(wasBlockInvoked, "OnBlockChanged invoked despite only being enabled disabled!");


//         yield return null;
//     }
//     [UnityTest]
//     public IEnumerator BlockingAbortsAct()  // Checks blocking aborts the ongoing act
//     {
//         // Perform Act
//         var act = new WaitInfiniAct();
//         act.Init("Test Act");
//         act.Perform();

//         var blockingAct = new WaitInfiniAct();
//         blockingAct.Init("Blocking Act");
//         blockingAct.AddToBlock(new() { act });
//         blockingAct.Perform();


//         // Assertions
//         Assert.IsFalse(act.IsOngoing(), "Act still ongoing despite being blocked!");


//         yield return null;
//     }
//     [UnityTest]
//     public IEnumerator PersistentlyBlockedActFailsToPerform()  // Checks persistently blocked act fails to perform
//     {
//         // Perform Act
//         var act = new WaitInfiniAct();
//         act.Init("Test Act");

//         var blockingAct = new WaitInfiniAct();
//         blockingAct.Init("Blocking Act");
//         blockingAct.AddToBlock(new() { act }, Act.BlockType.Persistent);
//         blockingAct.Perform();
//         act.Perform();


//         // Assertions
//         Assert.IsFalse(act.IsOngoing(), "Persistently blocked act performed despite being blocked!");


//         yield return null;
//     }
//     [UnityTest]
//     public IEnumerator UnblockedActCanPerform()  // Checks act which was just unblocked can perform
//     {
//         // Perform Act
//         var act = new WaitInfiniAct();
//         act.Init("Test Act");

//         var blockingAct = new WaitInfiniAct();
//         blockingAct.Init("Blocking Act");
//         blockingAct.AddToBlock(new() { act }, Act.BlockType.Persistent);
//         blockingAct.Perform();
//         blockingAct.RemoveFromBlock(new() { act });
//         act.Perform();


//         // Assertions
//         Assert.IsTrue(act.IsOngoing(), "Act did not perform despite being unblocked!");


//         yield return null;
//     }
//     [UnityTest]
//     public IEnumerator OneshotBlockedActEndsWhenMainActPerforms()  // Checks oneshot blocked act performing ends when main act performs
//     {
//         // Perform Act
//         var act = new WaitInfiniAct();
//         act.Init("Test Act");
//         act.Perform();

//         var blockingAct = new WaitInfiniAct();
//         blockingAct.Init("Blocking Act");
//         blockingAct.AddToBlock(new() { act }, Act.BlockType.Oneshot);
//         blockingAct.Perform();


//         // Assertions
//         Assert.IsFalse(act.IsOngoing(), "Oneshot blocked act still ongoing despite main act performing!");


//         yield return null;
//     }
//     [UnityTest]
//     public IEnumerator OneshotBlockedActCanPerformDuringMainActPerform()  // Checks oneshot blocked act can perform while main act still ongoing
//     {
//         // Perform Act
//         var blockingAct = new WaitInfiniAct();
//         blockingAct.Init("Blocking Act");

//         var act = new WaitInfiniAct();
//         blockingAct.AddToBlock(new() { act }, Act.BlockType.Oneshot);
//         blockingAct.Perform();
//         act.Init("Test Act");
//         act.Perform();


//         // Assertions
//         Assert.IsTrue(act.IsOngoing(), "Oneshot blocked act did not perform despite main act still ongoing!");
//         Assert.IsTrue(blockingAct.IsOngoing(), "Blocking act not ongoing!");


//         yield return null;
//     }
//     [UnityTest]
//     public IEnumerator AddingActToOngoingMainBlocksIt()  // Checks adding act to ongoing main act blocks that act
//     {
//         // Perform Act
//         var blockingAct = new WaitInfiniAct();
//         blockingAct.Init("Blocking Act");
//         blockingAct.Perform();

//         var act = new WaitInfiniAct();
//         act.Init("Test Act");
//         act.Perform();
//         blockingAct.AddToBlock(new() { act }, Act.BlockType.Persistent);


//         // Assertions
//         Assert.IsFalse(act.IsOngoing(), "Act still ongoing despite being added to block of an ongoing main act!");


//         yield return null;
//     }
//     [UnityTest]
//     public IEnumerator RemovingActFromOngoingMainUnblocksIt()  // Checks removing act from ongoing main act unblocks that act
//     {
//         // Perform Act
//         var blockingAct = new WaitInfiniAct();
//         blockingAct.Init("Blocking Act");
//         blockingAct.Perform();

//         var act = new WaitInfiniAct();
//         act.Init("Test Act");
//         blockingAct.AddToBlock(new() { act }, Act.BlockType.Persistent);
//         blockingAct.RemoveFromBlock(new() { act });
//         act.Perform();


//         // Assertions
//         Assert.IsTrue(act.IsOngoing(), "Act did not perform despite being removed from block of an ongoing main act!");


//         yield return null;
//     }
//     [UnityTest]
//     public IEnumerator PersistentBlockFailsWhenBlockingSelf()  // Checks persistent blocking fails when adding self act to block
//     {
//         // Prerequisites
//         LogAssert.Expect(LogType.Warning, "[Test Act] Trying to block self!");


//         // Perform Act
//         var act = new WaitInfiniAct();
//         act.isVerbose = true;
//         act.Init("Test Act");
//         act.AddToBlock(new() { act }, Act.BlockType.Persistent);
//         act.Perform();


//         // Assertions
//         Assert.IsTrue(act.IsOngoing(), "Act did not perform despite self block being ignored!");


//         yield return null;
//     }
//     [UnityTest]
//     public IEnumerator BlockingFailsWhenSamePrologueChain()  // Checks blocking fails when both acts are in the same prologue chain
//     {
//         // Prerequisites
//         var prologueAct = new WaitInfiniAct();
//         prologueAct.isVerbose = true;
//         prologueAct.Init("Prologue Act");
//         LogAssert.Expect(LogType.Warning, "[Prologue Act] Failed to block, Both Prologue Act and Main Act are in the same prologue chain!");


//         // Perform Act
//         var mainAct = new WaitInfiniAct();
//         mainAct.prologue = (a) => new() { prologueAct };
//         mainAct.AddToBlock(new() { prologueAct }, Act.BlockType.Persistent);
//         mainAct.Init("Main Act");
//         mainAct.Perform();


//         // Assertions
//         Assert.IsTrue(prologueAct.IsOngoing(), "Prologue act was blocked despite being in the same prologue chain as main act!");


//         yield return null;
//     }
// }
// public class ActEnablingTests
// {
//     [UnityTest]
//     public IEnumerator OnEnableChangedBroadcast()  // Checks enable changed broadcast with correct arguments when enabled and disabled
//     {
//         // Prerequisites
//         bool wasEnableChangedInvoked = false;
//         bool enableChangedArg = true;


//         // Perform Act
//         var act = new Act();
//         act.OnEnableChanged += (a, newEnabled) => { wasEnableChangedInvoked = true; enableChangedArg = newEnabled; };
//         act.Init("Test Act");
//         act.SetEnabled(false);


//         // Assertions
//         Assert.IsTrue(wasEnableChangedInvoked, "OnEnableChanged not invoked when disabled!");
//         Assert.IsFalse(enableChangedArg, "OnEnableChanged argument is true despite being disabled!");


//         // Re-enable
//         wasEnableChangedInvoked = false;
//         act.SetEnabled(true);


//         // Assertions
//         Assert.IsTrue(wasEnableChangedInvoked, "OnEnableChanged not invoked when re-enabled!");
//         Assert.IsTrue(enableChangedArg, "OnEnableChanged argument is false despite being re-enabled!");


//         yield return null;
//     }
//     [UnityTest]
//     public IEnumerator OnEnableChangedNotBroadcastOnBlockChange()  // Checks enable changed not broadcast when blocked or unblocked
//     {
//         // Prerequisites
//         bool wasEnableChangedInvoked = false;


//         // Perform Act
//         var act = new WaitInfiniAct();
//         act.OnEnableChanged += (a, newEnabled) => { wasEnableChangedInvoked = true; };
//         act.Init("Test Act");

//         var blockingAct = new WaitInfiniAct();
//         blockingAct.Init("Blocking Act");
//         blockingAct.AddToBlock(new() { act }, Act.BlockType.Persistent);
//         blockingAct.Perform();
//         blockingAct.RemoveFromBlock(new() { act });


//         // Assertions
//         Assert.IsFalse(wasEnableChangedInvoked, "OnEnableChanged invoked despite only being blocked unblocked!");


//         yield return null;
//     }
//     [UnityTest]
//     public IEnumerator DisablingAbortsAct()  // Checks disabling aborts the ongoing act
//     {
//         // Perform Act
//         var act = new WaitInfiniAct();
//         act.Init("Test Act");
//         act.Perform();
//         act.SetEnabled(false);


//         // Assertions
//         Assert.IsFalse(act.IsOngoing(), "Act still ongoing despite being disabled!");


//         yield return null;
//     }
//     [UnityTest]
//     public IEnumerator DisablingTheaterAbortsAct()  // Checks disabling theater aborts the ongoing act
//     {
//         // Prerequisites
//         var theater = new GameObject().AddComponent<Theater>();


//         // Perform Act
//         var act = new WaitInfiniAct();
//         act.Init("Test Act", theater);
//         act.Perform();
//         theater.SetEnabled(false);


//         // Assertions
//         Assert.IsFalse(act.IsOngoing(), "Act still ongoing despite theater being disabled!");


//         UnityEngine.Object.Destroy(theater.gameObject);
//         yield return null;
//     }
// }
// public class ActRetryingTests
// {
//     [UnityTest]
//     public IEnumerator RetryOutcomeWorksInternally()  // Checks retry outcome works when returned in Enter
//     {
//         // Perform Act
//         var act = new RetryOnceAct();
//         act.Init("Test Act");
//         act.Perform();


//         // Assertions
//         Assert.IsTrue(act.enterCount == 2, $"Act did not retry internally! Enter count={act.enterCount}");
//         Assert.IsTrue(act.GetOutcome() == Act.Outcome.Success, $"Act outcome incorrect after retry! Outcome={act.GetOutcome()}");


//         yield return null;
//     }
//     [UnityTest]
//     public IEnumerator RetryWorksExternally()  // Checks Retry() works when called externally
//     {
//         // Perform Act
//         var act = new WaitInfiniAct();
//         act.Init("Test Act");
//         act.Perform();
//         var enterCount = 0;
//         act.OnPreEnter += (a) => { enterCount++; };
//         act.Retry();


//         // Assertions
//         Assert.IsTrue(enterCount == 1, $"Act did not reperform when Retry() was called on ongoing act! Enter count={enterCount}");


//         yield return null;
//     }
//     [UnityTest]
//     public IEnumerator RetryPerformsActEvenIfNotOngoing()  // Checks Retry() performs the act even if not ongoing
//     {
//         // Perform Act
//         var act = new WaitInfiniAct();
//         act.Init("Test Act");
//         act.Retry();


//         // Assertions
//         Assert.IsTrue(act.IsOngoing(), "Act did not perform when Retry() was called while not ongoing!");


//         yield return null;
//     }
//     [UnityTest]
//     public IEnumerator RetryCancelsPrologues()  // Checks retry cancels prologues
//     {
//         // Prerequisites
//         var prologueAct = new WaitInfiniAct();
//         prologueAct.Init("Prologue Act");


//         // Perform Act
//         var mainAct = new RetryOnceAct();
//         mainAct.prologue = (a) => new() { prologueAct };
//         mainAct.Init("Main Act");
//         mainAct.Perform();


//         // Assertions
//         Assert.IsFalse(prologueAct.IsOngoing(), "Prologue act still ongoing despite main act retrying!");


//         yield return null;
//     }
//     [UnityTest]
//     public IEnumerator RetryDoesNotCancelEpilogues()  // Checks retry does not cancel epilogues
//     {
//         // Prerequisites
//         var mainAct = new RetryOnceAct();
//         mainAct.Init("Main Act");


//         // Perform Act
//         var epilogueAct = new Act();
//         epilogueAct.prologue = (a) => new() { mainAct };
//         epilogueAct.Init("Epilogue Act");
//         epilogueAct.Perform();


//         // Assertions
//         Assert.IsTrue(mainAct.enterCount == 2, $"Main act did not retry! Enter count={mainAct.enterCount}");
//         Assert.IsTrue(epilogueAct.GetOutcome() == Act.Outcome.Success, $"Epilogue act did not complete despite main act retrying! Outcome={epilogueAct.GetOutcome()}");


//         yield return null;
//     }
//     [UnityTest]
//     public IEnumerator FailingRetryChangesOutcomeToFailure()  // Checks failing to retry changes outcome to failure
//     {
//         // Perform Act
//         var act = new FailRetryAct();
//         act.Init("Test Act");
//         act.Perform();


//         // Assertions
//         Assert.IsTrue(act.GetOutcome() == Act.Outcome.Failure, $"Outcome not changed to Failure when retry failed! Outcome={act.GetOutcome()}");


//         yield return null;
//     }
// }
// public class ActAbortTests
// {
//     [UnityTest]
//     public IEnumerator OngoingActStopsOnAbort()  // Checks ongoing act stops when Abort() is invoked
//     {
//         // Perform Act
//         var act = new WaitInfiniAct();
//         act.Init("Test Act");
//         act.Perform();
//         act.Abort();


//         // Assertions
//         Assert.IsFalse(act.IsOngoing(), "Act still ongoing despite Abort() being invoked!");


//         yield return null;
//     }
//     [UnityTest]
//     public IEnumerator AbortWorksFromAnyLifeCycleAction()  // Checks aborting works even when invoked in any life cycle action
//     {
//         // Perform Act
//         var act = new WaitInfiniAct();
//         act.OnPreEnter += (a) => { a.Abort(); };
//         act.Init("Test Act");
//         act.Perform();


//         // Assertions
//         Assert.IsFalse(act.IsOngoing(), "Act still ongoing despite Abort() being called from a life cycle action!");


//         yield return null;
//     }
// }
// public class ActWarningsTests
// {
//     [UnityTest]
//     public IEnumerator NullTheaterWarningPrintedWhenVerbose()  // Checks null theater warning printed when verbose enabled
//     {
//         // Prerequisites
//         LogAssert.Expect(LogType.Warning, "[Test Act] Cannot perform deferred, Assign a theater first!");


//         // Perform Act
//         var act = new Act();
//         act.isVerbose = true;
//         act.Init("Test Act");
//         act.PerformDeferred();


//         // Assertions
//         Assert.IsTrue(act.GetPerformCount() == 0, "Act performed despite missing theater!");


//         yield return null;
//     }
//     [UnityTest]
//     public IEnumerator NullTheaterWarningNotPrintedWhenNotVerbose()  // Checks null theater warning not printed when verbose disabled
//     {
//         // Perform Act
//         var act = new Act();
//         act.Init("Test Act");
//         act.PerformDeferred();


//         // Assertions
//         LogAssert.NoUnexpectedReceived();


//         yield return null;
//     }
//     [UnityTest]
//     public IEnumerator CanPerformWarningsPrintedWhenVerbose()  // Checks CanPerform condition warnings printed when verbose enabled
//     {
//         // Prerequisites
//         LogAssert.Expect(LogType.Warning, "[Test Act] Cannot perform, act or theater is disabled!");


//         // Perform Act
//         var act = new Act();
//         act.isVerbose = true;
//         act.Init("Test Act");
//         act.SetEnabled(false);
//         act.Perform();


//         // Assertions
//         Assert.IsFalse(act.IsOngoing(), "Act performed despite being disabled!");


//         yield return null;
//     }
//     [UnityTest]
//     public IEnumerator CanPerformWarningsNotPrintedWhenNotVerbose()  // Checks CanPerform condition warnings not printed when verbose disabled
//     {
//         // Perform Act
//         var act = new Act();
//         act.Init("Test Act");
//         act.SetEnabled(false);
//         act.Perform();


//         // Assertions
//         LogAssert.NoUnexpectedReceived();


//         yield return null;
//     }
// }
// public class ActMiscTests
// {
//     [UnityTest]
//     public IEnumerator DidPerformTrueInSameTickFalseNext()  // Checks DidPerform true in same tick and false in next tick
//     {
//         // Prerequisites
//         var theater = new GameObject().AddComponent<Theater>();


//         // Perform Act
//         var act = new Act();
//         act.Init("Test Act", theater);
//         act.Perform();
//         var didPerformSameTick = act.DidPerform(Act.TickFlags.Tick);
//         yield return null;
//         var didPerformNextTick = act.DidPerform(Act.TickFlags.Tick);


//         // Assertions
//         Assert.IsTrue(didPerformSameTick, "DidPerform() false despite performing in the same tick!");
//         Assert.IsFalse(didPerformNextTick, "DidPerform() true despite performing in a previous tick!");


//         UnityEngine.Object.Destroy(theater.gameObject);
//     }
//     [UnityTest]
//     public IEnumerator IsOngoingTrueThroughoutPerformCycle()  // Checks IsOngoing true anywhere in perform cycle
//     {
//         // Prerequisites
//         var wasOngoingEverywhere = true;
//         var prologueAct = new Act();


//         // Perform Act
//         var act = new FinishableAct();
//         act.prologue = (a) => new() { prologueAct };
//         act.OnPrePrologue += (a) => { wasOngoingEverywhere &= a.IsOngoing(); };
//         act.OnPreEnter += (a) => { wasOngoingEverywhere &= a.IsOngoing(); };
//         act.OnPreExit += (a) => { wasOngoingEverywhere &= a.IsOngoing(); };
//         act.Init("Test Act");
//         act.Perform();
//         wasOngoingEverywhere &= act.IsOngoing();
//         act.CallFinish();
//         wasOngoingEverywhere &= !act.IsOngoing();


//         // Assertions
//         Assert.IsTrue(wasOngoingEverywhere, "IsOngoing() did not return true throughout perform cycle!");


//         yield return null;
//     }
//     [UnityTest]
//     public IEnumerator IsOngoingFalseWhenNotPerforming()  // Checks IsOngoing false if act is not performing
//     {
//         // Perform Act
//         var act = new Act();
//         act.Init("Test Act");


//         // Assertions
//         Assert.IsFalse(act.IsOngoing(), "IsOngoing() true despite act never performing!");


//         yield return null;
//     }
//     [UnityTest]
//     public IEnumerator IsOngoingFalseAfterCompletion()  // Checks IsOngoing false after act completed performing
//     {
//         // Perform Act
//         var act = new Act();
//         act.Init("Test Act");
//         act.Perform();


//         // Assertions
//         Assert.IsFalse(act.IsOngoing(), "IsOngoing() true despite act having completed performing!");


//         yield return null;
//     }
//     [UnityTest]
//     public IEnumerator IsActiveTrueExceptProloguing()  // Checks IsActive true anywhere except prologuing
//     {
//         // Prerequisites
//         bool wasActiveDuringPrologue = false;
//         bool wasActiveDuringEnter = false;
//         var prologueAct = new Act();


//         // Perform Act
//         var act = new FinishableAct();
//         act.prologue = (a) => new() { prologueAct };
//         act.OnPrePrologue += (a) => { wasActiveDuringPrologue = a.IsActive(); };
//         act.OnPreEnter += (a) => { wasActiveDuringEnter = a.IsActive(); };
//         act.Init("Test Act");
//         act.Perform();


//         // Assertions
//         Assert.IsFalse(wasActiveDuringPrologue, "IsActive() true during prologuing!");
//         Assert.IsTrue(wasActiveDuringEnter, "IsActive() false during entering!");


//         yield return null;
//     }
//     [UnityTest]
//     public IEnumerator IsActiveFalseWhenNotPerforming()  // Checks IsActive false if act is not performing
//     {
//         // Perform Act
//         var act = new Act();
//         act.Init("Test Act");


//         // Assertions
//         Assert.IsFalse(act.IsActive(), "IsActive() true despite act never performing!");


//         yield return null;
//     }
//     [UnityTest]
//     public IEnumerator IsActiveFalseAfterCompletion()  // Checks IsActive false after act completed performing
//     {
//         // Perform Act
//         var act = new Act();
//         act.Init("Test Act");
//         act.Perform();


//         // Assertions
//         Assert.IsFalse(act.IsActive(), "IsActive() true despite act having completed performing!");


//         yield return null;
//     }
//     [UnityTest]
//     public IEnumerator IsEnabledTrueWhenNotDisabled()  // Checks IsEnabled true if act not disabled
//     {
//         // Perform Act
//         var act = new Act();
//         act.Init("Test Act");


//         // Assertions
//         Assert.IsTrue(act.IsEnabled(), "IsEnabled() false despite act not being disabled!");


//         yield return null;
//     }
//     [UnityTest]
//     public IEnumerator IsEnabledFalseWhenDisabled()  // Checks IsEnabled false if act disabled
//     {
//         // Perform Act
//         var act = new Act();
//         act.Init("Test Act");
//         act.SetEnabled(false);


//         // Assertions
//         Assert.IsFalse(act.IsEnabled(), "IsEnabled() true despite act being disabled!");


//         yield return null;
//     }
//     [UnityTest]
//     public IEnumerator IsEnabledTrueAfterReenable()  // Checks IsEnabled true if act disabled then re-enabled
//     {
//         // Perform Act
//         var act = new Act();
//         act.Init("Test Act");
//         act.SetEnabled(false);
//         act.SetEnabled(true);


//         // Assertions
//         Assert.IsTrue(act.IsEnabled(), "IsEnabled() false despite act being re-enabled!");


//         yield return null;
//     }
//     [UnityTest]
//     public IEnumerator IsBlockedTrueWhenPersistentlyBlocked()  // Checks IsBlocked true when act persistently blocked
//     {
//         // Perform Act
//         var act = new WaitInfiniAct();
//         act.Init("Test Act");

//         var blockingAct = new WaitInfiniAct();
//         blockingAct.Init("Blocking Act");
//         blockingAct.AddToBlock(new() { act }, Act.BlockType.Persistent);
//         blockingAct.Perform();


//         // Assertions
//         Assert.IsTrue(act.IsBlocked(), "IsBlocked() false despite act being persistently blocked!");


//         yield return null;
//     }
//     [UnityTest]
//     public IEnumerator IsBlockedFalseWhenNotPersistentlyBlocked()  // Checks IsBlocked false when act not persistently blocked
//     {
//         // Perform Act
//         var act = new WaitInfiniAct();
//         act.Init("Test Act");


//         // Assertions
//         Assert.IsFalse(act.IsBlocked(), "IsBlocked() true despite act never being blocked!");


//         yield return null;
//     }
//     [UnityTest]
//     public IEnumerator IsBlockedFalseAfterUnblock()  // Checks IsBlocked false if act blocked then unblocked
//     {
//         // Perform Act
//         var act = new WaitInfiniAct();
//         act.Init("Test Act");

//         var blockingAct = new WaitInfiniAct();
//         blockingAct.Init("Blocking Act");
//         blockingAct.AddToBlock(new() { act }, Act.BlockType.Persistent);
//         blockingAct.Perform();
//         blockingAct.RemoveFromBlock(new() { act });


//         // Assertions
//         Assert.IsFalse(act.IsBlocked(), "IsBlocked() true despite act being unblocked!");


//         yield return null;
//     }
//     [UnityTest]
//     public IEnumerator CanTickReturnsAccurateValues()  // Checks CanTick true or false for all flag combinations
//     {
//         // Perform Act
//         var tickAct = new TickAct();
//         tickAct.Init("Tick Act");

//         var allAct = new TestAllTicksAct();
//         allAct.Init("All Tick Act");

//         var noneAct = new Act();
//         noneAct.Init("None Act");


//         // Assertions
//         Assert.IsTrue(tickAct.CanTick(Act.TickFlags.Tick), "CanTick() false despite Tick flag assigned!");
//         Assert.IsFalse(tickAct.CanTick(Act.TickFlags.PhysicsTick), "CanTick() true despite PhysicsTick flag not assigned!");
//         Assert.IsTrue(allAct.CanTick(Act.TickFlags.Tick) && allAct.CanTick(Act.TickFlags.PhysicsTick) && allAct.CanTick(Act.TickFlags.LateTick), "CanTick() false despite all flags assigned!");
//         Assert.IsFalse(noneAct.CanTick(Act.TickFlags.Tick) || noneAct.CanTick(Act.TickFlags.PhysicsTick) || noneAct.CanTick(Act.TickFlags.LateTick), "CanTick() true despite no flags assigned!");


//         yield return null;
//     }
//     [UnityTest]
//     public IEnumerator GetTheaterReturnsAccurateTheater()  // Checks GetTheater returns accurate theater
//     {
//         // Prerequisites
//         var theater = new GameObject().AddComponent<Theater>();


//         // Perform Act
//         var act = new Act();
//         act.Init("Test Act", theater);


//         // Assertions
//         Assert.IsTrue(act.GetTheater() == theater, $"GetTheater() is inaccurate! Theater={act.GetTheater()}");


//         UnityEngine.Object.Destroy(theater.gameObject);
//         yield return null;
//     }
//     [UnityTest]
//     public IEnumerator GetTheaterReturnsNullWhenNoTheaterAssigned()  // Checks GetTheater returns null when no theater assigned
//     {
//         // Perform Act
//         var act = new Act();
//         act.Init("Test Act");


//         // Assertions
//         Assert.IsTrue(act.GetTheater() == null, $"GetTheater() is not null despite no theater assigned! Theater={act.GetTheater()}");


//         yield return null;
//     }
//     [UnityTest]
//     public IEnumerator GetOwnerReturnsAccurateOwner()  // Checks GetOwner returns accurate owner
//     {
//         // Prerequisites
//         var theater = new GameObject().AddComponent<Theater>();


//         // Perform Act
//         var act = new Act();
//         act.Init("Test Act", theater);


//         // Assertions
//         Assert.IsTrue(act.GetOwner() == theater.gameObject, $"GetOwner() is inaccurate! Owner={act.GetOwner()}");


//         UnityEngine.Object.Destroy(theater.gameObject);
//         yield return null;
//     }
//     [UnityTest]
//     public IEnumerator GetOwnerReturnsNullWhenNoTheaterAssigned()  // Checks GetOwner returns null when no theater assigned
//     {
//         // Perform Act
//         var act = new Act();
//         act.Init("Test Act");


//         // Assertions
//         Assert.IsTrue(act.GetOwner() == null, $"GetOwner() is not null despite no theater assigned! Owner={act.GetOwner()}");


//         yield return null;
//     }
//     [UnityTest]
//     public IEnumerator GetStatusReturnsAccurateStatus()  // Checks GetStatus returns accurate status in every life cycle method
//     {
//         // Prerequisites
//         var statuses = new List<Act.Status>();
//         var prologueAct = new Act();


//         // Perform Act
//         var act = new TestAllTicksAct();
//         act.prologue = (a) => new() { prologueAct };
//         act.OnPrePrologue += (a) => { statuses.Add(a.GetStatus()); };
//         act.OnPreEnter += (a) => { statuses.Add(a.GetStatus()); };
//         act.OnPreTick += (a) => { statuses.Add(a.GetStatus()); };
//         act.Init("Test Act");
//         act.Perform();


//         // Assertions
//         Assert.IsTrue(statuses.Contains(Act.Status.Prologuing), "GetStatus() did not report Prologuing during prologue!");
//         Assert.IsTrue(statuses.Contains(Act.Status.Entering), "GetStatus() did not report Entering during enter!");


//         yield return null;
//     }
//     [UnityTest]
//     public IEnumerator GetStatusNoneBeforeAndAfterPerform()  // Checks GetStatus returns None before and after perform
//     {
//         // Perform Act
//         var act = new Act();
//         var statusBeforePerform = act.GetStatus();
//         act.Init("Test Act");
//         act.Perform();
//         var statusAfterPerform = act.GetStatus();


//         // Assertions
//         Assert.IsTrue(statusBeforePerform == Act.Status.None, $"GetStatus() not None before perform! Status={statusBeforePerform}");
//         Assert.IsTrue(statusAfterPerform == Act.Status.None, $"GetStatus() not None after perform! Status={statusAfterPerform}");


//         yield return null;
//     }
//     [UnityTest]
//     public IEnumerator GetOutcomeReturnsAccurateOutcome()  // Checks GetOutcome returns accurate outcome after exiting
//     {
//         // Perform Act
//         var successAct = new Act();
//         successAct.Init("Success Act");
//         successAct.Perform();

//         var failureAct = new FailOnEnterAct();
//         failureAct.Init("Failure Act");
//         failureAct.Perform();

//         var interruptedAct = new WaitInfiniAct();
//         interruptedAct.Init("Interrupted Act");
//         interruptedAct.Perform();
//         interruptedAct.Abort();


//         // Assertions
//         Assert.IsTrue(successAct.GetOutcome() == Act.Outcome.Success, $"GetOutcome() inaccurate for success! Outcome={successAct.GetOutcome()}");
//         Assert.IsTrue(failureAct.GetOutcome() == Act.Outcome.Failure, $"GetOutcome() inaccurate for failure! Outcome={failureAct.GetOutcome()}");
//         Assert.IsTrue(interruptedAct.GetOutcome() == Act.Outcome.Interrupted, $"GetOutcome() inaccurate for interrupted! Outcome={interruptedAct.GetOutcome()}");


//         yield return null;
//     }
//     [UnityTest]
//     public IEnumerator GetPerformCountAccurate()  // Checks GetPerformCount gives accurate perform counts
//     {
//         // Perform Act
//         var act = new ReperformableInfiAct();
//         act.Init("Test Act");
//         act.Perform();
//         act.Perform();
//         act.Perform();


//         // Assertions
//         Assert.IsTrue(act.GetPerformCount() == 3, $"GetPerformCount() inaccurate! Count={act.GetPerformCount()}");


//         yield return null;
//     }
//     [UnityTest]
//     public IEnumerator GetNameReturnsAccurateValue()  // Checks GetName returns accurate values
//     {
//         // Perform Act
//         var act = new Act();
//         act.Init("My Named Act");


//         // Assertions
//         Assert.IsTrue(act.GetName() == "My Named Act", $"GetName() inaccurate! Name={act.GetName()}");


//         yield return null;
//     }
//     [UnityTest]
//     public IEnumerator WriteLogPrintsMessageWithNameAndMessage()  // Checks WriteLog prints message with name and message
//     {
//         // Prerequisites
//         LogAssert.Expect(LogType.Warning, "[Test Act] Cannot perform, act or theater is disabled!");


//         // Perform Act
//         var act = new Act();
//         act.isVerbose = true;
//         act.Init("Test Act");
//         act.SetEnabled(false);
//         act.Perform();


//         // Assertions
//         LogAssert.NoUnexpectedReceived();


//         yield return null;
//     }
//     [UnityTest]
//     public IEnumerator ActPerformsWithoutTheater()  // Checks act performs without theater
//     {
//         // Perform Act
//         var act = new Act();
//         act.Init("Test Act");
//         act.Perform();


//         // Assertions
//         Assert.IsTrue(act.GetPerformCount() == 1, "Act did not perform without a theater!");


//         yield return null;
//     }
//     [UnityTest]
//     public IEnumerator ActProloguesWithoutTheater()  // Checks act prologues without theater
//     {
//         // Prerequisites
//         var prologueAct = new Act();
//         prologueAct.Init("Prologue Act");


//         // Perform Act
//         var mainAct = new Act();
//         mainAct.prologue = (a) => new() { prologueAct };
//         mainAct.Init("Main Act");
//         mainAct.Perform();


//         // Assertions
//         Assert.IsTrue(mainAct.GetOutcome() == Act.Outcome.Success, $"Act did not prologue without a theater! Outcome={mainAct.GetOutcome()}");


//         yield return null;
//     }
//     [UnityTest]
//     public IEnumerator ActBlocksWithoutTheater()  // Checks act blocks without theater
//     {
//         // Perform Act
//         var act = new WaitInfiniAct();
//         act.Init("Test Act");

//         var blockingAct = new WaitInfiniAct();
//         blockingAct.Init("Blocking Act");
//         blockingAct.AddToBlock(new() { act }, Act.BlockType.Persistent);
//         blockingAct.Perform();
//         act.Perform();


//         // Assertions
//         Assert.IsTrue(act.IsBlocked(), "Act did not block without a theater!");
//         Assert.IsFalse(act.IsOngoing(), "Blocked act performed despite being blocked!");


//         yield return null;
//     }
//     [UnityTest]
//     public IEnumerator ActFailsTickingWithoutTheater()  // Checks act fails ticking without theater
//     {
//         // Perform Act
//         var act = new TickAct();
//         act.Init("Test Act");
//         act.Perform();
//         yield return null;


//         // Assertions
//         Assert.IsTrue(act.callCount == 0, $"Act ticked despite having no theater! Call count={act.callCount}");
//     }
//     [UnityTest]
//     public IEnumerator ActFailsDeferredPerformWithoutTheater()  // Checks act fails deferred performing without theater
//     {
//         // Perform Act
//         var act = new Act();
//         act.Init("Test Act");
//         act.PerformDeferred();
//         yield return null;


//         // Assertions
//         Assert.IsTrue(act.GetPerformCount() == 0, "Act performed deferred despite having no theater!");
//     }
// }
