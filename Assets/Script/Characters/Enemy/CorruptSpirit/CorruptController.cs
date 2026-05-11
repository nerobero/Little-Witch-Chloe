using UnityEngine;
using Types;

public class CorruptController : EnemyControllerBase
{
    protected Transform playerTransform;

    [Header("Chasing Settings")]
    [SerializeField] protected float detectionRange = 10f;
    // [SerializeField] private float layerSwitchDelay = 1.5f;
    // private float lastLayerSwitchTime;

    protected override void PlayerDetected(bool bIsDifferentPlatform, GameObject hit)
    {
        _hasTarget = true;
        targetTimer = FORGET_TIME; // initialize the timer as 5 seconds.

        if(bIsDifferentPlatform)
        {

            Debug.Log("Blink");
            enemyMove.BlinkToOtherPlatform();
        }

        if(isProjectile)
        {
            enemyState = EMonsterState.Attack;
            enemyMove.SetMoveDirection(0); // Stop
        }
        else
        {
            enemyState = EMonsterState.Chase;
            enemyMove.MoveToTarget(hit.transform.position);
        }

        //enemyMove.targetPosition = hit.transform.position;
    }

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
