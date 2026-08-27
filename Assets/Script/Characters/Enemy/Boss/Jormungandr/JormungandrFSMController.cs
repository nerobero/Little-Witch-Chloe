using UnityEngine;
using Types;
using System.Collections.Generic;

public class JormungandrFSMController : BaseFSMAIController
{
    [Header("Melee Attack Damage origin")]
    public Transform mouth;

    [Header("Tail attack summon")]
    public GameObject tailObject;
    [SerializeField] private int tailSummonCount = 1;
    [SerializeField] private float tailSummonTimeInterval = 1.5f;
    public Transform tailSummonRoot;
    public float summonRange = 5.0f; // share the value with mushroom mine.

    [Header("Mushroom mine summon")]
    public GameObject mushroomPrefab;
    [SerializeField] private int mushroomSummonCount = 1;
    [SerializeField] private float shroomSummonTimeInterval = 1.5f;
    public Transform shroomSummonRoot;

    [Header("Poison Fog summon")]
    public GameObject poisonPrefab;
    [SerializeField] private int fogSummonCount = 3;
    [SerializeField] private float fogSummontimeInterval = 1.5f;
    [SerializeField] private float summonRadius = 3f;
    public Transform summonRoot;


    [Header("Animator - Flower")]
    public Animator flowerAnimator;
    private BossEnemyStatManager _statManager;

    [Header("Side switching")]
    public Transform leftAnchor;
    public Transform rightAnchor;
    [SerializeField] private int minTurnsBetweenSwitch = 3;
    [SerializeField] private int maxTurnsBetweenSwitch = 6;
    private static readonly int SwitchSideHash = Animator.StringToHash("SwitchSides");
    private int _turnsUntilSwitch;
    private bool _forceMeleeNext;
    private Vector3 _switchTargetPosition;

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

        _currentSideIsRight = Vector2.Distance(transform.position, rightAnchor.position)
            < Vector2.Distance(transform.position, leftAnchor.position);
        _turnsUntilSwitch = Random.Range(minTurnsBetweenSwitch, maxTurnsBetweenSwitch + 1);
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
        float sumExp = 0f;
        foreach (var (key, util) in rawUtils)
        {
            float exp = Mathf.Exp((util) / Temperature);
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
        if (_forceMeleeNext)
        {
            _forceMeleeNext = false;
            var melee = attacklist["melee"];
            Debug.Log("[FSM] decision made: melee (forced after side switch)");
            return (melee.AnimHash, melee.Strategy);
        }

        var probabilities = CalculateProbability(0.25f);
        float rand = 1f - Random.Range(0f, 1f);
        float cumulative = 0f;
        foreach (var prob in probabilities)
        {
            cumulative += prob.probability;
            if (rand <= cumulative)
            {
                var strat = attacklist[prob.key];
                Debug.Log($"[FSM] decision made: {prob.key}");
                return (strat.AnimHash, strat.Strategy);
            }
        }

        return (0, null);
    }


    protected override void Init()
    {
        base.Init();

        attacklist["melee"] = new AttackEntry(
            Animator.StringToHash("Melee"),
            new MeleeAttackStrategy(mouth, radius: 5f, damageAmount: 1f, EElementType.Water)
        );
        attacklist["projectile"] = new AttackEntry(
            Animator.StringToHash("Projectile"), 
            new ProjectileAttackStrategy(this, mouth, ESpawnType.WaterBall, timeInterval: 1f, damageAmount: 3f, orbitRadius: 5f, shootCount: 4)
        );
        attacklist["summon1"] = new AttackEntry(
            Animator.StringToHash("Summon"),
            new SummonAttackStrategy(this, mushroomPrefab, shroomSummonTimeInterval, mushroomSummonCount, shroomSummonRoot.position, summonRange: summonRange)
        );
        attacklist["summon2"] = new AttackEntry(
            Animator.StringToHash("Summon"),
            new SummonAttackStrategy(this, tailObject, tailSummonTimeInterval, tailSummonCount, tailSummonRoot.position, summonRange: summonRange)
        );
        attacklist["statusEffect"] = new AttackEntry(
            Animator.StringToHash("StatusEffect"),
            new SummonAttackStrategy(this, poisonPrefab, timeInterval: fogSummontimeInterval, poolSize: fogSummonCount, 
                                    position: summonRoot.position, orbitRadius: summonRadius)
        );
    }

    public void EnableFlower()
    {
        var idle = Animator.StringToHash("Idle");
        _mainAnimator.SetBool(idle, true);
        flowerAnimator.SetBool(idle, true);
    }


    protected override void StartAttackStrat()
    {
        if (!_forceMeleeNext && _turnsUntilSwitch <= 0)
        {
            BeginSwitchSide();
            return;
        }

        if (!_forceMeleeNext) _turnsUntilSwitch--;

        base.StartAttackStrat();
        TriggerAnimation(flowerAnimator,_currentAnimHash);
    }

    private void BeginSwitchSide()
    {
        _isActing = true;
        _forceMeleeNext = true;

        Transform target = _currentSideIsRight ? leftAnchor : rightAnchor;
        _switchTargetPosition = target.position;

        Debug.Log("[FSM] Switching sides...");

        TriggerAnimation(_mainAnimator, SwitchSideHash);
        TriggerAnimation(flowerAnimator, SwitchSideHash);
    }

    public override void Stun(float time)
    {
        base.Stun(time);
        flowerAnimator.SetBool(IsStunned, true);
        flowerAnimator.SetTrigger(StunTrigger);
    }

    public override void StunFinished()
    {
        base.StunFinished();
        flowerAnimator.SetBool(IsStunned, false);
    }

    // Call at the animation event, once the switch clip visually lands on the other side.
    public void SwitchSideProgress()
    {
        transform.position = _switchTargetPosition;
        _currentSideIsRight = !_currentSideIsRight;
        _turnsUntilSwitch = Random.Range(minTurnsBetweenSwitch, maxTurnsBetweenSwitch + 1);

        // left side = background, right side = foreground
        gameObject.layer = LayerManager.Instance.GetLayer(!_currentSideIsRight, "Enemy");

        // by default (right side) Jormungandr faces left, so localScale.x is positive there
        Vector3 scale = transform.localScale;
        scale.x = _currentSideIsRight ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);
        transform.localScale = scale;
    }

    public void SwitchSideComplete()
    {
        Debug.Log("[FSM] Switching sides completed!");
        StartAttackStrat();
    }
}
