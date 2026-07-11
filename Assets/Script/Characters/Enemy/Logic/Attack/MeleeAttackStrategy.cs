using System;
using UnityEngine;

public class MeleeAttackStrategy : IEnemyAttackStrategy
{   
    private Transform startPoint;

    

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
