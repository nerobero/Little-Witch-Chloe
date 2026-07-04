using UnityEngine;
using Types;

public class BossControllerBase : EnemyControllerBase
{
    void OnEnable()
    {
        BossEnemyStatManager bossStat = (BossEnemyStatManager)enemyStat;
        bossStat.OnStunned += enemyMove.Stun;
    }

    void OnDisable()
    {
        BossEnemyStatManager bossStat = (BossEnemyStatManager)enemyStat;
        bossStat.OnStunned -= enemyMove.Stun;
    }

    protected override void Think()
    {
        switch(enemyState)
        {
            case EMonsterState.Attack:
                // HERE ATTACK LOGIC
                FireProjectile();
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
}
// Collider hit box script? => hit detect => call damage? Two state synchronize the HP? <= event dispatcher? 