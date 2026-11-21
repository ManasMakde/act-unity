using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using System.Linq;
using UnityEngine;
using UnityEngine.TestTools;


public class ActPrologueComplexTests
{
    [UnityTest]
    public IEnumerator PrologueComplex1Instant()
    {
        var executionOrder = new List<string>();
        var idealOrder = new List<string>() { "C1", "C2", "B1", "B2", "A1", "C3", "C4", "B3", "B4", "A2", "Main" };


        var actC1 = new Act();
        actC1.OnPreEnter += (a) => executionOrder.Add(a.GetName());
        actC1.Init("C1");

        var actC2 = new Act();
        actC2.OnPreEnter += (a) => executionOrder.Add(a.GetName());
        actC2.Init("C2");

        var actC3 = new Act();
        actC3.OnPreEnter += (a) => executionOrder.Add(a.GetName());
        actC3.Init("C3");

        var actC4 = new Act();
        actC4.OnPreEnter += (a) => executionOrder.Add(a.GetName());
        actC4.Init("C4");



        var actB1 = new Act();
        actB1.OnPreEnter += (a) => executionOrder.Add(a.GetName());
        actB1.Init("B1");

        var actB2 = new Act();
        actB2.OnPreEnter += (a) => executionOrder.Add(a.GetName());
        actB2.Init("B2");

        var actB3 = new Act();
        actB3.OnPreEnter += (a) => executionOrder.Add(a.GetName());
        actB3.Init("B3");

        var actB4 = new Act();
        actB4.OnPreEnter += (a) => executionOrder.Add(a.GetName());
        actB4.Init("B4");



        var actA1 = new Act();
        actA1.prologue = (a) => new() { new() { actC1, actC2 }, new() { actB1, actB2 } };
        actA1.OnPreEnter += (a) => executionOrder.Add(a.GetName());
        actA1.Init("A1");

        var actA2 = new Act();
        actA2.prologue = (a) => new() { new() { actC3, actC4 }, new() { actB3, actB4 } };
        actA2.OnPreEnter += (a) => executionOrder.Add(a.GetName());
        actA2.Init("A2");



        var mainAct = new Act();
        mainAct.prologue = (a) => new() { new() { actA1, actA2 } };
        mainAct.OnPreEnter += (a) => executionOrder.Add(a.GetName());
        mainAct.Init("Main");


        // Perform & check twice
        for (int i = 0; i < 2; i++)
        {
            executionOrder.Clear();
            mainAct.Perform();

            var isInCorrectOrder = executionOrder.SequenceEqual(idealOrder);
            Assert.IsTrue(isInCorrectOrder, $"Incorrect execution order in attempt {i + 1}\nExpected Order=[{string.Join(", ", idealOrder)}]\nOutput Order=[{string.Join(", ", executionOrder)}]");

            Utilities.AssertChainCleared(actA1, prefix: "Instant: ");
            Utilities.AssertChainCleared(actA2, prefix: "Instant: ");

            Utilities.AssertChainCleared(actB1, prefix: "Instant: ");
            Utilities.AssertChainCleared(actB2, prefix: "Instant: ");
            Utilities.AssertChainCleared(actB3, prefix: "Instant: ");
            Utilities.AssertChainCleared(actB4, prefix: "Instant: ");

            Utilities.AssertChainCleared(actC1, prefix: "Instant: ");
            Utilities.AssertChainCleared(actC2, prefix: "Instant: ");
            Utilities.AssertChainCleared(actC3, prefix: "Instant: ");
            Utilities.AssertChainCleared(actC4, prefix: "Instant: ");

            Utilities.AssertChainCleared(mainAct, prefix: "Instant: ");
        }

        yield return null;
    }
    [UnityTest]
    public IEnumerator PrologueComplex1Timed()
    {
        var theater = new GameObject("Theater").AddComponent<Theater>();
        var executionOrder = new List<string>();
        var maxFrames = 100;


        var actC1 = new TickWaitAct();
        actC1.ticksToWait = 3;
        actC1.OnPreEnter += (a) => executionOrder.Add(a.GetName());
        actC1.Init("C1", theater);

        var actC2 = new TickWaitAct();
        actC2.ticksToWait = 1;
        actC2.OnPreEnter += (a) => executionOrder.Add(a.GetName());
        actC2.Init("C2", theater);

        var actC3 = new TickWaitAct();
        actC3.ticksToWait = 5;
        actC3.OnPreEnter += (a) => executionOrder.Add(a.GetName());
        actC3.Init("C3", theater);

        var actC4 = new TickWaitAct();
        actC4.ticksToWait = 2;
        actC4.OnPreEnter += (a) => executionOrder.Add(a.GetName());
        actC4.Init("C4", theater);



        var actB1 = new TickWaitAct();
        actB1.ticksToWait = 4;
        actB1.OnPreEnter += (a) => executionOrder.Add(a.GetName());
        actB1.Init("B1", theater);

        var actB2 = new TickWaitAct();
        actB2.ticksToWait = 2;
        actB2.OnPreEnter += (a) => executionOrder.Add(a.GetName());
        actB2.Init("B2", theater);

        var actB3 = new TickWaitAct();
        actB3.ticksToWait = 1;
        actB3.OnPreEnter += (a) => executionOrder.Add(a.GetName());
        actB3.Init("B3", theater);

        var actB4 = new TickWaitAct();
        actB4.ticksToWait = 3;
        actB4.OnPreEnter += (a) => executionOrder.Add(a.GetName());
        actB4.Init("B4", theater);



        var actA1 = new TickWaitAct();
        actA1.ticksToWait = 2;
        actA1.prologue = (a) => new() { new() { actC1, actC2 }, new() { actB1, actB2 } };
        actA1.OnPreEnter += (a) => executionOrder.Add(a.GetName());
        actA1.Init("A1", theater);

        var actA2 = new TickWaitAct();
        actA2.ticksToWait = 4;
        actA2.prologue = (a) => new() { new() { actC3, actC4 }, new() { actB3, actB4 } };
        actA2.OnPreEnter += (a) => executionOrder.Add(a.GetName());
        actA2.Init("A2", theater);



        var mainAct = new Act();
        mainAct.prologue = (a) => new() { new() { actA1, actA2 } };
        mainAct.OnPreEnter += (a) => executionOrder.Add(a.GetName());
        mainAct.Init("Main", theater);


        // Groups to compare
        var groupC12 = new List<string>() { "C1", "C2" };
        var groupC34 = new List<string>() { "C3", "C4" };
        var groupB12 = new List<string>() { "B1", "B2" };
        var groupB34 = new List<string>() { "B3", "B4" };
        var groupA1 = new List<string>() { "A1" };
        var groupA2 = new List<string>() { "A2" };
        var groupA = new List<string>() { "A1", "A2" };
        var groupMain = new List<string>() { "Main" };


        // Perform & check twice
        for (int i = 0; i < 2; i++)
        {
            executionOrder.Clear();
            mainAct.Perform();

            // Wait until main act ends
            int frame = 0;
            while (mainAct.IsPerforming() && frame < maxFrames)
            {
                yield return null;
                frame++;
            }

            var prefix = $"Timed attempt {i + 1}: ";
            Assert.IsFalse(mainAct.IsPerforming(), $"{prefix}Main act still performing after {maxFrames} frames!");

            Utilities.AssertGroupBefore(executionOrder, groupC12, groupB12, prefix);
            Utilities.AssertGroupBefore(executionOrder, groupB12, groupA1, prefix);
            Utilities.AssertGroupBefore(executionOrder, groupC34, groupB34, prefix);
            Utilities.AssertGroupBefore(executionOrder, groupB34, groupA2, prefix);
            Utilities.AssertGroupBefore(executionOrder, groupA, groupMain, prefix);

            Utilities.AssertChainCleared(actA1, prefix: prefix);
            Utilities.AssertChainCleared(actA2, prefix: prefix);

            Utilities.AssertChainCleared(actB1, prefix: prefix);
            Utilities.AssertChainCleared(actB2, prefix: prefix);
            Utilities.AssertChainCleared(actB3, prefix: prefix);
            Utilities.AssertChainCleared(actB4, prefix: prefix);

            Utilities.AssertChainCleared(actC1, prefix: prefix);
            Utilities.AssertChainCleared(actC2, prefix: prefix);
            Utilities.AssertChainCleared(actC3, prefix: prefix);
            Utilities.AssertChainCleared(actC4, prefix: prefix);

            Utilities.AssertChainCleared(mainAct, prefix: prefix);
        }

        UnityEngine.Object.Destroy(theater.gameObject);
    }



