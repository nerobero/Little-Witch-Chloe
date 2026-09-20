using UnityEngine;
using Types;
using Unity.VisualScripting;

public class ItemInventory : MonoBehaviour
{
    public ItemSlot[] itemSlots;
    public ItemIconDatabase itemIconDatabase;
    public int LastItemIndex = 0;

    public ItemSlot GetSlot(int index) => itemSlots[index];
    public ItemSlot[] GetAllSlots() => itemSlots;
    public int GetSlotCount() => itemSlots.Length;
    public int GetLastItemIndex() => LastItemIndex;

    private void Start()
    {
        int uiSlotCount = UIManager.Instance.Get<InventoryHUD>().itemSlots.Length;
        itemSlots = new ItemSlot[uiSlotCount];
        for (int i = 0; i < itemSlots.Length; i++)
        {
            itemSlots[i] = new ItemSlot();
        }
    }

    public bool AddItem(ECollectable itemType, int amount = 1)
    {
        for(int i = 0; i < LastItemIndex; ++i)
        {
            // already used slot and the same item is in the same slot
            if(itemSlots[i].isSlotUsed && itemSlots[i].item.itemType == itemType)
            {
                itemSlots[i].AddItemToSlot(itemIconDatabase.GetItemData(itemType), amount);
                return true;
            }
        }

        if(LastItemIndex < itemSlots.Length)
        {
            itemSlots[LastItemIndex].AddItemToSlot(itemIconDatabase.GetItemData(itemType), amount);
            LastItemIndex++;
            return true;    
        }

        // inventory is fulled
        return false; 
    }

    public int RemoveItem(ECollectable itemType, int amount = 1)
    {
        for(int i = 0; i < itemSlots.Length; i++)
        {
            if(itemSlots[i].isSlotUsed && itemSlots[i].item.itemType == itemType)
            {
                return itemSlots[i].RemoveItem(amount);
            }
        }

        return 0;
    }

    public void ResetState()
    {
        System.Array.Clear(itemSlots, 0, itemSlots.Length);
    }
}
