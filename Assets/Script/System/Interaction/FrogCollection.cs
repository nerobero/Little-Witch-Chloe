using UnityEngine;

/// <summary>
/// Collectable frog items that increases the player's max HP value
/// </summary>
public class FrogCollection : ItemBase
{
    [Header("Frog Setting")]
    [SerializeField] private float healAmount;
    [SerializeField] private LayerMask playerLayer;
    private int playerLayerIndex;

    void Start()
    {
        playerLayerIndex = (int)Mathf.Log(playerLayer.value, 2);
    }

    protected override bool OnInteract(Collider2D other)
    {
        if(other.gameObject.layer != playerLayerIndex)
        {
            return false;
        }
     
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

