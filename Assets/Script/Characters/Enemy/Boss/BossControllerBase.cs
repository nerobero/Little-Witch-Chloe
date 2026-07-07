using UnityEngine;
using Types;

public class BossControllerBase : EnemyControllerBase
{
    protected new BossAttack enemyAttack;
    
    void OnEnable()
    {
        BossEnemyStatManager bossStat = (BossEnemyStatManager)enemyStat;
        bossStat.onStunned += enemyMove.Stun;
    }

    void OnDisable()
    {
        BossEnemyStatManager bossStat = (BossEnemyStatManager)enemyStat;
        bossStat.onStunned -= enemyMove.Stun;
    }

    protected override void Think()
    {
        switch(enemyState)
        {
            case EMonsterState.Attack:
                // HERE ATTACK LOGIC
                Attack();
                //Invoke("Think", 0.5f);
            break;
            case EMonsterState.Chase:
                //enemyMove.MoveToTarget();
                //Invoke("Think", 0.5f);
            break;
            case EMonsterState.Idle:
                //CancelInvoke();
            break;
            default:
                enemyMove.Think();
                //Invoke("Think", 0.5f);
            break;

        }
    }

    protected virtual void Attack()
    {
        FireProjectile();
    }
}
// Collider hit box script? => hit detect => call damage? Two state synchronize the HP? <= event dispatcher? 