using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;


// 1. Are all "pre tick" & "post tick" actions being broadcasted with correct arguments?
// 1. Are all Tick() methods being invoked?
// 1. Is ticking not being invoked when tick flag is set to none?
// 1. Does ticking fail if not theater is assigned?
// 1. Does GetDelta() & GetPhysicsDelta() return accurate values?


public class ActTickTests
{
    [UnityTest]
    public IEnumerator OnPreAndPostTick()
    {
        // Tick
        {
            bool wasPreTickInvoked = false;
            Act preTickArg1 = null;
            bool wasPostTickInvoked = false;
            Act postTickArg1 = null;

            var theaterObj = new GameObject("Test Theater");
            var theater = theaterObj.AddComponent<Theater>();
            var act = new TickAct();
            act.OnPreTick += (a) => { wasPreTickInvoked = true; preTickArg1 = a; };
            act.OnPostTick += (a) => { wasPostTickInvoked = true; postTickArg1 = a; };
            act.Init("Test Act", theater);
            act.Perform();

            yield return null;

            // Assertions
            Assert.IsTrue(wasPreTickInvoked, "OnPreTick not invoked!");
            Assert.IsTrue(preTickArg1 == act, $"OnPreTick first argument is invalid! Arg1='{preTickArg1}'");
            Assert.IsTrue(wasPostTickInvoked, "OnPostTick not invoked!");
            Assert.IsTrue(postTickArg1 == act, $"OnPostTick first argument is invalid! Arg1='{postTickArg1}'");
        }

        // PhysicsTick
        {
            bool wasPrePhysicsTickInvoked = false;
            Act prePhysicsTickArg1 = null;
            bool wasPostPhysicsTickInvoked = false;
            Act postPhysicsTickArg1 = null;

            var theaterObj = new GameObject("Test Theater");
            var theater = theaterObj.AddComponent<Theater>();
            var act = new PhysicsTickAct();
            act.OnPrePhysicsTick += (a) => { wasPrePhysicsTickInvoked = true; prePhysicsTickArg1 = a; };
            act.OnPostPhysicsTick += (a) => { wasPostPhysicsTickInvoked = true; postPhysicsTickArg1 = a; };
            act.Init("Test Act", theater);
            act.Perform();

            yield return new WaitForFixedUpdate();

            // Assertions
            Assert.IsTrue(wasPrePhysicsTickInvoked, "OnPrePhysicsTick not invoked!");
            Assert.IsTrue(prePhysicsTickArg1 == act, $"OnPrePhysicsTick first argument is invalid! Arg1='{prePhysicsTickArg1}'");
            Assert.IsTrue(wasPostPhysicsTickInvoked, "OnPostPhysicsTick not invoked!");
            Assert.IsTrue(postPhysicsTickArg1 == act, $"OnPostPhysicsTick first argument is invalid! Arg1='{postPhysicsTickArg1}'");
        }

        // LateTick
        {
            bool wasPreLateTickInvoked = false;
            Act preLateTickArg1 = null;
            bool wasPostLateTickInvoked = false;
            Act postLateTickArg1 = null;

            var theaterObj = new GameObject("Test Theater");
            var theater = theaterObj.AddComponent<Theater>();
            var act = new LateTickAct();
            act.OnPreLateTick += (a) => { wasPreLateTickInvoked = true; preLateTickArg1 = a; };
            act.OnPostLateTick += (a) => { wasPostLateTickInvoked = true; postLateTickArg1 = a; };
            act.Init("Test Act", theater);
            act.Perform();

            yield return null;

            // Assertions
            Assert.IsTrue(wasPreLateTickInvoked, "OnPreLateTick not invoked!");
            Assert.IsTrue(preLateTickArg1 == act, $"OnPreLateTick first argument is invalid! Arg1='{preLateTickArg1}'");
            Assert.IsTrue(wasPostLateTickInvoked, "OnPostLateTick not invoked!");
            Assert.IsTrue(postLateTickArg1 == act, $"OnPostLateTick first argument is invalid! Arg1='{postLateTickArg1}'");
        }
    }
    [UnityTest]
    public IEnumerator Tick()
    {
        // Tick
        {
            var theaterObj = new GameObject("Test Theater");
            var theater = theaterObj.AddComponent<Theater>();
            var act = new TickAct();
            act.Init("Test Act", theater);
            act.Perform();

            yield return null;

            // Assertions
            Assert.IsTrue(1 <= act.callCount, $"Tick() not invoked! Call count='{act.callCount}'");
        }

        // PhysicsTick
        {
            var theaterObj = new GameObject("Test Theater");
            var theater = theaterObj.AddComponent<Theater>();
            var act = new PhysicsTickAct();
            act.Init("Test Act", theater);
            act.Perform();

            yield return new WaitForFixedUpdate();

            // Assertions
            Assert.IsTrue(1 <= act.callCount, $"PhysicsTick() not invoked! Call count='{act.callCount}'");
        }

        // LateTick
        {
            var theaterObj = new GameObject("Test Theater");
            var theater = theaterObj.AddComponent<Theater>();
            var act = new LateTickAct();
            act.Init("Test Act", theater);
            act.Perform();

            yield return null;

            // Assertions
            Assert.IsTrue(1 <= act.callCount, $"LateTick() not invoked! Call count='{act.callCount}'");
        }
    }
    [UnityTest]
    public IEnumerator TickWithFlagNone()
    {
        // Prerequisites
        bool wasPreTickInvoked = false;
        bool wasPrePhysicsTickInvoked = false;
        bool wasPreLateTickInvoked = false;

        // Perform Act
        var theaterObj = new GameObject("Test Theater");
        var theater = theaterObj.AddComponent<Theater>();
        var act = new NoneTickAct();
        act.OnPreTick += (a) => { wasPreTickInvoked = true; };
        act.OnPrePhysicsTick += (a) => { wasPrePhysicsTickInvoked = true; };
        act.OnPreLateTick += (a) => { wasPreLateTickInvoked = true; };
        act.Init("Test Act", theater);
        act.Perform();

        yield return new WaitForFixedUpdate();
        yield return null;

        // Assertions
        Assert.IsTrue(act.GetStatus() == Act.Status.None, $"Act status is not 'None' despite TickFlags.None! Status='{act.GetStatus()}'");
        Assert.IsFalse(wasPreTickInvoked, "OnPreTick invoked despite TickFlags.None!");
        Assert.IsFalse(wasPrePhysicsTickInvoked, "OnPrePhysicsTick invoked despite TickFlags.None!");
        Assert.IsFalse(wasPreLateTickInvoked, "OnPreLateTick invoked despite TickFlags.None!");
    }
    [UnityTest]
    public IEnumerator TickWithoutTheater()
    {
        // Tick type
        {
            var act = new TickAct();
            act.Init("Test Act");
            act.Perform();

            var actStatus = act.GetStatus();

            // Assertions
            Assert.IsTrue(actStatus == Act.Status.Entering, $"Act did not stay 'Entering' when ticking without theater! Status='{actStatus}'");
            Assert.IsTrue(act.callCount == 0, $"Tick() invoked despite missing theater! Call count='{act.callCount}'");
        }

        // PhysicsTick type
        {
            var act = new PhysicsTickAct();
            act.Init("Test Act");
            act.Perform();

            var actStatus = act.GetStatus();

            // Assertions
            Assert.IsTrue(actStatus == Act.Status.Entering, $"Act did not stay 'Entering' when physics ticking without theater! Status='{actStatus}'");
            Assert.IsTrue(act.callCount == 0, $"PhysicsTick() invoked despite missing theater! Call count='{act.callCount}'");
        }

        // LateTick type
        {
            var act = new LateTickAct();
            act.Init("Test Act");
            act.Perform();

            var actStatus = act.GetStatus();

            // Assertions
            Assert.IsTrue(actStatus == Act.Status.Entering, $"Act did not stay 'Entering' when late ticking without theater! Status='{actStatus}'");
            Assert.IsTrue(act.callCount == 0, $"LateTick() invoked despite missing theater! Call count='{act.callCount}'");
        }

        yield return null;
    }
    [UnityTest]
    public IEnumerator GetDeltaAndPhysicsDelta()
    {
        yield return null;

        // Assertions
        Assert.IsTrue(Act.GetDelta() == Time.deltaTime, $"GetDelta() does not match Time.deltaTime! GetDelta={Act.GetDelta()}  Time.deltaTime={Time.deltaTime}");
        Assert.IsTrue(Act.GetPhysicsDelta() == Time.fixedDeltaTime, $"GetPhysicsDelta() does not match Time.fixedDeltaTime! GetPhysicsDelta={Act.GetPhysicsDelta()}  Time.fixedDeltaTime={Time.fixedDeltaTime}");
    }
}
