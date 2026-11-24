using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;


// 1. Is "enable changed" action broadcasted with correct arguments when enabled/disabled?
// 2. Is "enable changed" action not broadcasted when reassigning same value?
// 3. Is "enable changed" action broadcasted when component or game object is disabled/enabled?

// 4. Is "act status change" action broadcasted with correct arguments on every status change?
// 5. Is "act status change" action not broadcasted for acts without theater?

// 6. Does PerformAll() perform all acts exactly once?
// 7. Does PerformAll() not perform all when theater is disabled?
// 8. Does PerformAll() not perform acts that are disabled, blocked or when theater is disabled?

// 9. Does RetryAll() retry performing acts and perform non performing acts?
// 10. Does RetryAll() not retry acts when theater is disabled?

// 11. Does AbortAll() abort all performing acts with "Interrupted" outcome?
// 12. Does AbortAll() not affect acts of another theater?
// 13. Can all acts perform again after AbortAll()?

// 14. Does SetEnabled() change IsEnabled()?
// 15. Does SetEnabled(false) abort all performing acts?
// 16. Does SetEnabled(false) cancel all deferred performs, retries and aborts?
// 17. Can acts perform again after SetEnabled(true)?

// 18. Does HasAct() return true only for acts added to the theater (false after Deinit(), other theater, null)?

// 19. Does IsEnabled() return false when SetEnabled(false), component disabled or game object inactive?

// 20. Does AreAnyPerforming() return correct state (none, some, all, after finish)?
// 21. Does GetAllPerformingActs() return only performing acts?
// 22. Does GetAllActs() return all acts assigned to theater and drop them after Deinit()?

// 23. Does disabling component, deactivating game object or destroying theater abort all acts and cancel deferred operations?
// 24. Do acts tick on matching updates (Update, FixedUpdate, LateUpdate) and stop after finish?


public class TheaterTests
{
    [UnityTest]
    public IEnumerator EnabledChanged()  // 1
    {
        // Prerequisites
        var theaterName = "Test Theater";
        var expectedCount = 2;
        var receivedTheaters = new List<Theater>();
        var receivedStates = new List<bool>();


        // Setup theater
        var theater = new GameObject(theaterName).AddComponent<Theater>();
        theater.OnEnableChanged += (t, newIsEnabled) =>
        {
            receivedTheaters.Add(t);
            receivedStates.Add(newIsEnabled);
        };


        // Disable then enable
        theater.SetEnabled(false);
        theater.SetEnabled(true);


        // Assertions
        Assert.IsTrue(receivedStates.Count == expectedCount, $"OnEnableChanged not invoked exactly {expectedCount} times! Count={receivedStates.Count}");
        Assert.IsTrue(receivedTheaters.All(t => t == theater), "OnEnableChanged first argument is invalid!");
        Assert.IsFalse(receivedStates[0], "OnEnableChanged second argument is not false on disable!");
        Assert.IsTrue(receivedStates[1], "OnEnableChanged second argument is not true on enable!");


        UnityEngine.Object.Destroy(theater.gameObject);
        yield return null;
    }
    [UnityTest]
    public IEnumerator EnabledChangedNotOnSameValue()  // 2
    {
        // Prerequisites
        var theaterName = "Test Theater";
        var invokeCount = 0;
        var expectedCountAfterFirstChange = 1;


        // Setup theater
        var theater = new GameObject(theaterName).AddComponent<Theater>();
        theater.OnEnableChanged += (t, newIsEnabled) => { invokeCount++; };


        // Reassign enabled value
        theater.SetEnabled(true);
        var countAfterSameEnable = invokeCount;


        // Disable twice
        theater.SetEnabled(false);
        theater.SetEnabled(false);


        // Assertions
        Assert.IsTrue(countAfterSameEnable == 0, $"OnEnableChanged invoked despite reassigning same value! Count={countAfterSameEnable}");
        Assert.IsTrue(invokeCount == expectedCountAfterFirstChange, $"OnEnableChanged invoked again despite disabling twice! Count={invokeCount}");


        UnityEngine.Object.Destroy(theater.gameObject);
        yield return null;
    }
    [UnityTest]
    public IEnumerator EnabledChangedOnComponentOrGameObject()  // 3
    {
        // Prerequisites
        var theaterName = "Test Theater";
        var expectedStates = new List<bool> { false, true, false, true };
        var receivedStates = new List<bool>();


        // Setup theater
        var theater = new GameObject(theaterName).AddComponent<Theater>();
        theater.OnEnableChanged += (t, newIsEnabled) => { receivedStates.Add(newIsEnabled); };


        // Toggle component then game object
        theater.enabled = false;
        theater.enabled = true;
        theater.gameObject.SetActive(false);
        theater.gameObject.SetActive(true);


        // Assertions
        Assert.IsTrue(receivedStates.SequenceEqual(expectedStates), $"OnEnableChanged states invalid! Received='{string.Join(",", receivedStates)}'  Expected='{string.Join(",", expectedStates)}'");


        UnityEngine.Object.Destroy(theater.gameObject);
        yield return null;
    }



