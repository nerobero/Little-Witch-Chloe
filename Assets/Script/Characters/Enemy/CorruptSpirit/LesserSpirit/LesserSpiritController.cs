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
                AttackStart();
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
}
