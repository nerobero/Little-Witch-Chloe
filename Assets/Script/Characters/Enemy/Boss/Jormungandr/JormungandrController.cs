using UnityEngine;

public class JormungandrController : BossControllerBase
{
    protected new JormungandrAttack enemyAttack;

    protected override void Think()
    {
        base.Think();
    }

    protected override void Attack()
    {
        if(enemyMove.IsBackground)
        {
            
        }
        // Use Tail Attack
        else
        {
            enemyAttack.TailAttack(enemyMove.targetPosition);
        }
    }
}