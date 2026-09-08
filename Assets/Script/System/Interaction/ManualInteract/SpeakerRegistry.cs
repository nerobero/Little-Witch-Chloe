using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Maps a dialogue speaker's display name (<see cref="Data.DialogueRow.speakerName"/>)
/// to the portrait sprite the dialogue panel should show for them.
/// Create one asset via Assets > Create > ScriptableObject > SpeakerRegistry and
/// reference it from the dialogue panel.
/// </summary>
[CreateAssetMenu(fileName = "SpeakerRegistry", menuName = "ScriptableObject/SpeakerRegistry")]
public class SpeakerRegistry : ScriptableObject
{
    [System.Serializable]
    public struct Entry
    {
        public string speakerName;
        public Sprite portrait;
    }

    [SerializeField] private Entry[] entries;

    // Built lazily from `entries` so it survives domain reloads / inspector edits.
    private Dictionary<string, Sprite> _lookup;

    /// <summary>
    /// Returns the portrait for <paramref name="speakerName"/>, or null (with a
    /// warning) if no entry matches.
    /// </summary>
    public Sprite Get(string speakerName)
    {
        if (_lookup == null)
            BuildLookup();

        if (_lookup.TryGetValue(speakerName, out Sprite portrait))
            return portrait;

        Debug.LogWarning($"[SpeakerRegistry] No portrait registered for speaker '{speakerName}'.");
        return null;
    }

    private void BuildLookup()
    {
        _lookup = new Dictionary<string, Sprite>(entries?.Length ?? 0);

        if (entries == null)
            return;

        foreach (Entry entry in entries)
        {
            if (string.IsNullOrEmpty(entry.speakerName))
                continue;

            _lookup[entry.speakerName] = entry.portrait;
        }
    }

    // Drop the cache when the asset is edited in the inspector at play time.
    private void OnValidate() => _lookup = null;
}
