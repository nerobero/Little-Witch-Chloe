using System;
using UnityEngine;

public class ProjectileAttackStrategy : IEnemyAttackStrategy
{
    public event Action<bool> OnAttackComplete;

    public bool Attack(GameObject target, bool useLastTarget = false)
    {
        throw new NotImplementedException();
    }

    public bool AttackFinished()
    {
        throw new NotImplementedException();
    }

}
