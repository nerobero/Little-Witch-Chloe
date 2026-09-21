using UnityEngine;
using Types;
using Unity.VisualScripting;

public class ItemInventory : MonoBehaviour
{
    public ItemSlot[] ingredientItemSlots;
    public ItemSlot[] commisionItemSlots;
    public ItemIconDatabase itemIconDatabase;
    public int lastIngredientItemIndex = 0;
    public int lastCommisionItemIndex = 0;

    public ItemSlot GetIngredientSlot(int index) => ingredientItemSlots[index];
    public ItemSlot[] GetAllIngredientSlots() => ingredientItemSlots;
    public int GetIngredientSlotCount() => ingredientItemSlots.Length;
    public int GetLastIngredientItemIndex() => lastIngredientItemIndex;

    public ItemSlot GetCommisionSlot(int index) => commisionItemSlots[index];
    public ItemSlot[] GetAllCommisionSlots() => commisionItemSlots;
    public int GetCommisionSlotCount() => commisionItemSlots.Length;
    public int GetLastCommisionItemIndex() => lastCommisionItemIndex;

    private void Start()
    {
        // ingredient slot initialization
        int uiSlotCount = UIManager.Instance.Get<InventoryHUD>().ingredientItemSlots.Length;
        ingredientItemSlots = new ItemSlot[uiSlotCount];
        for (int i = 0; i < ingredientItemSlots.Length; i++)
        {
            ingredientItemSlots[i] = new ItemSlot();
        }

        // commisino slot initialization
        uiSlotCount = UIManager.Instance.Get<InventoryHUD>().commisionItemSlots.Length;
        commisionItemSlots = new ItemSlot[uiSlotCount];
        for (int i = 0; i < commisionItemSlots.Length; i++)
        {
            commisionItemSlots[i] = new ItemSlot();
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

                if(lastCommisionItemIndex < commisionItemSlots.Length)
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

                if(lastIngredientItemIndex < ingredientItemSlots.Length)
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
        for(int i = 0; i < ingredientItemSlots.Length; i++)
        {
            if(ingredientItemSlots[i].isSlotUsed && ingredientItemSlots[i].item.itemType == itemType)
            {
                return ingredientItemSlots[i].RemoveItem(amount);
            }
        }

        return 0;
    }

    public void ResetState()
    {
        System.Array.Clear(ingredientItemSlots, 0, ingredientItemSlots.Length);
    }
}