    [UnityTest]
    public IEnumerator ActStatusChange()  // 4
    {
        // Prerequisites
        var theaterName = "Test Theater";
        var actName = "Test Act";
        var expectedStatuses = new List<Act.Status>
        {
            Act.Status.Prologuing,
            Act.Status.Entering,
            Act.Status.Exiting,
            Act.Status.Concluding,
            Act.Status.None
        };
        var receivedTheaters = new List<Theater>();
        var receivedActs = new List<Act>();
        var receivedStatuses = new List<Act.Status>();


        // Setup theater and act, Subscribed after init
        var theater = new GameObject(theaterName).AddComponent<Theater>();
        var act = new ManualFinishAct();
        act.Init(actName, theater);
        theater.OnActStatusChange += (t, a) =>
        {
            receivedTheaters.Add(t);
            receivedActs.Add(a);
            receivedStatuses.Add(a.GetStatus());
        };


        // Perform and finish
        act.Perform();
        act.ManualFinish();


        // Assertions
        Assert.IsTrue(receivedStatuses.SequenceEqual(expectedStatuses), $"OnActStatusChange statuses invalid! Received='{string.Join(",", receivedStatuses)}'  Expected='{string.Join(",", expectedStatuses)}'");
        Assert.IsTrue(receivedTheaters.All(t => t == theater), "OnActStatusChange first argument is invalid!");
        Assert.IsTrue(receivedActs.All(a => a == act), "OnActStatusChange second argument is invalid!");


        UnityEngine.Object.Destroy(theater.gameObject);
        yield return null;
    }
    [UnityTest]
    public IEnumerator ActStatusChangeNotForActsWithoutTheater()  // 5
    {
        // Prerequisites
        var theaterName = "Test Theater";
        var actName = "Test Act";
        var invokeCount = 0;


        // Setup theater and act without theater
        var theater = new GameObject(theaterName).AddComponent<Theater>();
        theater.OnActStatusChange += (t, a) => { invokeCount++; };

        var act = new ManualFinishAct();
        act.Init(actName);


        // Perform and finish
        act.Perform();
        act.ManualFinish();


        // Assertions
        Assert.IsTrue(invokeCount == 0, $"OnActStatusChange invoked for act without theater! Count={invokeCount}");


        UnityEngine.Object.Destroy(theater.gameObject);
        yield return null;
    }



