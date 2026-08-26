using System;
using Types;
using Unity.VisualScripting;
using UnityEngine;

public class MeleeAttackStrategy : IEnemyAttackStrategy
{   
    private Transform _startPoint;

    private float _radius = 2f;

    private float _halfHeight = 2f;

    private bool _isCapsule = false;

    private float _damageAmount = 0f;

    private EElementType _damageType;

    public MeleeAttackStrategy(Transform startPoint, float radius, float damageAmount, EElementType damageType)
    {
        _startPoint = startPoint;
        _radius = radius;
        _halfHeight = 0f;
        _damageAmount = damageAmount;
        _damageType = damageType;
    }

    public MeleeAttackStrategy(Transform startPoint, float radius, float halfHeight, float damageAmount, EElementType damageType)
    {
        _startPoint = startPoint;
        _radius = radius;
        _halfHeight = halfHeight;
        _isCapsule = true;
        _damageAmount = damageAmount;
        _damageType = damageType;
    }

    public event Action<bool> OnAttackComplete;

    public bool Attack(GameObject instigator, GameObject target, bool useLastTarget = false)
    {
        Collider2D collided;
        if (_isCapsule)
        {
            Vector2 capsuleSize = new Vector2(_radius, _halfHeight);
            collided = Physics2D.OverlapCapsule(_startPoint.position, capsuleSize, CapsuleDirection2D.Horizontal, 0f);
        }
        else
        {
            collided = Physics2D.OverlapCircle(_startPoint.position, _radius);
        }

        if (collided == null) return false;

        GameObject hitObject = useLastTarget ? target : collided.gameObject;
        if (!LayerManager.IsSameSide(instigator, hitObject)) return false;

        var stat = hitObject.GetComponent<StatManager>();

        if (stat == null) return false;

        stat.TakeDamage(instigator, _damageAmount, _damageType);

        return true;
    }

     // just dummy
    public void ChangeTargetPosition(Vector3 position)
    {
        
    }

    public bool AttackFinished()
    {
        Debug.Log("AttackFinished called?");
        OnAttackComplete?.Invoke(true);
        return true;
    }

    public bool AttackInturrupted()
    {
        OnAttackComplete?.Invoke(false);
        return true;
    }

    public float GetDamageNumber()
    {
        return _damageAmount;
    }
}
