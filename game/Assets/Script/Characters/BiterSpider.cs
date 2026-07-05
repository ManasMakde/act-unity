using UnityEngine;

public class BiterSpider : SpiderBase
{
    // Public Properties
    [SerializeField] bool isVenomous = false;
    [SerializeField] GameObject venomPrefab = null;

    // Act Properties
    [SerializeField] PerpetualAct liveAct = new();
    [SerializeField] GotoAct chaseAct = new();
    [SerializeField] WaitAct delayAttackAct = new();
    [SerializeField] AttackAct biteAct = new();


    // Override Properties
    protected override void Awake()
    {
        base.Awake();


        // Setup Live Act
        liveAct.Prologue += (Act act) =>
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


        // Setup Goto Act
        chaseAct.target = playerTransform;
        chaseAct.Init(theater, "Goto Act");


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
