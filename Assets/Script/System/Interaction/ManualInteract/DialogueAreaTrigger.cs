using UnityEngine;

/// <summary>
/// Automatic dialogue trigger. Starts a dialogue chain the first time the player
/// enters this volume, then stays inert until <see cref="ResetState"/> (level
/// reset). If a dialogue is already playing on entry, the entry is ignored and
/// the trigger stays armed for a later clean entry.
/// </summary>
public class DialogueAreaTrigger : EventTriggerBase, IResetable
{
    [Header("Dialogue")]
    [Tooltip("ID (currentridx) of the first line to play.")]
    [SerializeField] private uint startLineId;

    private bool _consumed;

    private void Start()
    {
        // Make sure the collider is a trigger even if it was left unchecked.
        GetComponent<BoxCollider2D>().isTrigger = true;

        // Register with the current scene so ResetState() runs on level reset.
        LevelManager.Instance.RegisterInstance(this);
    }

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        if (_consumed)
            return;

        // Layer-mask test (supports a mask with more than one player layer).
        if ((playerLayer.value & (1 << other.gameObject.layer)) == 0)
            return;

        if (DialogueTriggerUtil.TryStart(startLineId))
            _consumed = true;
    }

    public void ResetState() => _consumed = false;
}
