using UnityEngine;
using Types;

public class CorruptController : EnemyControllerBase
{
    protected Transform playerTransform;

    [Header("Chasing Settings")]
    [SerializeField] protected float detectionRange = 10f;

    protected virtual void Attack()
    {
        if (isProjectile)
        {
            // Current weapon is projectile
            FireProjectile();
        }
        else
        {
            // Current weapon is melee
            MeleeAttack();
        }
    }

    protected override void FireProjectile()
    {
        float probability = (float)Random.Range(0, 100) / 100.0f;

        if(probability >= 0.7f)
        {
            FireNormal();
        }
        else
        {
            // Temp
            FireCharged(0.0f);
        }
    }

    protected virtual void MeleeAttack()
    {
        
    }

}
