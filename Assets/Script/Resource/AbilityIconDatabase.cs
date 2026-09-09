using System.Collections.Generic;
using Types;
using UnityEngine;

[CreateAssetMenu(fileName = "AbilityIconDatabase", menuName = "ScriptableObject/AbilityIconDatabase")]
public class AbilityIconDatabase : ScriptableObject
{
    [SerializeField] private List<AbilityIconEntry> entries;

    public Sprite GetAbilityIcon(EAbilityType ability)
    {
        return entries.Find(entry => entry.abilityType == ability)?.abilityIcon;
    }

    public Sprite GetKeyIcon(EAbilityType ability)
    {
        return entries.Find(entry => entry.abilityType == ability)?.keyIcon;
    }
}
