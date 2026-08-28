using System;
using UnityEngine;
using FMOD.Studio;
using FMODUnity;


[RequireComponent(typeof(Collider2D))]
public class SoundTriggerOverworld : MonoBehaviour
{
    [SerializeField] private EventReference overworldMusicRef;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log($"[SoundTriggerObjectBog] Hit: {collision.gameObject.name}");
        if (LayerMask.LayerToName(collision.gameObject.layer).Contains("Player"))
        {
            if (!SoundManager.Instance.IsTrackPlaying(overworldMusicRef))
            {
                SoundManager.Instance.PlayOverworld();
            }
        }
    }
}