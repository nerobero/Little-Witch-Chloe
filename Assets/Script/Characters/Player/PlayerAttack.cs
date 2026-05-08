using UnityEngine;
using System.Collections.Generic;
using Types;
using System;
using Unity.VisualScripting;

/// <summary>
/// Processes the player's attack logic here
/// including spell switching, firing normal projectiles,
/// and firing charged attacks.
/// </summary>
public class PlayerAttack : MonoBehaviour
{

    [Header("Chloe's attack stats")]
    [SerializeField] private float chargeAttackCDtime = 0f;
    [SerializeField] private float changeSpellCDtime = 0f;
    // duration of inactivity before disabling the shoot-point
    [SerializeField] private float attackIdleTime = 0f; 
    // public bool IsAttacking => _isAttacking;
    // private bool _isAttacking = false;
    private float _ATKTimeSnapshot = -1f;
    private float _chargedATKTimeSnapshot = -1f; 

    private Dictionary<ESpawnType, bool> _spellList = new Dictionary<ESpawnType, bool>()
    {
        {ESpawnType.FireBall, true}, {ESpawnType.WaterBall, true},
        {ESpawnType.PoisonBall, false}, {ESpawnType.ElectricBall, false}, {ESpawnType.LightBall, false}
    };
    private ESpawnType _currentSpell = ESpawnType.FireBall;

    [Header("Chloe's shoot point")]
    [SerializeField] private SpriteRenderer _firePointObject;
    [SerializeField] private Transform _firePoint; // static child gameobject that represents the shoot point
    [SerializeField] private float _orbitRadius; // the radius of which the shoot point will rotate around Chloe
    private Vector2 _aimDirection = Vector2.up; // for storing the current aim direction
    private float _aimAngleDeg = 0f;

    private void Awake()
    {
        // at first, the fire point object is invisible until the player starts attacking:
        _firePointObject.enabled = false;
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
    /// Helper function that sets the visibility of the shoot point object
    /// </summary>
    /// <param name="shouldEnable">whether to enable/disable the shoot point</param>
    /// <returns>true if enabled, false if disabled</returns>
    private bool SetEnabledShootPoint(bool shouldEnable)
    {
        if (_firePointObject == null) return false;
        _firePointObject.enabled = shouldEnable;
        return _firePointObject.enabled;
    }

    /// <summary>
    /// Shoots normal damage projectiles. 
    /// </summary>
    public void FireNormal()
    {
        if (SetEnabledShootPoint(true))
        {
            var projectile = PoolObjectManager.Instance.Get(_currentSpell).GetComponent<ProjectileBase>();
            if (projectile == null) return;
            
            // taking the time snapshot for checking for inactivity:
            _ATKTimeSnapshot = Time.time;
            projectile.OnFired(_firePoint, _aimAngleDeg);
        }

    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="chargeRatio"></param>
    public void FireCharged(float chargeRatio)
    {
            
    }

    private void Update()
    {
        //if the player hadn't attacked for some time,
        //then set the shoot point invisible:
        if (_ATKTimeSnapshot >= 0f)
        {
            float idleFor = Time.time - _ATKTimeSnapshot;
            if (idleFor >= attackIdleTime)
                SetEnabledShootPoint(false);
        }
    }

    #region SpellUnlocking
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
    #endregion 
}