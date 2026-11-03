using System.Collections.Generic;
using Types;
using UnityEngine;

[CreateAssetMenu(fileName = "LovePotionIconDatabase", menuName = "ScriptableObject/LovePotionIconDatabase")]
public class LovePotionIconDatabase : ScriptableObject
{
    [SerializeField] private List<LovePotionIconEntry> entries;

    public Sprite GetIconSprite(ECollectable type)
    {
        return entries.Find(entry => entry.objType == type)?.objIcon;
    }
}
