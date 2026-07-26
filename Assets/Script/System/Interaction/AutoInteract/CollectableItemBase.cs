using Types;
using UnityEngine;

public class CollectableItemBase : ItemBase
{
    [Header("Collectable Item Settings")]
    [SerializeField] protected LayerMask playerLayer;
    [SerializeField] protected bool isBackgroundItem = false;
    public ECollectable CollectType;
    
    protected virtual void Awake()
    {
        spawnType = ESpawnType.Collections;
    }

    protected override bool OnInteract(Collider2D other)
    {
        return OnInteract_Helper(other);
    }

    protected bool OnInteract_Helper(Collider2D other)
    {
        int layer = (int)Mathf.Log(isBackground ? bgPlayerLayer : fgPlayeLayer, 2);

        if(other.gameObject.layer != layer) return false;

        PlayerMovement player = other.GetComponent<PlayerMovement>();

        if(player == null) return false;

        if (this.isBackgroundItem != player.IsBackground) return false;

        return OnInteract_HelperImpl(other);
    } 

    protected virtual bool OnInteract_HelperImpl(Collider2D other)
    {
        return true;
    }
}
