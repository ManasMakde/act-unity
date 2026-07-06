# 🎭 act-unity
This is the unity implementation of the **Act Pattern**  
For a complete explaination & implementation in other game engines visit the [main repository](https://github.com/ManasMakde/act)

> **Note**:  
> If you just want the `Act` & `Theater` class files look in the following paths:  
> 1. [`game/Assets/Script/ActSystem/Act.cs`](game/Assets/Script/ActSystem/Act.cs)  
> 1. [`game/Assets/Script/ActSystem/Theater.cs`](game/Assets/Script/ActSystem/Theater.cs)  



## ⚙️ Act Class

| Enums    | Constants |
|---------|----------|
| [TickFlags](#TickFlags) | `None`, `Tick`, `PhysicsTick`, `LateTick` |
| [Status](#Status) | `None`, `Prologuing`, `Entering`, `Ticking`, `Exiting` |
| [Outcome](#Outcome) | `Interrupted`, `Failure`, `Pending`, `Success`, `Retry` |
| [BlockType](#BlockType) | `Oneshot`, `Persistent` |


| Signature    | Events |
|--------------|-------|
| \<Act act\> | [OnPreSetup](#OnPreSetup) |
| \<Act act\> | [OnPostSetup](#OnPostSetup) |
| \<Act act\> | [OnPrePrologue](#OnPrePrologue) |
| \<Act act\> | [OnPostPrologue](#OnPostPrologue) |
| \<Act act\> | [OnPreEnter](#OnPreEnter) |
| \<Act act\> | [OnPostEnter](#OnPostEnter) |
| \<Act act\> | [OnPreTick](#OnPreTick) |
| \<Act act\> | [OnPostTick](#OnPostTick) |
| \<Act act\> | [OnPrePhysicsTick](#OnPrePhysicsTick) |
| \<Act act\> | [OnPostPhysicsTick](#OnPostPhysicsTick) |
| \<Act act\> | [OnPreLateTick](#OnPreLateTick) |
| \<Act act\> | [OnPostLateTick](#OnPostLateTick) |
| \<Act act\> | [OnPreExit](#OnPreExit) |
| \<Act act\> | [OnPostExit](#OnPostExit) |
| \<Act act\> | [OnPreCleanup](#OnPreCleanup) |
| \<Act act\> | [OnPostCleanup](#OnPostCleanup) |
| \<Act act,<br> bool newIsEnabled\> | [OnEnableChanged](#OnEnableChanged) |
| \<Act act,<br> Act blockingAct,<br> [BlockType](#BlockType) blockType,<br>bool didBlock\> | [OnBlockChanged](#OnBlockChanged) |


| Access | Type | Methods |
|--------|------|--------------|
| public | void | [Init](#Init)(Theater theater, string name, bool initiallyEnabled) |
| public | void | [Deinit](#Deinit)() |
| public | void | [Perform](#Perform)() |
| public | void | [PerformDeferred](#PerformDeferred)([TickFlags](#TickFlags) tickFlag) |
| public | void | [Retry](#Retry)() |
| public | void | [Abort](#Abort)() |
| public | void | [AddToBlock](#AddToBlock)(List\<Act\> acts, [BlockType](#BlockType) blockType) |
| public | void | [RemoveFromBlock](#RemoveFromBlock)(List\<Act\> acts) |
| public | void | [SetEnabled](#SetEnabled)(bool newEnabled) |
| public | bool | [DidPerform](#DidPerform)([TickFlags](#TickFlags)) |
| public | bool | [DidPerformEver](#DidPerformEver)() |
| public | bool | [IsOngoing](#IsOngoing)() |
| public | bool | [IsEnabled](#IsEnabled)() |
| public | bool | [IsBlocked](#IsBlocked)() |
| public | bool | [DidEnter](#DidEnter)() |
| public | bool | [CanTick](#CanTick)([TickFlags](#TickFlags) type) |
| public | [Outcome](#Outcome) | [GetOutcome](#GetOutcome)() |
| public | Theater | [GetTheater](#GetTheater)() |
| public | GameObject | [GetOwner](#GetOwner)() |
| public static | float | [GetDelta](#GetDelta)() |
| public static | float | [GetPhysicsDelta](#GetPhysicsDelta)() |
| public | string | [GetName](#GetName)() |
| public static | List\<Act\> | [Seq](#Seq)(List\<List\<Act\>\> pArrays) |
| protected virtual | void | [Setup](#Setup)() <abbr title="">Virtual</abbr> |
| protected virtual | bool | [CanPerform](#CanPerform)() <abbr title="">Virtual</abbr> |
| protected virtual | [Outcome](#Outcome) | [Enter](#Enter)() <abbr title="">Virtual</abbr> |
| protected virtual | [Outcome](#Outcome) | [Tick](#Tick)() <abbr title="">Virtual</abbr> |
| protected virtual | [Outcome](#Outcome) | [PhysicsTick](#PhysicsTick)() <abbr title="">Virtual</abbr> |
| protected virtual | [Outcome](#Outcome) | [LateTick](#LateTick)() <abbr title="">Virtual</abbr> |
| protected virtual | void | [Exit](#Exit)() <abbr title="">Virtual</abbr> |
| protected virtual | void | [Cleanup](#Cleanup)() <abbr title="">Virtual</abbr> |
| protected | void | [Finish](#Finish)([Outcome](#Outcome) newOutcome) |
| protected virtual | void | [BlockSelf](#BlockSelf)(Act byAct, [BlockType](#BlockType) blockType) <abbr title="">Virtual</abbr> |
| protected virtual | void | [UnblockSelf](#UnblockSelf)(Act byAct) <abbr title="">Virtual</abbr> |
| protected virtual | void | [BlockOthers](#BlockOthers)() <abbr title="">Virtual</abbr> |
| protected virtual | void | [UnblockOthers](#UnblockOthers)() <abbr title="">Virtual</abbr> |


| Access | Type | Properties |
|--------|------|--------------|
| public | Func\<Act, List\<Act\>\> | [Prologue](#Prologue) |
| public | List\<Func\<Act, bool\>\> | [PerformConditions](#PerformConditions) |
| protected | bool | [_canReperform](#_canReperform) |
| protected | [TickFlags](#TickFlags) | [_tickFlags](#_tickFlags) |


<br/>


## ⚙️ Theater Class

| Signature    | Events |
|--------------|-------|
| \<Theater theater, bool newEnabled\> | [OnEnableChanged](#OnEnableChangedTheater) |
| \<Theater theater, Act act\> | [OnPerformStart](#OnPerformStart) |
| \<Theater theater, Act act\> | [OnPerformEnd](#OnPerformEnd) |
| \<Theater theater\> | [OnAllPerformEnd](#OnAllPerformEnd) |


| Access | Type | Methods |
|--------|------|--------------|
| public | bool | [IsEnabled](#IsEnabledTheater)() |
| public | void | [SetEnabled](#SetEnabledTheater)(bool newEnabled) |
| public | void | [AbortAll](#AbortAll)() |
| public | bool | [AreAnyOngoing](#AreAnyOngoing)() |
| public | HashSet\<Act\> | [GetAllActs](#GetAllActs)() |


<br/>


## 📖 Act Descriptions

### <a id="TickFlags"></a> public enum TickFlags
- `None`: Indicates no ticking should occur.
- `Tick`: Indicates [`MonoBehaviour.Update()`][Unity-Update] should be invoked for the act.
- `PhysicsTick`: Indicates [`MonoBehaviour.FixedUpdate()`][Unity-FixedUpdate] should be invoked for the act.
- `LateTick`: Indicates [`MonoBehaviour.LateUpdate()`][Unity-LateUpdate] should be invoked for the act.


---


### <a id="Status"></a> public enum Status
- `None`: Indicates the act is not ongoing.  
- `Prologuing`: Indicates the act is waiting on pending prologues to complete.
- `Entering`: Indicates the act is carrying out it's core behaviour.  
- `Ticking`: Indicates the act is ticking within any or all of it's [Tick](#Tick)(), [PhysicsTick](#PhysicsTick)() or [LateTick](#LateTick)() methods.
- `Exiting`: Indicates the act perform has ended and is now finalizing.  


---


### <a id="Outcome"></a> public enum Outcome
- `Interrupted`: Indicates the act was interrupted externally while performing.  
- `Failure`: Indicates the act failed to complete it's core behaviour.  
- `Pending`: Indicates the act is still pending for it's core behaviour to complete which might also indicate ticking if [_tickFlags](#_tickFlags) is assigned.  
- `Success`: Indicates the act successfully completed it's core behaviour.  
- `Retry`:  Indicates the act is retrying it's core behaviour.  


---


### <a id="BlockType"></a> public enum BlockType
- `Oneshot`: Merely interrupts the act (if ongoing) when the blocker act starts performing.
- `Persistent`: Keeps the act blocked for the entire duration of the blocker act performing.  


---


### <a id="OnPreSetup"></a> public event Action\<Act act\> OnPreSetup
Invoked just before [Setup](#Setup)() method is called.


---


### <a id="OnPostSetup"></a> public event Action\<Act act\> OnPostSetup
Invoked just after [Setup](#Setup)() method has been called.


---


### <a id="OnPrePrologue"></a> public event Action\<Act act\> OnPrePrologue
Invoked just before prologue acts start performing.  
Will not be invoked if act has no prologues.


---


### <a id="OnPostPrologue"></a> public event Action\<Act act\> OnPostPrologue
Invoked just after all prologue acts have performed.  
Will not be invoked if act has no prologues or If any of the prologues failed.


---


### <a id="OnPreEnter"></a> public event Action\<Act act\> OnPreEnter
Invoked just before [Enter](#Enter)() method is called.


---


### <a id="OnPostEnter"></a> public event Action\<Act act\> OnPostEnter
Invoked just after [Enter](#Enter)() method has been called.


---


### <a id="OnPreTick"></a> public event Action\<Act act\> OnPreTick
Invoked just before [Tick](#Tick)() method is called.


---


### <a id="OnPostTick"></a> public event Action\<Act act\> OnPostTick
Invoked just after [Tick](#Tick)() method has been called.


---


### <a id="OnPrePhysicsTick"></a> public event Action\<Act act\> OnPrePhysicsTick
Invoked just before [PhysicsTick](#PhysicsTick)() method is called.


---


### <a id="OnPostPhysicsTick"></a> public event Action\<Act act\> OnPostPhysicsTick
Invoked just after [PhysicsTick](#PhysicsTick)() method has been called.


---


### <a id="OnPreLateTick"></a> public event Action\<Act act\> OnPreLateTick
Invoked just before [LateTick](#LateTick)() method is called.


---


### <a id="OnPostLateTick"></a> public event Action\<Act act\> OnPostLateTick
Invoked just after [LateTick](#LateTick)() method has been called.


---


### <a id="OnPreExit"></a> public event Action\<Act act\> OnPreExit
Invoked just before [Exit](#Exit)() method is called.


---


### <a id="OnPostExit"></a> public event Action\<Act act\> OnPostExit
Invoked just after [Exit](#Exit)() method has been called.


---


### <a id="OnPreCleanup"></a> public event Action\<Act act\> OnPreCleanup
Invoked just before [Cleanup](#Cleanup)() method is called.


---


### <a id="OnPostCleanup"></a> public event Action\<Act act\> OnPostCleanup
Invoked just after [Cleanup](#Cleanup)() method has been called.


---


### <a id="OnEnableChanged"></a> public event Action\<Act act, bool newIsEnabled\> OnEnableChanged
Invoked whenever the act has been enabled/disabled.


---


### <a id="OnBlockChanged"></a> public event Action\<Act act, Act blockingAct, [BlockType](#BlockType) blockType, bool didBlock\> OnBlockChanged
Invoked whenever the act has been blocked/unblocked.


---


### <a id="Init"></a> public void Init(Theater theater, string name = "", bool initiallyEnabled = true)
This method is used to initialize the act & it must be called once before you can call [`Perform()`](#Perform).  
Generally this will be called in [`MonoBehaviour.Awake()`][Unity-Awake] or [`MonoBehaviour.Start()`][Unity-Start] though it can be used elsewhere if required.  
```csharp
void Awake()
{
    theater = GetComponent<Theater>();
    myAct.OnPostEnter += (Act act) =>
    {
        Debug.Log("Before Entering");
    };
    myAct.Prologue += (Act act) =>
    {
        return new() { someAct }
    };
    myAct.myVar = 10;
    myAct.Init(theater, "My Act");
}
```
i.e. You should ideally set all Events, Prologues, Onetime Properties, etc before you call `Init()`.  
Calling `Init()` will internally call your overridden `Setup()` method.


---


### <a id="Deinit"></a> public void Deinit()
This method is used to deinitialize the act & it must be called before the act is destroyed.  
After calling this method [`Perform()`](#Perform) cannot be called unless you intialize again.  
Generally this will be called in [`MonoBehaviour.OnDestroy()`][Unity-OnDestroy].  
```csharp
void OnDestroy() 
{
    myAct.Deinit();
}
```
Calling `Deinit()` will internally call your overridden `Cleanup()` method.


---


### <a id="Perform"></a> public void Perform()
Call this method when you want your defined act behaviour to run. This will start the perform lifecycle of the act.  
```csharp
void FixedUpdate()
{
    moveAct.direction = getDirection();
    moveAct.Perform();
}
```


---


### <a id="PerformDeferred"></a> public void PerformDeferred([TickFlags](#TickFlags) tickFlag = TickFlags.PhysicsTick)
This will delay off the [`Perform()`](#Perform) until the next tick. Useful to avoid infinite recursion when trying to reperform an act.


---


### <a id="Retry"></a> public void Retry()
If the act is performing then this function will finish the act with [Outcome.Retry](#Outcome) which will cause the act to reperform.   
If the act is not performing this will simply call [`Perform()`](#Perform).   


---


### <a id="Abort"></a> public void Abort()
This will finish the act if it's performing with [Outcome.Interrupted](#Outcome).  
Won't do anything if the act was not performing.


---


### <a id="AddToBlock"></a> public void AddToBlock(List\<Act\> acts, [BlockType](#BlockType) blockType = BlockType.Persistent)
Stores which other acts to block while performing.  
```csharp
void Awake()
{
    theater = GetComponent<Theater>();
    damagedAct.AddToBlock(new() { walkAct });  // Walking is blocked while player is taking damage
    damagedAct.Init(theater, "Damaged Act");
}
```

Also look into [BlockType](#BlockType).


---


### <a id="RemoveFromBlock"></a> public void RemoveFromBlock(List\<Act\> acts)
Removes given acts from being blocked.


---


### <a id="SetEnabled"></a> public void SetEnabled(bool newEnabled)
Disables/Enables the act i.e. If an act is disabled then it can no longer [`Perform()`](#Perform) and any act that was ongoing will be interrupted.
```csharp
myAct.SetEnabled(false);  // Disable act
myAct.SetEnabled(true);  // Enable act
```


---


### <a id="DidPerform"></a> public bool DidPerform([TickFlags](#TickFlags) tickFlag = TickFlags.PhysicsTick)
Returns `true` if the act has performed atleast once in the span of the current tick.  
```csharp
void FixedUpdate()
{
    Debug.Log(myAct.DidPerform(TickFlags.PhysicsTick));  // false
    myAct.Perform();
    Debug.Log(myAct.DidPerform(TickFlags.PhysicsTick));  // true
}
```


---


### <a id="DidPerformEver"></a> public bool DidPerformEver()
Returns `true` if the act has performed even once since it was [initialized](#Init). Resets after act ha been [deinitialized](#Deinit).
```csharp
Debug.Log(myAct.DidPerformEver());  // false

myAct.Init();
Debug.Log(myAct.DidPerformEver());  // false

myAct.Perform();
Debug.Log(myAct.DidPerformEver());  // true

myAct.Deinit();
Debug.Log(myAct.DidPerformEver());  // false
```


---


### <a id="IsOngoing"></a> public bool IsOngoing()
Returns `true` if the act is currently performing.


---


### <a id="IsEnabled"></a> public bool IsEnabled()
Returns `true` if the act is currently enabled.


---


### <a id="IsBlocked"></a> public bool IsBlocked()
Returns `true` if the act is currently blocked by 1 or more other acts.


---


### <a id="DidEnter"></a> public bool DidEnter()
Returns `true` if the act has gone through [`Enter()`](#Enter) while performing.  
Useful for determining if the act reached [`Exit()`](#Exit) via enter/tick or via failed prologue in it's [lifecycle][Act-Lifecycle]  
```csharp
myAct1.Prologue += (Act act) => { return new() { null } };
myAct1.OnPostExit += (Act act) => {
    Debug.Log(myAct1.DidEnter());  // False
}
myAct1.Perform();


myAct2.OnPostExit += (Act act) => {
    Debug.Log(myAct2.DidEnter());  // True
}
myAct2.Perform();
```


---


### <a id="CanTick"></a> public bool CanTick([TickFlags](#TickFlags) type)
Returns `true` if the act can tick on the given flag type(s).


---


### <a id="GetOutcome"></a> public [Outcome](#Outcome) GetOutcome()
Returns the outcome of [`Enter()`](#Enter) or any of the tick methods.  
However this is only to be used inside the lifecycle methods since [`Exit()`](#Exit) will internally reset the flag.


---


### <a id="GetTheater"></a> public Theater GetTheater()
Returns the `Theater` the act belongs to.


---


### <a id="GetOwner"></a> public GameObject GetOwner()
Returns the [gameObject][Unity-GameObject] the `Theater` is attached to.


---


### <a id="GetDelta"></a> public static float GetDelta()
Returns [`Time.deltaTime`][Unity-DeltaTime]  
(Kept for consistency sake)


---


### <a id="GetPhysicsDelta"></a> public static float GetPhysicsDelta()
Returns [`Time.fixedDeltaTime`][Unity-FixedDeltaTime]  
(Kept for consistency sake)


---


### <a id="GetName"></a> public string GetName()
Returns the name of the act as passed to [`Init()`](#Init).  
Mainly useful for debugging purposes.


---


### <a id="Seq"></a> public static List\<Act\> Seq(List\<List\<Act\>\> pArrays)
This method is to be used **only** inside [Prologue](#Prologue), It allows you to call prologue acts in sequence.  
```csharp
myAct.Prologue += (Act act) => {
    return Act.Seq(new() {
        new() { myActA1 },
        new() { myActB1, myActB2 },
        new() { myActC1 },
    });
}
```
In the above example `myAct1` will perform first,  
then `myActB1` & `myActB2` will perform in parallel,  
then `myActC1` will perform last.  
And then after all prologue acts are complete would `myAct` be performed.  

This is how to do it without using `Seq()`:
```csharp
myAct.Prologue += (Act act) => {
    return new() { myActC1 };
}

myActC1.Prologue += (Act act) => {
    return new() { myActB1, myActB2 };
}

myActB1.Prologue += (Act act) => {
    return new() { myActA1 };
}

myActB2.Prologue += (Act act) => {
    return new() { myActA1 };
}
```


---


### <a id="Setup"></a> protected virtual void Setup()
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
        rigidBody2D = GetOwner().GetComponent<Rigidbody2D>();
        // etc etc
    }
}
```


---


### <a id="CanPerform"></a> protected virtual bool CanPerform()
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


### <a id="Enter"></a> protected virtual [Outcome](#Outcome) Enter()

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

If you want to use any of the tick methods [`Tick()`](#Tick), [`PhysicsTick()`](#PhysicsTick), [`LateTick()`](#LateTick) you must:
1. Assign [`_tickFlags`](#_tickFlags) with something other than [`TickFlags.None`](#TickFlags).
1. Return [`Outcome.Pending`](#Outcome) in `Enter()`, returning anything else will lead to [`Exit()`](#Exit).

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

        // Returning Pending continues ticking, returning anything else makes the act proceeed into Exit()
        return ReachedDestination() ? Outcome.Success : Outcome.Pending;  
    }
}
```

If `Outcome.Pending` is returned without the intent of ticking, [`Finish()`](#Finish) must be called so the act can proceed to [`Exit()`](#Exit).  
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


### <a id="Tick"></a> protected virtual [Outcome](#Outcome) Tick()
> **Note:** This method is only meant to be overridden never invoked, Except when using `base.Tick()`.

This method is meant to be overridden and should contain the visual frame ticking logic of the act.  
Look into [`Enter()`](#Enter) to understand how the return value works.


---


### <a id="PhysicsTick"></a> protected virtual [Outcome](#Outcome) PhysicsTick()
> **Note:** This method is only meant to be overridden never invoked, Except when using `base.PhysicsTick()`.

This method is meant to be overridden and should contain the physics frame ticking logic of the act.  
Look into [`Enter()`](#Enter) to understand how the return value works.


---


### <a id="LateTick"></a> protected virtual [Outcome](#Outcome) LateTick()
> **Note:** This method is only meant to be overridden never invoked, Except when using `base.LateTick()`.

This method is meant to be overridden and should contain the late frame ticking logic of the act.  
Look into [`Enter()`](#Enter) to understand how the return value works.


---


### <a id="Exit"></a> protected virtual void Exit()
> **Note:** This method is only meant to be overridden never invoked, Except when using `base.Exit()`.

This method is meant to be overridden and should contain the finialization logic after [Entering](#Enter).  
```csharp
[Serializable]
public class MoveAct : Act
{
    public Vector2 direction = new();

    protected override void Enter() { ... }

    protected override void Exit()
    {
        direction = Vector2.zero;
    }
}
```


---


### <a id="Cleanup"></a> protected virtual void Cleanup()
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


### <a id="Finish"></a> protected void Finish([Outcome](#Outcome) newOutcome = Outcome.Success)
This method is only meant to be invoked in [`Enter()`](#Enter) and should not be overridden.  


---


### <a id="BlockSelf"></a> protected virtual void BlockSelf(Act byAct, [BlockType](#BlockType) blockType)
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


### <a id="UnblockSelf"></a> protected virtual void UnblockSelf(Act byAct)
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


### <a id="BlockOthers"></a> protected virtual void BlockOthers()
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


### <a id="UnblockOthers"></a> protected virtual void UnblockOthers()
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


### <a id="Prologue"></a> public Func\<Act, List\<Act\>\> Prologue
`Default: (act) => new List<Act>()`  

Assign this with a function which returns a list of acts, All acts in that list will be performed in parallel before the main act is performed.  
If the list contains `null` or if any act failed to perform it will be treated as prologuing failed & directly proceeed to [`Exit()`](#Exit) with [`DidEnter()`](#DidPerform) as `false`.  
```csharp
myAct.Prologue += (Act act) => {

    if(toFail){
        return new() { null };  // This will intentionally fail the act
    }

    return new() { myAct1, myAct2 };  // myAct1 & myAct2 will be performed in parallel
}
```


---


### <a id="PerformConditions"></a> public List\<Func\<Act, bool\>\> PerformConditions
`Default: new List<Func<Act, bool>>()`  

Used when overriding [`CanPerform()`](#CanPerform) isn't sufficient and additional external conditions are required.  
```csharp
void FixedUpdate()
{
    jumpAct.Perform();
}
void Awake()
{
    jumpAct.PerformConditions.Add(()=> {
        return Input.GetKeyDown(KeyCode.Space)  // Only jump when spacebar is pressed
    })
    jumpAct.Init(theater, "Jump Act");
}
```


---


### <a id="_canReperform"></a> protected bool _canReperform
> **Note:** Should only be assigned inside the [`Setup()`](#Setup) method.  

`Default: false` 

If `true` then calling `Perform()` while act is already performing will finish interruptively current perform and then reperform.  
If `false` then current ongoing perform must be completed before calling `Perform()` again.


---


### <a id="_tickFlags"></a> protected [TickFlags](#TickFlags) _tickFlags
> **Note:** Should only be assigned inside the [`Setup()`](#Setup) method.  

`Default: TickFlags.None`  

Determines which tick methods are to be called. Look into [`Enter()`](#Enter) & [`TickFlags`](#TickFlags) to learn more.


<br/>


## 📖 Theater Descriptions

### <a id="OnEnableChangedTheater"></a> public event Action\<Theater theater, bool newEnabled\> OnEnableChanged
Invoked whenever theater has been enabled/disabled.


---


### <a id="OnPerformStart"></a> public event Action\<Theater theater, Act act\> OnPerformStart
Invoked whenever any of acts assigned to the theater has started to performed.


---


### <a id="OnPerformEnd"></a> public event Action\<Theater theater, Act act\> OnPerformEnd
Invoked whenever any of acts assigned to the theater has completed performing.


---


### <a id="OnAllPerformEnd"></a> public event Action\<Theater theater\> OnAllPerformEnd
Invoked whenever all of the acts assigned to the theater have completed performing and none are ongoing anymore.  
Useful for idle checking, etc.


---


### <a id="IsEnabledTheater"></a> public bool IsEnabled()
Returns `true` if theater is currently enabled.

---


### <a id="SetEnabledTheater"></a> public void SetEnabled(bool newEnabled)
Disables/Enables theater i.e. If a theater is disabled then all acts assigned to it can no longer [`Perform()`](#Perform) and any act that was ongoing will be interrupted.
```csharp
theater.SetEnabled(false);  // Disable theater
theater.SetEnabled(true);  // Enable theater
```


---


### <a id="AbortAll"></a> public void AbortAll()
Calls [`Abort()`](#Abort) on all currently ongoing acts.


---


### <a id="AreAnyOngoing"></a> public bool AreAnyOngoing()
Returns `true` if any act is currently performing.


---


### <a id="GetAllActs"></a> public HashSet\<Act\> GetAllActs()
Returns a list of all the acts assigned to the theater.


<br>


## 🤝 Contribution
You can contribute in the following ways:
1. Report bugs or suggest features by opening a [new issue](https://github.com/ManasMakde/act-unity/issues/new).
2. Write test cases.
3. Sponsor this project.



## ❤️ Sponsor
If this project has been useful for you consider [supporting][Sponsor] its development.  
Any support motivates to keep the project well maintained, documented & growing.



## 🔑 License
MIT © [Manas Ravindra Makde](https://manasmakde.github.io/)



[Sponsor]: https://github.com/sponsors/ManasMakde
[Act-Lifecycle]: https://github.com/ManasMakde/act/blob/main/images/act-lifecycle.png
[Unity-Awake]: https://docs.unity3d.com/ScriptReference/MonoBehaviour.Awake.html
[Unity-Start]: https://docs.unity3d.com/ScriptReference/MonoBehaviour.Start.html
[Unity-Update]: https://docs.unity3d.com/ScriptReference/MonoBehaviour.Update.html
[Unity-FixedUpdate]: https://docs.unity3d.com/ScriptReference/MonoBehaviour.FixedUpdate.html
[Unity-LateUpdate]: https://docs.unity3d.com/ScriptReference/MonoBehaviour.LateUpdate.html
[Unity-OnDestroy]: https://docs.unity3d.com/ScriptReference/MonoBehaviour.OnDestroy.html
[Unity-GameObject]: https://docs.unity3d.com/ScriptReference/Component-gameObject.html
[Unity-DeltaTime]: https://docs.unity3d.com/ScriptReference/Time-deltaTime.html
[Unity-FixedDeltaTime]: https://docs.unity3d.com/ScriptReference/Time-fixedDeltaTime.html
