using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using System.Linq;
using UnityEngine;
using UnityEngine.TestTools;


// 1. Does this prologue chain work:   Main.p=[[A1, A2]]   A1.p=[[C1, C2], [B1, B2]]   A2.p=[[C3, C4], [B3, B4]]
// 2. Does above prologue chain work timed?

// 3. Does this prologue chain work:   Main.p=[[A1, A2]]   A1.p=[[D1, D2], [C1, C2], [B1]]   A2.p=[[D1, D2], [C1, C2], [B1]]
// 4. Does above prologue chain work timed?

// 5. Does this prologue chain work:   Main.p=[[A1, A2]]   A1.p=[[D1, D2], [C1, C2], [B1, B2]]   A2.p=[[D1, D2], [C1, C2], [B1, B2]]
// 6. Does above prologue chain work timed?

// 7. Does this prologue chain work:   Main.p=[[A1, A2]]   A1.p=[[B1, B2]]   A2.p=[[B3, B4]]   B1.p=[[Y1, Y2], [X1, X2]]   B2.p=[[Y3, Y4], [X3, X4]]   B3.p=[[Y5, Y6], [X5, X6]]   B4.p=[[Y7, Y8], [X7, X8]]
// 8. Does above prologue chain work timed?

// 9. Does this prologue chain work:   Main.p=[[A1, A2]]   A1.p=[[D1, D2], [C1, C2], [B1, B2]]   A2.p=[[D3, D4], [C3, C4], [B3, B4]]
// 10. Does above prologue chain work timed?

// 11. Does this prologue chain work:   Main.p=[[A1, A2]]   A1.p=[[C1, C2], [B1, B2]]   A2.p=[[C3, C4], [B1, B2]]
// 12. Does above prologue chain work timed?

// 13. Does this prologue chain work:   Main.p=[[A1, A2]]   A1.p=[[D1, D2], [C1, C2], [B1, B2]]   A2.p=[[D3, D4], [C1, C2], [B1, B2]]
// 14. Does above prologue chain work timed?

// 15. Does this prologue chain work:   Main.p=[[A1, A2]]   A1.p=[[D1, D2], [C1, C2], [B1, B2]]   A2.p=[[D1, D2], [C3, C4], [B1, B2]]
// 16. Does above prologue chain work timed?


public class ActPrologueComplexTests
{
    [UnityTest]
    public IEnumerator PrologueComplex1Instant()  // 1
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
    public IEnumerator PrologueComplex1Timed()  // 2
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
    public IEnumerator PrologueComplex2Instant()  // 3
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
    public IEnumerator PrologueComplex2Timed()  // 4
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


    [UnityTest]
    public IEnumerator PrologueComplex3Instant()  // 5
    {
        var executionOrder = new List<string>();
        var idealOrder = new List<string>() { "D1", "D2", "C1", "C2", "B1", "B2", "A1", "D1", "D2", "C1", "C2", "B1", "B2", "A2", "Main" };


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

        var actB2 = new Act();
        actB2.OnPreEnter += (a) => executionOrder.Add(a.GetName());
        actB2.Init("B2");



        var actA1 = new Act();
        actA1.prologue = (a) => new() { new() { actD1, actD2 }, new() { actC1, actC2 }, new() { actB1, actB2 } };
        actA1.OnPreEnter += (a) => executionOrder.Add(a.GetName());
        actA1.Init("A1");

        var actA2 = new Act();
        actA2.prologue = (a) => new() { new() { actD1, actD2 }, new() { actC1, actC2 }, new() { actB1, actB2 } };
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
            Utilities.AssertChainCleared(actB2, prefix: prefix);

            Utilities.AssertChainCleared(actA1, prefix: prefix);
            Utilities.AssertChainCleared(actA2, prefix: prefix);

            Utilities.AssertChainCleared(mainAct, prefix: prefix);
        }

