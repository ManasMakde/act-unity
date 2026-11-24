using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;


// 1. Does retry deferred work?
// 2. Does retry deferred fail without theater?
// 3. Does retry deferred not immediately retry the act?
// 4. Does act retry once when deferred twice?
// 5. Does retry deferred combined tick flags fire once only for whichever comes first?
// 6. Does cancel retry deferred work?


public class ActRetryDeferredTests
{
    // Checks epilogue survived the retry
    private static void AssertEpilogueKept(Act pAct, Act eAct, string prefix = "")
    {
        // var epilogueActs = Act.Friend.GetEpilogueActs(pAct);
        // Assert.IsTrue(epilogueActs.ContainsKey(eAct), $"{prefix}Prologue act lost its epilogue after retry! Epilogue count={epilogueActs.Count}");
        Assert.IsTrue(eAct.IsPerforming(), $"{prefix}Epilogue act stopped performing after prologue retried!");
        Assert.IsTrue(eAct.GetStatus() == Act.Status.Prologuing, $"{prefix}Epilogue act did not stay 'Prologuing' after prologue retried! Status={eAct.GetStatus()}");
        Assert.IsTrue(eAct.GetPerformCount() == 1, $"{prefix}Epilogue act performed again after prologue retried! Perform Count={eAct.GetPerformCount()}");
    }


