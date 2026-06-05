using Types;
using UnityEngine;

/// <summary>
/// Collectible herb items for Chloe's commissions. 
/// </summary>
public class CommissionHerb : CollectableItemBase
{
    protected override bool OnInteract_HelperImpl(Collider2D other)
    {
        return GameManager.Instance.OnCommHerbCollected(CollectType);
    }
}
