using UnityEngine;

/// <summary>
/// Movement class for corrupted intermediate spirits.
/// The actual blinking logic should be invoked within the animation sequence.
/// </summary>
public class IntermedSpiritMovement : CorruptMovement
{
    private IntermedSpiritAnimController _castedAnimController;
    protected override void Awake()
    {
        base.Awake();
        _castedAnimController = _animController as IntermedSpiritAnimController;
    }

    public override void SetMoveDirection(float direction)
    {
        MoveDir = direction;
        _animController.FlipCharacter(-MoveDir);
    }
    
    public override void MoveToTarget(Vector2 target)
    {
        base.MoveToTarget(target);
        _castedAnimController.SetToIdle();
    }
    public override void OnBlinkCallback()
    {
        _castedAnimController.SetToSeenTrans();
    }
}
