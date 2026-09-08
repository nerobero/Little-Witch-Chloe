using System;
using System.Collections.Generic;
using Data;
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
        IsPlaying = true;

        DialogueStarted?.Invoke();
    }

    /// <summary>
    /// Returns the line currently being shown.
    /// </summary>
    /// <returns>current speaker, current line text, and whether this speaker is
    /// the same as the previous line's speaker</returns>
    public (string speaker, string dialogueText, bool IsSameSpeaker) ReturnDialogueLine()
    {
        DialogueRow row = _linesById[_currentId];
        string currentSpeaker = row.speakerName;
        string currentLine = row.dialogueText;
        return (currentSpeaker, currentLine, currentSpeaker.Equals(_prevSpeakerName));
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

        _currentId = row.nextridx;
    }

    /// <summary>
    /// Walks the chain from <paramref name="startLineId"/> and returns the distinct
    /// speaker names in first-seen order, capped at 2 (the panel's portrait count).
    /// </summary>
    public List<string> GetChainSpeakers(uint startLineId)
    {
        var speakers = new List<string>(2);
        var visited = new HashSet<uint>();

        uint id = startLineId;
        while (visited.Add(id) && _linesById.TryGetValue(id, out DialogueRow row))
        {
            if (!speakers.Contains(row.speakerName))
            {
                if (speakers.Count >= 2)
                    Debug.LogWarning($"[DialogueSystem] Chain at ID {startLineId} has 3+ speakers; '{row.speakerName}' will not get a portrait slot.");
                else
                    speakers.Add(row.speakerName);
            }

            if (row.hasDialogueEnded) break;
            id = row.nextridx;
        }

        return speakers;
    }
}
