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

    // protected override bool OnInteract_HelperImpl(Collider2D other)
    // {
    //     bool collected = DispatchCollectionEvents();

    //     if (collected)
    //     {
    //         FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/pickup");
    //         UIManager.Instance.Get<InventoryHUD>().AddItem(CollectType);
    //     }

    //     return collected;
    // }
}
