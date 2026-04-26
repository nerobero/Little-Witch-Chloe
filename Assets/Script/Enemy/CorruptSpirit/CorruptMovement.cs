using UnityEngine;

public class CorruptMovement : EnemyMovement
{
    public override void Think()
    {
        base.Think();
        // float offsetX = MoveDir * Random.Range(minDistance, maxDistance);
        // float offsetY = MoveDir * Random.Range(minDistance, maxDistance);
        
        // // Make target position in 2~3 grids on the x-y axis from its spawn point.
        // Vector2 targetPosition = spawnPosition + new Vector2(offsetX, offsetY);

        // SetMoveDirection(MoveDir);
    }
}
