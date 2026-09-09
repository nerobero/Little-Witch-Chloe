using System;
using System.Collections.Generic;
using Data;
using Types;
using UnityEngine;

/// <summary>
/// Singleton manager that keeps track of the current progress of dialogues.
/// Lines are looked up by their ID (<see cref="DialogueRow.currentridx"/>) rather
/// than by array position, so a trigger can start a conversation at any line.
/// </summary>
public class DialogueSystem : MonoSingletonBase<DialogueSystem>
{
    public event Action DialogueEnded;
    public event Action DialogueStarted;

    // Every dialogue row, keyed by its line ID for O(1) lookup.
    private readonly Dictionary<uint, DialogueRow> _linesById = new();

    // Last explicit emotion seen for each speaker in the current chain, so a line
    // with EEmotion.Unspecified can keep the speaker's previous expression.
    private readonly Dictionary<string, EEmotion> _emotionBySpeaker = new();

    private uint _currentId;
    private string _prevSpeakerName = "";

    /// <summary>
    /// True while a dialogue chain is on screen. Interaction / player-input code
    /// checks this to block other actions until the chain ends.
    /// </summary>
    public bool IsPlaying { get; private set; }

    protected override void Awake()
    {
        dontDestroy = true;
        base.Awake();

        foreach (DialogueRow row in DataTableRegistry.Get<DialogueRow>().Records)
            _linesById[row.currentridx] = row;
    }

    /// <summary>
    /// Begins a dialogue chain at the given line ID and notifies listeners
    /// (e.g. the dialogue panel) via <see cref="DialogueStarted"/>.
    /// </summary>
    public void StartDialogue(uint startLineId)
    {
        if (!_linesById.ContainsKey(startLineId))
        {
            Debug.LogError($"[DialogueSystem] No dialogue line with ID {startLineId}.");
            return;
        }

        _currentId = startLineId;
        _prevSpeakerName = "";
        _emotionBySpeaker.Clear();
        IsPlaying = true;

        DialogueStarted?.Invoke();
    }

    /// <summary>
    /// Returns the line currently being shown.
    /// </summary>
    /// <returns>current speaker, current line text, whether this speaker is the
    /// same as the previous line's speaker, and the resolved portrait emotion</returns>
    public (string speaker, string dialogueText, bool isSameSpeaker, EEmotion emotion) ReturnDialogueLine()
    {
        DialogueRow row = _linesById[_currentId];
        string currentSpeaker = row.speakerName;
        string currentLine = row.dialogueText;
        return (currentSpeaker, currentLine, currentSpeaker.Equals(_prevSpeakerName), ResolveEmotion(row));
    }

    /// <summary>
    /// Turns a row's raw <see cref="DialogueRow.emotion"/> into a concrete emotion:
    /// an explicit value is used as-is and remembered for that speaker; Unspecified
    /// falls back to the speaker's last explicit emotion, or Neutral if they have none.
    /// </summary>
    private EEmotion ResolveEmotion(DialogueRow row)
    {
        if (row.emotion != EEmotion.Unspecified)
        {
            _emotionBySpeaker[row.speakerName] = row.emotion;
            return row.emotion;
        }

        return _emotionBySpeaker.TryGetValue(row.speakerName, out EEmotion last)
            ? last
            : EEmotion.Neutral;
    }

    /// <summary>
    /// Moves to the next line. If the line just shown was the end of the chain,
    /// ends the dialogue and invokes <see cref="DialogueEnded"/> instead of advancing.
    /// </summary>
    public void Advance()
    {
        DialogueRow row = _linesById[_currentId];
        _prevSpeakerName = row.speakerName;

        if (row.hasDialogueEnded)
        {
            IsPlaying = false;
            DialogueEnded?.Invoke();
            return;
        }

        if (!_linesById.ContainsKey(row.nextridx))
        {
            Debug.LogError($"[DialogueSystem] Line {_currentId} points to missing nextridx {row.nextridx}; ending dialogue. (Stale DialogueLinesData.bytes?)");
            IsPlaying = false;
            DialogueEnded?.Invoke();
            return;
        }

        _currentId = row.nextridx;
    }

    /// <summary>
    /// Walks the chain from the line currently pointed at (call right after
    /// <see cref="StartDialogue"/>) and returns the distinct speaker names in
    /// first-seen order, capped at 2 (the panel's portrait count).
    /// </summary>
    public List<string> GetChainSpeakers()
    {
        var speakers = new List<string>(2);
        var visited = new HashSet<uint>();

        uint id = _currentId;
        while (visited.Add(id) && _linesById.TryGetValue(id, out DialogueRow row))
        {
            if (!speakers.Contains(row.speakerName))
            {
                if (speakers.Count >= 2)
                    Debug.LogWarning($"[DialogueSystem] Chain at ID {_currentId} has 3+ speakers; '{row.speakerName}' will not get a portrait slot.");
                else
                    speakers.Add(row.speakerName);
            }

            if (row.hasDialogueEnded) break;
            id = row.nextridx;
        }

        return speakers;
    }
}
