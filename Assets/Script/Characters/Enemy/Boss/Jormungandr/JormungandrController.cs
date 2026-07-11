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
        enemyAttack.Attack(enemyMove.targetPosition);
    }
}