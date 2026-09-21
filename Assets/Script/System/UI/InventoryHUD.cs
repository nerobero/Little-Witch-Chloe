using System.Linq;
using Types;
using UnityEngine;

public class InventoryHUD : UIBase
{
    [SerializeField] private GameObject herbGroups;
    [SerializeField] private GameObject potionGroups;
    public ItemSlotPanel[] ingredientItemSlots;
    public ItemSlotPanel[] commisionItemSlots;
    public ItemIconDatabase itemIconDatabase;
    public int LastItemIndex = 0;

    [SerializeField] private ItemInventory inventoryData;

    protected override void Awake()
    {
        isStackable = true;
        isPauseable = true;
        isInputDisable = true;

        commisionItemSlots = herbGroups.GetComponentsInChildren<ItemSlotPanel>(true);
        ingredientItemSlots = potionGroups.GetComponentsInChildren<ItemSlotPanel>(true);

        base.Awake();
        Hide();
    }

    // public void Register(ItemSlotPanel itemSlot)
    // {
    //     itemSlots.Append(itemSlot);
    // }

    protected override void SubscribeEvents()
    {
        
    }

    protected override void UnsubscribeEvents()
    {
        
    }

    public override void Show()
    {
        if(PlayerController.Instance != null)
        {
            // if(inventoryData == null)
            // {
            //     inventoryData = PlayerController.Instance.Inventory;
            // }
            PlayerController.Instance.InputContext.UI.Enable();
            PlayerController.Instance.InputContext.BaseInputAction.Disable();
        }
        base.Show();
    }

    public override void Hide()
    {
        base.Hide();
    }

    public void RefreshDisplay()
    {
        for(int i = 0; i < ingredientItemSlots.Length; ++i)
        {
            // already used slot and the same item is in the same slot
            ItemSlot slot = inventoryData.GetIngredientSlot(i);
            ingredientItemSlots[i].Display(slot);
        }

        for(int i = 0; i < commisionItemSlots.Length; ++i)
        {
            // already used slot and the same item is in the same slot
            ItemSlot slot = inventoryData.GetCommisionSlot(i);
            commisionItemSlots[i].Display(slot);
        }
    }

    public bool AddItem(ECollectable itemType)
    {
        if(inventoryData.AddItem(itemType))
        {
            RefreshDisplay();
            return true;
        }
        
        return false;
    }
}
