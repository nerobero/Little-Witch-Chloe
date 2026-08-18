using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Types;

public class StatusEffectSlotContainer : UIBase
{
    public RectTransform layoutGroupRect;
    public GameObject slotPrefab;

    private Dictionary<EStatusEffectType, GameObject> ActiveSlots 
        = new Dictionary<EStatusEffectType, GameObject>();


    /// <summary>
    /// Subscribes events from the related systems.  
    /// </summary>
    protected override void SubscribeEvents()
    {
        
    }

    /// <summary>
    /// Unsubscribes events from the related systems.
    /// </summary>
    protected override void UnsubscribeEvents()
    {
        
    }
    

    // Update is called once per frame
    void Update()
    {
        
    }

    
    public void AddNewItem(ActiveStatusEffect Effect)
    {
        // 1. Instantiate the prefab
        GameObject newSlot = Instantiate(slotPrefab);

        // 2. Set parent with worldPositionStays = false
        newSlot.transform.SetParent(layoutGroupRect, false);

        // 3. Force layout rebuild to update positions instantly
        newSlot.GetComponent<UIStatusEffectSlot>().Init(Effect);
        //ActiveSlots[Effect] = newSlot;
        LayoutRebuilder.ForceRebuildLayoutImmediate(layoutGroupRect);
    }
}
