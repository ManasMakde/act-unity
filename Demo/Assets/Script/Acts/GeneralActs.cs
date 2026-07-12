using System;
using System.Collections;
using UnityEngine;


[Serializable]
public class PerpetualAct : Act
{
	// Public Properties
	[HideInInspector] public bool toPerpetuate = true;


	// Override Methods
	protected override void Setup()
    {
        _canReperform = true;
        if (toPerpetuate)
        {
            PerformDeferred();
        }
    }
    protected override void Exit()
    {
        if (toPerpetuate){
			PerformDeferred();
        }
    }
	protected override void UnblockSelf(Act byAct)
    {
        base.UnblockSelf(byAct);
		if (toPerpetuate && !IsBlocked())
        {
			PerformDeferred();
        }
    }
}

[Serializable]
public class GotoAct : Act
{
    // Public Properties
    [SerializeField] public float speed = 5f;
    [SerializeField] public float acceptanceRadius = 1.0f;
    [HideInInspector] public Vector2 location = new();
    [HideInInspector] public Transform target = null;
    [HideInInspector] public Rigidbody2D rb;


    // Public Method
    public bool IsWithinRange()
    {
        Vector2 destination = GetDestination();
        float distance = Vector2.Distance(rb.position, destination);
        return distance <= acceptanceRadius;  
    }


    // Private Methods
    private Vector2 GetDestination()
    {
        return target != null ? (Vector2)target.position : location;
    }


    // Override Methods
    protected override void Setup()
    {
        // Auto get rigidBody if not provided
        if(rb == null){
            rb = GetOwner().GetComponent<Rigidbody2D>();
        }        
        rb.gravityScale = 0f;  // No gravity for top down


        // Enable ticking
        _tickFlags = TickFlags.PhysicsTick;
    }
    protected override bool CanPerform()
    {
        return rb != null;
    }
    protected override Outcome PhysicsTick()
    {
        // Exit if within range of target
        if (IsWithinRange())
        {
            return Outcome.Success;
        }


        // Move towards destination
        Vector2 destination = GetDestination();
        Vector2 direction = (destination - rb.position).normalized;
        Vector2 nextPosition = rb.position + direction * speed * Time.fixedDeltaTime;
        rb.MovePosition(nextPosition);
        return Outcome.Pending;
    }
}

[Serializable]
public class AttackAct : Act
{
    // Public Properties
    [SerializeField] public float damageAmount = 10.0f;
    [HideInInspector] public Transform target;


    // Override methods
    protected override bool CanPerform()
    {
        return target != null && target.GetComponent<IDamageable>() != null;
    }
    protected override Outcome Enter()
    {
        var damageable = target.GetComponent<IDamageable>();
        damageable.TakeDamage(damageAmount);
        return Outcome.Success;
    }
}

[Serializable]
public class WaitAct : Act
{
    // Public Properties
    [SerializeField] public float duration = 5.0f;

    // Private Properties
    private Coroutine waitCoroutine;


    // Private Methods
    private IEnumerator WaitRoutine()
    {
        yield return new WaitForSeconds(duration);
        Finish(Outcome.Success);
    }

    // Override Methods
	protected override Outcome Enter()
    {
        waitCoroutine = GetTheater().StartCoroutine(WaitRoutine());
        return Outcome.Pending;
    }
	protected override void Exit()
    {
        if (waitCoroutine != null)
        {
            GetTheater().StopCoroutine(waitCoroutine);
            waitCoroutine = null;
        }
    }
}

[Serializable]
public class MoveAct : Act
{
    // Public Properties
    [SerializeField] public float speed = 5f;
    [HideInInspector] public Vector2 direction = new();
    [HideInInspector] public Rigidbody2D rb;


    // Override Methods
    protected override void Setup()
    {
        if(rb == null){
            rb = GetOwner().GetComponent<Rigidbody2D>();
        }
        rb.gravityScale = 0f;  // No gravity for top down
    }
    protected override bool CanPerform()
    {
        return rb != null;
    }
    protected override Outcome Enter()
    {
        rb.MovePosition(rb.position + direction * speed * GetPhysicsDelta());
        return Outcome.Success;
    }
    protected override void Exit()
    {
        direction = Vector2.zero;
    }
}

[Serializable]
public class ShootAct : Act
{
    // Public Properties
    [SerializeField] public GameObject projectilePrefab;
    [HideInInspector] public Vector2 spawnLocation = new();
    [HideInInspector] public bool spawnAtOwner = true;
    [HideInInspector] public Vector2 direction = new();


    // Override Methods
    protected override void Setup()
    {
    }
    protected override bool CanPerform()
    {
        return projectilePrefab != null;
    }
    protected override Outcome Enter()
    {
        // Spawn Bullet
        var spawnPosition = spawnAtOwner ? GetOwner().transform.position : (Vector3)spawnLocation;
        GameObject bullet = MonoBehaviour.Instantiate(projectilePrefab, spawnPosition, Quaternion.identity);
       
       
        // Set bullet direction
        ProjectileBase bulletScript = bullet.GetComponent<ProjectileBase>();
        bulletScript.direction = direction;
        bulletScript.SetOwner(GetOwner());
        
        
        return Outcome.Success;
    }
    protected override void Exit()
    {
        direction = Vector2.zero;
    }
}

[Serializable]
public class DamageAct : Act
{
    // Public Properties
    [HideInInspector] public HealthSystem healthSystem;
    [HideInInspector] public float amount = 5.0f;
    [HideInInspector] public bool canDie = true;
    [HideInInspector] public bool toAnimate = false;
    [HideInInspector] public float flashDuration = 0.5f;
    [HideInInspector] public float flashInterval = 0.1f;
    [HideInInspector] public float flashAlpha = 0.3f; // Target transparency (0 = invisible, 1 = opaque)
    [HideInInspector] public SpriteRenderer spriteRenderer;


