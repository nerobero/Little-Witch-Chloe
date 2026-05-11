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

    public override void OnBlinkCallback()
    {
        _castedAnimController.SetToSeenTrans();
    }
}
