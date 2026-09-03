using UnityEngine;

/// <summary>
/// Invisible trigger volume placed at the mouth of a boss arena. The first time the player
/// crosses it, it activates the boss's GameObject(s) - which starts the Entrance clip, since
/// that is the Animator's default state - and tells the FSM to run its entrance sequence
/// (player detection on, attacks held until the clip's <c>EntranceComplete()</c> event fires).
///
/// One-shot: this only fires once per play session. The death / level-reset replay of the
/// entrance is driven by the FSM's own <see cref="BaseFSMAIController.ResetState"/> (the boss
/// stays active after the first entry), so this is intentionally not <c>IResetable</c>.
///
/// Camera lock and the arena's invisible wall are handled by their own separate triggers
/// (see CameraLockTrigger / BossLoading).
/// </summary>
[RequireComponent(typeof(BoxCollider2D))]
public class BossRealmEntranceTrigger : MonoBehaviour
{
    [Tooltip("Boss FSM controller to wake up when the player enters the realm.")]
    [SerializeField] private BaseFSMAIController boss;

    [Tooltip("GameObjects to SetActive(true) on entry - boss body, flower, water splash, etc. " +
             "The boss's own GameObject is always activated even if not listed here.")]
    [SerializeField] private GameObject[] realmObjects;

    private bool _fired;

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

        for (int i = 0; i < realmObjects.Length; i++)
        {
            if (realmObjects[i] != null) realmObjects[i].SetActive(true);
        }
        boss.gameObject.SetActive(true);

        boss.BeginEntrance();
    }
}
