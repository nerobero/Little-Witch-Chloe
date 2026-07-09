using UnityEngine;

public class LesserSpiritMovement : CorruptMovement
{
    protected override void Start()
    {
        base.Start();

        offset = new Vector2(_spriteRender.bounds.size.x * MoveDir, 0f);
        jumpOffset = new Vector2(_spriteRender.bounds.size.x * MoveDir / 2f, curJumpHeight);
    }

    // public override void SetMoveDirection(float direction)
    // {
    //     MoveDir = direction;
    //     _animController.FlipCharacter(-MoveDir);
    // }

    public override void StopChasing()
    {
        _animController.SetToIsAttacking(false);
        base.StopChasing();
    }
}
