using UnityEngine;
using Types;

public class EnemyCharacterBase : StatManager
{
    EnemyHPWidget enemyHP;

    protected EnemyControllerBase controller;

    [Header("BodyCollisionDamage")]
    [SerializeField] protected float damage;
    [SerializeField] protected float damageInterval = 1f;
    private float lastDamageTime = 0f;

    protected override void Start()
    {
        base.Start();
        OnTakenDamageEvent = "event:/SFX/EnemyDamaged";
        controller = GetComponent<EnemyControllerBase>();

        enemyHP = GetComponent<EnemyHPWidget>();

        if(enemyHP == null)
        {
            return;
        }
        
        enemyHP.SetTarget();
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        Debug.Log("[Jormungandr]" + collision);
        if (IsDead) return;
        if (LayerMask.LayerToName(collision.gameObject.layer).Contains("Player")
        && Time.time >= lastDamageTime + damageInterval)
        {
            collision.gameObject.GetComponent<PlayerStatManager>().TakeDamageHelper(gameObject, damage, EElementType.None);
            lastDamageTime = Time.time;
        }
    }


    public override bool TakeDamage(GameObject instigator, float damageAmount, EElementType damageElement)
    {
        bool result = base.TakeDamage(instigator, damageAmount, damageElement);

        if(result)
        {
            var animController = GetComponent<EnemyAnimController>();
            
            if(animController == null) return result;

            animController.Hurt();
        }
        return result;
    }

    public override void ResetState()
    {
        base.ResetState();

        enemyHP?.SetTarget();
    }

    // public override void AddBuff(EStatusEffectType type, float magnitude, GameObject instigator)
    // {
    //     controller.ApplyStatusEffect(type, magnitude, instigator);
    // }

    // public override void DispelBuff(EStatusEffectType type, float magnitude)
    // {
    //     controller.RemoveStatusEffect(type, magnitude);
    // }

    // public override void AddDebuff(EStatusEffectType type, float magnitude, GameObject instigator)
    // {
    //     controller.ApplyStatusEffect(type, magnitude, instigator);
    // }

    // public override void RemoveDebuff(EStatusEffectType type, float magnitude)
    // {
    //     controller.RemoveStatusEffect(type, magnitude);
    // }
}
