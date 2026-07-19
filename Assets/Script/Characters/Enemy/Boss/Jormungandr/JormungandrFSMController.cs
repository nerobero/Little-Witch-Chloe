using UnityEngine;
using Types;
using NUnit.Framework.Interfaces;

public class JormungandrFSMController : BaseFSMAIController
{
    public Transform mouth;
    public GameObject tailObject;
    [SerializeField] private int tailSummonCount = 1;
    [SerializeField] private float tailSummonTimeInterval = 1.5f;
    public Transform summonRoot;

    public GameObject mushroomPrefab;
    [SerializeField] private int mushroomSummonCount = 1;
    [SerializeField] private float shroomSummonTimeInterval = 1.5f;

    public Animator flowerAnimator;

    protected override void OnAwake()
    {
        base.OnAwake();

    }

    protected override (int animTriggerHash, IEnemyAttackStrategy strat) ChooseNextStrategy()
    {
        var strat = attacklist["melee"];
        return (strat.AnimHash, strat.Strategy);
    }


    protected override void Init()
    {
        base.Init();

        attacklist["melee"] = new AttackEntry(
            Animator.StringToHash("Melee"),
            new MeleeAttackStrategy(mouth, radius: 5f, damageAmount: 30f, EElementType.Water)
        );
        // attacklist["projectile"] = new AttackEntry(
        //     Animator.StringToHash("Projectile"),
        //     new ProjectileAttackStrategy()
        // );
        attacklist["summon1"] = new AttackEntry(
            Animator.StringToHash("SummonMushroom"),
            new SummonAttackStrategy(this, mushroomPrefab, shroomSummonTimeInterval, mushroomSummonCount,summonRoot.position)
        );
        attacklist["summon2"] = new AttackEntry(
            Animator.StringToHash("SummonTail"),
            new SummonAttackStrategy(this, tailObject, tailSummonTimeInterval, tailSummonCount, summonRoot.position)
        );
    }

    protected override void StartAttackStrat()
    {
        base.StartAttackStrat();
        flowerAnimator.SetTrigger(_currentAnimHash);
    }
}