        yield return null;
    }
    [UnityTest]
    public IEnumerator PrologueComplex3Timed()  // 6
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

        var actB2 = new TickWaitAct();
        actB2.ticksToWait = 5;
        actB2.OnPreEnter += (a) => executionOrder.Add(a.GetName());
        actB2.Init("B2", theater);



        var actA1 = new TickWaitAct();
        actA1.ticksToWait = 5;
        actA1.prologue = (a) => new() { new() { actD1, actD2 }, new() { actC1, actC2 }, new() { actB1, actB2 } };
        actA1.OnPreEnter += (a) => executionOrder.Add(a.GetName());
        actA1.Init("A1", theater);

        var actA2 = new TickWaitAct();
        actA2.ticksToWait = 1;
        actA2.prologue = (a) => new() { new() { actD1, actD2 }, new() { actC1, actC2 }, new() { actB1, actB2 } };
        actA2.OnPreEnter += (a) => executionOrder.Add(a.GetName());
        actA2.Init("A2", theater);



        var mainAct = new Act();
        mainAct.prologue = (a) => new() { new() { actA1, actA2 } };
        mainAct.OnPreEnter += (a) => executionOrder.Add(a.GetName());
        mainAct.Init("Main", theater);


        // Groups to compare
        var groupD = new List<string>() { "D1", "D2" };
        var groupC = new List<string>() { "C1", "C2" };
        var groupB = new List<string>() { "B1", "B2" };
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
            Utilities.AssertChainCleared(actB2, prefix: prefix);

            Utilities.AssertChainCleared(actA1, prefix: prefix);
            Utilities.AssertChainCleared(actA2, prefix: prefix);

            Utilities.AssertChainCleared(mainAct, prefix: prefix);
        }

        UnityEngine.Object.Destroy(theater.gameObject);
    }


    [UnityTest]
    public IEnumerator PrologueComplex4Instant()  // 7
    {
        var executionOrder = new List<string>();
        var idealOrder = new List<string>()
        {
            "Y1", "Y2", "X1", "X2", "B1",
            "Y3", "Y4", "X3", "X4", "B2",
            "A1",
            "Y5", "Y6", "X5", "X6", "B3",
            "Y7", "Y8", "X7", "X8", "B4",
            "A2",
            "Main"
        };


        // Local helper to create acts
        Act MakeAct(string actName)
        {
            var act = new Act();
            act.OnPreEnter += (a) => executionOrder.Add(a.GetName());
            act.Init(actName);
            return act;
        }


        var actX1 = MakeAct("X1");
        var actX2 = MakeAct("X2");
        var actX3 = MakeAct("X3");
        var actX4 = MakeAct("X4");
        var actX5 = MakeAct("X5");
        var actX6 = MakeAct("X6");
        var actX7 = MakeAct("X7");
        var actX8 = MakeAct("X8");

        var actY1 = MakeAct("Y1");
        var actY2 = MakeAct("Y2");
        var actY3 = MakeAct("Y3");
        var actY4 = MakeAct("Y4");
        var actY5 = MakeAct("Y5");
        var actY6 = MakeAct("Y6");
        var actY7 = MakeAct("Y7");
        var actY8 = MakeAct("Y8");



        var actB1 = MakeAct("B1");
        actB1.prologue = (a) => new() { new() { actY1, actY2 }, new() { actX1, actX2 } };

        var actB2 = MakeAct("B2");
        actB2.prologue = (a) => new() { new() { actY3, actY4 }, new() { actX3, actX4 } };

        var actB3 = MakeAct("B3");
        actB3.prologue = (a) => new() { new() { actY5, actY6 }, new() { actX5, actX6 } };

        var actB4 = MakeAct("B4");
        actB4.prologue = (a) => new() { new() { actY7, actY8 }, new() { actX7, actX8 } };



        var actA1 = MakeAct("A1");
        actA1.prologue = (a) => new() { new() { actB1, actB2 } };

        var actA2 = MakeAct("A2");
        actA2.prologue = (a) => new() { new() { actB3, actB4 } };



        var mainAct = MakeAct("Main");
        mainAct.prologue = (a) => new() { new() { actA1, actA2 } };


        // All acts to check
        var allActs = new List<Act>()
        {
            actX1, actX2, actX3, actX4, actX5, actX6, actX7, actX8,
            actY1, actY2, actY3, actY4, actY5, actY6, actY7, actY8,
            actB1, actB2, actB3, actB4,
            actA1, actA2,
            mainAct
        };


        // Perform & check twice
        for (int i = 0; i < 2; i++)
        {
            executionOrder.Clear();
            mainAct.Perform();

            var prefix = $"Instant attempt {i + 1}: ";
            var isInCorrectOrder = executionOrder.SequenceEqual(idealOrder);
            Assert.IsTrue(isInCorrectOrder, $"{prefix}Incorrect execution order, Expected Order=[{string.Join(", ", idealOrder)}]\nOutput Order=[{string.Join(", ", executionOrder)}]");

            foreach (var act in allActs)
            {
                Utilities.AssertChainCleared(act, prefix: prefix);
            }
        }

        yield return null;
    }
    [UnityTest]
    public IEnumerator PrologueComplex4Timed()  // 8
    {
        var theater = new GameObject("Theater").AddComponent<Theater>();
        var executionOrder = new List<string>();
        var maxFrames = 200;


        // Local helper to create acts
        TickWaitAct MakeAct(string actName, int ticks)
        {
            var act = new TickWaitAct();
            act.ticksToWait = ticks;
            act.OnPreEnter += (a) => executionOrder.Add(a.GetName());
            act.Init(actName, theater);
            return act;
        }


        var actX1 = MakeAct("X1", 2);
        var actX2 = MakeAct("X2", 4);
        var actX3 = MakeAct("X3", 1);
        var actX4 = MakeAct("X4", 3);
        var actX5 = MakeAct("X5", 5);
        var actX6 = MakeAct("X6", 2);
        var actX7 = MakeAct("X7", 3);
        var actX8 = MakeAct("X8", 1);

        var actY1 = MakeAct("Y1", 3);
        var actY2 = MakeAct("Y2", 1);
        var actY3 = MakeAct("Y3", 4);
        var actY4 = MakeAct("Y4", 2);
        var actY5 = MakeAct("Y5", 1);
        var actY6 = MakeAct("Y6", 5);
        var actY7 = MakeAct("Y7", 2);
        var actY8 = MakeAct("Y8", 3);



        var actB1 = MakeAct("B1", 2);
        actB1.prologue = (a) => new() { new() { actY1, actY2 }, new() { actX1, actX2 } };

        var actB2 = MakeAct("B2", 4);
        actB2.prologue = (a) => new() { new() { actY3, actY4 }, new() { actX3, actX4 } };

        var actB3 = MakeAct("B3", 1);
        actB3.prologue = (a) => new() { new() { actY5, actY6 }, new() { actX5, actX6 } };

        var actB4 = MakeAct("B4", 3);
        actB4.prologue = (a) => new() { new() { actY7, actY8 }, new() { actX7, actX8 } };



        var actA1 = MakeAct("A1", 2);
        actA1.prologue = (a) => new() { new() { actB1, actB2 } };

        var actA2 = MakeAct("A2", 4);
        actA2.prologue = (a) => new() { new() { actB3, actB4 } };



        var mainAct = new Act();
        mainAct.prologue = (a) => new() { new() { actA1, actA2 } };
        mainAct.OnPreEnter += (a) => executionOrder.Add(a.GetName());
        mainAct.Init("Main", theater);


        // Groups to compare
        var groupY12 = new List<string>() { "Y1", "Y2" };
        var groupX12 = new List<string>() { "X1", "X2" };
        var groupY34 = new List<string>() { "Y3", "Y4" };
        var groupX34 = new List<string>() { "X3", "X4" };
        var groupY56 = new List<string>() { "Y5", "Y6" };
        var groupX56 = new List<string>() { "X5", "X6" };
        var groupY78 = new List<string>() { "Y7", "Y8" };
        var groupX78 = new List<string>() { "X7", "X8" };
        var groupB1 = new List<string>() { "B1" };
        var groupB2 = new List<string>() { "B2" };
        var groupB3 = new List<string>() { "B3" };
        var groupB4 = new List<string>() { "B4" };
        var groupB12 = new List<string>() { "B1", "B2" };
        var groupB34 = new List<string>() { "B3", "B4" };
        var groupA1 = new List<string>() { "A1" };
        var groupA2 = new List<string>() { "A2" };
        var groupA = new List<string>() { "A1", "A2" };
        var groupMain = new List<string>() { "Main" };


        // All acts to check
        var allActs = new List<Act>()
        {
            actX1, actX2, actX3, actX4, actX5, actX6, actX7, actX8,
            actY1, actY2, actY3, actY4, actY5, actY6, actY7, actY8,
            actB1, actB2, actB3, actB4,
            actA1, actA2,
            mainAct
        };


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

            Utilities.AssertGroupBefore(executionOrder, groupY12, groupX12, prefix);
            Utilities.AssertGroupBefore(executionOrder, groupX12, groupB1, prefix);
            Utilities.AssertGroupBefore(executionOrder, groupY34, groupX34, prefix);
            Utilities.AssertGroupBefore(executionOrder, groupX34, groupB2, prefix);
            Utilities.AssertGroupBefore(executionOrder, groupY56, groupX56, prefix);
            Utilities.AssertGroupBefore(executionOrder, groupX56, groupB3, prefix);
            Utilities.AssertGroupBefore(executionOrder, groupY78, groupX78, prefix);
            Utilities.AssertGroupBefore(executionOrder, groupX78, groupB4, prefix);
            Utilities.AssertGroupBefore(executionOrder, groupB12, groupA1, prefix);
            Utilities.AssertGroupBefore(executionOrder, groupB34, groupA2, prefix);
            Utilities.AssertGroupBefore(executionOrder, groupA, groupMain, prefix);

            foreach (var act in allActs)
            {
                Utilities.AssertChainCleared(act, prefix: prefix);
            }
        }

        UnityEngine.Object.Destroy(theater.gameObject);
    }


    [UnityTest]
    public IEnumerator PrologueComplex5Instant()  // 9
    {
        var executionOrder = new List<string>();
        var idealOrder = new List<string>() { "D1", "D2", "C1", "C2", "B1", "B2", "A1", "D3", "D4", "C3", "C4", "B3", "B4", "A2", "Main" };


        var actD1 = new Act();
        actD1.OnPreEnter += (a) => executionOrder.Add(a.GetName());
        actD1.Init("D1");

        var actD2 = new Act();
        actD2.OnPreEnter += (a) => executionOrder.Add(a.GetName());
        actD2.Init("D2");

        var actD3 = new Act();
        actD3.OnPreEnter += (a) => executionOrder.Add(a.GetName());
        actD3.Init("D3");

        var actD4 = new Act();
        actD4.OnPreEnter += (a) => executionOrder.Add(a.GetName());
        actD4.Init("D4");



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
        actA1.prologue = (a) => new() { new() { actD1, actD2 }, new() { actC1, actC2 }, new() { actB1, actB2 } };
        actA1.OnPreEnter += (a) => executionOrder.Add(a.GetName());
        actA1.Init("A1");

        var actA2 = new Act();
        actA2.prologue = (a) => new() { new() { actD3, actD4 }, new() { actC3, actC4 }, new() { actB3, actB4 } };
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
            Utilities.AssertChainCleared(actD3, prefix: prefix);
            Utilities.AssertChainCleared(actD4, prefix: prefix);

            Utilities.AssertChainCleared(actC1, prefix: prefix);
            Utilities.AssertChainCleared(actC2, prefix: prefix);
            Utilities.AssertChainCleared(actC3, prefix: prefix);
            Utilities.AssertChainCleared(actC4, prefix: prefix);

            Utilities.AssertChainCleared(actB1, prefix: prefix);
            Utilities.AssertChainCleared(actB2, prefix: prefix);
            Utilities.AssertChainCleared(actB3, prefix: prefix);
            Utilities.AssertChainCleared(actB4, prefix: prefix);

            Utilities.AssertChainCleared(actA1, prefix: prefix);
            Utilities.AssertChainCleared(actA2, prefix: prefix);

            Utilities.AssertChainCleared(mainAct, prefix: prefix);
        }

        yield return null;
    }
    [UnityTest]
    public IEnumerator PrologueComplex5Timed()  // 10
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

        var actD3 = new TickWaitAct();
        actD3.ticksToWait = 1;
        actD3.OnPreEnter += (a) => executionOrder.Add(a.GetName());
        actD3.Init("D3", theater);

        var actD4 = new TickWaitAct();
        actD4.ticksToWait = 3;
        actD4.OnPreEnter += (a) => executionOrder.Add(a.GetName());
        actD4.Init("D4", theater);



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
        actA1.prologue = (a) => new() { new() { actD1, actD2 }, new() { actC1, actC2 }, new() { actB1, actB2 } };
        actA1.OnPreEnter += (a) => executionOrder.Add(a.GetName());
        actA1.Init("A1", theater);

        var actA2 = new TickWaitAct();
        actA2.ticksToWait = 4;
        actA2.prologue = (a) => new() { new() { actD3, actD4 }, new() { actC3, actC4 }, new() { actB3, actB4 } };
        actA2.OnPreEnter += (a) => executionOrder.Add(a.GetName());
        actA2.Init("A2", theater);



        var mainAct = new Act();
        mainAct.prologue = (a) => new() { new() { actA1, actA2 } };
        mainAct.OnPreEnter += (a) => executionOrder.Add(a.GetName());
        mainAct.Init("Main", theater);


        // Groups to compare
        var groupD12 = new List<string>() { "D1", "D2" };
        var groupD34 = new List<string>() { "D3", "D4" };
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

            Utilities.AssertGroupBefore(executionOrder, groupD12, groupC12, prefix);
            Utilities.AssertGroupBefore(executionOrder, groupC12, groupB12, prefix);
            Utilities.AssertGroupBefore(executionOrder, groupB12, groupA1, prefix);
            Utilities.AssertGroupBefore(executionOrder, groupD34, groupC34, prefix);
            Utilities.AssertGroupBefore(executionOrder, groupC34, groupB34, prefix);
            Utilities.AssertGroupBefore(executionOrder, groupB34, groupA2, prefix);
            Utilities.AssertGroupBefore(executionOrder, groupA, groupMain, prefix);

            Utilities.AssertChainCleared(actD1, prefix: prefix);
            Utilities.AssertChainCleared(actD2, prefix: prefix);
            Utilities.AssertChainCleared(actD3, prefix: prefix);
            Utilities.AssertChainCleared(actD4, prefix: prefix);

            Utilities.AssertChainCleared(actC1, prefix: prefix);
            Utilities.AssertChainCleared(actC2, prefix: prefix);
            Utilities.AssertChainCleared(actC3, prefix: prefix);
            Utilities.AssertChainCleared(actC4, prefix: prefix);

            Utilities.AssertChainCleared(actB1, prefix: prefix);
            Utilities.AssertChainCleared(actB2, prefix: prefix);
            Utilities.AssertChainCleared(actB3, prefix: prefix);
            Utilities.AssertChainCleared(actB4, prefix: prefix);

            Utilities.AssertChainCleared(actA1, prefix: prefix);
            Utilities.AssertChainCleared(actA2, prefix: prefix);

            Utilities.AssertChainCleared(mainAct, prefix: prefix);
        }

        UnityEngine.Object.Destroy(theater.gameObject);
    }


    [UnityTest]
    public IEnumerator PrologueComplex6Instant()  // 11
    {
        var executionOrder = new List<string>();
        var idealOrder = new List<string>() { "C1", "C2", "B1", "B2", "A1", "C3", "C4", "B1", "B2", "A2", "Main" };


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



        var actA1 = new Act();
        actA1.prologue = (a) => new() { new() { actC1, actC2 }, new() { actB1, actB2 } };
        actA1.OnPreEnter += (a) => executionOrder.Add(a.GetName());
        actA1.Init("A1");

        var actA2 = new Act();
        actA2.prologue = (a) => new() { new() { actC3, actC4 }, new() { actB1, actB2 } };
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

            Utilities.AssertChainCleared(actC1, prefix: prefix);
            Utilities.AssertChainCleared(actC2, prefix: prefix);
            Utilities.AssertChainCleared(actC3, prefix: prefix);
            Utilities.AssertChainCleared(actC4, prefix: prefix);

            Utilities.AssertChainCleared(actB1, prefix: prefix);
            Utilities.AssertChainCleared(actB2, prefix: prefix);

            Utilities.AssertChainCleared(actA1, prefix: prefix);
            Utilities.AssertChainCleared(actA2, prefix: prefix);

            Utilities.AssertChainCleared(mainAct, prefix: prefix);
        }

        yield return null;
    }
    [UnityTest]
    public IEnumerator PrologueComplex6Timed()  // 12
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



        var actA1 = new TickWaitAct();
        actA1.ticksToWait = 2;
        actA1.prologue = (a) => new() { new() { actC1, actC2 }, new() { actB1, actB2 } };
        actA1.OnPreEnter += (a) => executionOrder.Add(a.GetName());
        actA1.Init("A1", theater);

        var actA2 = new TickWaitAct();
        actA2.ticksToWait = 4;
        actA2.prologue = (a) => new() { new() { actC3, actC4 }, new() { actB1, actB2 } };
        actA2.OnPreEnter += (a) => executionOrder.Add(a.GetName());
        actA2.Init("A2", theater);



        var mainAct = new Act();
        mainAct.prologue = (a) => new() { new() { actA1, actA2 } };
        mainAct.OnPreEnter += (a) => executionOrder.Add(a.GetName());
        mainAct.Init("Main", theater);


        // Groups to compare
        var groupC12 = new List<string>() { "C1", "C2" };
        var groupC34 = new List<string>() { "C3", "C4" };
        var groupB = new List<string>() { "B1", "B2" };
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

            Utilities.AssertGroupBefore(executionOrder, groupC12, groupB, prefix);
            Utilities.AssertGroupBefore(executionOrder, groupC34, groupB, prefix);
            Utilities.AssertGroupBefore(executionOrder, groupB, groupA1, prefix);
            Utilities.AssertGroupBefore(executionOrder, groupB, groupA2, prefix);
            Utilities.AssertGroupBefore(executionOrder, groupA, groupMain, prefix);

            Utilities.AssertChainCleared(actC1, prefix: prefix);
            Utilities.AssertChainCleared(actC2, prefix: prefix);
            Utilities.AssertChainCleared(actC3, prefix: prefix);
            Utilities.AssertChainCleared(actC4, prefix: prefix);

            Utilities.AssertChainCleared(actB1, prefix: prefix);
            Utilities.AssertChainCleared(actB2, prefix: prefix);

            Utilities.AssertChainCleared(actA1, prefix: prefix);
            Utilities.AssertChainCleared(actA2, prefix: prefix);

            Utilities.AssertChainCleared(mainAct, prefix: prefix);
        }

        UnityEngine.Object.Destroy(theater.gameObject);
    }


    [UnityTest]
    public IEnumerator PrologueComplex7Instant()  // 13
    {
        var executionOrder = new List<string>();
        var idealOrder = new List<string>() { "D1", "D2", "C1", "C2", "B1", "B2", "A1", "D3", "D4", "C1", "C2", "B1", "B2", "A2", "Main" };


        var actD1 = new Act();
        actD1.OnPreEnter += (a) => executionOrder.Add(a.GetName());
        actD1.Init("D1");

        var actD2 = new Act();
        actD2.OnPreEnter += (a) => executionOrder.Add(a.GetName());
        actD2.Init("D2");

        var actD3 = new Act();
        actD3.OnPreEnter += (a) => executionOrder.Add(a.GetName());
        actD3.Init("D3");

        var actD4 = new Act();
        actD4.OnPreEnter += (a) => executionOrder.Add(a.GetName());
        actD4.Init("D4");



        var actC1 = new Act();
        actC1.OnPreEnter += (a) => executionOrder.Add(a.GetName());
        actC1.Init("C1");

        var actC2 = new Act();
        actC2.OnPreEnter += (a) => executionOrder.Add(a.GetName());
        actC2.Init("C2");



        var actB1 = new Act();
        actB1.OnPreEnter += (a) => executionOrder.Add(a.GetName());
        actB1.Init("B1");

        var actB2 = new Act();
        actB2.OnPreEnter += (a) => executionOrder.Add(a.GetName());
        actB2.Init("B2");



        var actA1 = new Act();
        actA1.prologue = (a) => new() { new() { actD1, actD2 }, new() { actC1, actC2 }, new() { actB1, actB2 } };
        actA1.OnPreEnter += (a) => executionOrder.Add(a.GetName());
        actA1.Init("A1");

        var actA2 = new Act();
        actA2.prologue = (a) => new() { new() { actD3, actD4 }, new() { actC1, actC2 }, new() { actB1, actB2 } };
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
            Utilities.AssertChainCleared(actD3, prefix: prefix);
            Utilities.AssertChainCleared(actD4, prefix: prefix);

            Utilities.AssertChainCleared(actC1, prefix: prefix);
            Utilities.AssertChainCleared(actC2, prefix: prefix);

            Utilities.AssertChainCleared(actB1, prefix: prefix);
            Utilities.AssertChainCleared(actB2, prefix: prefix);

            Utilities.AssertChainCleared(actA1, prefix: prefix);
            Utilities.AssertChainCleared(actA2, prefix: prefix);

            Utilities.AssertChainCleared(mainAct, prefix: prefix);
        }

        yield return null;
    }
    [UnityTest]
    public IEnumerator PrologueComplex7Timed()  // 14
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

        var actD3 = new TickWaitAct();
        actD3.ticksToWait = 1;
        actD3.OnPreEnter += (a) => executionOrder.Add(a.GetName());
        actD3.Init("D3", theater);

        var actD4 = new TickWaitAct();
        actD4.ticksToWait = 3;
        actD4.OnPreEnter += (a) => executionOrder.Add(a.GetName());
        actD4.Init("D4", theater);



        var actC1 = new TickWaitAct();
        actC1.ticksToWait = 3;
        actC1.OnPreEnter += (a) => executionOrder.Add(a.GetName());
        actC1.Init("C1", theater);

        var actC2 = new TickWaitAct();
        actC2.ticksToWait = 1;
        actC2.OnPreEnter += (a) => executionOrder.Add(a.GetName());
        actC2.Init("C2", theater);



        var actB1 = new TickWaitAct();
        actB1.ticksToWait = 4;
        actB1.OnPreEnter += (a) => executionOrder.Add(a.GetName());
        actB1.Init("B1", theater);

        var actB2 = new TickWaitAct();
        actB2.ticksToWait = 2;
        actB2.OnPreEnter += (a) => executionOrder.Add(a.GetName());
        actB2.Init("B2", theater);



        var actA1 = new TickWaitAct();
        actA1.ticksToWait = 2;
        actA1.prologue = (a) => new() { new() { actD1, actD2 }, new() { actC1, actC2 }, new() { actB1, actB2 } };
        actA1.OnPreEnter += (a) => executionOrder.Add(a.GetName());
        actA1.Init("A1", theater);

        var actA2 = new TickWaitAct();
        actA2.ticksToWait = 4;
        actA2.prologue = (a) => new() { new() { actD3, actD4 }, new() { actC1, actC2 }, new() { actB1, actB2 } };
        actA2.OnPreEnter += (a) => executionOrder.Add(a.GetName());
        actA2.Init("A2", theater);



        var mainAct = new Act();
        mainAct.prologue = (a) => new() { new() { actA1, actA2 } };
        mainAct.OnPreEnter += (a) => executionOrder.Add(a.GetName());
        mainAct.Init("Main", theater);


        // Groups to compare
        var groupD12 = new List<string>() { "D1", "D2" };
        var groupD34 = new List<string>() { "D3", "D4" };
        var groupC = new List<string>() { "C1", "C2" };
        var groupB = new List<string>() { "B1", "B2" };
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

            Utilities.AssertGroupBefore(executionOrder, groupD12, groupC, prefix);
            Utilities.AssertGroupBefore(executionOrder, groupD34, groupC, prefix);
            Utilities.AssertGroupBefore(executionOrder, groupC, groupB, prefix);
            Utilities.AssertGroupBefore(executionOrder, groupB, groupA1, prefix);
            Utilities.AssertGroupBefore(executionOrder, groupB, groupA2, prefix);
            Utilities.AssertGroupBefore(executionOrder, groupA, groupMain, prefix);

            Utilities.AssertChainCleared(actD1, prefix: prefix);
            Utilities.AssertChainCleared(actD2, prefix: prefix);
            Utilities.AssertChainCleared(actD3, prefix: prefix);
            Utilities.AssertChainCleared(actD4, prefix: prefix);

            Utilities.AssertChainCleared(actC1, prefix: prefix);
            Utilities.AssertChainCleared(actC2, prefix: prefix);

            Utilities.AssertChainCleared(actB1, prefix: prefix);
            Utilities.AssertChainCleared(actB2, prefix: prefix);

            Utilities.AssertChainCleared(actA1, prefix: prefix);
            Utilities.AssertChainCleared(actA2, prefix: prefix);

            Utilities.AssertChainCleared(mainAct, prefix: prefix);
        }

        UnityEngine.Object.Destroy(theater.gameObject);
    }


    [UnityTest]
    public IEnumerator PrologueComplex8Instant()  // 15
    {
        var executionOrder = new List<string>();
        var idealOrder = new List<string>() { "D1", "D2", "C1", "C2", "B1", "B2", "A1", "D1", "D2", "C3", "C4", "B1", "B2", "A2", "Main" };


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



        var actA1 = new Act();
        actA1.prologue = (a) => new() { new() { actD1, actD2 }, new() { actC1, actC2 }, new() { actB1, actB2 } };
        actA1.OnPreEnter += (a) => executionOrder.Add(a.GetName());
        actA1.Init("A1");

        var actA2 = new Act();
        actA2.prologue = (a) => new() { new() { actD1, actD2 }, new() { actC3, actC4 }, new() { actB1, actB2 } };
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
            Utilities.AssertChainCleared(actC3, prefix: prefix);
            Utilities.AssertChainCleared(actC4, prefix: prefix);

            Utilities.AssertChainCleared(actB1, prefix: prefix);
            Utilities.AssertChainCleared(actB2, prefix: prefix);

            Utilities.AssertChainCleared(actA1, prefix: prefix);
            Utilities.AssertChainCleared(actA2, prefix: prefix);

            Utilities.AssertChainCleared(mainAct, prefix: prefix);
        }

        yield return null;
    }
    [UnityTest]
    public IEnumerator PrologueComplex8Timed()  // 16
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



        var actA1 = new TickWaitAct();
        actA1.ticksToWait = 2;
        actA1.prologue = (a) => new() { new() { actD1, actD2 }, new() { actC1, actC2 }, new() { actB1, actB2 } };
        actA1.OnPreEnter += (a) => executionOrder.Add(a.GetName());
        actA1.Init("A1", theater);

        var actA2 = new TickWaitAct();
        actA2.ticksToWait = 4;
        actA2.prologue = (a) => new() { new() { actD1, actD2 }, new() { actC3, actC4 }, new() { actB1, actB2 } };
        actA2.OnPreEnter += (a) => executionOrder.Add(a.GetName());
        actA2.Init("A2", theater);



        var mainAct = new Act();
        mainAct.prologue = (a) => new() { new() { actA1, actA2 } };
        mainAct.OnPreEnter += (a) => executionOrder.Add(a.GetName());
        mainAct.Init("Main", theater);


        // Groups to compare
        var groupD = new List<string>() { "D1", "D2" };
        var groupC12 = new List<string>() { "C1", "C2" };
        var groupC34 = new List<string>() { "C3", "C4" };
        var groupB = new List<string>() { "B1", "B2" };
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

            Utilities.AssertGroupBefore(executionOrder, groupD, groupC12, prefix);
            Utilities.AssertGroupBefore(executionOrder, groupD, groupC34, prefix);
            Utilities.AssertGroupBefore(executionOrder, groupC12, groupB, prefix);
            Utilities.AssertGroupBefore(executionOrder, groupC34, groupB, prefix);
            Utilities.AssertGroupBefore(executionOrder, groupB, groupA1, prefix);
            Utilities.AssertGroupBefore(executionOrder, groupB, groupA2, prefix);
            Utilities.AssertGroupBefore(executionOrder, groupA, groupMain, prefix);

            Utilities.AssertChainCleared(actD1, prefix: prefix);
            Utilities.AssertChainCleared(actD2, prefix: prefix);

            Utilities.AssertChainCleared(actC1, prefix: prefix);
            Utilities.AssertChainCleared(actC2, prefix: prefix);
            Utilities.AssertChainCleared(actC3, prefix: prefix);
            Utilities.AssertChainCleared(actC4, prefix: prefix);

            Utilities.AssertChainCleared(actB1, prefix: prefix);
            Utilities.AssertChainCleared(actB2, prefix: prefix);

            Utilities.AssertChainCleared(actA1, prefix: prefix);
            Utilities.AssertChainCleared(actA2, prefix: prefix);

            Utilities.AssertChainCleared(mainAct, prefix: prefix);
        }

        UnityEngine.Object.Destroy(theater.gameObject);
    }
}
