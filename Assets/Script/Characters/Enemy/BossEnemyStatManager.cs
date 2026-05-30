using UnityEngine;
using Types;
using System;

/// <summary>
/// Separate stat manager for the boss NPCs.
/// </summary>
public class BossEnemyStatManager : StatManager
{
    [SerializeField] protected EElementType subCharacElement;

    public override bool TakeDamage(GameObject instigator, float damageAmount, EElementType damageElement)
    {
        Debug.Log(IsDead);

        if (IsDead || damageAmount <= 0.0f)
            return false;

        float mainDamage = CalculateActualDamage(damageAmount, damageElement, mainCharacElement);
        float subDamage = CalculateActualDamage(damageAmount, damageElement, subCharacElement);
        float actualDamage = Mathf.Max(mainDamage,subDamage);

        currentHP = Mathf.Clamp(currentHP - actualDamage, 0.0f, maxHP);
        FMODUnity.RuntimeManager.PlayOneShot(OnTakenDamageEvent);
        InvokeOnHPChanged(currentHP, maxHP, instigator);

        if (currentHP == 0.0f)
        {
            Death();
        }

        return true;
    }
}
