using UnityEngine;
using Types;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "ItemIconDatabase", menuName = "ScriptableObject/ItemIconDatabase")]
public class ItemIconDatabase : ScriptableObject
{
    [SerializeField] private List<ItemIconEntry> entries;

    public Sprite GetItemIcon(ECollectable item)
    {
        return entries.Find(entry => entry.itemType == item)?.itemIcon;
    }

    public ItemIconEntry GetItemData(ECollectable itemType)
    {
        return entries.Find(entry => entry.itemType == itemType);
    }
}
