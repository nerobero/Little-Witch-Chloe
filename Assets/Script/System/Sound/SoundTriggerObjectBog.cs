using System;
using UnityEngine;
using FMOD.Studio;
using FMODUnity;


[RequireComponent(typeof(Collider2D))]
public class SoundTriggerObjectBog : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log($"[SoundTriggerObjectBog] Hit: {collision.gameObject.name}");
        if (LayerMask.LayerToName(collision.gameObject.layer).Contains("Player"))
        {
            SoundManager.Instance.PlayBogMusic();
        }
    }
}