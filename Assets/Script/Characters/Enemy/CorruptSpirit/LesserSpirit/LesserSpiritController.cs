using UnityEngine;
using Types;

public class LesserSpiritController : CorruptController
{
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
        if(isProjectile)
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

    protected override void FireCharged(float chargeRatio)
    {
        
    }

    protected override void MeleeAttack()
    {
        //enemyMove.MoveToTarget();
    }
}
