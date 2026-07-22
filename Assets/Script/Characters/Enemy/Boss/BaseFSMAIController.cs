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

    private GameObject _currentTarget;

    private IEnemyAttackStrategy _currentStrat;
    
    [Header("Animator - Main body")]
    public Animator _mainAnimator;

    private bool _isActing = false;

    protected int _currentAnimHash;

    [Header("Cooldown time between action turn")]
    [SerializeField] protected float actionCooldown = 1f;
    private float _nextActionTime = 0f;

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

    protected void Update()
    {
        Debug.Log($"[FSM] Update tick, isActing={_isActing}");
        if (_isActing) return; // if already acting, then no need to process the rest of logic.
        if (Time.time < _nextActionTime) return; //if in cooldown for this action turn, return

        StartAttackStrat();
    }

    protected virtual void StartAttackStrat()
    {
        var result = ChooseNextStrategy();
        _currentStrat = result.strat; // may need to return a keyvalue pair rather than IEnemyAttackStrategy
        _currentStrat.OnAttackComplete += HandleAttackEnd;
        _currentAnimHash = result.animTriggerHash;
        _mainAnimator.SetTrigger(_currentAnimHash); // <-- string as the hashed anim trigger 
        _isActing = true;

    }

    public void HandleAttack()
    {
        _currentStrat.Attack(this.gameObject, _currentTarget);
    }

    public void AttackEnd()
    {
        _currentStrat.AttackFinished();
    }

    private void HandleAttackEnd(bool success)
    {
        _isActing = false;
        _currentStrat.OnAttackComplete -= HandleAttackEnd; //unsubscribing since this attack is done.
        _nextActionTime = Time.time + actionCooldown; // start cooldown once the attack ends

        if (!success)
        {
            // processes logic when the attack had been interrupted by a strong attack/stun:
            HandleAttackFail();
        }
    }

    protected virtual void HandleAttackFail() { }

    public void PlaySFX(string eventPath)
    {
        FMODUnity.RuntimeManager.PlayOneShot(eventPath);
    }
}