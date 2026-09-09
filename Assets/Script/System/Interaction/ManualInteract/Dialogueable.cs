using UnityEngine;

/// <summary>
/// Interactable that starts a dialogue chain at <see cref="startLineId"/> when
/// the player interacts with it.
/// </summary>
public class Dialogueable : InteractableBase
{
    [Header("Dialogue")]
    [Tooltip("ID (currentridx) of the first line to play.")]
    [SerializeField] private uint startLineId;

    [Tooltip("If false, the dialogue plays only once until ResetState() is called.")]
    [SerializeField] private bool repeatable = false;

    private bool _consumed;

    protected override void Interact_Impl()
    {
        // Only spend a one-shot trigger if the chain actually began.
        if (DialogueTriggerUtil.TryStart(startLineId))
            _consumed = true;
    }

    public override bool CanInteract()
        => !DialogueSystem.Instance.IsPlaying
           && (repeatable || !_consumed);

    public override void ResetState()
    {
        base.ResetState();
        _consumed = false;
    }
}
