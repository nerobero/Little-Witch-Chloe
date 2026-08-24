using System;
using Data;
using UnityEngine;

/// <summary>
/// Singleton manager that keeps track of the current progress of dialogues.
/// </summary>
public class DialogueSystem : MonoSingletonBase<DialogueSystem>
{
    public event Action DialogueEnded;
    public event Action DialogueStarted;
    private DialogueRow[] _dialogueLines;
    private int _currentIndex = 0;
    private string _prevSpeakerName = "";
    private bool _hasDialogueEnded = false;

    protected override void Awake()
    {
        dontDestroy = true;
        base.Awake();
        _dialogueLines = DataTableRegistry.Get<DialogueRow>().Records;
    }

    /// <summary>
    /// Returns the current dialogue line.
    /// If the current dialogue is the end of the dialogue,
    /// then invoke the DialogueEnded Event
    /// </summary>
    /// <returns>a tuple of current speaker and current dialogue line</returns>
    public (string speaker, string dialogueText, bool IsSameSpeaker) ReturnDialogueLine()
    {
        if (_hasDialogueEnded)
        {
            DialogueEnded.Invoke();
            _hasDialogueEnded = false; // resetting it after invoking the event
            return ("", "", false);
        }
        DialogueRow row = _dialogueLines[_currentIndex];
        string currentSpeaker = row.speakerName;
        string currentLine = row.dialogueText;
        return (currentSpeaker, currentLine, currentSpeaker.Equals(_prevSpeakerName));
    }

    /// <summary>
    /// Updates the dialogue index and checks if the next dialogue is the end.
    /// </summary>
    public void UpdateLineIndex()
    {
        DialogueRow row = _dialogueLines[_currentIndex];
        _prevSpeakerName = row.speakerName;
        _currentIndex = (int)row.nextridx;
        _hasDialogueEnded = row.hasDialogueEnded;
    }
}
