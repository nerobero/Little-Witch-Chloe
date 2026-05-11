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
    [SerializeField] protected ESpawnType _currentSpell = ESpawnType.FireBall;

    [SerializeField] protected float damageAmount;

    protected EnemyAnimController _animController;
    
    protected float _ATKTimeSnapshot = -1f;
    protected float _chargedATKTimeSnapshot = -1f;

    [Header("Chloe's shoot point")]
    [SerializeField] protected SpriteRenderer _firePointObject;
    [SerializeField] protected Transform _firePoint; // static child gameobject that represents the shoot point
    [SerializeField] protected float _orbitRadius; // the radius of which the shoot point will rotate around Chloe
    protected Vector2 _aimDirection = Vector2.up; // for storing the current aim direction
    protected float _aimAngleDeg = 0f;

    protected bool isBackground;

    public virtual void Attack(GameObject target)
    {
        
    }

    /// <summary>
    /// Rotates and re-positions the shoot-point based on the position of the mouse cursor.
    /// 1. normalizes the direction vector and set the _aimDirection as that value
    /// 2. change the position of the shoot point
    /// 3. rotate the shoot point so that its up direction faces the cursor
    /// </summary>
    /// <param name="dir">the incoming direction</param>
    public void SetAimDirection(Vector2 dir)
    {
        // normalizing the direction vector before multiplying the orbit radius
        Vector2 normalizedDir = Vector2.Normalize(dir);
        _aimDirection = normalizedDir;

        // calculating the aim angle by getting the arctangent and converting the value to degrees
        // (the 2D vector is relative to the positive X axis)
        _aimAngleDeg = Mathf.Atan2(_aimDirection.y, _aimDirection.x) * Mathf.Rad2Deg;

        // setting the shoot point's position based on Chloe's world position:
        _firePoint.position = (Vector2)transform.position + _aimDirection * _orbitRadius;

        // rotating the shoot point so that its up direction faces the cursor
        _firePoint.rotation = Quaternion.Euler(0, 0, _aimAngleDeg - 90f);
    }

    /// <summary>
    /// Shoots normal damage projectiles. 
    /// </summary>
    public virtual void FireNormal()
    {
        // extra sanity check before actually shooting
        // if (!CanCastThisSpell(_currentSpell)) return;

        var projectile = PoolObjectManager.Instance.Get(_currentSpell).GetComponent<ProjectileBase>();
        if (projectile == null) return;

        // taking the time snapshot for checking for inactivity:
        _ATKTimeSnapshot = Time.time;
        // _animController?.SetToIsAttacking(true);
        _animController.SetToIsAttacking();
        projectile.OnFired(_firePoint, _aimAngleDeg, isBackground, gameObject);
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
