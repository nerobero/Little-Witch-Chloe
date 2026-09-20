using UnityEngine;
using Types;

public class ItemSlot
{
    public ItemIconEntry item { get; private set; }

    public int amount { get; private set; }
    public bool isSlotUsed { get; private set; }

    public bool AddItemToSlot(ItemIconEntry itemData, int number)
    {
        if(isSlotUsed)
        {
            amount += number;
            return true;
        }

        item = itemData;
        amount = number;
        isSlotUsed = true;
        return true;
    }

    public void ClearSlot()
    {
        item = null;
        amount = 0;
        isSlotUsed = false;
    }

    public int RemoveItem(int number)
    {
        int removed = Mathf.Min(amount, number);
        amount -= removed;

        if(amount <= 0)
        {
            ClearSlot();
        }

        return removed;
    }
}
