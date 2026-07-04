using System;
using UnityEngine;

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
