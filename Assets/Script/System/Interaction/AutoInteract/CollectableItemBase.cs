using Types;
using UnityEngine;

public class CollectableItemBase : ItemBase
{
    [Header("Collectable Item Settings")]
    [SerializeField] protected LayerMask playerLayer;
    [SerializeField] protected bool isBackgroundItem = false;
    public ECollectable CollectType;

    [Header("Collection Systems")]
    [SerializeField] protected bool isCommissionHerb = false;
    [SerializeField] protected bool isLovePotionIngredient = false;

    protected virtual void Awake()
    {
        spawnType = ESpawnType.Collections;
    }

    protected override bool OnInteract(Collider2D other)
    {
        if(!_canInteract) return false;
        
        return OnInteract_Helper(other);
    }

    protected bool OnInteract_Helper(Collider2D other)
    {
        //int layer = (int)Mathf.Log(isBackground ? bgPlayerLayer : fgPlayeLayer, 2);


        //PlayerMovement player = other.GetComponent<PlayerMovement>();

        //if(player == null) return false;

        //if (this.isBackgroundItem != player.IsBackground) return false;
        if(LayerMask.LayerToName(other.gameObject.layer).Contains("Player"))
        {
            if(LayerManager.IsSameSide(gameObject, other.gameObject))
            {
                Debug.Log($"{gameObject}: Pass IsSameSide");
                return OnInteract_HelperImpl(other);
            }
        }

        Debug.Log($"{gameObject}: return OnInteract_Helper() false");
        return false;
    } 

    protected virtual bool OnInteract_HelperImpl(Collider2D other)
    {
        bool collected = DispatchCollectionEvents();

        if (collected)
        {
            FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/pickup");
            UIManager.Instance.Get<InventoryHUD>().AddItem(CollectType);
        }

        return collected;
    }

    /// <summary>
    /// Notifies GameManager of this item's collection for whichever systems
    /// (commission herb / love potion ingredient) it's flagged as belonging to.
    /// Subclasses that override OnInteract_HelperImpl with their own logic
    /// (e.g. FrogCollection) must call this explicitly to still participate.
    /// </summary>
    protected bool DispatchCollectionEvents()
    {
        bool collected = false;

        if (isCommissionHerb)
            collected |= GameManager.Instance.OnCommHerbCollected(CollectType);

        if (isLovePotionIngredient)
            collected |= LovePotionManager.Instance.OnLoveIngredientCollected(CollectType);

        return collected;
    }
}
