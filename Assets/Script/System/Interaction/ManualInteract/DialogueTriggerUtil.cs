/// <summary>
/// Shared entry point for anything that starts a dialogue chain (the manual
/// <see cref="Dialogueable"/> and the automatic <see cref="DialogueAreaTrigger"/>).
/// Keeps the "only start if nothing is playing, and confirm it actually began"
/// rule in one place; each caller owns its own once/repeat bookkeeping.
/// </summary>
public static class DialogueTriggerUtil
{
    /// <summary>
    /// Starts the chain at <paramref name="startLineId"/> unless a dialogue is
    /// already on screen. Returns true only if the chain actually began
    /// (<see cref="DialogueSystem.StartDialogue"/> no-ops on an unknown ID).
    /// </summary>
    public static bool TryStart(uint startLineId)
    {
        if (DialogueSystem.Instance.IsPlaying)
            return false;

        DialogueSystem.Instance.StartDialogue(startLineId);
        return DialogueSystem.Instance.IsPlaying;
    }
}
