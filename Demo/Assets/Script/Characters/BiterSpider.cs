using UnityEngine;


public class BiterSpider : SpiderBase
{
    // Public Properties
    [SerializeField] bool isVenomous = false;
    [SerializeField] GameObject venomPrefab = null;


    // Act Properties
    [SerializeField] PerpetualAct liveAct = new();
    [SerializeField] PerpetualAct lookPerpAct = new();
    [SerializeField] GotoAct chaseAct = new();
    [SerializeField] LookAct lookAct = new();
    [SerializeField] WaitAct delayAttackAct = new();
    [SerializeField] AttackAct biteAct = new();


    // Override Properties
    protected override void Awake()
    {
        // Animate when damaged
        damageAct.toAnimate = true;
        damageAct.AddToBlock(new() { liveAct, lookPerpAct });  // Stop AI behaviour while damage animation is being played


        base.Awake();


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
