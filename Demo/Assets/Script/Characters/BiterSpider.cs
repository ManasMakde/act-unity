using System;
using UnityEngine;


[RequireComponent(typeof(Theater))]
public class BiterSpider : MonoBehaviour, IDamageable
{
    // Public Actions
    public event Action OnKilled;


    // Private Properties
    [SerializeField] bool isVenomous = false;
    [SerializeField] GameObject venomPrefab;
    [SerializeField] HealthSystem healthSystem = new();
    [SerializeField] EventfulAnimator eventfulAnimator;
    Transform playerTransform;


    // Animation Properties
    [SerializeField] AnimationClip idleAnim;
    [SerializeField] AnimationClip walkAnim;


    // Act Properties
    [SerializeField] Theater theater;
    [SerializeField] PerpetualAct liveAct = new();
    [SerializeField] GotoAct chaseAct = new();
    [SerializeField] PerpetualAct lookPerpAct = new();
    [SerializeField] LookAct lookAct = new();
    [SerializeField] WaitAct delayAttackAct = new();
    [SerializeField] AttackAct biteAct = new();
    [SerializeField] DamageAct damageAct = new();


    // Interface Methods
    public void TakeDamage(float amount)
    {
        damageAct.amount = amount;
        damageAct.Perform();
    }


    // Override Properties
    void Update()
    {
        // Return if any override animation playing
        if (damageAct.IsActive() || biteAct.IsActive())
        {
            return;
        }


        // Walk
        if (chaseAct.IsActive())
        {
            eventfulAnimator.Play(walkAnim);
        }


        // Idle
        else
        {
            eventfulAnimator.Play(idleAnim);
        }
    }
    void Awake()
    {
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


        // Setup Animator
        eventfulAnimator = GetComponentInChildren<EventfulAnimator>();


        // Setup Acts
        theater = GetComponent<Theater>();

        liveAct.prologue += (Act act) =>
        {
            // Attack -> Wait
            if (chaseAct.IsWithinRange())
            {
                return Act.Seq(new() { new() { biteAct }, new() { delayAttackAct } });
            }


            // Goto player -> Wait
            return Act.Seq(new() { new() { chaseAct }, new() { delayAttackAct } });
        };
        liveAct.Init(theater, "Live Act");

        lookAct.turnType = LookAct.TurnType.Continuous;
        lookAct.turnSpeed = -1.0f;
        lookAct.targetTransform = playerTransform;
        lookAct.Init(theater, "Turn Act");

        lookPerpAct.prologue += (Act act) => new() { lookAct };
        lookPerpAct.Init(theater, "Look Act");

        chaseAct.target = playerTransform;
        chaseAct.Init(theater, "Chase Act");

        biteAct.OnPostEnter += (Act act) =>
        {
            if (isVenomous)
            {
                Instantiate(venomPrefab, biteAct.target);
            }
        };
        biteAct.target = playerTransform;
        biteAct.Init(theater, "Bite Attack Act");

        delayAttackAct.Init(theater, "Wait Act");

        damageAct.toFlash = true;
        damageAct.healthSystem = healthSystem;
        damageAct.AddToBlock(new() { liveAct, lookPerpAct });  // Stop AI behaviour while damaged
        damageAct.OnPostExit += (Act act) =>
        {
            if (Mathf.Approximately(healthSystem.currentHealth, 0.0f))
            {
                OnKilled?.Invoke();
            }
        };
        damageAct.Init(theater, "Damage Act");
    }
}