    [UnityTest]
    public IEnumerator PerformAllPerformsAllOnce()  // 6
    {
        // Prerequisites
        var theaterName = "Test Theater";
        var actNamePrefix = "Test Act ";
        var actCount = 3;
        var expectedPerformCount = 1;


        // Setup theater and acts
        var theater = new GameObject(theaterName).AddComponent<Theater>();
        var acts = new List<Act>();
        for (var i = 0; i < actCount; i++)
        {
            var act = new Act();
            act.Init(actNamePrefix + i, theater);
            acts.Add(act);
        }


        // Perform all
        theater.PerformAll();


        // Assertions
        foreach (var act in acts)
        {
            Assert.IsTrue(act.GetPerformCount() == expectedPerformCount, $"{act.GetName()} did not perform exactly once! Perform Count={act.GetPerformCount()}");
        }


        UnityEngine.Object.Destroy(theater.gameObject);
        yield return null;
    }
    [UnityTest]
    public IEnumerator PerformAllNotWhenTheaterDisabled()  // 7
    {
        // Prerequisites
        var theaterName = "Test Theater";
        var actNamePrefix = "Test Act ";
        var actCount = 3;
        var expectedPerformCount = 0;


        // Setup disabled theater and acts
        var theater = new GameObject(theaterName).AddComponent<Theater>();
        var acts = new List<Act>();
        for (var i = 0; i < actCount; i++)
        {
            var act = new Act();
            act.Init(actNamePrefix + i, theater);
            acts.Add(act);
        }
        theater.SetEnabled(false);


        // Perform all
        theater.PerformAll();


        // Assertions
        foreach (var act in acts)
        {
            Assert.IsTrue(act.GetPerformCount() == expectedPerformCount, $"{act.GetName()} performed despite theater being disabled! Perform Count={act.GetPerformCount()}");
        }


        UnityEngine.Object.Destroy(theater.gameObject);
        yield return null;
    }
    [UnityTest]
    public IEnumerator PerformAllSkipsDisabledAndBlockedActs()  // 8
    {
        // Prerequisites
        var theaterName = "Test Theater";
        var expectedPerformCount = 1;
        var expectedSkippedCount = 0;


        // Setup theater and acts
        var theater = new GameObject(theaterName).AddComponent<Theater>();

        var normalAct = new Act();
        normalAct.Init("Normal Act", theater);

        var disabledAct = new Act();
        disabledAct.Init("Disabled Act", theater);
        disabledAct.SetEnabled(false);

        var blockedAct = new Act();
        blockedAct.Init("Blocked Act", theater);

        var blockerAct = new WaitInfiniAct();
        blockerAct.Init("Blocker Act");
        blockerAct.block = (Act a) => new() { { blockedAct, Act.BlockType.Persistent } };
        blockerAct.Perform();


        // Perform all
        theater.PerformAll();


        // Assertions
        Assert.IsTrue(blockedAct.IsBlocked(), "Blocked act is not blocked!");
        Assert.IsTrue(normalAct.GetPerformCount() == expectedPerformCount, $"Normal act did not perform exactly once! Perform Count={normalAct.GetPerformCount()}");
        Assert.IsTrue(disabledAct.GetPerformCount() == expectedSkippedCount, $"Disabled act performed! Perform Count={disabledAct.GetPerformCount()}");
        Assert.IsTrue(blockedAct.GetPerformCount() == expectedSkippedCount, $"Blocked act performed! Perform Count={blockedAct.GetPerformCount()}");


        UnityEngine.Object.Destroy(theater.gameObject);
        yield return null;
    }



    [UnityTest]
    public IEnumerator RetryAllRetriesAndPerforms()  // 9
    {
        // Prerequisites
        var theaterName = "Test Theater";
        var expectedRetriedCount = 2;
        var expectedPerformedCount = 1;


        // Setup theater and acts
        var theater = new GameObject(theaterName).AddComponent<Theater>();

        var performingAct = new WaitInfiniAct();
        performingAct.Init("Performing Act", theater);
        performingAct.Perform();

        var idleAct = new Act();
        idleAct.Init("Idle Act", theater);


        // Retry all
        theater.RetryAll();


        // Assertions
        Assert.IsTrue(performingAct.GetPerformCount() == expectedRetriedCount, $"Performing act did not retry! Perform Count={performingAct.GetPerformCount()}");
        Assert.IsTrue(performingAct.GetStatus() == Act.Status.Entering, $"Performing act did not re enter after retrying! Status={performingAct.GetStatus()}");
        Assert.IsTrue(idleAct.GetPerformCount() == expectedPerformedCount, $"Idle act did not perform on RetryAll()! Perform Count={idleAct.GetPerformCount()}");


        UnityEngine.Object.Destroy(theater.gameObject);
        yield return null;
    }
    [UnityTest]
    public IEnumerator RetryAllNotWhenTheaterDisabled()  // 10
    {
        // Prerequisites
        var theaterName = "Test Theater";
        var expectedPerformingCount = 1;
        var expectedIdleCount = 0;


        // Setup theater and acts
        var theater = new GameObject(theaterName).AddComponent<Theater>();

        var performingAct = new WaitInfiniAct();
        performingAct.Init("Performing Act", theater);
        performingAct.Perform();

        var idleAct = new Act();
        idleAct.Init("Idle Act", theater);


        // Disable theater then retry all
        theater.SetEnabled(false);
        theater.RetryAll();


        // Assertions
        Assert.IsTrue(performingAct.GetPerformCount() == expectedPerformingCount, $"Performing act retried despite theater being disabled! Perform Count={performingAct.GetPerformCount()}");
        Assert.IsFalse(performingAct.IsPerforming(), "Performing act is still performing despite theater being disabled!");
        Assert.IsTrue(idleAct.GetPerformCount() == expectedIdleCount, $"Idle act performed despite theater being disabled! Perform Count={idleAct.GetPerformCount()}");


        UnityEngine.Object.Destroy(theater.gameObject);
        yield return null;
    }



