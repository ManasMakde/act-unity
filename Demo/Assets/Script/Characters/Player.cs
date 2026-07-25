using System;
using UnityEngine;

[RequireComponent(typeof(Theater))]
public class Player : MonoBehaviour, IDamageable, ITrappable
{
    // Public Actions
    public event Action OnDeath;


    // Act Properties
    private Theater theater;
    public MoveAct moveAct = new();
    public ShootAct shootAct = new();
    public DamageAct damageAct = new();
    public LookAct lookAct = new();


    // Private Properties
    [SerializeField] private HealthSystem healthSystem;
    [SerializeField] private float barrelLength = 1f; // Distance from player center to barrel tip


    // Interface Methods
    public void TakeDamage(float amount)
    {
        damageAct.amount = amount;
        damageAct.Perform();
    }
    public void Trap(float duration)
    {
        moveAct.SetEnabled(false);
    }
    public void Untrap()
    {
        moveAct.SetEnabled(true);
    }


    // Override Methods
    void Update()
    {
        // Move
        moveAct.direction = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;


        // Look towards mouse constantly
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        lookAct.targetRotation = lookAct.RotationTowardsPosition(mouseWorldPosition);


        // Shoot
        if (Input.GetMouseButtonDown(0))
        {
            shootAct.direction = ((Vector2)mouseWorldPosition - (Vector2)transform.position).normalized;
            shootAct.spawnLocation = (Vector2)transform.position + shootAct.direction * barrelLength;
            shootAct.Perform();
        }
    }
    void FixedUpdate()
    {
        moveAct.Perform();
    }
    void Awake()
    {
        // Setup Health System
        healthSystem = new HealthSystem();


        // Setup acts
        theater = GetComponent<Theater>();

        moveAct.Init(theater, "Move Act");
        
        shootAct.spawnAtOwner = false;
        shootAct.Init(theater, "Shoot Act");
        
        damageAct.OnPostEnter += (Act act) =>
        {
            Debug.Log($"OUCH! Player was damaged -{damageAct.amount}");
        };
        damageAct.healthSystem = healthSystem;
        damageAct.Init(theater, "Damage Act");
      
        lookAct.turnType = LookAct.TurnType.Continuous;
        lookAct.turnSpeed = -1.0f;
        lookAct.Init(theater, "Look Act");
        lookAct.Perform();
    }
}
