using UnityEngine;
using Types;

public class MushroomMineController : BaseMonsterController
{
    BoxCollider2D triggerCollider;
    protected override void Start()
    {
        base.Start();

        triggerCollider = GetComponent<BoxCollider2D>();
    }
    protected override void Think()
    {
        switch(enemyState)
        {
            case EMonsterState.Attack:
                // HERE ATTACK LOGIC
                FireProjectile();
                Invoke("Think", 2);
            break;
            case EMonsterState.Chase:
                //enemyMove.MoveToTarget();
                Invoke("Think", 2);
            break;
            case EMonsterState.Idle:
                CancelInvoke();
            break;

        }
    }

    protected override void PlayerDetected(bool bIsDifferentPlatform, Collider2D hit)
    {
        _hasTarget = true;
        enemyState = EMonsterState.Chase;
        enemyMove.targetPosition = hit.transform.position;
    }
}
