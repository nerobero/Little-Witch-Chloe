using UnityEngine;
using Types;

/// <summary>
/// Separate stat manager for the boss NPCs. (weakpoint Stats)
/// </summary>
public class BossWeakPointStat : EnemyCharacterBase
{
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

        // Call reactivate function after 10 seconds for reactivate the weak point.
        Invoke("Reactivate", 10);

        gameObject.SetActive(false); // Use this logic until our animation asset is ready?
    }

    private void Reactivate()
    {
        gameObject.SetActive(true);
        IsDead = false;
        currentHP = maxHP;
    }
}