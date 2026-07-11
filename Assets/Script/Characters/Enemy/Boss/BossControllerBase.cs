using UnityEngine;
using Types;

public class BossControllerBase : EnemyControllerBase
{
    protected new BossAttack enemyAttack;
    protected EBossState bossState;
    protected GameObject target;
    
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
        switch(bossState)
        {
            case EBossState.Attack:
                // HERE ATTACK LOGIC
                Attack();
                //Invoke("Think", 0.5f);
            break;
            case EBossState.Rampage:
                Rampage();
            break;
            case EBossState.Idle:
                //CancelInvoke();
            break;
            default:
                //enemyMove.Think();
                //Invoke("Think", 0.5f);
            break;

        }
    }

    protected virtual void Attack()
    {
        enemyAttack.Attack(enemyMove.targetPosition);
    }

    protected virtual void Rampage()
    {
        enemyAttack.Rampage();
    }
}
// Collider hit box script? => hit detect => call damage? Two state synchronize the HP? <= event dispatcher? 