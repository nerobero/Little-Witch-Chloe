using UnityEngine;

/// <summary>
/// Movement class for corrupted intermediate spirits.
/// The actual blinking logic should be invoked within the animation sequence.
/// </summary>
public class IntermedSpiritMovement : CorruptMovement
{
    protected new IntermedSpiritAnimController _animController;

    public override void SetMoveDirection(float direction)
    {
        MoveDir = direction;
        _animController.FlipCharacter(-MoveDir);
    }
    
    public override void OnBlinkCallback()
    {
        _animController.SetToSeenTrans();
    }
}
