using UnityEngine;
using Types;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.Multiplayer.PlayMode;

public class JormungandrFSMController : BaseFSMAIController
{
    [Header("Melee Attack Damage origin")]
    public Transform mouth;

    [Header("Tail attack summon")]
    public GameObject tailObject;
    [SerializeField] private int tailSummonCount = 1;
    [SerializeField] private float tailSummonTimeInterval = 1.5f;
    public Transform summonRoot;

    [Header("Mushroom mine summon")]
    public GameObject mushroomPrefab;
    [SerializeField] private int mushroomSummonCount = 1;
    [SerializeField] private float shroomSummonTimeInterval = 1.5f;

    [Header("Animator - Flower")]
    public Animator flowerAnimator;
    public SpriteRenderer flower;
    private BossEnemyStatManager _statManager;

    [Header("Graphs")]
    public AnimationCurve HealthCurve;
    public AnimationCurve DamageCurve;
    public AnimationCurve DistanceCurve;
    [SerializeField] private float HealthUtilWeight = 0.33f;
    [SerializeField] private float DamageUtilWeight = 0.33f;
    [SerializeField] private float DistanceUtilWeight = 0.33f; 

    protected override void OnAwake()
    {
        base.OnAwake();
        _statManager = GetComponent<BossEnemyStatManager>();
    }

    protected override List<(string key, float probability)> CalculateProbability(float Temperature)
    {
        var probabilities = new List<(string key, float probability)>();
        float UtilHealthCurve = HealthCurve.Evaluate(_statManager.CurrentHP);
        float distance = Vector2.Distance(_currentTarget.transform.position, transform.position);
        float UtilDistanceCurve = DistanceCurve.Evaluate(distance);

        if (attacklist.Count == 0 || Temperature == 0f) return probabilities;

        // 1. Calculating the raw util scores for each attack strategy
        var rawUtils = new List<(string key, float util)>();
        foreach (string key in attacklist.Keys)
        {
            var strat = attacklist[key];
            float UtilDamage = DamageCurve.Evaluate(strat.Strategy.GetDamageNumber());
            float TotalNormalUtil =
                (HealthUtilWeight * UtilHealthCurve + DamageUtilWeight * UtilDamage + DistanceUtilWeight * UtilDistanceCurve) /
                (HealthUtilWeight + DamageUtilWeight + DistanceUtilWeight);
            rawUtils.Add((key, TotalNormalUtil));
        }

        // 2. Softmax probability conversion:
        float maxUtil = float.NegativeInfinity;
        foreach (var (_, util) in rawUtils) maxUtil = Mathf.Max(maxUtil, util);

        float sumExp = 0f;
        foreach (var (key, util) in rawUtils)
        {
            float exp = Mathf.Exp((util - maxUtil) / Temperature);
            probabilities.Add((key, exp));
            sumExp += exp;
        }

        // 3. Normalizing the softmax probabilities and saving the value:
        for (int i = 0; i < probabilities.Count; i++)
            probabilities[i] = (probabilities[i].key, probabilities[i].probability / sumExp);

        return probabilities;
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
        attacklist["statusEffect"] = new AttackEntry(
            Animator.StringToHash("StatusEffect"),
             new SummonAttackStrategy(this, tailObject, tailSummonTimeInterval, tailSummonCount, summonRoot.position) // <-- TODO: CHANGE
        );
    }

    public void EnableFlower()
    {
        if (flower != null)
            flower.enabled = true;

        var idle = Animator.StringToHash("Idle");
        _mainAnimator.SetBool(idle, true);
        flowerAnimator.SetBool(idle, true);
    }

    public void DisableFlower()
    {
        if (flower != null)
            flower.enabled = false;
    }

    protected override void StartAttackStrat()
    {
        base.StartAttackStrat();
        TriggerAnimation(flowerAnimator,_currentAnimHash);
    }
}
