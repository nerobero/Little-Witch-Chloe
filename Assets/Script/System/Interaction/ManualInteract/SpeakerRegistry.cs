using System.Collections.Generic;
using Types;
using UnityEngine;

/// <summary>
/// Maps a dialogue speaker's display name (<see cref="Data.DialogueRow.speakerName"/>)
/// and an <see cref="EEmotion"/> to the portrait sprite the dialogue panel shows.
/// Create one asset via Assets > Create > ScriptableObject > SpeakerRegistry and
/// reference it from the dialogue panel.
/// </summary>
[CreateAssetMenu(fileName = "SpeakerRegistry", menuName = "ScriptableObject/SpeakerRegistry")]
public class SpeakerRegistry : ScriptableObject
{
    [System.Serializable]
    public struct EmotionSprite
    {
        public EEmotion emotion;
        public Sprite sprite;
    }

    [System.Serializable]
    public struct Entry
    {
        public string speakerName;
        public EmotionSprite[] portraits;
    }

    [SerializeField] private Entry[] entries;

    // speakerName -> (emotion -> sprite). Built lazily so it survives domain
    // reloads and inspector edits.
    private Dictionary<string, Dictionary<EEmotion, Sprite>> _lookup;

    /// <summary>
    /// Returns the portrait for <paramref name="speakerName"/> at
    /// <paramref name="emotion"/>. Falls back to that speaker's Neutral sprite if
    /// the requested emotion is not authored; returns null (with a warning) if
    /// neither exists or the speaker is unknown.
    /// </summary>
    public Sprite Get(string speakerName, EEmotion emotion)
    {
        if (_lookup == null)
            BuildLookup();

        if (!_lookup.TryGetValue(speakerName, out Dictionary<EEmotion, Sprite> byEmotion))
        {
            Debug.LogWarning($"[SpeakerRegistry] No entry for speaker '{speakerName}'.");
            return null;
        }

        if (byEmotion.TryGetValue(emotion, out Sprite sprite))
            return sprite;

        if (emotion != EEmotion.Neutral && byEmotion.TryGetValue(EEmotion.Neutral, out Sprite neutral))
            return neutral;

        Debug.LogWarning($"[SpeakerRegistry] Speaker '{speakerName}' has no sprite for {emotion} or Neutral.");
        return null;
    }

    private void BuildLookup()
    {
        _lookup = new Dictionary<string, Dictionary<EEmotion, Sprite>>(entries?.Length ?? 0);

        if (entries == null)
            return;

        foreach (Entry entry in entries)
        {
            if (string.IsNullOrEmpty(entry.speakerName))
                continue;

            var byEmotion = new Dictionary<EEmotion, Sprite>();
            if (entry.portraits != null)
            {
                foreach (EmotionSprite portrait in entry.portraits)
                    byEmotion[portrait.emotion] = portrait.sprite;
            }

            _lookup[entry.speakerName] = byEmotion;
        }
    }

    // Drop the cache when the asset is edited in the inspector at play time.
    private void OnValidate() => _lookup = null;
}
