using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlotPanel : UIBase
{
    [SerializeField] private Image itemIcon;
    [SerializeField] private TMP_Text itemCountText;

    protected override void Awake()
    {
        itemIcon.enabled = false;
        itemCountText.SetText("");
        base.Awake();
    }

    protected override void SubscribeEvents()
    {
        
    }

    protected override void UnsubscribeEvents()
    {
        
    }

    public void Display(ItemSlot slot)
    {
        if(slot.isSlotUsed)
        {
            itemIcon.sprite = slot.item.itemIcon;
            itemIcon.enabled = true;
            itemCountText.SetText(slot.amount.ToString());
        }
        else
        {
            itemIcon.enabled = false;
            itemCountText.SetText("");
        }
    }

    public void Clear()
    {
        itemIcon.sprite = null;
        itemIcon.enabled = false;
        itemCountText.SetText("");
    }
}
