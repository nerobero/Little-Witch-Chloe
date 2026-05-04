using UnityEngine;
using Types;

/// <summary>
/// Class for scroll items, no regeneration logic present.
/// Unlocks a new spell for the player.
/// </summary>
public class ScrollItem : ItemBase
{
    /*
    _spellType = the type of spell that the player will unlock
    base.spawnType = the type of item, this class will be ESpawnType.Scroll
    */
    [SerializeField] private ESpawnType _spellType;

    private void Awake()
    {
        base.spawnType = ESpawnType.ScrollItem;
    }

    /// <summary>
    /// Attempts to unlock this spell for the player upon interaction.
    /// </summary>
    /// <param name="other">the collided gameObject</param>
    /// <returns>true if the new spell has been successfully unlocked</returns>
    protected override bool OnInteract(Collider2D other)
    {
        var playerAttackComp = other.gameObject.GetComponent<PlayerAttack>();
        if (playerAttackComp == null) return false; // cannot get the component, then return false

        return playerAttackComp.UnlockSpell(_spellType);
    }
}
