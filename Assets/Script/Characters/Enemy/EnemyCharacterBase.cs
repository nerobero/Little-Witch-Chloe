using UnityEngine;
using Types;

public class EnemyCharacterBase : StatManager
{
    protected override void Start()
    {
        base.Start();
        OnTakenDamageEvent = "event:/SFX/EnemyDamaged";

        var enemyHP = GetComponent<EnemyHPWidget>();

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
}
