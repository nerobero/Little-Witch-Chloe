using System.Collections;
using Types;
using UnityEngine;

/// <summary>
/// Collectable frog items that increases the player's max HP value
/// </summary>
public class FrogCollection : CollectableItemBase
{
    [Header("Frog Setting")]
    [SerializeField] private float healAmount;
    private int playerLayerIndex;
    private Animator _animator;
    private static readonly int IsCollectedTrigHash = Animator.StringToHash("IsCollectedTrig");

    // private static readonly int IsIdleHash = Animator.StringToHash("IsIdle");


    protected override void Awake()
    {
        base.Awake();
        CollectType = ECollectable.FrogCollectible;
        _animator = GetComponent<Animator>();
        // _animator.SetBool(IsIdleHash, true);
    }

    void Start()
    {
        playerLayerIndex = (int)Mathf.Log(playerLayer.value, 2);
    }

    protected override bool OnInteract_HelperImpl(Collider2D other)
    {
        GameManager.Instance.OnFrogCollected();

        var stat = other.GetComponent<StatManager>();

        if (stat == null)
        {
            Debug.Log("Stat null");
            return false;
        }

        return stat.IncreaseMaxHP(healAmount);
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        _animator.SetTrigger(IsCollectedTrigHash);
        OnInteract(collision);
    }

    public void AfterCollectAnimation()
    {

        PoolObjectManager.Instance.Return(spawnType, gameObject);
    }

    public void PlaySFX()
    {
        FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/Frog");
    }
}

