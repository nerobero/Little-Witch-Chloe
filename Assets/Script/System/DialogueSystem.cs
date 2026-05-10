using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Singleton manager that keeps track of the current progress of dialogues.
/// </summary>
public class DialogueSystem : MonoBehaviour
{
    struct DialogueRow
    {
        public uint currentridx;
        public string speakerName;
        public string dialogueText;
        public uint nextridx;
        public bool hasDialogueEnded;
    }

    public DialogueSystem Instance => _instance;
    private DialogueSystem _instance;

    [SerializeField] private static readonly TextAsset _dialogueCSV;
    public event Action DialogueEnded;
    private List<DialogueRow> _dialogueLines;
    private int _currentIndex = 0;
    private bool _hasDialogueEnded = false;


    private void Awake()
    {
        if (_instance != this && _instance != null)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            _instance = this;
            ReadDialogueData();
            DontDestroyOnLoad(gameObject);
        }
    }

    /// <summary>
    /// Helper function that reads dialogue CSV data and parses it into DialogueRows
    /// </summary>
    private void ReadDialogueData()
    {
        _dialogueLines = CSVParser.Parse(_dialogueCSV, cols => new DialogueRow
        {
            currentridx = uint.TryParse(cols[0], out uint ridx) ? ridx : 0,
            speakerName = cols[1],
            dialogueText = cols[2],
            nextridx = uint.TryParse(cols[3], out uint next) ? next : 0,
            hasDialogueEnded = bool.TryParse(cols[4], out bool ended) ? ended : false,
        });
    }

    /// <summary>
    /// Returns the current dialogue line.
    /// If the current dialogue is the end of the dialogue,
    /// then invoke the DialogueEnded Event
    /// </summary>
    /// <returns>a tuple of current speaker and current dialogue line</returns>
    public (string speaker, string dialogueText) ReturnDialogueLine()
    {
        if (_hasDialogueEnded)
        {
            DialogueEnded.Invoke();
            _hasDialogueEnded = false; // resetting it after invoking the event
            return ("", "");
        }
        DialogueRow row = _dialogueLines[_currentIndex];
        string currentSpeaker = row.speakerName;
        string currentLine = row.dialogueText;
        return (currentSpeaker, currentLine);
    }

    /// <summary>
    /// Updates the dialogue index and checks if the next dialogue is the end.
    /// </summary>
    public void UpdateLineIndex()
    {
        DialogueRow row = _dialogueLines[_currentIndex];
        _currentIndex = (int)row.nextridx;
        _hasDialogueEnded = row.hasDialogueEnded;
    }
}
