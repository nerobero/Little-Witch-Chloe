using UnityEngine;
using Types;

public class CorruptController : EnemyControllerBase
{
    protected Transform playerTransform;

    [Header("Chasing Settings")]
    [SerializeField] protected float detectionRange = 10f;

    protected virtual void AttackStart()
    {
        bool shouldFacingLeft = enemyMove.targetPosition.x < transform.position.x;
        // IsFacingRight of enemy is same as is facing left
        if (shouldFacingLeft != enemyMove.AnimController.IsFacingRight)
        {
            enemyMove.SetMoveDirection(enemyMove.MoveDir * -1);
        }

        enemyMove.AnimController.SetToIsAttacking();
    }

    protected virtual void onAttack()
    {
        bool shouldFacingLeft = enemyMove.targetPosition.x < transform.position.x;
        // IsFacingRight of enemy is same as is facing left
        if (shouldFacingLeft != enemyMove.AnimController.IsFacingRight)
        {
            Debug.Log("Turn?");
            
            enemyMove.SetMoveDirection(enemyMove.MoveDir * -1);
        }

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
            enemyAttack.SetAimDirection(enemyMove.targetPosition - (Vector2)transform.position);
            enemyAttack.FireNormal();
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
