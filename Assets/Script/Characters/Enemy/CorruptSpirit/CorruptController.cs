using UnityEngine;
using Types;

public class CorruptController : EnemyControllerBase
{
    private Transform playerTransform;

    [Header("Chasing Settings")]
    [SerializeField] private float detectionRange = 10f;
    // [SerializeField] private float layerSwitchDelay = 1.5f;
    // private float lastLayerSwitchTime;

    // AI behavior
    protected override void Think()
    {
        // If this enemy's state is attack, then stop move logic and start to attack.
        if(enemyState == EMonsterState.Attack)
        {
            enemyMove.CancelInvoke();

            // HERE ATTACK LOGIC
            Attack();
        }
        // else then start to move
        else
        {
            enemyMove.Think();
        }
    }

    void Attack()
    {
        // Current weapon is projectile
        FireProjectile();

        // Current weapon is melee
        MeleeAttack();
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
