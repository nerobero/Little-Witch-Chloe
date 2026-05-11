using UnityEngine;

/// <summary>
/// Collectable frog items that increases the player's max HP value
/// </summary>
public class FrogCollection : ItemBase
{
    [Header("Frog Setting")]
    [SerializeField] private float healAmount;
    protected override bool OnInteract(Collider2D other)
    {
        GameManager.Instance.OnFrogCollected();

        var stat = other.GetComponent<StatManager>();
        
        return stat.IncreaseMaxHP(healAmount);
    }
}
