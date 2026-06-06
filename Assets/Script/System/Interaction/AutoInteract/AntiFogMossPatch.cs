using UnityEngine;
using Types;

public class AntiFogMossPatch : CollectableItemBase
{
    
    
    
    private void Awake()
    {
        CollectType = ECollectable.AntiFogMossPatch;
    }

    protected override bool OnInteract_HelperImpl(Collider2D other)
    {
        return base.OnInteract_HelperImpl(other);
    }
}
