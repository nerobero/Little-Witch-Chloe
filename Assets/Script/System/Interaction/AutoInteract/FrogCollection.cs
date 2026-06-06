using Types;
using UnityEngine;

/// <summary>
/// Collectable frog items that increases the player's max HP value
/// </summary>
public class FrogCollection : CollectableItemBase
{
    [Header("Frog Setting")]
    [SerializeField] private float healAmount;
    // private int playerLayerIndex;

    private void Awake()
    {
        CollectType = ECollectable.FrogCollectible;
    }

    void Start()
    {
        playerLayerIndex = (int)Mathf.Log(playerLayer.value, 2);
    }

    protected override bool OnInteract_HelperImpl(Collider2D other)
    {
        GameManager.Instance.OnFrogCollected();

        var stat = other.GetComponent<StatManager>();

        if(stat == null)
        {
            Debug.Log("Stat null");
            return false;
        }
        
        return stat.IncreaseMaxHP(healAmount);
    }
}