    [UnityTest]
    public IEnumerator PrologueComplex2Instant()
    {
        var executionOrder = new List<string>();
        var idealOrder = new List<string>() { "D1", "D2", "C1", "C2", "B1", "A1", "D1", "D2", "C1", "C2", "B1", "A2", "Main" };


        var actD1 = new Act();
        actD1.OnPreEnter += (a) => executionOrder.Add(a.GetName());
        actD1.Init("D1");

        var actD2 = new Act();
        actD2.OnPreEnter += (a) => executionOrder.Add(a.GetName());
        actD2.Init("D2");



        var actC1 = new Act();
        actC1.OnPreEnter += (a) => executionOrder.Add(a.GetName());
        actC1.Init("C1");

        var actC2 = new Act();
        actC2.OnPreEnter += (a) => executionOrder.Add(a.GetName());
        actC2.Init("C2");



        var actB1 = new Act();
        actB1.OnPreEnter += (a) => executionOrder.Add(a.GetName());
        actB1.Init("B1");



        var actA1 = new Act();
        actA1.prologue = (a) => new() { new() { actD1, actD2 }, new() { actC1, actC2 }, new() { actB1 } };
        actA1.OnPreEnter += (a) => executionOrder.Add(a.GetName());
        actA1.Init("A1");

        var actA2 = new Act();
        actA2.prologue = (a) => new() { new() { actD1, actD2 }, new() { actC1, actC2 }, new() { actB1 } };
        actA2.OnPreEnter += (a) => executionOrder.Add(a.GetName());
        actA2.Init("A2");



        var mainAct = new Act();
        mainAct.prologue = (a) => new() { new() { actA1, actA2 } };
        mainAct.OnPreEnter += (a) => executionOrder.Add(a.GetName());
        mainAct.Init("Main");


        // Perform & check twice
        for (int i = 0; i < 2; i++)
        {
            executionOrder.Clear();
            mainAct.Perform();

            var prefix = $"Instant attempt {i + 1}: ";
            var isInCorrectOrder = executionOrder.SequenceEqual(idealOrder);
            Assert.IsTrue(isInCorrectOrder, $"{prefix}Incorrect execution order, Expected Order=[{string.Join(", ", idealOrder)}]\nOutput Order=[{string.Join(", ", executionOrder)}]");

            Utilities.AssertChainCleared(actD1, prefix: prefix);
            Utilities.AssertChainCleared(actD2, prefix: prefix);

            Utilities.AssertChainCleared(actC1, prefix: prefix);
            Utilities.AssertChainCleared(actC2, prefix: prefix);

            Utilities.AssertChainCleared(actB1, prefix: prefix);

            Utilities.AssertChainCleared(actA1, prefix: prefix);
            Utilities.AssertChainCleared(actA2, prefix: prefix);

            Utilities.AssertChainCleared(mainAct, prefix: prefix);
        }

        yield return null;
    }
    [UnityTest]
    public IEnumerator PrologueComplex2Timed()
    {
        var theater = new GameObject("Theater").AddComponent<Theater>();
        var executionOrder = new List<string>();
        var maxFrames = 100;


        var actD1 = new TickWaitAct();
        actD1.ticksToWait = 2;
        actD1.OnPreEnter += (a) => executionOrder.Add(a.GetName());
        actD1.Init("D1", theater);

        var actD2 = new TickWaitAct();
        actD2.ticksToWait = 4;
        actD2.OnPreEnter += (a) => executionOrder.Add(a.GetName());
        actD2.Init("D2", theater);



        var actC1 = new TickWaitAct();
        actC1.ticksToWait = 3;
        actC1.OnPreEnter += (a) => executionOrder.Add(a.GetName());
        actC1.Init("C1", theater);

        var actC2 = new TickWaitAct();
        actC2.ticksToWait = 1;
        actC2.OnPreEnter += (a) => executionOrder.Add(a.GetName());
        actC2.Init("C2", theater);



        var actB1 = new TickWaitAct();
        actB1.ticksToWait = 2;
        actB1.OnPreEnter += (a) => executionOrder.Add(a.GetName());
        actB1.Init("B1", theater);



        var actA1 = new TickWaitAct();
        actA1.ticksToWait = 5;
        actA1.prologue = (a) => new() { new() { actD1, actD2 }, new() { actC1, actC2 }, new() { actB1 } };
        actA1.OnPreEnter += (a) => executionOrder.Add(a.GetName());
        actA1.Init("A1", theater);

        var actA2 = new TickWaitAct();
        actA2.ticksToWait = 1;
        actA2.prologue = (a) => new() { new() { actD1, actD2 }, new() { actC1, actC2 }, new() { actB1 } };
        actA2.OnPreEnter += (a) => executionOrder.Add(a.GetName());
        actA2.Init("A2", theater);



        var mainAct = new Act();
        mainAct.prologue = (a) => new() { new() { actA1, actA2 } };
        mainAct.OnPreEnter += (a) => executionOrder.Add(a.GetName());
        mainAct.Init("Main", theater);


        // Groups to compare
        var groupD = new List<string>() { "D1", "D2" };
        var groupC = new List<string>() { "C1", "C2" };
        var groupB = new List<string>() { "B1" };
        var groupA = new List<string>() { "A1", "A2" };
        var groupMain = new List<string>() { "Main" };


        // Perform & check twice
        for (int i = 0; i < 2; i++)
        {
            executionOrder.Clear();
            mainAct.Perform();

            // Wait until main act ends
            int frame = 0;
            while (mainAct.IsPerforming() && frame < maxFrames)
            {
                yield return null;
                frame++;
            }

            var prefix = $"Timed attempt {i + 1}: ";
            Assert.IsFalse(mainAct.IsPerforming(), $"{prefix}Main act still performing after {maxFrames} frames!");

            Utilities.AssertGroupBefore(executionOrder, groupD, groupC, prefix);
            Utilities.AssertGroupBefore(executionOrder, groupC, groupB, prefix);
            Utilities.AssertGroupBefore(executionOrder, groupB, groupA, prefix);
            Utilities.AssertGroupBefore(executionOrder, groupA, groupMain, prefix);

            Utilities.AssertChainCleared(actD1, prefix: prefix);
            Utilities.AssertChainCleared(actD2, prefix: prefix);

            Utilities.AssertChainCleared(actC1, prefix: prefix);
            Utilities.AssertChainCleared(actC2, prefix: prefix);

            Utilities.AssertChainCleared(actB1, prefix: prefix);

            Utilities.AssertChainCleared(actA1, prefix: prefix);
            Utilities.AssertChainCleared(actA2, prefix: prefix);

            Utilities.AssertChainCleared(mainAct, prefix: prefix);
        }

        UnityEngine.Object.Destroy(theater.gameObject);
    }
}
