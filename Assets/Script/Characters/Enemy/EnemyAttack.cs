using UnityEngine;
using Types;
using System.Collections.Generic;

public class EnemyAttack : MonoBehaviour
{
    // private static PlayerAttack _instance;
    // public static PlayerAttack Instance => _instance;
    [Header("Monster's attack stats")]
    [SerializeField] protected float chargeAttackCDtime = 0f;
    [SerializeField] protected float changeSpellCDtime = 0f;
    protected Dictionary<ESpawnType, bool> _spellList = new Dictionary<ESpawnType, bool>()
    {
        {ESpawnType.FireBall, true}, {ESpawnType.WaterBall, true},
        {ESpawnType.PoisonBall, false}, {ESpawnType.ElectricBall, false}, {ESpawnType.LightBall, false}
    };
    protected ESpawnType _currentSpell = ESpawnType.FireBall;

    [SerializeField] protected float damageAmount;

    public virtual void Attack(GameObject target)
    {
        
    }

    /// <summary>
    /// Shoots normal damage projectiles. 
    /// </summary>
    public virtual void FireNormal()
    {
        // extra sanity check before actually shooting
        // if (!CanCastThisSpell(_currentSpell)) return;

        PoolObjectManager.Instance.Get(_currentSpell);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="chargeRatio"></param>
    public virtual void FireCharged(float chargeRatio)
    {

    }

    /// <summary>
    /// Unlocks the given spell by setting the value to be true
    /// </summary>
    /// <param name="unlockedType">the new spell type to attempt unlocking</param>
    /// <returns>true if the spell has been properly unlocked</returns>
    public virtual bool UnlockSpell(ESpawnType unlockedType)
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
    public virtual bool SetCurrentSpell(ESpawnType type)
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
    public virtual bool CanCastThisSpell(ESpawnType type)
    {
        bool value = false;
        return _spellList.TryGetValue(type, out value) && value;
    }
}