    [UnityTest]
    public IEnumerator AbortAllInterruptsAllActs()  // 11
    {
        // Prerequisites
        var theaterName = "Test Theater";
        var actNamePrefix = "Test Act ";
        var actCount = 3;


        // Setup theater and perform acts
        var theater = new GameObject(theaterName).AddComponent<Theater>();
        var acts = new List<Act>();
        for (var i = 0; i < actCount; i++)
        {
            var act = new WaitInfiniAct();
            act.Init(actNamePrefix + i, theater);
            act.Perform();
            acts.Add(act);
        }


        // Abort all
        theater.AbortAll();


        // Assertions
        foreach (var act in acts)
        {
            Assert.IsFalse(act.IsPerforming(), $"{act.GetName()} still performing despite AbortAll()!");
            Assert.IsTrue(act.GetOutcome() == Act.Outcome.Interrupted, $"{act.GetName()} outcome is not 'Interrupted'! Outcome={act.GetOutcome()}");
        }


        UnityEngine.Object.Destroy(theater.gameObject);
        yield return null;
    }
    [UnityTest]
    public IEnumerator AbortAllNotAffectOtherTheater()  // 12
    {
        // Prerequisites
        var theaterName = "Test Theater";
        var otherTheaterName = "Other Test Theater";


        // Setup theaters and perform acts
        var theater = new GameObject(theaterName).AddComponent<Theater>();
        var otherTheater = new GameObject(otherTheaterName).AddComponent<Theater>();

        var act = new WaitInfiniAct();
        act.Init("Test Act", theater);
        act.Perform();

        var otherAct = new WaitInfiniAct();
        otherAct.Init("Other Test Act", otherTheater);
        otherAct.Perform();


        // Abort all on first theater
        theater.AbortAll();


        // Assertions
        Assert.IsFalse(act.IsPerforming(), "Act still performing despite AbortAll()!");
        Assert.IsTrue(otherAct.IsPerforming(), "Act of other theater aborted by AbortAll()!");


        UnityEngine.Object.Destroy(theater.gameObject);
        UnityEngine.Object.Destroy(otherTheater.gameObject);
        yield return null;
    }
    [UnityTest]
    public IEnumerator PerformAfterAbortAll()  // 13
    {
        // Prerequisites
        var theaterName = "Test Theater";
        var actNamePrefix = "Test Act ";
        var actCount = 3;


        // Setup theater and perform acts
        var theater = new GameObject(theaterName).AddComponent<Theater>();
        var acts = new List<Act>();
        for (var i = 0; i < actCount; i++)
        {
            var act = new WaitInfiniAct();
            act.Init(actNamePrefix + i, theater);
            act.Perform();
            acts.Add(act);
        }


        // Abort all then reperform
        theater.AbortAll();
        foreach (var act in acts)
        {
            act.Perform();
        }


        // Assertions
        foreach (var act in acts)
        {
            Assert.IsTrue(act.IsPerforming(), $"{act.GetName()} did not perform after AbortAll()!");
        }


        UnityEngine.Object.Destroy(theater.gameObject);
        yield return null;
    }



