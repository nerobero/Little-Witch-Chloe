using UnityEngine;
using Types;

public class IntermedSpiritController : CorruptController
{
    protected override void Think()
    {
        switch (enemyState)
        {
            case EMonsterState.Attack:
                CancelInvoke();
                Attack();
                break;
            case EMonsterState.Chase:
                // enemyMove.MoveToTarget();
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

    protected override void FireProjectile()
    {
        float probability = (float)Random.Range(0, 100) / 100.0f;

        if (probability >= 0.7f)
        {
            enemyAttack.SetAimDirection(enemyMove.targetPosition - (Vector2)transform.position);
            enemyAttack.FireNormal();
        }
        else
        {
            FireCharged(0.0f);
        }
    }
}
