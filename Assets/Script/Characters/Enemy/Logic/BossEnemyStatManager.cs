using UnityEngine;
using Types;
using System;

/// <summary>
/// Separate stat manager for the boss NPCs. (body/main Stats)
/// </summary>
public class BossEnemyStatManager : EnemyCharacterBase
{
    [SerializeField] private BossWeakPointStat weakPointStat;
    [SerializeField] private float damage;
    [SerializeField] private float damageInterval = 1f;
    [SerializeField] private float stunDuration = 2f;
    private float lastDamageTime = 0f;

    public Action<float> onStunned;

    protected override void Start()
    {
        base.Start();
        weakPointStat.OnDeath += KnockedDown;
        // Should unsubscribe the ondeath event after death finish
    }

    // public override bool TakeDamage(GameObject instigator, float damageAmount, EElementType damageElement)
    // {
    //     if (IsDead || damageAmount <= 0.0f)
    //         return false;

    //     float actualDamage = CalculateActualDamage(damageAmount, damageElement, mainCharacElement);

    //     currentHP = Mathf.Clamp(currentHP - actualDamage, 0.0f, maxHP);
    //     FMODUnity.RuntimeManager.PlayOneShot(OnTakenDamageEvent);
    //     InvokeOnHPChanged(currentHP, maxHP, instigator);

    //     if (currentHP == 0.0f)
    //     {
    //         Death();
    //     }

    //     return true;
    // }

    void OnCollisionStay2D(Collision2D collision)
    {
        Debug.Log("[Jormungandr]" + collision);
        if (LayerMask.LayerToName(collision.gameObject.layer).Contains("Player")
        && Time.time >= lastDamageTime + damageInterval)
        {
            collision.gameObject.GetComponent<PlayerStatManager>().TakeDamageHelper(gameObject, damage, EElementType.None);
            lastDamageTime = Time.time;
        }
    }

    public override void Death()
    {
        base.Death();
        weakPointStat.OnDeath -= KnockedDown;
        weakPointStat.FinalizeDeath();
    }

    public void KnockedDown()
    {
        Debug.Log("Knockdown?");
        TakeDamage(this.gameObject, weakPointStat.MaxHP * 2, EElementType.None);

        // the self-damage above may have been lethal - don't stun a boss that's already dead,
        // or the Stun trigger fires on top of the Dead state and cancels the death animation
        if (IsDead) return;

        // keeps the weak point's respawn timer in sync with how long the body is actually stunned for
        weakPointStat.SetReactivateDelay(stunDuration);

        ActiveStatusEffect pooledEffect = PoolObjectManager.Instance.GetStatusEffect();
        var stat = GetComponent<StatManager>();
        Debug.Log("stunned");

        StatusEffect effect = new StatusEffect(stat, EStatusEffectType.Stun, EStatusEffectCategory.CrowdControl,
                            ECrowdControlType.Stunned, null, "Stunned", stunDuration, stunDuration);

        pooledEffect.SetEffect(effect);
        pooledEffect.SetInstigator(this.gameObject);

        // 2. Adjust the debuff(stun)
        stat.BuffComp.Add(pooledEffect);
    }

    public override void ResetState()
    {
        base.ResetState();

        weakPointStat.OnDeath += KnockedDown;
        weakPointStat.ResetState();
    }
}
