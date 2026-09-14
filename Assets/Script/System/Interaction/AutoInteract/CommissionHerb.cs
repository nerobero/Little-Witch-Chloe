using Types;
using UnityEngine;

/// <summary>
/// Collectible herb items for Chloe's commissions. 
/// </summary>
public class CommissionHerb : CollectableItemBase
{
    protected override void Awake()
    {
        base.Awake();
        isCommissionHerb = true;
    }
}
