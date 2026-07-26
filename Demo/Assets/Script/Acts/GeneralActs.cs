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
        if (toPerpetuate)
        {
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
        if (rb == null)
        {
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
    [SerializeField] public AnimationClip animation;
    [HideInInspector] public Transform target;


    // Private Properties
    [HideInInspector] public EventfulAnimator eventfulAnimator;


    // Private Methods
    private void OnAnimationEnded(AnimationClip clip)
    {
        // Ignore if not the attack clip
        if (clip != animation)
        {
            return;
        }

        Finish(Outcome.Success);
    }


    // Override methods
    protected override void Setup()
    {
        if (animation != null)
        {
            eventfulAnimator = GetOwner().GetComponentInChildren<EventfulAnimator>();
        }
    }
    protected override bool CanPerform()
    {
        // Failed if no target
        if (target == null)
        {
            WriteLog("Failed to perform! No target provided!");
            return false;
        }


        // Fail if no damage interface
        if (target.GetComponent<IDamageable>() == null)
        {
            WriteLog("Failed to perform! Target has no damageable interface");
            return false;
        }


        // Fail if to animate and no eventful animator found
        if (animation != null && eventfulAnimator == null)
        {
            WriteLog("Failed to perform! No eventful animator provided!");
            return false;
        }

        return true;
    }
    protected override Outcome Enter()
    {
        var damageable = target.GetComponent<IDamageable>();
        damageable.TakeDamage(damageAmount);


        // Return if not to animate
        if (animation == null)
        {
            return Outcome.Success;
        }


        // Play animation
        eventfulAnimator.OnAnimationEnded += OnAnimationEnded;
        eventfulAnimator.Play(animation);
        return Outcome.Pending;

    }
    protected override void Exit()
    {
        // Stop animation
        if (eventfulAnimator != null)
        {
            eventfulAnimator.Stop();
            eventfulAnimator.OnAnimationEnded -= OnAnimationEnded;
        }
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
    [SerializeField] public bool useBorder = false;
    [SerializeField] public Rect border = new Rect(-10f, -10f, 20f, 20f);
    [HideInInspector] public Vector2 direction = new();
    [HideInInspector] public Rigidbody2D rb;


    // Override Methods
    protected override void Setup()
    {
        if (rb == null)
        {
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
        Vector2 nextPosition = rb.position + direction * speed * GetPhysicsDelta();

        if (useBorder)
        {
            nextPosition.x = Mathf.Clamp(nextPosition.x, border.xMin, border.xMax);
            nextPosition.y = Mathf.Clamp(nextPosition.y, border.yMin, border.yMax);
        }

        rb.MovePosition(nextPosition);
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
    [SerializeField] public float delayAmount = 0f;
    [HideInInspector] public Vector2 spawnLocation = new();
    [HideInInspector] public bool spawnAtOwner = true;
    [HideInInspector] public Vector2 direction = new();
    [HideInInspector] public System.Type[] ignoreList;
    public AnimationClip animation;


    // Private Properties
    [HideInInspector] public EventfulAnimator eventfulAnimator;
    private Coroutine delayCoroutine; // Track the running delay coroutine
    private bool _animationPlaying = false; // True while shoot animation is playing
    private bool _shootPending = false; // True while waiting on delay to shoot


    // Private Methods
    private IEnumerator DelayedShoot()
    {
        yield return new WaitForSeconds(delayAmount);
        SpawnBullet();
        _shootPending = false;
        TryFinish();
    }
    private void SpawnBullet()
    {
        // Spawn Bullet
        var spawnPosition = spawnAtOwner ? GetOwner().transform.position : (Vector3)spawnLocation;
        GameObject bullet = MonoBehaviour.Instantiate(projectilePrefab, spawnPosition, Quaternion.identity);


        // Set bullet direction and owner
        ProjectileBase bulletScript = bullet.GetComponent<ProjectileBase>();
        bulletScript.direction = direction;
        bulletScript.SetOwner(GetOwner());


        // Set ignore list if any provided
        if (ignoreList != null)
        {
            bulletScript.SetIgnoreList(ignoreList);
        }
    }
    private void OnAnimationEnded(AnimationClip clip)
    {
        // Ignore if not the shoot clip
        if (clip != animation)
        {
            return;
        }

        // Mark animation done
        _animationPlaying = false;
        TryFinish();
    }
    private void TryFinish()
    {
        // Wait for both animation and shoot delay to complete
        if (_animationPlaying || _shootPending)
        {
            return;
        }

        Finish(Outcome.Success);
    }


    // Override Methods
    protected override void Setup()
    {
        if (animation != null)
        {
            eventfulAnimator = GetOwner().GetComponentInChildren<EventfulAnimator>();
        }
    }
    protected override bool CanPerform()
    {
        return projectilePrefab != null && (animation == null || eventfulAnimator != null);
    }
    protected override Outcome Enter()
    {
        // Start animation
        if (animation != null)
        {
            _animationPlaying = true;
            eventfulAnimator.OnAnimationEnded += OnAnimationEnded;
            eventfulAnimator.Play(animation);
        }


        // Shoot instantly or after delay
        if (delayAmount <= 0f)
        {
            SpawnBullet();
        }
        else
        {
            _shootPending = true;
            delayCoroutine = GetTheater().StartCoroutine(DelayedShoot());
        }


        // Pending if either started, else success
        return (_animationPlaying || _shootPending) ? Outcome.Pending : Outcome.Success;
    }
    protected override void Exit()
    {
        // Stop delay coroutine
        if (delayCoroutine != null)
        {
            GetTheater().StopCoroutine(delayCoroutine);
            delayCoroutine = null;
        }


        // Stop animation
        if (eventfulAnimator != null)
        {
            eventfulAnimator.Stop();
            eventfulAnimator.OnAnimationEnded -= OnAnimationEnded;
        }


        // Reset state
        direction = Vector2.zero;
        _animationPlaying = false;
        _shootPending = false;
    }
}

[Serializable]
public class DamageAct : Act
{
    // Public Properties
    public AnimationClip animation;
    [HideInInspector] public HealthSystem healthSystem;
    [HideInInspector] public float amount = 5.0f;
    [HideInInspector] public bool canDie = true;
    [HideInInspector] public bool toFlash = false;
    [HideInInspector] public float flashDuration = 0.5f;
    [HideInInspector] public float flashInterval = 0.1f;
    [HideInInspector] public float flashAlpha = 0.3f; // Target transparency (0 = invisible, 1 = opaque)


    // Private Properties
    [HideInInspector] public EventfulAnimator eventfulAnimator;
    [HideInInspector] public SpriteRenderer spriteRenderer;


    // Private Properties
    private Coroutine AnimCoroutine; // Track the running coroutine
    private bool _animationPlaying = false; // True while damage animation is playing
    private bool _flashPlaying = false; // True while flash coroutine is playing
    private bool _toDie = false;


    // Private Methods
    private IEnumerator Flash(float duration)
    {
        // Change opacity in intervals
        Color originalColor = spriteRenderer.color;
        Color flashedColor = new Color(originalColor.r, originalColor.g, originalColor.b, flashAlpha);
        float elapsed = 0f;
        while (elapsed < duration)
        {
            spriteRenderer.color = spriteRenderer.color == originalColor ? flashedColor : originalColor;
            yield return new WaitForSeconds(flashInterval);
            elapsed += flashInterval;
        }


        // Reset color
        spriteRenderer.color = originalColor;
        OnFlashEnded();
    }
    private void OnAnimationEnded(AnimationClip clip)
    {
        // Ignore if not the damage clip
        if (clip != animation)
        {
            return;
        }

        // Mark animation done
        _animationPlaying = false;
        TryFinish();
    }
    private void OnFlashEnded()
    {
        // Mark flash done
        _flashPlaying = false;
        TryFinish();
    }
    private void TryFinish()
    {
        // Wait for both animation and flash to complete
        if (_animationPlaying || _flashPlaying)
        {
            return;
        }

        Finish(Outcome.Success);
    }


    // Override Methods
    protected override void Setup()
    {
        _canReperform = true;

        if (toFlash)
        {
            spriteRenderer = GetOwner().GetComponentInChildren<SpriteRenderer>();
        }
        if (animation != null)
        {
            eventfulAnimator = GetOwner().GetComponentInChildren<EventfulAnimator>();
        }
    }
    protected override bool CanPerform()
    {
        // Return false if invalid health system
        if (healthSystem == null)
        {
            WriteLog("Failed to perform! No health system assigned");
            return false;
        }


        // Return false if invalid  sprite renderer
        if (toFlash && spriteRenderer == null)
        {
            WriteLog("Failed to perform! Flash requested but no sprite renderer found");
            return false;
        }


        // Return false if invalid  sprite renderer
        if (animation != null && eventfulAnimator == null)
        {
            WriteLog("Failed to perform! Invalid Eventful animator");
            return false;
        }


        return true;
    }
    protected override Outcome Enter()
    {
        // Reduce health
        healthSystem.ReduceHealth(amount);


        // Death
        if (canDie && Mathf.Approximately(healthSystem.currentHealth, 0f))
        {
            _toDie = true;
        }


        // Start animation
        if (animation != null)
        {
            _animationPlaying = true;
            eventfulAnimator.OnAnimationEnded += OnAnimationEnded;
            eventfulAnimator.Play(animation);
        }


        // Start flash, duration matches animation length if animation is playing
        if (toFlash)
        {
            float flashLength = animation != null ? animation.length : flashDuration;
            _flashPlaying = true;
            AnimCoroutine = GetTheater().StartCoroutine(Flash(flashLength));
        }


        // Pending if either started, else success
        return (_animationPlaying || _flashPlaying) ? Outcome.Pending : Outcome.Success;
    }
    protected override void Exit()
    {
        // Stop animation
        if (AnimCoroutine != null)
        {
            GetTheater().StopCoroutine(AnimCoroutine);
            AnimCoroutine = null;
        }


        // Stop animation
        if (eventfulAnimator != null)
        {
            eventfulAnimator.Stop();
            eventfulAnimator.OnAnimationEnded -= OnAnimationEnded;
        }


        // Reset flashing changes
        if (toFlash && spriteRenderer != null)
        {
            Color currentColor = spriteRenderer.color;
            spriteRenderer.color = new Color(currentColor.r, currentColor.g, currentColor.b, 1f);
        }


        // Die
        if (_toDie)
        {
            MonoBehaviour.Destroy(GetOwner());
        }


        // Reset playing flags
        _animationPlaying = false;
        _flashPlaying = false;
        _toDie = false;
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
    [SerializeField] public float turnSpeed = 150f;  // Set to negative if to snap turn instantly 
    [SerializeField] public float acceptanceAngle = 0.5f;  // In deg, used to decide if goal rotation reached
    [HideInInspector] public Rigidbody2D rb;


    // Private Properties
    private Coroutine followCoroutine;


    // Static Method
    public float RotationTowardsPosition(Vector2 position)
    {

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
        return turnSpeed < 0.0f ? goalRotation : Mathf.MoveTowardsAngle(rb.rotation, goalRotation, turnSpeed * deltaTime);
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
