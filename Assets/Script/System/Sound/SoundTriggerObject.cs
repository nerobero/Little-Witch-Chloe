using System;
using UnityEngine;

/// <summary>
/// Represents the state of the BGM that should be played
/// when triggered by the SoundTriggerObject
/// </summary>
public enum BGMState
{
    Ambience = 0,
    IntroTrans = 1,
    MainLoop = 2, 
    EndTrans = 3,
    End = 4,
}

/// <summary>
/// Collider objects that triggers different phases of the BGM
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class SoundTriggerObject : MonoBehaviour
{
    [SerializeField] private BGMState bgmState;
    private float _globalParam => (float)bgmState;

    // private bool _hasInteracted = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log($"[SoundTriggerObject] Hit: {collision.gameObject.name}");
        if (LayerMask.LayerToName(collision.gameObject.layer).Contains("Player"))
        {
            if (bgmState == BGMState.Ambience)
            {
                SoundManager.Instance.PlayMainMusic();
            }

            if (SoundManager.Instance.GetGlobalValue() != _globalParam)
            {
                SoundManager.Instance.SetGlobalValue(_globalParam);
            }
        }
    }
}