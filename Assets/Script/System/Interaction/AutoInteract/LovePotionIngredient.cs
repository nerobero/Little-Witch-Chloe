using UnityEngine;

/// <summary>
/// Collectible ingredient items for the love potion.
/// </summary>
public class LovePotionIngredient : CollectableItemBase
{
    protected override void Awake()
    {
        base.Awake();
        isLovePotionIngredient = true;
    }
}
