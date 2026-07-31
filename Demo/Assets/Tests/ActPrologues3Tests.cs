using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

// 1. Does prologue chain {{actA, actA1, actA3}} work?
// 1. Does prologue chain {{actA, actA1, actA3}, {actB1, actB2, actB3}} work?
// 1. Does prologue chain {{actA, actA1, actA3}, {actB1, actB2, actB3}, {actC1, actC2, actC3}} work?


public class ActPrologues3Tests
{
    [UnityTest]
    public IEnumerator Prologues3()
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
    public IEnumerator Prologues3x3()
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
    public IEnumerator Prologues3x3x3()
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
}
