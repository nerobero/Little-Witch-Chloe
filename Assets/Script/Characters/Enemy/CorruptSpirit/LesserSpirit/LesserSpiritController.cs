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
