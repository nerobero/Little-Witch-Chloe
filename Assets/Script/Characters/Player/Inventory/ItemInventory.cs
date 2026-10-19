using UnityEngine;
using Types;
using Unity.VisualScripting;
using System.Collections.Generic;

public class ItemInventory : MonoBehaviour
{
    public List<ItemSlot> ingredientItemSlots = new List<ItemSlot>();
    public List<ItemSlot> commisionItemSlots  = new List<ItemSlot>();
    public ItemIconDatabase itemIconDatabase;
    public int lastIngredientItemIndex = 0;
    public int lastCommisionItemIndex = 0;

    public ItemSlot GetIngredientSlot(int index) => ingredientItemSlots[index];
    public List<ItemSlot> GetAllIngredientSlots() => ingredientItemSlots;
    public int GetIngredientSlotCount() => ingredientItemSlots.Count;
    public int GetLastIngredientItemIndex() => lastIngredientItemIndex;

    public ItemSlot GetCommisionSlot(int index) => commisionItemSlots[index];
    public List<ItemSlot> GetAllCommisionSlots() => commisionItemSlots;
    public int GetCommisionSlotCount() => commisionItemSlots.Count;
    public int GetLastCommisionItemIndex() => lastCommisionItemIndex;

    private void Start()
    {
        // ingredient slot initialization
        int uiSlotCount = UIManager.Instance.Get<InventoryHUD>().ingredientItemSlots.Length;
        // ingredientItemSlots = new ItemSlot[uiSlotCount];
        for (int i = 0; i < uiSlotCount; i++)
        {

           ingredientItemSlots.Add(new ItemSlot());
            // ingredientItemSlots[i] = new ItemSlot();
        }

        // commisino slot initialization
        uiSlotCount = UIManager.Instance.Get<InventoryHUD>().commisionItemSlots.Length;
        // commisionItemSlots = new ItemSlot[uiSlotCount];
        for (int i = 0; i < uiSlotCount; i++)
        {
            commisionItemSlots.Add(new ItemSlot());
            // commisionItemSlots[i] = new ItemSlot();
        }
    }

    public bool AddItem(ECollectable itemType, int amount = 1)
    {
        switch(itemType)
        {
            // commision herb
            case ECollectable.CommDigestHerb:
            case ECollectable.CommColdHerb:
            case ECollectable.CommFeverHerb:
            {
                for(int i = 0; i < lastCommisionItemIndex; ++i)
                {
                    // already used slot and the same item is in the same slot
                    if(commisionItemSlots[i].isSlotUsed && commisionItemSlots[i].item.itemType == itemType)
                    {
                        commisionItemSlots[i].AddItemToSlot(itemIconDatabase.GetItemData(itemType), amount);
                        return true;
                    }
                }

                if(lastCommisionItemIndex < commisionItemSlots.Count)
                {
                    commisionItemSlots[lastCommisionItemIndex].AddItemToSlot(itemIconDatabase.GetItemData(itemType), amount);
                    lastCommisionItemIndex++;
                    return true;    
                }
            }
            break;

            // love potion ingredient
            case ECollectable.FruitA:
            case ECollectable.FruitB:
            case ECollectable.JorFlower:
            case ECollectable.FireCore:
            case ECollectable.Winterberry:
            {    
                for(int i = 0; i < lastIngredientItemIndex; ++i)
                {
                    // already used slot and the same item is in the same slot
                    if(ingredientItemSlots[i].isSlotUsed && ingredientItemSlots[i].item.itemType == itemType)
                    {
                        ingredientItemSlots[i].AddItemToSlot(itemIconDatabase.GetItemData(itemType), amount);
                        return true;
                    }
                }

                if(lastIngredientItemIndex < ingredientItemSlots.Count)
                {
                    ingredientItemSlots[lastIngredientItemIndex].AddItemToSlot(itemIconDatabase.GetItemData(itemType), amount);
                    lastIngredientItemIndex++;
                    return true;    
                }
            }
            break;
        }

        // inventory is fulled
        return false; 
    }

    public int RemoveItem(ECollectable itemType, int amount = 1)
    {
        for(int i = 0; i < ingredientItemSlots.Count; i++)
        {
            if(ingredientItemSlots[i].isSlotUsed && ingredientItemSlots[i].item.itemType == itemType)
            {
                return ingredientItemSlots[i].RemoveItem(amount);
            }
        }

        return 0;
    }

    /// <summary>
    /// Collects the used slots (commission herbs, then ingredients) for saving.
    /// </summary>
    public List<Data.SavedObjectiveData> GetSavedItems()
    {
        var saved = new List<Data.SavedObjectiveData>();

        foreach(ItemSlot slot in commisionItemSlots)
        {
            if(slot.isSlotUsed)
                saved.Add(new Data.SavedObjectiveData(slot.item.itemType, slot.amount));
        }

        foreach(ItemSlot slot in ingredientItemSlots)
        {
            if(slot.isSlotUsed)
                saved.Add(new Data.SavedObjectiveData(slot.item.itemType, slot.amount));
        }

        return saved;
    }

    /// <summary>
    /// Replaces the current slot contents with the saved ones.
    /// The caller must refresh the InventoryHUD afterwards.
    /// </summary>
    public void LoadSavedItems(List<Data.SavedObjectiveData> savedItems)
    {
        foreach(ItemSlot slot in commisionItemSlots) slot.ClearSlot();
        foreach(ItemSlot slot in ingredientItemSlots) slot.ClearSlot();
        lastCommisionItemIndex = 0;
        lastIngredientItemIndex = 0;

        if(savedItems == null) return;

        foreach(Data.SavedObjectiveData saved in savedItems)
        {
            AddItem(saved.collectableType, saved.collectedCount);
        }
    }

    public void ResetState()
    {
        // Keep the slot lists (only Start() fills them); emptying them would make every later AddItem fail.
        foreach(ItemSlot slot in ingredientItemSlots) slot.ClearSlot();
        foreach(ItemSlot slot in commisionItemSlots) slot.ClearSlot();
        lastIngredientItemIndex = 0;
        lastCommisionItemIndex = 0;

        UIManager.Instance.Get<InventoryHUD>()?.RefreshDisplay();
    }
}
