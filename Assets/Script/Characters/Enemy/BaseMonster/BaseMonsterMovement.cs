using UnityEngine;

public class BaseMonsterMovement : EnemyMovement
{
    protected Vector2 spawnPosition;

    protected override void Start()
    {
        // Memory the spawn point
        spawnPosition = transform.position;
        base.Start();
    }

    public override void Think()
    {
        base.Think();

        if(isArrived)
        {
            //SetMoveDirection(MoveDir);

            float offsetX = MoveDir * Random.Range(minDistance, maxDistance);
            float offsetY = MoveDir * Random.Range(minDistance, maxDistance);
            
            // Make target position in 2~3 grids on the x-y axis from its spawn point.
            targetPosition = spawnPosition + new Vector2(offsetX, offsetY);

            Debug.Log(targetPosition);

            isArrived = false;
        }
    }

    protected override void CheckArrived()
    {
        if(isChasing)
        {
            // Check if the move to the target location is completed
            if(Vector2.Distance(transform.position, targetPosition) <= 0.01f)
            {
                isArrived = true;
                // reset the spawn position to current position
                //spawnPosition = transform.position;
                // Cancel all invoke function
                //CancelInvoke();

                // Think next behavior immediately.
                //Think();
            }
            
        }
        // if it is not chasing
        else
        {
            // compare the x values.
            if(Mathf.Abs(transform.position.x - targetPosition.x) <= 0.01f)
            {
                isArrived = true;
            }
        }
    }

    public override void BlinkToOtherPlatform()
    {
        /*
        'Blinking' is basically the term for teleporting between the foreground and background platforms.
        We may need to have our own calculation system for determining where on the platform Chloe should
        teleport to. 
        */

        base.BlinkToOtherPlatform();
    }
}
