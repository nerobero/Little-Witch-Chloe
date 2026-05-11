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
        var stat = other.GetComponent<StatManager>();
        
        return stat.IncreaseMaxHP(healAmount);
    }
}
