using UnityEngine;
using Types;
using System;

/// <summary>
/// Separate stat manager for the boss NPCs. (body/main Stats)
/// </summary>
public class BossEnemyStatManager : StatManager
{
    [SerializeField] private BossWeakPointStat weakPointStat;

    protected override void Start()
    {
        base.Start();
        weakPointStat.OnDeath += KnockedDown;
        // Should unsubscribe the ondeath event after death finish
    }

    public override bool TakeDamage(GameObject instigator, float damageAmount, EElementType damageElement)
    {
        if (IsDead || damageAmount <= 0.0f)
            return false;

        float actualDamage = CalculateActualDamage(damageAmount, damageElement, mainCharacElement);

        currentHP = Mathf.Clamp(currentHP - actualDamage, 0.0f, maxHP);
        FMODUnity.RuntimeManager.PlayOneShot(OnTakenDamageEvent);
        InvokeOnHPChanged(currentHP, maxHP, instigator);

        if (currentHP == 0.0f)
        {
            Death();
        }

        return true;
    }

    public override void Death()
    {
        base.Death();
        weakPointStat.OnDeath -= KnockedDown;
    }

    public void KnockedDown()
    {
        TakeDamage(this.gameObject, weakPointStat.MaxHP * 2, EElementType.None);
        //Stun();
    }


}
