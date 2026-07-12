using UnityEngine;

[RequireComponent(typeof(Theater))]
public abstract class SpiderBase : MonoBehaviour, IDamageable
{
    // Act Properties
    protected Theater theater;
    public DamageAct damageAct = new();

    // Protected Methods
    protected Transform playerTransform;
    protected HealthSystem healthSystem;


    // Interface Methods
    public void TakeDamage(float amount)
    {
        damageAct.amount = amount;
        damageAct.Perform();
    }

    // Override Methods
    protected virtual void Awake()
    {
        // Setup Health System
        healthSystem = new HealthSystem();


        // Get Player
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerTransform = playerObj.transform;
        }
        else
        {
            enabled = false;  // Disable if player not found
        }


        // Setup Act
        theater = GetComponent<Theater>();
        damageAct.OnPostEnter += (Act act) =>
        {
            Debug.Log($"Spider damaged -{damageAct.amount}");
        };
        damageAct.healthSystem = healthSystem;
        damageAct.Init(theater, "Damage Act");
    }
}