    // Private Properties
    private Coroutine AnimCoroutine; // Track the running coroutine


    // Private Methods
    private IEnumerator Animate()
    {
        // Change opacity in intervals
        Color originalColor = spriteRenderer.color;
        Color flashedColor = new Color(originalColor.r, originalColor.g, originalColor.b, flashAlpha);
        float elapsed = 0f;
        while (elapsed < flashDuration)
        {
            spriteRenderer.color = spriteRenderer.color == originalColor ? flashedColor : originalColor;
            yield return new WaitForSeconds(flashInterval);
            elapsed += flashInterval;
        }


        // Reset color
        spriteRenderer.color = originalColor;
        Finish(Outcome.Success);
    }


    // Override Methods
    protected override void Setup()
    {
        _canReperform = true;

        if (toAnimate)
        {
            spriteRenderer = GetOwner().GetComponent<SpriteRenderer>();
        }
    }
    protected override bool CanPerform()
    {
        return healthSystem != null && (!toAnimate || spriteRenderer != null);
    }
    protected override Outcome Enter()
    {
        // Reduce health
        healthSystem.ReduceHealth(amount);


        // Death
        if (canDie && Mathf.Approximately(healthSystem.CurrentHealth, 0f))
        {
            MonoBehaviour.Destroy(GetOwner());
            return Outcome.Success;
        }


        // Animate
        if (toAnimate)
        {
            AnimCoroutine = GetTheater().StartCoroutine(Animate());
            return Outcome.Pending;
        }

        return Outcome.Success;
    }
    protected override void Exit()
    {
        // Stop animation
        if (AnimCoroutine != null)
        {
            GetTheater().StopCoroutine(AnimCoroutine);
            AnimCoroutine = null;
        }


        // Reset animation changes
        if (toAnimate && spriteRenderer != null)
        {
            Color currentColor = spriteRenderer.color;
            spriteRenderer.color = new Color(currentColor.r, currentColor.g, currentColor.b, 1f);
        }
    }
}

[Serializable]
public class LookAct : Act
{
    // Enums
    public enum TurnType
    {
        Once,  // Turns only once towards target
        UntilFacing,  // Turns until target has been reached
        Continuous  // Follows target indefinitely
    }


    // Public Properties
    [SerializeField] public Transform targetTransform = null;
    [SerializeField] public float targetRotation = 0f;  // Rotation towards which to turn
    [SerializeField] public TurnType turnType = TurnType.UntilFacing;
    [SerializeField] public float followTimeout = 0f;  // 0 or less means indefinitely
    [SerializeField] public bool instantTurn = false;  // If true then snaps rotation instantly else drags at turn speed
    [SerializeField] public float turnSpeed = 150f;
    [SerializeField] public float acceptanceAngle = 0.5f;  // In deg, used to decide if goal rotation reached
    [HideInInspector] public Rigidbody2D rb;


    // Private Properties
    private Coroutine followCoroutine;


    // Static Method
    public float RotationTowardsPosition(Vector2 position){

        Vector2 direction = (Vector2)position - rb.position;
        return Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
    }


    // Private Methods
    private float GetGoalRotation()
    {
        // Turn towards target actor if assigned
        if (targetTransform == null)
        {
            return targetRotation;
        }

        return RotationTowardsPosition((Vector2)targetTransform.position);
    }
    private float CalcRotationLerp(float goalRotation, float deltaTime)
    {
        // Snap instantly or drag at turn speed
        return instantTurn ? goalRotation : Mathf.MoveTowardsAngle(rb.rotation, goalRotation, turnSpeed * deltaTime);
    }
    private IEnumerator FollowDurationRoutine()
    {
        yield return new WaitForSeconds(followTimeout);
        Finish(Outcome.Success);
    }


    // Override Methods
    protected override void Setup()
    {
        // Auto get rigidBody if not provided
        if (rb == null)
        {
            rb = GetOwner().GetComponent<Rigidbody2D>();
        }

        // Enable ticking
        _tickFlags = TickFlags.PhysicsTick;
    }
    protected override bool CanPerform()
    {
        return rb != null;
    }
    protected override Outcome Enter()
    {
        // Turn single time and finish
        if (turnType == TurnType.Once)
        {
            float goalRotation = GetGoalRotation();
            rb.MoveRotation(CalcRotationLerp(goalRotation, GetPhysicsDelta()));
            return Outcome.Success;
        }


        // Stop follow after given positive duration
        if (turnType == TurnType.Continuous && followTimeout > 0f)
        {
            followCoroutine = GetTheater().StartCoroutine(FollowDurationRoutine());
        }

        return Outcome.Pending;
    }
    protected override Outcome PhysicsTick()
    {
        // Turn owner
        float goalRotation = GetGoalRotation();
        rb.MoveRotation(CalcRotationLerp(goalRotation, GetPhysicsDelta()));


        // Keep ticking if not meant to stop at goal
        if (turnType != TurnType.UntilFacing)
        {
            return Outcome.Pending;
        }


        // Exit if reached goal rotation
        float angleDiff = Mathf.Abs(Mathf.DeltaAngle(rb.rotation, goalRotation));
        return angleDiff <= acceptanceAngle ? Outcome.Success : Outcome.Pending;
    }
    protected override void Exit()
    {
        // Clear follow coroutine
        if (followCoroutine != null)
        {
            GetTheater().StopCoroutine(followCoroutine);
            followCoroutine = null;
        }
    }
}
