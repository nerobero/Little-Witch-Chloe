using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Types;

public class ProjectileAttackStrategy : IEnemyAttackStrategy
{
    public event Action<bool> OnAttackComplete;

    private readonly MonoBehaviour _owner;
    private readonly Transform _firePoint;
    private readonly ESpawnType _projectileType;
    private readonly float _timeInterval;
    private readonly float _damageAmount;
    private readonly int _shootCount;
    private readonly bool _firedAtBackground;

    private Coroutine _projectileRoutine;
    private GameObject _instigator;
    private GameObject _target;

    // projectiles fired by this attack instance that are still in flight;
    // forced back to the pool once the attack completes
    private readonly List<ProjectileBase> _activeProjectiles = new();

    public ProjectileAttackStrategy(MonoBehaviour owner, Transform firePoint, ESpawnType projType,
                                    float timeInterval, float damageAmount, int shootCount,
                                    bool firedAtBackground = false)
    {
        _owner = owner;
        _firePoint = firePoint;
        _projectileType = projType;
        _timeInterval = timeInterval;
        _damageAmount = damageAmount;
        _shootCount = shootCount;
        _firedAtBackground = firedAtBackground;
    }

    public bool Attack(GameObject instigator, GameObject target, bool useLastTarget = false)
    {
        if (_shootCount <= 0) return false;
        if (_projectileRoutine != null) return false; // already shooting
        if (instigator == null || target == null) return false;

        _instigator = instigator;
        _target = target;

        _projectileRoutine = _owner.StartCoroutine(ShootCoroutine());

        return true;
    }

    private IEnumerator ShootCoroutine()
    {
        for (int i = 0; i < _shootCount; i++)
        {
            Shoot();
            yield return new WaitForSeconds(_timeInterval);
        }
        _projectileRoutine = null;
        AttackFinished();
    }

    private void Shoot()
    {
        if (_target == null) return;

        var (_, aimAngle) = SetAimDirection(_firePoint.position, _target.transform.position);

        var projectile = PoolObjectManager.Instance.Get(_projectileType).GetComponent<ProjectileBase>();
        var instigatorStat = _instigator.GetComponent<StatManager>();

        projectile.OnFired(_firePoint, aimAngle, _damageAmount, _firedAtBackground, _instigator, instigatorStat);
        _activeProjectiles.Add(projectile);
    }

    private (Vector2 direction, float angleDeg) SetAimDirection(Vector3 instigatorPos, Vector3 targetPos)
    {
        Vector2 aimDirection = ((Vector2)(targetPos - instigatorPos)).normalized;
        float aimAngleDeg = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg - 90f;
        return (aimDirection, aimAngleDeg);
    }

    public bool AttackFinished()
    {
        // force any projectiles still in flight from this attack back into the pool
        foreach (var projectile in _activeProjectiles)
        {
            if (projectile != null && projectile.gameObject.activeInHierarchy)
                projectile.ResetState();
        }
        _activeProjectiles.Clear();

        OnAttackComplete?.Invoke(true);
        return true;
    }

    public float GetDamageNumber()
    {
        return _damageAmount * _shootCount;
    }
}
