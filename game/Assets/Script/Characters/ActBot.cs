using System;
using UnityEngine;

[RequireComponent(typeof(Theater))]
public class ActBot : MonoBehaviour, IDamageable, ITrappable
{
    // Public Actions
    public event Action OnDeath;

    // Act Properties
    private Theater theater;
    public MoveAct moveAct = new();
    public ShootAct shootAct = new();
    public DamageAct damageAct = new();
    
    // Private Properties
    [SerializeField] private HealthSystem healthSystem;


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


        // Shoot
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            shootAct.direction = ((Vector2)mouseWorldPosition - (Vector2)transform.position).normalized;
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
        shootAct.Init(theater, "Shoot Act");
        damageAct.OnPostEnter += (Act act) =>
        {
            Debug.Log($"OUCH! Player was damaged -{damageAct.amount}");
        };
        damageAct.healthSystem = healthSystem;
        damageAct.Init(theater, "Damage Act");
    }
}
