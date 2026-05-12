using FMOD.Studio;
using FMODUnity;
using UnityEngine;
public class ForestMusicState0 : MonoBehaviour
{
    private SoundManager soundManager;
    private float forestLevelValue = 1f;

    private void Start()
    {
        soundManager = FindFirstObjectByType<SoundManager>();
        if (soundManager == null)
            Debug.LogError("SoundManager not found in scene!");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {


        print("Hit: " + collision.gameObject.name);
        if (collision.gameObject.name == "Player")
        {
            if (soundManager.GetGlobalValue() != 0f)
            {
                soundManager.SetGlobalValue(0f);
            }
        }
    }
}

