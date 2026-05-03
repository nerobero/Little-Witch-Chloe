using UnityEngine;

public class BaseMonsterMovement : EnemyMovement
{
    private Vector2 spawnPosition;

    void Start()
    {
        // Memory the spawn point
        spawnPosition = transform.position;
    }

    public override void Think()
    {
        base.Think();
        float offsetX = MoveDir * Random.Range(minDistance, maxDistance);
        float offsetY = MoveDir * Random.Range(minDistance, maxDistance);
        
        // Make target position in 2~3 grids on the x-y axis from its spawn point.
        targetPosition = spawnPosition + new Vector2(offsetX, offsetY);

        SetMoveDirection(MoveDir);
    }   
}
