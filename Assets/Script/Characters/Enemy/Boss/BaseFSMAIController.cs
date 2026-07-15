using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base AIController class. Uses FSM pattern to determine the next move
/// done by the owning agent
/// </summary>
public class BaseFSMAIController : MonoBehaviour
{
    public struct AttackEntry
    {
        public int AnimHash; //already hashed values for optimization reasons
        public IEnemyAttackStrategy Strategy;

        public AttackEntry(int v, IEnemyAttackStrategy attackStrat) : this()
        {
            AnimHash = v;
            Strategy = attackStrat;
        }
    }

    // key = attack strat type
    // value = attack entry (anim hash, strategy)
    protected Dictionary<string, AttackEntry> attacklist = new();


    private IEnemyAttackStrategy _currentStrat;

    private bool _isActing = false;

    private void Awake()
    {
        OnAwake();
    }

    protected virtual void OnAwake()
    {
        Init();    
    }

    protected virtual void Init() { }

    protected virtual (int animTriggerHash, IEnemyAttackStrategy strat) ChooseNextStrategy()
    {
        // TODO: process logic to choose different strategies:
        return (-1, null);
    }

    private void Update()
    {
        if (_isActing) return; // if already acting, then no need to process the rest of logic

        var result = ChooseNextStrategy();
        _currentStrat = result.strat; // may need to return a keyvalue pair rather than IEnemyAttackStrategy
        _currentStrat.OnAttackComplete += HandleAttackEnd;
        // TODO: animator.SetTrigger(result.animTriggerHash); <-- string as the hashed anim trigger 
        _isActing = true;
    }

    private void HandleAttackEnd(bool success)
    {
        _isActing = false;
        _currentStrat.OnAttackComplete -= HandleAttackEnd; //unsubscribing since this attack is done.

        if (!success)
        {
            // processes logic when the attack had been interrupted by a strong attack/stun:
            HandleAttackFail();
        }
    }

    protected virtual void HandleAttackFail() { }
}