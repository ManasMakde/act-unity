using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;


// 1. Does abort deferred work?
// 2. Does abort deferred fail without theater?
// 3. Does abort deferred not immediately abort the act?
// 4. Does abort deferred combined tick flags fire once only for whichever comes first?
// 5. Does cancel abort deferred work?


public class ActAbortDeferredTests
{
    [UnityTest]
    public IEnumerator AbortDeferredWorks()  // 1
    {
        // Prerequisites
        var theater = new GameObject().AddComponent<Theater>();


        // Physics tick deferred abort
        {
            var act = new WaitInfiniAct();
            act.Init("Physics Act", theater);
            act.Perform();
            act.AbortDeferred(Act.TickFlags.PhysicsTick);

            yield return new WaitForFixedUpdate();
            yield return null;


            // Assertions
            Assert.IsFalse(act.IsPerforming(), "Physics: Act did not abort after physics tick!");
            Assert.IsTrue(act.GetOutcome() == Act.Outcome.Interrupted, $"Physics: Act outcome is not 'Interrupted' after deferred abort! Outcome={act.GetOutcome()}");
        }


        // Tick deferred abort
        {
            var act = new WaitInfiniAct();
            act.Init("Tick Act", theater);
            act.Perform();
            act.AbortDeferred(Act.TickFlags.Tick);

            yield return null;
            yield return null;


            // Assertions
            Assert.IsFalse(act.IsPerforming(), "Tick: Act did not abort after tick!");
            Assert.IsTrue(act.GetOutcome() == Act.Outcome.Interrupted, $"Tick: Act outcome is not 'Interrupted' after deferred abort! Outcome={act.GetOutcome()}");
        }


        // Late tick deferred abort
        {
            var act = new WaitInfiniAct();
            act.Init("Late Tick Act", theater);
            act.Perform();
            act.AbortDeferred(Act.TickFlags.LateTick);

            yield return null;
            yield return null;


            // Assertions
            Assert.IsFalse(act.IsPerforming(), "LateTick: Act did not abort after late tick!");
            Assert.IsTrue(act.GetOutcome() == Act.Outcome.Interrupted, $"LateTick: Act outcome is not 'Interrupted' after deferred abort! Outcome={act.GetOutcome()}");
        }


        UnityEngine.Object.Destroy(theater.gameObject);
    }
    [UnityTest]
    public IEnumerator AbortDeferWithoutTheater()  // 2
    {
        // Perform Act
        var act = new WaitInfiniAct();
        act.Init("Test Act");
        act.Perform();
        act.AbortDeferred();

        yield return null;
        yield return null;
        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();


        // Assertions
        Assert.IsTrue(act.IsPerforming(), "Act aborted despite missing theater!");
        Assert.IsTrue(act.GetOutcome() == Act.Outcome.Pending, $"Act outcome changed despite missing theater! Outcome={act.GetOutcome()}");
    }
    [UnityTest]
    public IEnumerator AbortDeferredDoesNotImmediatelyAbort()  // 3
    {
        // Prerequisites
        var theater = new GameObject().AddComponent<Theater>();


        // Perform Act
        var act = new WaitInfiniAct();
        act.Init("Test Act", theater);
        act.Perform();
        act.AbortDeferred(Act.TickFlags.PhysicsTick);


        // Assertions
        Assert.IsTrue(act.IsPerforming(), "Act aborted immediately despite being deferred!");
        Assert.IsTrue(act.GetOutcome() == Act.Outcome.Pending, $"Act outcome changed immediately despite being deferred! Outcome={act.GetOutcome()}");


        UnityEngine.Object.Destroy(theater.gameObject);
        yield return null;
    }
    [UnityTest]
    public IEnumerator AbortDeferredCombinedFlagsFireOnFirstAndClearOther()  // 4
    {
        // Prerequisites
        var theater = new GameObject().AddComponent<Theater>();


        // Tick and PhysicsTick combo
        {
            var act = new WaitInfiniAct();
            act.Init("Test Act", theater);
            act.Perform();
            act.AbortDeferred(Act.TickFlags.Tick | Act.TickFlags.PhysicsTick);

            yield return new WaitForFixedUpdate();

            var performingAfterPhysicsTick = act.IsPerforming();


            // Reperform so a second abort would be visible
            act.Perform();

            yield return null;

            var performingAfterTick = act.IsPerforming();


            // Assertions
            Assert.IsFalse(performingAfterPhysicsTick, "Tick|PhysicsTick: Act did not abort on first tick type to fire!");
            Assert.IsTrue(act.GetPerformCount() == 2, $"Tick|PhysicsTick: Act did not reperform after abort! Perform Count={act.GetPerformCount()}");
            Assert.IsTrue(performingAfterTick, "Tick|PhysicsTick: Act aborted again on second tick type despite being cleared!");
        }


        // Tick and LateTick combo
        {
            var act = new WaitInfiniAct();
            act.Init("Test Act", theater);
            act.Perform();
            act.AbortDeferred(Act.TickFlags.Tick | Act.TickFlags.LateTick);

            yield return null;

            var performingAfterTick = act.IsPerforming();


            // Reperform so a second abort would be visible
            act.Perform();

            yield return null;

            var performingAfterLateTick = act.IsPerforming();


            // Assertions
            Assert.IsFalse(performingAfterTick, "Tick|LateTick: Act did not abort on first tick type to fire!");
            Assert.IsTrue(act.GetPerformCount() == 2, $"Tick|LateTick: Act did not reperform after abort! Perform Count={act.GetPerformCount()}");
            Assert.IsTrue(performingAfterLateTick, "Tick|LateTick: Act aborted again on second tick type despite being cleared!");
        }


        // PhysicsTick and LateTick combo
        {
            var act = new WaitInfiniAct();
            act.Init("Test Act", theater);
            act.Perform();
            act.AbortDeferred(Act.TickFlags.PhysicsTick | Act.TickFlags.LateTick);

            yield return new WaitForFixedUpdate();

            var performingAfterPhysicsTick = act.IsPerforming();


            // Reperform so a second abort would be visible
            act.Perform();

            yield return null;

            var performingAfterLateTick = act.IsPerforming();


            // Assertions
            Assert.IsFalse(performingAfterPhysicsTick, "PhysicsTick|LateTick: Act did not abort on first tick type to fire!");
            Assert.IsTrue(act.GetPerformCount() == 2, $"PhysicsTick|LateTick: Act did not reperform after abort! Perform Count={act.GetPerformCount()}");
            Assert.IsTrue(performingAfterLateTick, "PhysicsTick|LateTick: Act aborted again on second tick type despite being cleared!");
        }


        // Tick, PhysicsTick and LateTick combo
        {
            var act = new WaitInfiniAct();
            act.Init("Test Act", theater);
            act.Perform();
            act.AbortDeferred(Act.TickFlags.Tick | Act.TickFlags.PhysicsTick | Act.TickFlags.LateTick);

            yield return new WaitForFixedUpdate();

            var performingAfterPhysicsTick = act.IsPerforming();


            // Reperform so a second abort would be visible
            act.Perform();

            yield return null;

            var performingAfterRemainingTicks = act.IsPerforming();


            // Assertions
            Assert.IsFalse(performingAfterPhysicsTick, "Tick|PhysicsTick|LateTick: Act did not abort on first tick type to fire!");
            Assert.IsTrue(act.GetPerformCount() == 2, $"Tick|PhysicsTick|LateTick: Act did not reperform after abort! Perform Count={act.GetPerformCount()}");
            Assert.IsTrue(performingAfterRemainingTicks, "Tick|PhysicsTick|LateTick: Act aborted again on remaining tick types despite being cleared!");
        }


        UnityEngine.Object.Destroy(theater.gameObject);
    }
    [UnityTest]
    public IEnumerator CancelAbortDeferredWorks()  // 5
    {
        // Prerequisites
        var theater = new GameObject().AddComponent<Theater>();


        // Physics deferred abort cancelled
        {
            var act = new WaitInfiniAct();
            act.Init("Physics Act", theater);
            act.Perform();
            act.AbortDeferred(Act.TickFlags.PhysicsTick);
            act.CancelAbortDeferred();

            yield return new WaitForFixedUpdate();
            yield return null;


            // Assertions
            Assert.IsTrue(act.IsPerforming(), "Physics: Act aborted despite deferred abort being cancelled!");
            Assert.IsTrue(act.GetOutcome() == Act.Outcome.Pending, $"Physics: Act outcome changed despite deferred abort being cancelled! Outcome={act.GetOutcome()}");
        }


        // Tick deferred abort cancelled
        {
            var act = new WaitInfiniAct();
            act.Init("Tick Act", theater);
            act.Perform();
            act.AbortDeferred(Act.TickFlags.Tick);
            act.CancelAbortDeferred();

            yield return null;
            yield return null;


            // Assertions
            Assert.IsTrue(act.IsPerforming(), "Tick: Act aborted despite deferred abort being cancelled!");
            Assert.IsTrue(act.GetOutcome() == Act.Outcome.Pending, $"Tick: Act outcome changed despite deferred abort being cancelled! Outcome={act.GetOutcome()}");
        }


        // Late Tick deferred abort cancelled
        {
            var act = new WaitInfiniAct();
            act.Init("Late Tick Act", theater);
            act.Perform();
            act.AbortDeferred(Act.TickFlags.LateTick);
            act.CancelAbortDeferred();

            yield return null;
            yield return null;


            // Assertions
            Assert.IsTrue(act.IsPerforming(), "LateTick: Act aborted despite deferred abort being cancelled!");
            Assert.IsTrue(act.GetOutcome() == Act.Outcome.Pending, $"LateTick: Act outcome changed despite deferred abort being cancelled! Outcome={act.GetOutcome()}");
        }


        UnityEngine.Object.Destroy(theater.gameObject);
    }
}
