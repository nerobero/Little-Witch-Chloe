using System;
using UnityEngine;

/// <summary>
/// Strategy interface for processing different attack patterns
/// without hardcoding the logic for each attack type inside their controller
/// </summary>
public interface IEnemyAttackStrategy
{
    public event Action<bool> OnAttackComplete;
    public bool Attack(GameObject instigator, GameObject target, bool useLastTarget = false);
    public bool AttackFinished();
}