    [UnityTest]
    public IEnumerator SetEnabledChangesIsEnabled()  // 14
    {
        // Prerequisites
        var theaterName = "Test Theater";


        // Toggle enabled
        var theater = new GameObject(theaterName).AddComponent<Theater>();

        var isEnabledByDefault = theater.IsEnabled();
        theater.SetEnabled(false);
        var isEnabledAfterDisable = theater.IsEnabled();
        theater.SetEnabled(true);
        var isEnabledAfterEnable = theater.IsEnabled();


        // Assertions
        Assert.IsTrue(isEnabledByDefault, "IsEnabled() false despite theater being enabled by default!");
        Assert.IsFalse(isEnabledAfterDisable, "IsEnabled() true despite SetEnabled(false)!");
        Assert.IsTrue(isEnabledAfterEnable, "IsEnabled() false despite SetEnabled(true)!");


        UnityEngine.Object.Destroy(theater.gameObject);
        yield return null;
    }
    [UnityTest]
    public IEnumerator SetEnabledFalseAbortsAllActs()  // 15
    {
        // Prerequisites
        var theaterName = "Test Theater";
        var actNamePrefix = "Test Act ";
        var actCount = 3;


        // Setup theater and perform acts
        var theater = new GameObject(theaterName).AddComponent<Theater>();
        var acts = new List<Act>();
        for (var i = 0; i < actCount; i++)
        {
            var act = new WaitInfiniAct();
            act.Init(actNamePrefix + i, theater);
            act.Perform();
            acts.Add(act);
        }


        // Disable theater
        theater.SetEnabled(false);


        // Assertions
        foreach (var act in acts)
        {
            Assert.IsFalse(act.IsPerforming(), $"{act.GetName()} still performing despite theater being disabled!");
            Assert.IsTrue(act.GetOutcome() == Act.Outcome.Interrupted, $"{act.GetName()} outcome is not 'Interrupted'! Outcome={act.GetOutcome()}");
        }


        UnityEngine.Object.Destroy(theater.gameObject);
        yield return null;
    }
    [UnityTest]
    public IEnumerator SetEnabledFalseCancelsDeferred()  // 16
    {
        // Prerequisites
        var theaterName = "Test Theater";
        var allFlags = Act.TickFlags.Tick | Act.TickFlags.PhysicsTick | Act.TickFlags.LateTick;
        var expectedPerformDeferCount = 0;
        var expectedRetryDeferCount = 2;


        // Setup theater and acts
        var theater = new GameObject(theaterName).AddComponent<Theater>();

        var performDeferAct = new Act();
        performDeferAct.Init("Perform Defer Act", theater);

        var retryDeferAct = new WaitInfiniAct();
        retryDeferAct.Init("Retry Defer Act", theater);
        retryDeferAct.Perform();

        var abortDeferAct = new WaitInfiniAct();
        abortDeferAct.Init("Abort Defer Act", theater);
        abortDeferAct.Perform();


        // Defer operations then disable and enable theater
        performDeferAct.PerformDeferred(allFlags);
        retryDeferAct.RetryDeferred(allFlags);
        abortDeferAct.AbortDeferred(allFlags);

        theater.SetEnabled(false);
        theater.SetEnabled(true);


        // Reperform so leftover deferred operations would be visible
        retryDeferAct.Perform();
        abortDeferAct.Perform();

        yield return new WaitForFixedUpdate();
        yield return null;


        // Assertions
        Assert.IsTrue(performDeferAct.GetPerformCount() == expectedPerformDeferCount, $"Deferred perform fired despite theater being disabled! Perform Count={performDeferAct.GetPerformCount()}");
        Assert.IsTrue(retryDeferAct.GetPerformCount() == expectedRetryDeferCount, $"Deferred retry fired despite theater being disabled! Perform Count={retryDeferAct.GetPerformCount()}");
        Assert.IsTrue(abortDeferAct.IsPerforming(), "Deferred abort fired despite theater being disabled!");


        UnityEngine.Object.Destroy(theater.gameObject);
    }
    [UnityTest]
    public IEnumerator PerformAfterSetEnabledTrue()  // 17
    {
        // Prerequisites
        var theaterName = "Test Theater";


        // Setup theater and act
        var theater = new GameObject(theaterName).AddComponent<Theater>();
        var act = new WaitInfiniAct();
        act.Init("Test Act", theater);


        // Perform while disabled then enabled
        theater.SetEnabled(false);
        act.Perform();
        var performingWhileDisabled = act.IsPerforming();

        theater.SetEnabled(true);
        act.Perform();


        // Assertions
        Assert.IsFalse(performingWhileDisabled, "Act performed while theater was disabled!");
        Assert.IsTrue(act.IsPerforming(), "Act did not perform after theater was enabled again!");


        UnityEngine.Object.Destroy(theater.gameObject);
        yield return null;
    }



