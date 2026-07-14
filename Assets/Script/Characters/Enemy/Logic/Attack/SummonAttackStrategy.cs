using System;
using UnityEngine;

public class SummonAttackStrategy : IEnemyAttackStrategy
{
    public event Action<bool> OnAttackComplete;

    public bool Attack(GameObject instigator, GameObject target, bool useLastTarget = false)
    {
        throw new NotImplementedException();
    }

    public bool AttackFinished()
    {
        throw new NotImplementedException();
    }

}
