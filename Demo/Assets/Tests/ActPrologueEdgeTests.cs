using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;


// 1. Does prologue chain {{actA}, {actB1, actB2}} work?
// 1. Does prologue chain {{actA} , {actB1, actB2}, {actC}} work?
// 1. Does prologue chain {{actA1, actA2} , {actB}, {actC1, actC2}} work?


public class ActPrologueEdgeTests
{
    [UnityTest]
    public IEnumerator Prologues1x2()
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
    public IEnumerator Prologues1x2x1()
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
    public IEnumerator Prologues2x1x2()
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
}