    [UnityTest]
    public IEnumerator HasActValid()  // 18
    {
        // Prerequisites
        var theaterName = "Test Theater";
        var otherTheaterName = "Other Test Theater";


        // Setup theaters and acts
        var theater = new GameObject(theaterName).AddComponent<Theater>();
        var otherTheater = new GameObject(otherTheaterName).AddComponent<Theater>();

        var act = new Act();
        act.Init("Test Act", theater);

        var otherAct = new Act();
        otherAct.Init("Other Test Act", otherTheater);

        var neverAddedAct = new Act();
        neverAddedAct.Init("Never Added Act");


        // Check before and after deinit
        var hasActBeforeDeinit = theater.HasAct(act);
        act.Deinit();
        var hasActAfterDeinit = theater.HasAct(act);


        // Assertions
        Assert.IsTrue(hasActBeforeDeinit, "HasAct() false for act added to theater!");
        Assert.IsFalse(hasActAfterDeinit, "HasAct() true after Deinit()!");
        Assert.IsFalse(theater.HasAct(otherAct), "HasAct() true for act of other theater!");
        Assert.IsFalse(theater.HasAct(neverAddedAct), "HasAct() true for act without theater!");
        Assert.IsFalse(theater.HasAct(null), "HasAct() true for null!");


        UnityEngine.Object.Destroy(theater.gameObject);
        UnityEngine.Object.Destroy(otherTheater.gameObject);
        yield return null;
    }



    [UnityTest]
    public IEnumerator IsEnabledFalseCases()  // 19
    {
        // Prerequisites
        var theaterName = "Test Theater";


        // Toggle each condition one at a time
        var theater = new GameObject(theaterName).AddComponent<Theater>();

        theater.SetEnabled(false);
        var isEnabledSetDisabled = theater.IsEnabled();
        theater.SetEnabled(true);
        var isEnabledSetReenabled = theater.IsEnabled();

        theater.enabled = false;
        var isEnabledComponentDisabled = theater.IsEnabled();
        theater.enabled = true;
        var isEnabledComponentReenabled = theater.IsEnabled();

        theater.gameObject.SetActive(false);
        var isEnabledObjectInactive = theater.IsEnabled();
        theater.gameObject.SetActive(true);
        var isEnabledObjectReactivated = theater.IsEnabled();


        // Assertions
        Assert.IsFalse(isEnabledSetDisabled, "IsEnabled() true despite SetEnabled(false)!");
        Assert.IsTrue(isEnabledSetReenabled, "IsEnabled() false after SetEnabled(true)!");
        Assert.IsFalse(isEnabledComponentDisabled, "IsEnabled() true despite component being disabled!");
        Assert.IsTrue(isEnabledComponentReenabled, "IsEnabled() false after component was enabled again!");
        Assert.IsFalse(isEnabledObjectInactive, "IsEnabled() true despite game object being inactive!");
        Assert.IsTrue(isEnabledObjectReactivated, "IsEnabled() false after game object was reactivated!");


        UnityEngine.Object.Destroy(theater.gameObject);
        yield return null;
    }



