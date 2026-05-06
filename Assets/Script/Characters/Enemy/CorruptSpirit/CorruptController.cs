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
        switch(enemyState)
        {
            case EMonsterState.Attack:

                // HERE ATTACK LOGIC
                Attack();
                Invoke("Think", 2);
            break;
            case EMonsterState.Chase:
                //enemyMove.MoveToTarget();
                Invoke("Think", 2);
            break;
            case EMonsterState.Idle:
                CancelInvoke();
            break;
            default:
                enemyMove.Think();
                Invoke("Think", 2);
            break;

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
