using UnityEngine;
using System.Collections.Generic;
using Types;

/// <summary>
/// Processes the player's attack logic here
/// including spell switching, firing normal projectiles,
/// and firing charged attacks.
/// </summary>
public class PlayerAttack : MonoBehaviour
{
    // private static PlayerAttack _instance;
    // public static PlayerAttack Instance => _instance;

    [Header("Chloe's attack stats")]
    [SerializeField] private float chargeAttackCDtime = 0f;
    [SerializeField] private float changeSpellCDtime = 0f;
    private Dictionary<ESpawnType, bool> _spellList = new Dictionary<ESpawnType, bool>()
    {
        {ESpawnType.FireBall, true}, {ESpawnType.WaterBall, true},
        {ESpawnType.PoisonBall, false}, {ESpawnType.ElectricBall, false}, {ESpawnType.LightBall, false}
    };
    private ESpawnType _currentSpell = ESpawnType.FireBall;

    private void Awake()
    {
        // _instance = this;
    }

    /// <summary>
    /// Shoots normal damage projectiles. 
    /// </summary>
    public void FireNormal()
    {
        // extra sanity check before actually shooting
        // if (!CanCastThisSpell(_currentSpell)) return;

        PoolObjectManager.Instance.Get(_currentSpell);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="chargeRatio"></param>
    public void FireCharged(float chargeRatio)
    {

    }

    /// <summary>
    /// Unlocks the given spell by setting the value to be true
    /// </summary>
    /// <param name="unlockedType">the new spell type to attempt unlocking</param>
    /// <returns>true if the spell has been properly unlocked</returns>
    public bool UnlockSpell(ESpawnType unlockedType)
    {
        if (!_spellList.ContainsKey(unlockedType)) return false;

        _spellList[unlockedType] = true;
        return true;
    }

    /// <summary>
    /// Changes the current spell type to the given type.
    /// Returns boolean for any potential visual/auditory feedback.
    /// </summary>
    /// <param name="type">the spell type to attempt to change to.</param>
    /// <returns>true when the spell has been successfully changed</returns>
    public bool SetCurrentSpell(ESpawnType type)
    {
        // if you can't cast this spell, then you can't set to the current spell either.
        if (!CanCastThisSpell(type)) return false;

        // otherwise, set the given type as the current:
        _currentSpell = type;
        return true;
    }

    /// <summary>
    /// Checks if the given spell type can be used by the player.
    /// </summary>
    /// <param name="type">the spell type to check</param>
    /// <returns>true if 1. the spell exists in the dictionary and 2. returned value is true</returns>
    public bool CanCastThisSpell(ESpawnType type)
    {
        bool value = false;
        return _spellList.TryGetValue(type, out value) && value;
    }

    /// <summary>
    /// Unlocks the spell when collided with the spell scroll object.
    /// </summary>
    /// <param name="other">the collider for the scroll object</param>
    private void OnTriggerEnter2D(Collider2D other)
    {
        var interactedSpell = other.gameObject.GetComponent<ScrollItem>();
        if (interactedSpell == null) return; //if cannot get the component, then premature return

        UnlockSpell(interactedSpell.spellType);
    }
}