    [UnityTest]
    public IEnumerator AreAnyPerformingValid()  // 20
    {
        // Prerequisites
        var theaterName = "Test Theater";


        // Setup theater and acts
        var theater = new GameObject(theaterName).AddComponent<Theater>();

        var actA = new WaitInfiniAct();
        actA.Init("Test Act A", theater);

        var actB = new ManualFinishAct();
        actB.Init("Test Act B", theater);


        // Check at every stage
        var anyWhenNone = theater.AreAnyPerforming();

        actA.Perform();
        var anyWhenSome = theater.AreAnyPerforming();

        actB.Perform();
        var anyWhenAll = theater.AreAnyPerforming();

        actA.Abort();
        actB.ManualFinish();
        var anyAfterFinish = theater.AreAnyPerforming();


        // Assertions
        Assert.IsFalse(anyWhenNone, "AreAnyPerforming() true despite no acts performing!");
        Assert.IsTrue(anyWhenSome, "AreAnyPerforming() false despite one act performing!");
        Assert.IsTrue(anyWhenAll, "AreAnyPerforming() false despite all acts performing!");
        Assert.IsFalse(anyAfterFinish, "AreAnyPerforming() true after all acts finished!");


        UnityEngine.Object.Destroy(theater.gameObject);
        yield return null;
    }
    [UnityTest]
    public IEnumerator GetAllPerformingActsValid()  // 21
    {
        // Prerequisites
        var theaterName = "Test Theater";
        var expectedPerformingCount = 2;
        var expectedCountAfterAbort = 1;


        // Setup theater and acts
        var theater = new GameObject(theaterName).AddComponent<Theater>();

        var actA = new WaitInfiniAct();
        actA.Init("Test Act A", theater);
        actA.Perform();

        var actB = new WaitInfiniAct();
        actB.Init("Test Act B", theater);
        actB.Perform();

        var idleAct = new Act();
        idleAct.Init("Idle Act", theater);


        // Check before and after abort
        var performingActs = theater.GetAllPerformingActs().ToList();
        actA.Abort();
        var performingActsAfterAbort = theater.GetAllPerformingActs().ToList();


        // Assertions
        Assert.IsTrue(performingActs.Contains(actA), "GetAllPerformingActs() does not contain Act A!");
        Assert.IsTrue(performingActs.Contains(actB), "GetAllPerformingActs() does not contain Act B!");
        Assert.IsFalse(performingActs.Contains(idleAct), "GetAllPerformingActs() contains idle act!");
        Assert.IsTrue(performingActs.Count == expectedPerformingCount, $"GetAllPerformingActs() returned incorrect count! Count={performingActs.Count}");
        Assert.IsTrue(performingActsAfterAbort.Count == expectedCountAfterAbort && performingActsAfterAbort.Contains(actB), $"GetAllPerformingActs() invalid after abort! Count={performingActsAfterAbort.Count}");


        UnityEngine.Object.Destroy(theater.gameObject);
        yield return null;
    }
    [UnityTest]
    public IEnumerator GetAllActsValid()  // 22
    {
        // Prerequisites
        var theaterName = "Test Theater";
        var actNamePrefix = "Test Act ";
        var actCount = 3;


        // Setup theater and acts
        var theater = new GameObject(theaterName).AddComponent<Theater>();
        var acts = new List<Act>();
        for (var i = 0; i < actCount; i++)
        {
            var act = new Act();
            act.Init(actNamePrefix + i, theater);
            acts.Add(act);
        }


        // Check before and after deinit
        var allActs = theater.GetAllActs().ToList();
        var deinitAct = acts[0];
        deinitAct.Deinit();
        var allActsAfterDeinit = theater.GetAllActs().ToList();


        // Assertions
        foreach (var act in acts)
        {
            Assert.IsTrue(allActs.Contains(act), $"GetAllActs() does not contain {act.GetName()}!");
        }
        Assert.IsTrue(allActs.Count == actCount, $"GetAllActs() returned incorrect count! Count={allActs.Count}");
        Assert.IsFalse(allActsAfterDeinit.Contains(deinitAct), "GetAllActs() still contains act after Deinit()!");
        Assert.IsTrue(allActsAfterDeinit.Count == actCount - 1, $"GetAllActs() returned incorrect count after Deinit()! Count={allActsAfterDeinit.Count}");


        UnityEngine.Object.Destroy(theater.gameObject);
        yield return null;
    }



