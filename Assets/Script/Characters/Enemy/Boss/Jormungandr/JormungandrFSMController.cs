using UnityEngine;
using Types;

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

    protected override void OnAwake()
    {
        base.OnAwake();

    }
    protected override void Init()
    {
        base.Init();

        attacklist["melee"] = new AttackEntry(
            Animator.StringToHash("Melee"),
            new MeleeAttackStrategy(mouth, radius: 5f, damageAmount: 30f, EElementType.Water)
        );
        attacklist["projectile"] = new AttackEntry(
            Animator.StringToHash("Projectile"),
            new ProjectileAttackStrategy()
        );
        attacklist["summon1"] = new AttackEntry(
            Animator.StringToHash("SummonMushroom"),
            new SummonAttackStrategy(this, mushroomPrefab, shroomSummonTimeInterval, mushroomSummonCount,summonRoot.position)
        );
        attacklist["summon2"] = new AttackEntry(
            Animator.StringToHash("SummonTail"),
            new SummonAttackStrategy(this, tailObject, tailSummonTimeInterval, tailSummonCount, summonRoot.position)
        );
    }
}
