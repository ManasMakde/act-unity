# 🎭 Act Unity Documentation v0.4.0-alpha

## ⚙️ Act Class

| Enums   | Constants |
|---------|----------|
| [TickFlags](#tickflags) | `None`, `Tick`, `PhysicsTick`, `LateTick` |
| [Status](#status) | `None`, `Initializing`, `Prologuing`, `Entering`, `Ticking`, `Exiting`, `Concluding`, `Deinitializing` |
| [Outcome](#outcome) | `None`, `Interrupted`, `Failure`, `Pending`, `Success`, `Retry` |
| [BlockType](#blocktype) | `Interrupt`, `Persistent` |


| Signature    | Events |
|--------------|-------|
| \<Act act\> | [OnPreSetup](#onpresetup) |
| \<Act act\> | [OnPostSetup](#onpostsetup) |
| \<Act act\> | [OnPrePerformReq](#onpreperformreq) |
| \<Act act,<br> bool willPerform\> | [OnPostPerformReq](#onpostperformreq) |
| \<Act act\> | [OnPerformStart](#onperformstartact) |
| \<Act act\> | [OnPrePrologue](#onpreprologue) |
| \<Act act,<br> Act pAct,<br> [Outcome](#outcome) pOutcome\> | [OnPrologueComplete](#onprologuecomplete) |
| \<Act act\> | [OnPostPrologue](#onpostprologue) |
| \<Act act\> | [OnPreEnter](#onpreenter) |
| \<Act act\> | [OnPostEnter](#onpostenter) |
| \<Act act\> | [OnPreTick](#onpretick) |
| \<Act act\> | [OnPostTick](#onposttick) |
| \<Act act\> | [OnPrePhysicsTick](#onprephysicstick) |
| \<Act act\> | [OnPostPhysicsTick](#onpostphysicstick) |
| \<Act act\> | [OnPreLateTick](#onprelatetick) |
| \<Act act\> | [OnPostLateTick](#onpostlatetick) |
| \<Act act\> | [OnPreExit](#onpreexit) |
| \<Act act\> | [OnPostExit](#onpostexit) |
| \<Act act\> | [OnPerformEnd](#onperformendact) |
| \<Act act\> | [OnPreCleanup](#onprecleanup) |
| \<Act act\> | [OnPostCleanup](#onpostcleanup) |
| \<Act act,<br> bool newIsEnabled\> | [OnEnableChanged](#onenablechanged) |
| \<Act act,<br> Act blockingAct,<br> [BlockType](#blocktype) blockType,<br>bool didBlock\> | [OnBlockChanged](#onblockchanged) |


| Access | Type | Properties |
|--------|------|--------------|
| public | Func\<Act, bool\> | [condition](#condition) |
| public | Func\<Act, List\<List\<Act\>\>\> | [prologue](#prologue) |
| public | Func\<Act, Dictionary\<Act, [BlockType](#blocktype)\>\> | [block](#block) |
| public | bool | [isVerbose](#isverbose) |
| protected | string | [_name](#_name) |
| protected | bool | [_canReperform](#_canreperform) |
| protected | [TickFlags](#tickflags) | [_tickFlags](#_tickflags) |


| Access | Type | Methods |
|--------|------|--------------|
| public | void | [Init](#init)(string newName = "", Theater newTheater = null) |
| public | void | [Deinit](#deinit)() |
| public | bool | [Perform](#perform)() |
| public | void | [Retry](#retry)() |
| public | void | [Abort](#abort)() |
| public | void | [PerformDeferred](#performdeferred)([TickFlags](#tickflags) tickFlag = TickFlags.PhysicsTick) |
| public | void | [RetryDeferred](#retrydeferred)([TickFlags](#tickflags) tickFlag = TickFlags.PhysicsTick) |
| public | void | [AbortDeferred](#abortdeferred)([TickFlags](#tickflags) tickFlag = TickFlags.PhysicsTick) |
| public | void | [CancelPerformDeferred](#cancelperformdeferred)() |
| public | void | [CancelRetryDeferred](#cancelretrydeferred)() |
| public | void | [CancelAbortDeferred](#cancelabortdeferred)() |
| public | void | [SetEnabled](#setenabled)(bool newEnabled) |
| public | bool | [IsPerforming](#isperforming)() |
| public | bool | [IsOngoing](#isongoing)() |
| public | bool | [IsRetrying](#isretrying)() |
| public | bool | [IsEnabled](#isenabled)() |
| public | bool | [IsBlocked](#isblocked)() |
| public | bool | [IsBlockedBy](#isblockedby)(Act byAct) |
| public | bool | [HasInitialized](#hasinitialized)() |
| public | bool | [CanTick](#cantick)([TickFlags](#tickflags) type = TickFlags.Tick \| TickFlags.PhysicsTick \| TickFlags.LateTick) |
| public | string | [GetName](#getname)() |
| public | [Status](#status) | [GetStatus](#getstatus)() |
| public | [Status](#status) | [GetPrevStatus](#getprevstatus)() |
| public | [Outcome](#outcome) | [GetOutcome](#getoutcome)() |
| public | [TickFlags](#tickflags) | [GetTickFlags](#gettickflags)() |
| public | IReadOnlyCollection\<Act\> | [GetBlockedByActs](#getblockedbyacts)() |
| public | IReadOnlyDictionary\<Act, [BlockType](#blocktype)\> | [GetActsToBlock](#getactstoblock)() |
| public | int | [GetPerformCount](#getperformcount)() |
| public | int | [GetTickCount](#gettickcount)() |
| public | int | [GetPhysicsTickCount](#getphysicstickcount)() |
| public | int | [GetLateTickCount](#getlatetickcount)() |
| public | int | [GetLifetimePerformCount](#getlifetimeperformcount)() |
| public | int | [GetLifetimeTickCount](#getlifetimetickcount)() |
| public | int | [GetLifetimePhysicsTickCount](#getlifetimephysicstickcount)() |
| public | int | [GetLifetimeLateTickCount](#getlifetimelatetickcount)() |
| public | Theater | [GetTheater](#gettheater)() |
| protected virtual | void | [Setup](#setup)() <abbr title="">Virtual</abbr> |
| protected virtual | bool | [CanPerform](#canperform)() <abbr title="">Virtual</abbr> |
| protected virtual | [Outcome](#outcome) | [Enter](#enter)() <abbr title="">Virtual</abbr> |
| protected virtual | [Outcome](#outcome) | [Tick](#tick)() <abbr title="">Virtual</abbr> |
| protected virtual | [Outcome](#outcome) | [PhysicsTick](#physicstick)() <abbr title="">Virtual</abbr> |
| protected virtual | [Outcome](#outcome) | [LateTick](#latetick)() <abbr title="">Virtual</abbr> |
| protected virtual | void | [Exit](#exit)() <abbr title="">Virtual</abbr> |
| protected virtual | void | [Cleanup](#cleanup)() <abbr title="">Virtual</abbr> |
| protected | void | [Finish](#finish)([Outcome](#outcome) newOutcome = Outcome.Success) |
| protected virtual | void | [BlockSelf](#blockself)(Act byAct, [BlockType](#blocktype) blockType) <abbr title="">Virtual</abbr> |
| protected virtual | void | [UnblockSelf](#unblockself)(Act byAct) <abbr title="">Virtual</abbr> |
| protected virtual | void | [BlockOthers](#blockothers)() <abbr title="">Virtual</abbr> |
| protected virtual | void | [UnblockOthers](#unblockothers)() <abbr title="">Virtual</abbr> |
| protected virtual | void | [WriteLog](#writelog)(string message) <abbr title="">Virtual</abbr> |


<br/>


## ⚙️ Theater Class

| Signature    | Events |
|--------------|-------|
| \<Theater theater, bool newEnabled\> | [OnEnableChanged](#onenablechangedtheater) |
| \<Theater theater, Act act\> | [OnActStatusChange](#onactstatuschange) |


| Access | Type | Methods |
|--------|------|--------------|
| public | void | [PerformAll](#performall)() |
| public | void | [RetryAll](#retryall)() |
| public | void | [AbortAll](#abortall)() |
| public | void | [SetEnabled](#setenabledtheater)(bool newEnabled) |
| public | bool | [HasAct](#hasact)(Act act) |
| public | bool | [IsEnabled](#isenabledtheater)() |
| public | bool | [AreAnyPerforming](#areanyperforming)() |
| public | IReadOnlyCollection\<Act\> | [GetAllPerformingActs](#getallperformingacts)() |
| public | IReadOnlyCollection\<Act\> | [GetAllActs](#getallacts)() |


<br/>


## 📖 Act Descriptions

### <a id="tickflags"></a> public enum TickFlags
- `None`: Indicates no ticking should occur.
- `Tick`: Indicates [`MonoBehaviour.Update()`][Unity-Update] should be invoked for the act.
- `PhysicsTick`: Indicates [`MonoBehaviour.FixedUpdate()`][Unity-FixedUpdate] should be invoked for the act.
- `LateTick`: Indicates [`MonoBehaviour.LateUpdate()`][Unity-LateUpdate] should be invoked for the act.


---


### <a id="status"></a> public enum Status
- `None`: Indicates the act is not ongoing.  
- `Initializing`: Indicates the act is initializing i.e. [`Setup()`](#setup) is invoked.  
- `Prologuing`: Indicates the act is waiting on pending prologues to complete.  
- `Entering`: Indicates the act is carrying out its core behaviour.  
- `Ticking`: Indicates the act is ticking within any or all of its [`Tick()`](#tick), [`PhysicsTick()`](#physicstick) or [`LateTick()`](#latetick) methods.
- `Exiting`: Indicates the act's core behaviour has ended. Can only be reached if `Entering` happened first.  
- `Concluding`: (For internal use) Indicates the act is wrapping up after performing regardless of whether `Entering` happened first or not.
- `Deinitializing`: Indicates the act is deinitializing i.e. [`Cleanup()`](#cleanup) is invoked.  


---


### <a id="outcome"></a> public enum Outcome
- `None`: Indicates the act has not performed yet.  
- `Interrupted`: Indicates the act was interrupted externally while performing.  
- `Failure`: Indicates the act failed to complete its core behaviour.  
- `Pending`: Indicates the act is still pending for its core behaviour to complete.  
- `Success`: Indicates the act successfully completed its core behaviour.  
- `Retry`:  Indicates the act is retrying its core behaviour.  


---


### <a id="blocktype"></a> public enum BlockType
- `Interrupt`: Merely interrupts the act (if ongoing) just when the blocker act starts performing.
- `Persistent`: Keeps the act blocked for the entire duration of the blocker act performing.  


---


### <a id="onpresetup"></a> public event Action\<Act act\> OnPreSetup
Invoked just before [`Setup()`](#setup) method is called.


---


### <a id="onpostsetup"></a> public event Action\<Act act\> OnPostSetup
Invoked just after [`Setup()`](#setup) method has been called.


---


### <a id="onpreperformreq"></a> public event Action\<Act act\> OnPrePerformReq
Invoked whenever a perform is requested, before checking if the act can perform.


---


### <a id="onpostperformreq"></a> public event Action\<Act act, bool willPerform\> OnPostPerformReq
Invoked whenever a perform is requested, after checking if the act can perform.  
`willPerform` is `true` if the perform condition is met.


---


### <a id="onperformstartact"></a> public event Action\<Act act\> OnPerformStart
Invoked just as [`Perform()`](#perform) has started, before prologuing.


---


### <a id="onpreprologue"></a> public event Action\<Act act\> OnPrePrologue
Invoked just before prologue acts start performing.  
Will not be invoked if act has no prologues.


---


### <a id="onprologuecomplete"></a> public event Action\<Act act, Act pAct, [Outcome](#outcome) pOutcome\> OnPrologueComplete
Invoked whenever a single prologue act has completed.


---


### <a id="onpostprologue"></a> public event Action\<Act act\> OnPostPrologue
Invoked just after all prologue acts have performed.  
Will not be invoked if act has no prologues or If any of the prologues failed.


---


### <a id="onpreenter"></a> public event Action\<Act act\> OnPreEnter
Invoked just before [`Enter()`](#enter) method is called.


---


### <a id="onpostenter"></a> public event Action\<Act act\> OnPostEnter
Invoked just after [`Enter()`](#enter) method has been called.


---


### <a id="onpretick"></a> public event Action\<Act act\> OnPreTick
Invoked just before [`Tick()`](#tick) method is called.


---


### <a id="onposttick"></a> public event Action\<Act act\> OnPostTick
Invoked just after [`Tick()`](#tick) method has been called.


---


### <a id="onprephysicstick"></a> public event Action\<Act act\> OnPrePhysicsTick
Invoked just before [`PhysicsTick()`](#physicstick) method is called.


---


### <a id="onpostphysicstick"></a> public event Action\<Act act\> OnPostPhysicsTick
Invoked just after [`PhysicsTick()`](#physicstick) method has been called.


---


### <a id="onprelatetick"></a> public event Action\<Act act\> OnPreLateTick
Invoked just before [`LateTick()`](#latetick) method is called.


---


### <a id="onpostlatetick"></a> public event Action\<Act act\> OnPostLateTick
Invoked just after [`LateTick()`](#latetick) method has been called.


---


### <a id="onpreexit"></a> public event Action\<Act act\> OnPreExit
Invoked just before [`Exit()`](#exit) method is called.


---


### <a id="onpostexit"></a> public event Action\<Act act\> OnPostExit
Invoked just after [`Exit()`](#exit) method has been called.


---


### <a id="onperformendact"></a> public event Action\<Act act\> OnPerformEnd
Invoked just as [`Perform()`](#perform) has completely ended.


---


### <a id="onprecleanup"></a> public event Action\<Act act\> OnPreCleanup
Invoked just before [`Cleanup()`](#cleanup) method is called.


---


### <a id="onpostcleanup"></a> public event Action\<Act act\> OnPostCleanup
Invoked just after [`Cleanup()`](#cleanup) method has been called.


---


### <a id="onenablechanged"></a> public event Action\<Act act, bool newIsEnabled\> OnEnableChanged
Invoked whenever the act has been enabled/disabled.


---


### <a id="onblockchanged"></a> public event Action\<Act act, Act blockingAct, [BlockType](#blocktype) blockType, bool didBlock\> OnBlockChanged
Invoked whenever the act has been blocked/unblocked.


---


### <a id="prologue"></a> public Func\<Act, List\<List\<Act\>\>\> prologue  
`Default: null`

If assigned, Then all acts returned by this function will be performed before this main act.  
The return value of the function is a nested list.  
The outer list are all the act groups to be performed in sequence.   
The inner list are act groups which will all be performed in parallel.  
If `null` is anywhere in the return value then the main act will fail.  
If `prologue` itself is `null` then prologuing is skipped.  
If any of the prologue acts fail or are interrupted, then the main act itself will also fail or be interrupted.   

Example 1:
```csharp
myMainAct.prologue += (Act act) => {
    return new() {
        new() { myActA1, myActA2 },
        new() { myActB1, myActB2, myActB3 },
        new() { myActC1 },
    };
};
```

First `A1` & `A2` will enter in parallel,  
After both are completed only then `B1`, `B2` & `B3` will enter,  
After all 3 are completed only then `C1` will enter,  
Then finally `main` will enter.  

Example 2:  
```csharp
myMainAct.prologue = (Act act) => {
    return null;
    // or return new() { null };
    // or return new() { new() { null } };
    // or return new() { new() { myActA1, null } };
};
```
In this case `main` will fail.

Example 3:
```csharp
myMainAct.prologue = (Act act) => {
    return new();
};
// 
// or 
// 
// myMainAct.prologue = null;
```
In this case `main` will directly continue to enter.  


---


### <a id="block"></a> public Func\<Act, Dictionary\<Act, [BlockType](#blocktype)\>\> block
`Default: null`

If assigned, Then all acts returned by this function will be blocked by this act when it starts performing.  
```csharp
void Awake()
{
    theater = GetComponent<Theater>();
    damagedAct.block = (Act act) => {
        return new() { { walkAct, Act.BlockType.Persistent } };  // Walking is blocked while player is taking damage
    };
    damagedAct.Init("Damaged Act", theater);
}
```


---


### <a id="condition"></a> public Func\<Act, bool\> condition
`Default: null`  

Used when overriding [`CanPerform()`](#canperform) isn't sufficient and an additional external condition is required.  
```csharp
void FixedUpdate()
{
    jumpAct.Perform();
}
void Awake()
{
    jumpAct.condition = (Act act) => {
        return Input.GetKeyDown(KeyCode.Space);  // Only jump when spacebar is pressed
    };
    jumpAct.Init("Jump Act", theater);
}
```


---


### <a id="isverbose"></a> public bool isVerbose
`Default: false`  

Controls whether or not to print warnings. Set to `false` to silence them.


---


### <a id="_name"></a> protected string _name
`Default: ""`  

Name of the act, Mainly useful for debugging purposes.


---


### <a id="_canreperform"></a> protected bool _canReperform
`Default: false` 

> **Note:** Should only be assigned inside the [`Setup()`](#setup) method.  

If `true` then calling `Perform()` on an ongoing act will abort the act interruptively and then perform.  
If `false` then current ongoing perform must be completed/aborted manually before calling `Perform()` again.


---


### <a id="_tickflags"></a> protected [TickFlags](#tickflags) _tickFlags
`Default: TickFlags.None`  

> **Note:** Should only be assigned inside the [`Setup()`](#setup) method.  

Determines which tick methods are to be called. Look into [`Enter()`](#enter) & [`TickFlags`](#tickflags) to learn more.


---


### <a id="init"></a> public void Init(string newName = "", Theater newTheater = null)
This method is used to initialize the act. Generally this will be called in [`MonoBehaviour.Awake()`][Unity-Awake] or [`MonoBehaviour.Start()`][Unity-Start] though it can be used elsewhere if required.  
```csharp
void Awake()
{
    theater = GetComponent<Theater>();
    myAct.OnPostEnter += (Act act) =>
    {
        Debug.Log("Before Entering");
    };
    myAct.prologue += (Act act) =>
    {
        return new() { someAct };
    };
    myAct.myVar = 10;
    myAct.Init("My Act", theater);
}
```
i.e. You should ideally set all Events, Prologues, Blocks, Conditions, Onetime Properties, etc before you call `Init()`.  
Calling `Init()` will internally call your overridden `Setup()` method.


---


### <a id="deinit"></a> public void Deinit()
This method is used to deinitialize the act & it must be called before the act is destroyed.  
Generally this will be called in [`MonoBehaviour.OnDestroy()`][Unity-OnDestroy].  
```csharp
void OnDestroy() 
{
    myAct.Deinit();
}
```
Calling `Deinit()` will abort the act if it was performing, then internally call your overridden `Cleanup()` method.  
It also resets [`condition`](#condition), [`prologue`](#prologue) & [`block`](#block) to `null` however events remain unaffected.


---


### <a id="perform"></a> public bool Perform()
Call this method when you want your defined act behaviour to run. This will start the perform lifecycle of the act. Returns `false` if act could not perform.  
```csharp
void FixedUpdate()
{
    moveAct.direction = getDirection();
    moveAct.Perform();
}
```


---


### <a id="retry"></a> public void Retry()
If the act is performing then this function will finish the act with [Outcome.Retry](#outcome) which will cause the act to reperform. 
If the act is not performing this will simply call [`Perform()`](#perform).   


---


### <a id="abort"></a> public void Abort()
This will finish the act if its performing with [Outcome.Interrupted](#outcome).  
Won't do anything if the act was not performing.


---


### <a id="performdeferred"></a> public void PerformDeferred([TickFlags](#tickflags) tickFlag = TickFlags.PhysicsTick)
> **Note:** Requires a `Theater` to be assigned.

This will delay off the [`Perform()`](#perform) until the next designated tick. Useful to avoid infinite recursion when trying to reperform an act.


---


### <a id="retrydeferred"></a> public void RetryDeferred([TickFlags](#tickflags) tickFlag = TickFlags.PhysicsTick)
> **Note:** Requires a `Theater` to be assigned.

This will delay off the [`Retry()`](#retry) until the next designated tick.


---


### <a id="abortdeferred"></a> public void AbortDeferred([TickFlags](#tickflags) tickFlag = TickFlags.PhysicsTick)
> **Note:** Requires a `Theater` to be assigned.

This will delay off the [`Abort()`](#abort) until the next designated tick.



---


### <a id="cancelperformdeferred"></a> public void CancelPerformDeferred()
> **Note:** Requires a `Theater` to be assigned.

This will cancel the requested [`PerformDeferred()`](#performdeferred) if not already performed yet.  


---


### <a id="cancelretrydeferred"></a> public void CancelRetryDeferred()
> **Note:** Requires a `Theater` to be assigned.

This will cancel the requested [`RetryDeferred()`](#retrydeferred) if not already retried yet.  


---


### <a id="cancelabortdeferred"></a> public void CancelAbortDeferred()
> **Note:** Requires a `Theater` to be assigned.

This will cancel the requested [`AbortDeferred()`](#abortdeferred) if not already aborted yet.  


---


### <a id="setenabled"></a> public void SetEnabled(bool newEnabled)
Disables/Enables the act i.e. If an act is disabled then it can no longer [`Perform()`](#perform) and any act that was ongoing will be interrupted.
```csharp
myAct.SetEnabled(false);  // Disable act
myAct.SetEnabled(true);  // Enable act
```


---


### <a id="isperforming"></a> public bool IsPerforming()
Returns `true` if the act is currently performing.


---


### <a id="isongoing"></a> public bool IsOngoing()
Returns `true` if the act is currently performing but is past prologuing i.e. is carrying out the core behaviour.


---


### <a id="isretrying"></a> public bool IsRetrying()
Returns `true` if the act is currently performing due to a retry.


---


### <a id="isenabled"></a> public bool IsEnabled()
Returns `true` if the act is currently enabled.


---


### <a id="isblocked"></a> public bool IsBlocked()
Returns `true` if the act is currently blocked by 1 or more other acts.


---


### <a id="isblockedby"></a> public bool IsBlockedBy(Act byAct)
Checks if the act is currently blocked by given act.  


---


### <a id="hasinitialized"></a> public bool HasInitialized()
Returns `true` if the act has been [initialized](#init). Resets to `false` once [deinitialized](#deinit)


---


### <a id="cantick"></a> public bool CanTick([TickFlags](#tickflags) type = TickFlags.Tick | TickFlags.PhysicsTick | TickFlags.LateTick)
Returns `true` if the act can tick on the given flag type(s).


---


### <a id="getname"></a> public string GetName()
Returns the name of the act as passed to [`Init()`](#init).  
Mainly useful for debugging purposes.


---


### <a id="getstatus"></a> public [Status](#status) GetStatus()
Returns the current [Status](#status) of the act.


---


### <a id="getprevstatus"></a> public [Status](#status) GetPrevStatus()
Returns the previous [Status](#status) the act was in before the current one.


---


### <a id="getoutcome"></a> public [Outcome](#outcome) GetOutcome()
Returns the outcome of the last perform of the act.  


---


### <a id="gettickflags"></a> public [TickFlags](#tickflags) GetTickFlags()
Returns the assigned [`_tickFlags`](#_tickflags)


---


### <a id="getblockedbyacts"></a> public IReadOnlyCollection\<Act\> GetBlockedByActs()
Returns all the acts currently blocking this act.


---


### <a id="getactstoblock"></a> public IReadOnlyDictionary\<Act, [BlockType](#blocktype)\> GetActsToBlock()
Returns all the acts this act is currently blocking along with their [BlockType](#blocktype).  


---


### <a id="getperformcount"></a> public int GetPerformCount()
Returns how many times the act has performed since [initialized](#init). Resets once [deinitialized](#deinit).


---


### <a id="gettickcount"></a> public int GetTickCount()
Returns how many times [`Tick()`](#tick) has been invoked since [initialized](#init). Resets once [deinitialized](#deinit).


---


### <a id="getphysicstickcount"></a> public int GetPhysicsTickCount()
Returns how many times [`PhysicsTick()`](#physicstick) has been invoked since [initialized](#init). Resets once [deinitialized](#deinit).


---


### <a id="getlatetickcount"></a> public int GetLateTickCount()
Returns how many times [`LateTick()`](#latetick) has been invoked since [initialized](#init). Resets once [deinitialized](#deinit).


---


### <a id="getlifetimeperformcount"></a> public int GetLifetimePerformCount()
Returns how many times the act has performed since its existance. Never resets.


---


### <a id="getlifetimetickcount"></a> public int GetLifetimeTickCount()
Returns how many times [`Tick()`](#tick) has been invoked since the act existed. Never resets.



---


### <a id="getlifetimephysicstickcount"></a> public int GetLifetimePhysicsTickCount()
Returns how many times [`PhysicsTick()`](#physicstick) has been invoked since the act existed. Never resets.


---


### <a id="getlifetimelatetickcount"></a> public int GetLifetimeLateTickCount()
Returns how many times [`LateTick()`](#latetick) has been invoked since the act existed. Never resets.


---


### <a id="gettheater"></a> public Theater GetTheater()
> **Note:** Requires a `Theater` to be assigned.

Returns the `Theater` the act belongs to. returns null if theater not assigned.


---


### <a id="setup"></a> protected virtual void Setup()
> **Note:** This method is only meant to be overridden never invoked, Except when using `base.Setup()`.

This method is meant to be overridden and should contain your initialization logic inside it.
```csharp
[Serializable]
public class MyAct : Act
{
    Rigidbody2D rigidBody2D;

	protected override void Setup(){
        _canReperform = true;
        _tickFlags = TickFlags.Tick;
        rigidBody2D = GetTheater().GetComponent<Rigidbody2D>();
        // etc etc
    }
}
```


---


### <a id="canperform"></a> protected virtual bool CanPerform()
> **Note:** This method is only meant to be overridden never invoked, Except when using `base.CanPerform()`.

This method is meant to be overridden and should contain conditions on whether or not `Perform()` can be called.
```csharp
[Serializable]
public class RunAct : Act
{
	protected override bool CanPerform(){
        return isOnGround();  // Cannot run if not on ground
    }
}
```


---


### <a id="enter"></a> protected virtual [Outcome](#outcome) Enter()

> **Note:** This method is only meant to be overridden never invoked, Except when using `base.Enter()`.

This method is meant to be overridden and should contain the core behaviour of the act.  
The return value dictates the outcome of the act. Possible return values are:  
- `Outcome.Failure`  
- `Outcome.Pending`  
- `Outcome.Success`  
- `Outcome.Retry`   
Do not return `Outcome.Interrupted` that is reserved for external cancellation.

```csharp
[Serializable]
public class RunAct : Act
{
	protected override Outcome Enter(){
        bool didMove;

        // Run logic here...

        return didMove ? Outcome.Success : Outcome.Failure;
    }
}
```

If you want to use any of the tick methods [`Tick()`](#tick), [`PhysicsTick()`](#physicstick), [`LateTick()`](#latetick) you must:
1. Assign [`_tickFlags`](#_tickflags) with something other than [`TickFlags.None`](#tickflags).
1. Return [`Outcome.Pending`](#outcome) in `Enter()`, returning anything else will lead to [`Exit()`](#exit).

```csharp
[Serializable]
public class GotoAct : Act
{
	protected override void Setup(){
        _tickFlags = TickFlags.PhysicsTick;
    }

	protected override Outcome Enter(){
        
        if(AtDestination()){  // Return as success if already at destination
            return Outcome.Success;
        }

        return Outcome.Pending;
    }
	
    protected override Outcome PhysicsTick(){

        // Move logic here...

        // Returning Pending continues ticking, returning anything else makes the act proceed into Exit()
        return ReachedDestination() ? Outcome.Success : Outcome.Pending;  
    }
}
```

If `Outcome.Pending` is returned without the intent of ticking, [`Finish()`](#finish) must be called so the act can proceed to [`Exit()`](#exit).  
```csharp
[Serializable]
public class EmoteAct : Act
{
	protected override Outcome Enter(){
        
        PlayAnimation();

        OnAnimationEnded += (bool didPlay) => {
            Finish(didPlay ? Outcome.Success : Outcome.Failure);
        };

        return Outcome.Pending;
    }
}
```
Return `Outcome.Retry` if you want the act to perform again without continuing with epilogue acts first.


---


### <a id="tick"></a> protected virtual [Outcome](#outcome) Tick()
> **Note:** This method is only meant to be overridden never invoked, Except when using `base.Tick()`.  
> Also requires a `Theater` to be assigned.

This method is meant to be overridden and should contain the visual frame ticking logic of the act.  
Look into [`Enter()`](#enter) to understand how the return value works.


---


### <a id="physicstick"></a> protected virtual [Outcome](#outcome) PhysicsTick()
> **Note:** This method is only meant to be overridden never invoked, Except when using `base.PhysicsTick()`.  
> Also requires a `Theater` to be assigned.

This method is meant to be overridden and should contain the physics frame ticking logic of the act.  
Look into [`Enter()`](#enter) to understand how the return value works.


---


### <a id="latetick"></a> protected virtual [Outcome](#outcome) LateTick()
> **Note:** This method is only meant to be overridden never invoked, Except when using `base.LateTick()`.  
> Also requires a `Theater` to be assigned.

This method is meant to be overridden and should contain the late frame ticking logic of the act.  
Look into [`Enter()`](#enter) to understand how the return value works.


---


### <a id="exit"></a> protected virtual void Exit()
> **Note:** This method is only meant to be overridden never invoked, Except when using `base.Exit()`.

This method is meant to be overridden and should contain the finalization logic after [Entering](#enter).  
```csharp
[Serializable]
public class MoveAct : Act
{
    public Vector2 direction = new();

    protected override Outcome Enter() { ... }

    protected override void Exit()
    {
        direction = Vector2.zero;
    }
}
```


---


### <a id="cleanup"></a> protected virtual void Cleanup()
> **Note:** This method is only meant to be overridden never invoked, Except when using `base.Cleanup()`.

This method is meant to be overridden and should contain your deinitialization logic inside it.
```csharp
[Serializable]
public class MyAct : Act
{
    Rigidbody2D rigidBody2D;

	protected override void Cleanup(){
        rigidBody2D = null;
        // etc etc
    }
}
```


---


### <a id="finish"></a> protected void Finish([Outcome](#outcome) newOutcome = Outcome.Success)
This method is only meant to be invoked in [`Enter()`](#enter) or any of the ticking methods. It should not be overridden.  


---


### <a id="blockself"></a> protected virtual void BlockSelf(Act byAct, [BlockType](#blocktype) blockType)
This method is used internally, Only kept incase some special functionality needs to be hooked when act is being blocked. 
```csharp
[Serializable]
public class MyAct : Act
{
	protected override void BlockSelf(Act byAct, BlockType blockType) {

        base.BlockSelf(byAct, blockType);

        // Custom functionality
    }
}
```


---


### <a id="unblockself"></a> protected virtual void UnblockSelf(Act byAct)
This method is used internally, Only kept incase some special functionality needs to be hooked when act is being unblocked. 
```csharp
[Serializable]
public class MyAct : Act
{
	protected override void UnblockSelf(Act byAct) {

        base.UnblockSelf(byAct);

        // Custom functionality
    }
}
```


---


### <a id="blockothers"></a> protected virtual void BlockOthers()
This method is used internally, Only kept incase some special functionality needs to be hooked when act is blocking others. 
```csharp
[Serializable]
public class MyAct : Act
{
    protected override void BlockOthers() {

        base.BlockOthers();

        // Custom functionality
    }
}
```


---


### <a id="unblockothers"></a> protected virtual void UnblockOthers()
This method is used internally, Only kept incase some special functionality needs to be hooked when act is unblocking others. 
```csharp
[Serializable]
public class MyAct : Act
{
    protected override void UnblockOthers() {

        base.UnblockOthers();

        // Custom functionality
    }
}
```


---


### <a id="writelog"></a> protected virtual void WriteLog(string message)
Used internally to log warning messages when [isVerbose](#isverbose) is `true`. Only kept incase some special functionality needs to be hooked when a warning is logged.


---


<br/>


## 📖 Theater Descriptions

### <a id="onenablechangedtheater"></a> public event Action\<Theater theater, bool newEnabled\> OnEnableChanged
Invoked whenever theater has been enabled/disabled.  
Also invoked when the `Theater` component or its `GameObject` is enabled/disabled.


---


### <a id="onactstatuschange"></a> public event Action\<Theater theater, Act act\> OnActStatusChange
Invoked whenever any act associated with the theater has its status changed.


---


### <a id="performall"></a> public void PerformAll()
Performs all acts associated with the theater.


---


### <a id="retryall"></a> public void RetryAll()
Retries all acts associated with the theater.


---


### <a id="abortall"></a> public void AbortAll()
Aborts all acts associated with the theater.


---


### <a id="setenabledtheater"></a> public void SetEnabled(bool newEnabled)
Disables/Enables theater i.e. If a theater is disabled then all acts assigned to it can no longer [`Perform()`](#perform), any act that was ongoing will be interrupted and any pending deferred calls will be cancelled.
```csharp
theater.SetEnabled(false);  // Disable theater
theater.SetEnabled(true);  // Enable theater
```


---


### <a id="hasact"></a> public bool HasAct(Act act)
Checks if the given act is associated with the theater.  


---


### <a id="isenabledtheater"></a> public bool IsEnabled()
Returns `true` if theater is currently enabled.


---


### <a id="areanyperforming"></a> public bool AreAnyPerforming()
Checks if any acts associated with the theater are currently performing.  


---


### <a id="getallperformingacts"></a> public IReadOnlyCollection\<Act\> GetAllPerformingActs()
Returns all acts associated with the theater that are currently performing.  


---


### <a id="getallacts"></a> public IReadOnlyCollection\<Act\> GetAllActs()
Returns all the acts assigned to the theater.

[Unity-Awake]: https://docs.unity3d.com/ScriptReference/MonoBehaviour.Awake.html
[Unity-Start]: https://docs.unity3d.com/ScriptReference/MonoBehaviour.Start.html
[Unity-Update]: https://docs.unity3d.com/ScriptReference/MonoBehaviour.Update.html
[Unity-FixedUpdate]: https://docs.unity3d.com/ScriptReference/MonoBehaviour.FixedUpdate.html
[Unity-LateUpdate]: https://docs.unity3d.com/ScriptReference/MonoBehaviour.LateUpdate.html
[Unity-OnDestroy]: https://docs.unity3d.com/ScriptReference/MonoBehaviour.OnDestroy.html
