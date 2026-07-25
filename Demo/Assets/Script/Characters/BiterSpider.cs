using UnityEngine;


public class BiterSpider : SpiderBase
{
    // Public Properties
    [SerializeField] bool isVenomous = false;
    [SerializeField] GameObject venomPrefab = null;
    [SerializeField] EventfulAnimator eventfulAnimator = null;


    // Animation Properties
    [SerializeField] public AnimationClip idleAnim = null;
    [SerializeField] public AnimationClip walkAnim = null;


    // Act Properties
    [SerializeField] PerpetualAct liveAct = new();
    [SerializeField] PerpetualAct lookPerpAct = new();
    [SerializeField] GotoAct chaseAct = new();
    [SerializeField] LookAct lookAct = new();
    [SerializeField] WaitAct delayAttackAct = new();
    [SerializeField] AttackAct biteAct = new();


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
    protected override void Awake()
    {
        // Animate when damaged
        damageAct.toFlash = true;
        damageAct.AddToBlock(new() { liveAct, lookPerpAct });  // Stop AI behaviour while damage animation is being played


        base.Awake();


        // Setup Animator
        eventfulAnimator = GetComponentInChildren<EventfulAnimator>();


        // Setup Live Act
        liveAct.prologue += (Act act) =>
        {
            // Attack then Wait
            if (chaseAct.IsWithinRange())
            {
                return Act.Seq(new() { new() { biteAct }, new() { delayAttackAct } });
            }

            // Goto player then Wait
            return Act.Seq(new() { new() { chaseAct }, new() { delayAttackAct } });
        };
        liveAct.Init(theater, "Live Act");


        // Setup Look & Look Perp Act
        lookAct.turnType = LookAct.TurnType.Continuous;
        lookAct.turnSpeed = -1.0f;
        lookAct.targetTransform = playerTransform;
        lookAct.Init(theater, "Turn Act");
        lookPerpAct.prologue += (Act act) => new() { lookAct };
        lookPerpAct.Init(theater, "Look Act");


        // Setup Chase Act
        chaseAct.target = playerTransform;
        chaseAct.Init(theater, "Chase Act");


        // Setup Attack Act
        biteAct.target = playerTransform;
        biteAct.OnPostEnter += (Act act) =>
        {
            if (isVenomous)
            {
                Instantiate(venomPrefab, biteAct.target);
            }
        };
        biteAct.Init(theater, "Bite Attack Act");


        // Setup Wait Act
        delayAttackAct.Init(theater, "Wait Act");
    }
}
