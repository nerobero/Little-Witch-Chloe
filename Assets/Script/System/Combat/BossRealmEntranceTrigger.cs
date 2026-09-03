using System.Collections;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

/// <summary>
/// Invisible trigger volume placed at the mouth of a boss arena. 
/// </summary>
[RequireComponent(typeof(BoxCollider2D))]
public class BossRealmEntranceTrigger : MonoBehaviour
{
    [Tooltip("Boss FSM controller to wake up when the player enters the realm.")]
    [SerializeField] private BaseFSMAIController boss;

    [Tooltip("GameObjects to SetActive(true) when the boss activates - boss body, flower, water " +
             "splash, etc. The boss's own GameObject is always activated even if not listed here.")]
    [SerializeField] private GameObject[] realmObjects;

    [Header("Timing")]
    [Tooltip("Seconds between the player crossing the trigger and the BGM starting.")]
    [SerializeField] private float bgmDelay = 0f;

    [Tooltip("Seconds between the BGM starting and the boss activating / beginning its entrance.")]
    [SerializeField] private float bossActivateDelay = 0f;

    [Header("Audio")]
    [Tooltip("FMOD event for the boss battle BGM. Left null = no music started here.")]
    [SerializeField] private EventReference bossBGM;

    [Tooltip("Before the boss BGM starts, stop every other event on bus:/MUSIC so the " +
             "previous track (overworld / bog / etc.) doesn't keep playing underneath it.")]
    [SerializeField] private bool stopOtherMusic = true;

    [Tooltip("Let the previous music fade out instead of cutting immediately.")]
    [SerializeField] private bool fadeOutOtherMusic = true;

    private bool _fired;
    private EventInstance _bgmInstance;
    private bool _bgmStarted;

    // Editor convenience: make sure the collider is a trigger when the component is added.
    private void Reset()
    {
        GetComponent<BoxCollider2D>().isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_fired) return;
        if (!LayerMask.LayerToName(other.gameObject.layer).Contains("Player")) return;

        if (boss == null)
        {
            Debug.LogWarning("[BossRealmEntranceTrigger] No boss assigned; ignoring player entry.", this);
            return;
        }

        _fired = true;
        StartCoroutine(RunSequence());
    }

    private IEnumerator RunSequence()
    {
        if (bgmDelay > 0f) yield return new WaitForSeconds(bgmDelay);
        StartBGM();

        if (bossActivateDelay > 0f) yield return new WaitForSeconds(bossActivateDelay);
        ActivateBoss();
    }

    private void StartBGM()
    {
        // Kill whatever was playing first, so it doesn't get caught by the stop below.
        if (stopOtherMusic)
        {
            RuntimeManager.StudioSystem.getBus("bus:/MUSIC", out var musicBus);
            musicBus.stopAllEvents(fadeOutOtherMusic ? FMOD.Studio.STOP_MODE.ALLOWFADEOUT : FMOD.Studio.STOP_MODE.IMMEDIATE);
        }

        if (bossBGM.IsNull) return;

        _bgmInstance = RuntimeManager.CreateInstance(bossBGM);
        _bgmInstance.start();
        _bgmStarted = true;
    }

    private void ActivateBoss()
    {
        for (int i = 0; i < realmObjects.Length; i++)
        {
            if (realmObjects[i] != null) realmObjects[i].SetActive(true);
        }
        boss.gameObject.SetActive(true);

        boss.BeginEntrance();
    }

    private void OnDestroy()
    {
        if (!_bgmStarted) return;
        _bgmInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        _bgmInstance.release();
    }
}