    [UnityTest]
    public IEnumerator RetryDeferredWorks()  // 1
    {
        // Prerequisites
        var theater = new GameObject().AddComponent<Theater>();


        // Physics tick deferred retry
        {
            var pAct = new WaitInfiniAct();
            pAct.Init("Physics Prologue Act", theater);

            var mainAct = new WaitInfiniAct();
            mainAct.prologue = (a) => new() { new() { pAct } };
            mainAct.Init("Physics Main Act", theater);
            mainAct.Perform();

            pAct.RetryDeferred(Act.TickFlags.PhysicsTick);
            yield return new WaitForFixedUpdate();
            yield return null;


            // Assertions
            Assert.IsTrue(pAct.GetPerformCount() == 2, $"Physics: Act did not retry after physics tick! Perform Count={pAct.GetPerformCount()}");
            Assert.IsTrue(pAct.GetStatus() == Act.Status.Entering, $"Physics: Act did not re enter after retrying! Status={pAct.GetStatus()}");
            AssertEpilogueKept(pAct, mainAct, "Physics: ");
        }


        // Tick deferred retry
        {
            var pAct = new WaitInfiniAct();
            pAct.Init("Tick Prologue Act", theater);

            var mainAct = new WaitInfiniAct();
            mainAct.prologue = (a) => new() { new() { pAct } };
            mainAct.Init("Tick Main Act", theater);
            mainAct.Perform();

            pAct.RetryDeferred(Act.TickFlags.Tick);
            yield return null;
            yield return null;


            // Assertions
            Assert.IsTrue(pAct.GetPerformCount() == 2, $"Tick: Act did not retry after tick! Perform Count={pAct.GetPerformCount()}");
            Assert.IsTrue(pAct.GetStatus() == Act.Status.Entering, $"Tick: Act did not re enter after retrying! Status={pAct.GetStatus()}");
            AssertEpilogueKept(pAct, mainAct, "Tick: ");
        }


        // Late tick deferred retry
        {
            var pAct = new WaitInfiniAct();
            pAct.Init("Late Tick Prologue Act", theater);

            var mainAct = new WaitInfiniAct();
            mainAct.prologue = (a) => new() { new() { pAct } };
            mainAct.Init("Late Tick Main Act", theater);
            mainAct.Perform();

            pAct.RetryDeferred(Act.TickFlags.LateTick);
            yield return null;
            yield return null;


            // Assertions
            Assert.IsTrue(pAct.GetPerformCount() == 2, $"LateTick: Act did not retry after late tick! Perform Count={pAct.GetPerformCount()}");
            Assert.IsTrue(pAct.GetStatus() == Act.Status.Entering, $"LateTick: Act did not re enter after retrying! Status={pAct.GetStatus()}");
            AssertEpilogueKept(pAct, mainAct, "LateTick: ");
        }


        UnityEngine.Object.Destroy(theater.gameObject);
    }
    [UnityTest]
    public IEnumerator RetryDeferWithoutTheater()  // 2
    {
        // Perform Acts
        var pAct = new WaitInfiniAct();
        pAct.Init("Prologue Act");

        var mainAct = new WaitInfiniAct();
        mainAct.prologue = (a) => new() { new() { pAct } };
        mainAct.Init("Main Act");
        mainAct.Perform();

        pAct.RetryDeferred();

        yield return null;
        yield return null;
        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();


        // Assertions
        Assert.IsTrue(pAct.GetPerformCount() == 1, $"Act retried despite missing theater! Perform Count={pAct.GetPerformCount()}");
        Assert.IsTrue(pAct.GetStatus() == Act.Status.Entering, $"Act did not stay 'Entering' when retry deferred without theater! Status={pAct.GetStatus()}");
        AssertEpilogueKept(pAct, mainAct);
    }
    [UnityTest]
    public IEnumerator RetryDeferredNotImmediatelyRetry()  // 3
    {
        // Prerequisites
        var theater = new GameObject().AddComponent<Theater>();


        // Perform Acts
        var pAct = new WaitInfiniAct();
        pAct.Init("Prologue Act", theater);

        var mainAct = new WaitInfiniAct();
        mainAct.prologue = (a) => new() { new() { pAct } };
        mainAct.Init("Main Act", theater);
        mainAct.Perform();

        pAct.RetryDeferred(Act.TickFlags.PhysicsTick);


        // Assertions
        Assert.IsTrue(pAct.GetPerformCount() == 1, $"Act retried immediately despite being deferred! Perform Count={pAct.GetPerformCount()}");
        AssertEpilogueKept(pAct, mainAct);


        UnityEngine.Object.Destroy(theater.gameObject);
        yield return null;
    }
    [UnityTest]
    public IEnumerator RetriesOnceWhenDeferredTwice()  // 4
    {
        // Prerequisites
        var theater = new GameObject().AddComponent<Theater>();


        // Tick deferred twice
        {
            var pAct = new WaitInfiniAct();
            pAct.Init("Tick Prologue Act", theater);

            var mainAct = new WaitInfiniAct();
            mainAct.prologue = (a) => new() { new() { pAct } };
            mainAct.Init("Tick Main Act", theater);
            mainAct.Perform();

            pAct.RetryDeferred(Act.TickFlags.Tick);
            pAct.RetryDeferred(Act.TickFlags.Tick);
            yield return null;
            yield return null;


            // Assertions
            Assert.IsTrue(pAct.GetPerformCount() == 2, $"Tick: Act did not retry once despite being deferred twice! Perform Count={pAct.GetPerformCount()}");
            AssertEpilogueKept(pAct, mainAct, "Tick: ");
        }


        // Physics tick deferred twice
        {
            var pAct = new WaitInfiniAct();
            pAct.Init("Physics Prologue Act", theater);

            var mainAct = new WaitInfiniAct();
            mainAct.prologue = (a) => new() { new() { pAct } };
            mainAct.Init("Physics Main Act", theater);
            mainAct.Perform();

            pAct.RetryDeferred(Act.TickFlags.PhysicsTick);
            pAct.RetryDeferred(Act.TickFlags.PhysicsTick);
            yield return new WaitForFixedUpdate();
            yield return null;


            // Assertions
            Assert.IsTrue(pAct.GetPerformCount() == 2, $"Physics: Act did not retry once despite being deferred twice! Perform Count={pAct.GetPerformCount()}");
            AssertEpilogueKept(pAct, mainAct, "Physics: ");
        }


        // Late tick deferred twice
        {
            var pAct = new WaitInfiniAct();
            pAct.Init("Late Tick Prologue Act", theater);

            var mainAct = new WaitInfiniAct();
            mainAct.prologue = (a) => new() { new() { pAct } };
            mainAct.Init("Late Tick Main Act", theater);
            mainAct.Perform();

            pAct.RetryDeferred(Act.TickFlags.LateTick);
            pAct.RetryDeferred(Act.TickFlags.LateTick);
            yield return null;
            yield return null;


            // Assertions
            Assert.IsTrue(pAct.GetPerformCount() == 2, $"LateTick: Act did not retry once despite being deferred twice! Perform Count={pAct.GetPerformCount()}");
            AssertEpilogueKept(pAct, mainAct, "LateTick: ");
        }


        UnityEngine.Object.Destroy(theater.gameObject);
    }
    [UnityTest]
    public IEnumerator RetryDeferredCombinedFlags()  // 5
    {
        // Prerequisites
        var theater = new GameObject().AddComponent<Theater>();


        // Tick and PhysicsTick combo
        {
            var pAct = new WaitInfiniAct();
            pAct.Init("Tick Physics Prologue Act", theater);

            var mainAct = new WaitInfiniAct();
            mainAct.prologue = (a) => new() { new() { pAct } };
            mainAct.Init("Tick Physics Main Act", theater);
            mainAct.Perform();

            pAct.RetryDeferred(Act.TickFlags.Tick | Act.TickFlags.PhysicsTick);

            yield return new WaitForFixedUpdate();

            var countAfterPhysicsTick = pAct.GetPerformCount();

            yield return null;

            var countAfterTick = pAct.GetPerformCount();


            // Assertions
            Assert.IsTrue(countAfterPhysicsTick == 2, $"Tick|PhysicsTick: Act did not retry on first tick type to fire! Perform Count={countAfterPhysicsTick}");
            Assert.IsTrue(countAfterTick == 2, $"Tick|PhysicsTick: Act retried again on second tick type despite being cleared! Perform Count={countAfterTick}");
            AssertEpilogueKept(pAct, mainAct, "Tick|PhysicsTick: ");
        }


        // Tick and LateTick combo
        {
            var pAct = new WaitInfiniAct();
            pAct.Init("Tick Late Prologue Act", theater);

            var mainAct = new WaitInfiniAct();
            mainAct.prologue = (a) => new() { new() { pAct } };
            mainAct.Init("Tick Late Main Act", theater);
            mainAct.Perform();

            pAct.RetryDeferred(Act.TickFlags.Tick | Act.TickFlags.LateTick);

            yield return null;

            var countAfterTick = pAct.GetPerformCount();

            yield return null;

            var countAfterLateTick = pAct.GetPerformCount();


            // Assertions
            Assert.IsTrue(countAfterTick == 2, $"Tick|LateTick: Act did not retry on first tick type to fire! Perform Count={countAfterTick}");
            Assert.IsTrue(countAfterLateTick == 2, $"Tick|LateTick: Act retried again on second tick type despite being cleared! Perform Count={countAfterLateTick}");
            AssertEpilogueKept(pAct, mainAct, "Tick|LateTick: ");
        }


        // PhysicsTick and LateTick combo
        {
            var pAct = new WaitInfiniAct();
            pAct.Init("Physics Late Prologue Act", theater);

            var mainAct = new WaitInfiniAct();
            mainAct.prologue = (a) => new() { new() { pAct } };
            mainAct.Init("Physics Late Main Act", theater);
            mainAct.Perform();

            pAct.RetryDeferred(Act.TickFlags.PhysicsTick | Act.TickFlags.LateTick);

            yield return new WaitForFixedUpdate();

            var countAfterPhysicsTick = pAct.GetPerformCount();

            yield return null;

            var countAfterLateTick = pAct.GetPerformCount();


            // Assertions
            Assert.IsTrue(countAfterPhysicsTick == 2, $"PhysicsTick|LateTick: Act did not retry on first tick type to fire! Perform Count={countAfterPhysicsTick}");
            Assert.IsTrue(countAfterLateTick == 2, $"PhysicsTick|LateTick: Act retried again on second tick type despite being cleared! Perform Count={countAfterLateTick}");
            AssertEpilogueKept(pAct, mainAct, "PhysicsTick|LateTick: ");
        }


        // Tick, PhysicsTick and LateTick combo
        {
            var pAct = new WaitInfiniAct();
            pAct.Init("All Prologue Act", theater);

            var mainAct = new WaitInfiniAct();
            mainAct.prologue = (a) => new() { new() { pAct } };
            mainAct.Init("All Main Act", theater);
            mainAct.Perform();

            pAct.RetryDeferred(Act.TickFlags.Tick | Act.TickFlags.PhysicsTick | Act.TickFlags.LateTick);

            yield return new WaitForFixedUpdate();

            var countAfterPhysicsTick = pAct.GetPerformCount();

            yield return null;

            var countAfterRemainingTicks = pAct.GetPerformCount();


            // Assertions
            Assert.IsTrue(countAfterPhysicsTick == 2, $"Tick|PhysicsTick|LateTick: Act did not retry on first tick type to fire! Perform Count={countAfterPhysicsTick}");
            Assert.IsTrue(countAfterRemainingTicks == 2, $"Tick|PhysicsTick|LateTick: Act retried again on remaining tick types despite being cleared! Perform Count={countAfterRemainingTicks}");
            AssertEpilogueKept(pAct, mainAct, "Tick|PhysicsTick|LateTick: ");
        }


        UnityEngine.Object.Destroy(theater.gameObject);
    }
    [UnityTest]
    public IEnumerator CancelRetryDeferredWorks()  // 6
    {
        // Prerequisites
        var theater = new GameObject().AddComponent<Theater>();


        // Physics deferred retry cancelled
        {
            var pAct = new WaitInfiniAct();
            pAct.Init("Physics Prologue Act", theater);

            var mainAct = new WaitInfiniAct();
            mainAct.prologue = (a) => new() { new() { pAct } };
            mainAct.Init("Physics Main Act", theater);
            mainAct.Perform();

            pAct.RetryDeferred(Act.TickFlags.PhysicsTick);
            pAct.CancelRetryDeferred();

            yield return new WaitForFixedUpdate();
            yield return null;


            // Assertions
            Assert.IsTrue(pAct.GetPerformCount() == 1, $"Physics: Act retried despite deferred retry being cancelled! Perform Count={pAct.GetPerformCount()}");
            AssertEpilogueKept(pAct, mainAct, "Physics: ");
        }


        // Tick deferred retry cancelled
        {
            var pAct = new WaitInfiniAct();
            pAct.Init("Tick Prologue Act", theater);

            var mainAct = new WaitInfiniAct();
            mainAct.prologue = (a) => new() { new() { pAct } };
            mainAct.Init("Tick Main Act", theater);
            mainAct.Perform();

            pAct.RetryDeferred(Act.TickFlags.Tick);
            pAct.CancelRetryDeferred();

            yield return null;
            yield return null;


            // Assertions
            Assert.IsTrue(pAct.GetPerformCount() == 1, $"Tick: Act retried despite deferred retry being cancelled! Perform Count={pAct.GetPerformCount()}");
            AssertEpilogueKept(pAct, mainAct, "Tick: ");
        }


        // Late Tick deferred retry cancelled
        {
            var pAct = new WaitInfiniAct();
            pAct.Init("Late Tick Prologue Act", theater);

            var mainAct = new WaitInfiniAct();
            mainAct.prologue = (a) => new() { new() { pAct } };
            mainAct.Init("Late Tick Main Act", theater);
            mainAct.Perform();

            pAct.RetryDeferred(Act.TickFlags.LateTick);
            pAct.CancelRetryDeferred();

            yield return null;
            yield return null;


            // Assertions
            Assert.IsTrue(pAct.GetPerformCount() == 1, $"LateTick: Act retried despite deferred retry being cancelled! Perform Count={pAct.GetPerformCount()}");
            AssertEpilogueKept(pAct, mainAct, "LateTick: ");
        }


        UnityEngine.Object.Destroy(theater.gameObject);
    }
}