    [UnityTest]
    public IEnumerator LifecycleAbortsAllAndCancelsDeferred()  // 23
    {
        // Prerequisites
        var theaterName = "Test Theater";
        var allFlags = Act.TickFlags.Tick | Act.TickFlags.PhysicsTick | Act.TickFlags.LateTick;
        var expectedPerformDeferCount = 0;


        // Component disabled
        {
            var theater = new GameObject(theaterName).AddComponent<Theater>();

            var act = new WaitInfiniAct();
            act.Init("Component Act", theater);
            act.Perform();
            act.AbortDeferred(allFlags);

            var deferAct = new Act();
            deferAct.Init("Component Defer Act", theater);
            deferAct.PerformDeferred(allFlags);

            theater.enabled = false;
            var performingAfterDisable = act.IsPerforming();
            theater.enabled = true;

            act.Perform();

            yield return new WaitForFixedUpdate();
            yield return null;


            // Assertions
            Assert.IsFalse(performingAfterDisable, "Component: Act still performing after component disabled!");
            Assert.IsTrue(act.IsPerforming(), "Component: Deferred abort fired after component disabled!");
            Assert.IsTrue(deferAct.GetPerformCount() == expectedPerformDeferCount, $"Component: Deferred perform fired after component disabled! Perform Count={deferAct.GetPerformCount()}");

            UnityEngine.Object.Destroy(theater.gameObject);
        }


        // Game object deactivated
        {
            var theater = new GameObject(theaterName).AddComponent<Theater>();

            var act = new WaitInfiniAct();
            act.Init("Object Act", theater);
            act.Perform();
            act.AbortDeferred(allFlags);

            var deferAct = new Act();
            deferAct.Init("Object Defer Act", theater);
            deferAct.PerformDeferred(allFlags);

            theater.gameObject.SetActive(false);
            var performingAfterDeactivate = act.IsPerforming();
            theater.gameObject.SetActive(true);

            act.Perform();

            yield return new WaitForFixedUpdate();
            yield return null;


            // Assertions
            Assert.IsFalse(performingAfterDeactivate, "Object: Act still performing after game object deactivated!");
            Assert.IsTrue(act.IsPerforming(), "Object: Deferred abort fired after game object deactivated!");
            Assert.IsTrue(deferAct.GetPerformCount() == expectedPerformDeferCount, $"Object: Deferred perform fired after game object deactivated! Perform Count={deferAct.GetPerformCount()}");

            UnityEngine.Object.Destroy(theater.gameObject);
        }


        // Theater destroyed
        {
            var theater = new GameObject(theaterName).AddComponent<Theater>();

            var act = new WaitInfiniAct();
            act.Init("Destroy Act", theater);
            act.Perform();

            UnityEngine.Object.Destroy(theater.gameObject);

            yield return null;


            // Assertions
            Assert.IsFalse(act.IsPerforming(), "Destroy: Act still performing after theater destroyed!");
        }
    }
    [UnityTest]
    public IEnumerator ActsTickOnMatchingUpdates()  // 24
    {
        // Prerequisites
        var theaterName = "Test Theater";
        var waitFrames = 3;
        var noTickCount = 0;


        // Setup theater and acts
        var theater = new GameObject(theaterName).AddComponent<Theater>();

        var tickAct = new TickAct();
        tickAct.Init("Tick Act", theater);
        tickAct.Perform();

        var physicsTickAct = new PhysicsTickAct();
        physicsTickAct.Init("Physics Tick Act", theater);
        physicsTickAct.Perform();

        var lateTickAct = new LateTickAct();
        lateTickAct.Init("Late Tick Act", theater);
        lateTickAct.Perform();


        // Wait for updates
        for (var i = 0; i < waitFrames; i++)
        {
            yield return new WaitForFixedUpdate();
            yield return null;
        }


        // Assertions for matching updates
        Assert.IsTrue(tickAct.GetTickCount() > noTickCount, "Tick act did not tick on Update!");
        Assert.IsTrue(tickAct.GetPhysicsTickCount() == noTickCount && tickAct.GetLateTickCount() == noTickCount, "Tick act ticked on non matching updates!");
        Assert.IsTrue(physicsTickAct.GetPhysicsTickCount() > noTickCount, "Physics tick act did not tick on FixedUpdate!");
        Assert.IsTrue(physicsTickAct.GetTickCount() == noTickCount && physicsTickAct.GetLateTickCount() == noTickCount, "Physics tick act ticked on non matching updates!");
        Assert.IsTrue(lateTickAct.GetLateTickCount() > noTickCount, "Late tick act did not tick on LateUpdate!");
        Assert.IsTrue(lateTickAct.GetTickCount() == noTickCount && lateTickAct.GetPhysicsTickCount() == noTickCount, "Late tick act ticked on non matching updates!");


        // Finish acts and store counts
        tickAct.Abort();
        physicsTickAct.Abort();
        lateTickAct.Abort();

        var tickCountAfterFinish = tickAct.GetTickCount();
        var physicsTickCountAfterFinish = physicsTickAct.GetPhysicsTickCount();
        var lateTickCountAfterFinish = lateTickAct.GetLateTickCount();

        for (var i = 0; i < waitFrames; i++)
        {
            yield return new WaitForFixedUpdate();
            yield return null;
        }


        // Assertions for finished acts
        Assert.IsTrue(tickAct.GetTickCount() == tickCountAfterFinish, $"Tick act kept ticking after finish! Count={tickAct.GetTickCount()}");
        Assert.IsTrue(physicsTickAct.GetPhysicsTickCount() == physicsTickCountAfterFinish, $"Physics tick act kept ticking after finish! Count={physicsTickAct.GetPhysicsTickCount()}");
        Assert.IsTrue(lateTickAct.GetLateTickCount() == lateTickCountAfterFinish, $"Late tick act kept ticking after finish! Count={lateTickAct.GetLateTickCount()}");


        UnityEngine.Object.Destroy(theater.gameObject);
        yield return null;
    }
}
