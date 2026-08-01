using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;


// 1. Are "pre setup" & "post setup" actions being broadcasted (with correct arguments)?
// 1. Is Setup() being invoked?
// 1. Does calling Init() twice fail?
// 1. Is theater assigned after initialization?
// 1. Is name assigned after initialization?
// 1. Is initially enabled/disabled working?


public class ActInitializeTests
{
    [UnityTest]
    public IEnumerator OnPreAndPostSetup()
    {
        // Prerequisites
        bool wasPreSetupInvoked = false;
        Act preSetupArg1 = null;
        bool wasPostSetupInvoked = false;
        Act postSetupArg1 = null;


        // Perform Act
        var act = new Act();
        act.OnPreSetup += (a) => { wasPreSetupInvoked = true; preSetupArg1 = a; };
        act.OnPostSetup += (a) => { wasPostSetupInvoked = true; postSetupArg1 = a; };
        act.Init("Test Act");


        // Assertions
        Assert.IsTrue(wasPreSetupInvoked, "OnPreSetup not invoked!");
        Assert.IsTrue(preSetupArg1 == act, $"OnPreSetup first argument is invalid! Arg1=`{preSetupArg1}`");
        Assert.IsTrue(wasPostSetupInvoked, "OnPostSetup not invoked!");
        Assert.IsTrue(postSetupArg1 == act, $"OnPostSetup first argument is invalid! Arg1=`{preSetupArg1}`");


        yield return null;
    }
    [UnityTest]
    public IEnumerator Setup()
    {
        // Perform Act
        var act = new SetupAct();
        act.Init("Test Act");


        // Assertions
        Assert.IsTrue(act.callCount == 1, $"Setup() not invoked exactly once! Call count={act.callCount}");


        yield return null;
    }
    [UnityTest]
    public IEnumerator InitCalledTwice()
    {
        // Prerequisites
        var preSetupCount = 0;
        var postSetupCount = 0;

        // Perform Act
        var act = new Act();
        act.OnPreSetup += (a) => { preSetupCount++; };
        act.OnPostSetup += (a) => { postSetupCount++; };
        act.Init("Test Act");
        act.Init("Test Act");


        // Assertions
        Assert.IsTrue(preSetupCount == 1, $"OnPreSetup invoked more than once despite calling Init() twice! Count={preSetupCount}");
        Assert.IsTrue(postSetupCount == 1, $"OnPostSetup invoked more than once despite calling Init() twice! Count={postSetupCount}");

        yield return null;
    }
    [UnityTest]
    public IEnumerator TheaterAfterInit()
    {
        // Check if theater is invalid if not assigned in init
        var act1 = new Act();
        act1.Init("Test Act");
        var theater1 = act1.GetTheater();


        // Check if theater is valid if assigned in init
        var validTheater = new GameObject().AddComponent<Theater>();
        var act2 = new Act();
        act2.Init("Test Act 2", validTheater);
        var theater2 = act2.GetTheater();


        // Assertions
        Assert.IsTrue(theater1 == null, $"Theater is not null despite not being passed to Init()! Theater='{theater1}'");
        Assert.IsTrue(theater2 == validTheater, $"Theater is null despite being passed to Init()! Theater='{theater2}'");


        UnityEngine.Object.Destroy(validTheater.gameObject);
        yield return null;
    }
    [UnityTest]
    public IEnumerator NameAfterInit()
    {
        // Perform Act
        var actName = "My Named Act";
        var act = new Act();
        act.Init(actName);


        // Assertions
        Assert.IsTrue(act.GetName() == actName, $"Name is invalid after Init()! Name='{act.GetName()}' Original Name='{actName}'");


        yield return null;
    }
    [UnityTest]
    public IEnumerator InitiallyEnabledOrDisabled()
    {
        // Perform Act
        var enabledAct = new Act();
        enabledAct.Init("Enabled Act", null, true);

        var disabledAct = new Act();
        disabledAct.Init("Disabled Act", null, false);


        // Assertions
        Assert.IsTrue(enabledAct.IsEnabled(), "Act is not enabled despite initiallyEnabled being true!");
        Assert.IsTrue(!disabledAct.IsEnabled(), "Act is enabled despite initiallyEnabled being false!");
        Assert.IsTrue(!disabledAct.IsBlocked(), "Initially disabled act is blocked!");


        yield return null;
    }
}
