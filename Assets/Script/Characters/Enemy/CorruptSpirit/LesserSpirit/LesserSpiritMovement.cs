using UnityEngine;

public class LesserSpiritMovement : CorruptMovement
{
    public override void SetMoveDirection(float direction)
    {
        MoveDir = direction;
        _animController.FlipCharacter(-MoveDir);
    }

    public override void StopChasing()
    {
        _animController.SetToIsAttacking(false);
        base.StopChasing();
    }
}
