using UnityEngine;
using Types;

public class EnemyCharacterBase : StatManager
{
    EnemyHPWidget enemyHP;

    protected EnemyControllerBase controller;

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

    public override void AddBuff(EStatusEffectType type, float magnitude)
    {
        controller.ApplyStatusEffect(type, magnitude);
    }

    public override void DispelBuff(EStatusEffectType type, float magnitude)
    {
        controller.RemoveStatusEffect(type, magnitude);
    }

    public override void AddDebuff(EStatusEffectType type, float magnitude)
    {
        controller.ApplyStatusEffect(type, magnitude);
    }

    public override void RemoveDebuff(EStatusEffectType type, float magnitude)
    {
        controller.RemoveStatusEffect(type, magnitude);
    }
}
