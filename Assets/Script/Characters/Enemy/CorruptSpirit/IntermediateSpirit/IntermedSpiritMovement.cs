using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Movement class for corrupted intermediate spirits.
/// The actual blinking logic should be invoked within the animation sequence.
/// </summary>
public class IntermedSpiritMovement : CorruptMovement
{
    //protected new IntermedSpiritAnimController _animController;
    protected IntermedSpiritAnimController _spiritAnimController;

    protected override void Start()
    {
        base.Start();
        _spiritAnimController = _animController as IntermedSpiritAnimController;
    }

    public override void SetMoveDirection(float direction)
    {
        MoveDir = direction;
        _spiritAnimController.FlipCharacter(-MoveDir);
    }
    
    public override void OnBlinkCallback()
    {
        _spiritAnimController.SetToSeenTrans();
    }
}
