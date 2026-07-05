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
        // Debug.Log(GetOwner().GetComponent<Rigidbody2D>());
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
    [SerializeField] public GameObject bulletPrefab;
    [HideInInspector] public Vector2 spawnLocation = new();
    [HideInInspector] public bool spawnAtOwner = true;
    [HideInInspector] public Vector2 bulletDirection = new();


    // Override Methods
    protected override void Setup()
    {
    }
    protected override bool CanPerform()
    {
        return bulletPrefab != null;
    }
    protected override Outcome Enter()
    {
        // Spawn Bullet
        var spawnPosition = spawnAtOwner ? GetOwner().transform.position : new Vector3(spawnLocation.x, spawnLocation.y, 0.0f);
        GameObject bullet = MonoBehaviour.Instantiate(bulletPrefab, spawnPosition, Quaternion.identity);
       
       
        // Set bullet direction
        Bullet bulletScript = bullet.GetComponent<Bullet>();
        bulletScript.direction = bulletDirection;
        
        return Outcome.Success;
    }
    protected override void Exit()
    {
        bulletDirection = Vector2.zero;
    }
}


[Serializable]
public class DamageAct : Act
{
    // Public Properties
    [HideInInspector] public HealthSystem healthSystem;
    [HideInInspector] public float amount = 5.0f;
    [HideInInspector] public bool canDie = true;


    // Override Methods
    protected override bool CanPerform()
    {
        return healthSystem != null;
    }
    protected override Outcome Enter()
    {
        healthSystem.ReduceHealth(amount);
        if (canDie && Mathf.Approximately(healthSystem.CurrentHealth, 0f))
        {
            MonoBehaviour.Destroy(GetOwner());
        }
        return Outcome.Success;
    }
}
