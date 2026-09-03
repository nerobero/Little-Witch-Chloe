using System.Collections.Generic;
using UnityEngine;
using Types;

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

/// <summary>
/// Base AIController class. Uses FSM pattern to determine the next move
/// done by the owning agent
/// </summary>
public class BaseFSMAIController : MonoBehaviour, IResetable, IStatusEffect
{
    // key = attack strat type
    // value = attack entry (anim hash, strategy)
    protected Dictionary<string, AttackEntry> attacklist = new();

    protected GameObject _currentTarget;

    private IEnemyAttackStrategy _currentStrat;

    [Header("Animator - Main body")]
    public Animator _mainAnimator;
    protected static readonly int IsStunned = Animator.StringToHash("IsStunned");
    protected static readonly int IsDeadHash = Animator.StringToHash("IsDead");
    // one-shot entry into the Stunned-Intro state; IsStunned (level-based) drives everything after that
    protected static readonly int StunTrigger = Animator.StringToHash("StunTrigger");
    // one-shot entry into the boss's Entrance clip; fired by BossRealmEntranceTrigger (and again on level reset)
    protected static readonly int EntranceTrigger = Animator.StringToHash("Entrance");
    // set true by the Entrance clip's EnableFlower event; cleared on reset so the entrance can replay
    protected static readonly int IdleBool = Animator.StringToHash("Idle");
    public Transform _eyePoint;

    protected bool _isActing = false;

    private bool _isActive = false;
    private bool _isStunned = false;
    private bool _isDeadState = false;

    // Entrance sequence: true while the Entrance clip is playing. Player detection is allowed
    // during this window (so the first attack turn has a valid target) but attack selection is not.
    private bool _inEntrance = false;
    private bool _fightStarted = false;
    private bool _detectionEnabled = false;

    protected int _currentAnimHash;

    [Header("Cooldown time between action turn")]
    [SerializeField] protected float actionCooldown = 1f;
    private float _nextActionTime = 0f, _nextRecoveryTime = 0f;

    [Header("Detection")]
    public LayerMask playerLayer;
    [SerializeField] protected float viewDistance;
    [SerializeField] protected float viewHeight;

    protected BossEnemyStatManager bossStat;

    [Header("BossField")]
    [SerializeField] private Collider2D foregroundPlatform;
    [SerializeField] private Collider2D backgroundPlatform;

    public float ForegroundMinX => foregroundPlatform.bounds.min.x;
    public float ForegroundMaxX => foregroundPlatform.bounds.max.x;
    public float ForegroundY    => foregroundPlatform.bounds.max.y;
    
    public float BackgroundMinX => backgroundPlatform.bounds.min.x;
    public float BackgroundMaxX => backgroundPlatform.bounds.max.x;
    public float BackgroundY    => backgroundPlatform.bounds.max.y;

    [Header("Side switching")]
    protected bool _currentSideIsRight;
    public bool CurrentSideIsRight => _currentSideIsRight;

    private void Awake()
    {
        OnAwake();
    }

    protected virtual void OnAwake()
    {
        Init();
    }

    protected virtual void Init()
    {
        LevelManager.Instance.RegisterInstance(this);
        bossStat = GetComponent<BossEnemyStatManager>();
        bossStat.onStunned += Stun;
        bossStat.OnDeath += HandleDeath;
    }

    // Called once the body's own HP hits 0. Stops the FSM for good and plays the body's Dead animation.
    protected virtual void HandleDeath()
    {
        Debug.Log("[FSM] Boss died");
        _isDeadState = true;
        bossStat.OnDeath -= HandleDeath;

        _currentStrat?.AttackInturrupted();
        _mainAnimator.SetBool(IsDeadHash, true);
    }

    protected virtual (int animTriggerHash, IEnemyAttackStrategy strat) ChooseNextStrategy()
    {
        return (-1, null);
    }

    protected virtual void OnBecameInvisible()
    {
        _isActive = false;
        _currentTarget = null;
    }


    protected virtual List<(string key, float probability)> CalculateProbability(float Temperature)
    {
        var probabilities = new List<(string key, float probability)>();
        return probabilities;
    }

    protected virtual void OnBecameVisible()
    {
        _isActive = true;
    }

    #region Entrance

    /// <summary>
    /// Kicks off the boss's entrance: reveals it via the Entrance animation and lets the FSM
    /// start detecting the player, while holding off attack selection until
    /// <see cref="EntranceComplete"/> fires. Safe to call repeatedly - no-ops once the entrance
    /// has already run (or the boss is dead). Wired from BossRealmEntranceTrigger and re-invoked
    /// by <see cref="ResetState"/> on level reset.
    /// </summary>
    public void BeginEntrance()
    {
        if (_inEntrance || _fightStarted || _isDeadState) return;

        _inEntrance = true;
        _detectionEnabled = true;
        _currentTarget = null;

        PlayEntranceAnimation();
    }

    // Fires the Entrance trigger on the body animator. Overridden by bosses that drive extra
    // animators in parallel (e.g. Jormungandr's flower).
    protected virtual void PlayEntranceAnimation()
    {
        TriggerAnimation(_mainAnimator, EntranceTrigger);
    }

    /// <summary>
    /// Call from an Animation Event on the last frame of the Entrance clip. Ends the entrance
    /// window and hands control to the FSM.
    /// </summary>
    public void EntranceComplete()
    {
        _inEntrance = false;
        _fightStarted = true;
        _isActive = true;
    }

    #endregion

    protected void Update()
    {
        // Debug.Log($"[FSM] Update tick, isActing={_isActing}");
        if (_isDeadState) return;
        if (_isStunned) return;
        if (_inEntrance) return; // entrance animation still playing; no attack selection yet
        if (_isActing || !_isActive) return; // if already acting, then no need to process the rest of logic.
        if (Time.time < _nextActionTime) return; //if in cooldown for this action turn, return
        if (_currentTarget == null) return;

        StartAttackStrat();
    }

    protected void FixedUpdate()
    {
        if ((!_isActive && !_detectionEnabled) || _currentTarget != null) return;

        Collider2D hit;

        Vector3 worldEyePoint = _eyePoint.position;

        hit = Physics2D.OverlapBox(worldEyePoint, new Vector2(viewDistance, viewHeight), 0.0f, playerLayer);


        if (hit != null && hit.GetComponent<PlayerController>() != null)
        {
            _currentTarget = hit.gameObject;
            Debug.Log("[FSM] Player detected!");
            _isActive = true;
        }
    }

#if UNITY_EDITOR
    protected void OnDrawGizmos()
    {
        Vector3 worldEyePoint = _eyePoint.position;
        
        // Overlap Box:
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(worldEyePoint, new Vector3(viewDistance, viewHeight, 0));
        Vector2 forward = -transform.localScale.x * transform.right;

        // Center Axis
        Gizmos.color = Color.cyan;
        Gizmos.DrawRay(worldEyePoint, forward * viewDistance);
    }
#endif

    protected virtual void StartAttackStrat()
    {
        var result = ChooseNextStrategy();
        if (result.strat == null) return;
        _currentStrat = result.strat; // may need to return a keyvalue pair rather than IEnemyAttackStrategy
        _currentStrat.OnAttackComplete += HandleAttackEnd;
        _currentAnimHash = result.animTriggerHash;
        TriggerAnimation(_mainAnimator, _currentAnimHash);
        _isActing = true;

    }

    public void HandleAttack()
    {
        _currentStrat.Attack(this.gameObject, _currentTarget);
    }

    public void AttackComplete()
    {
        Debug.Log($"[FSM] AttackEnd called, _currentStrat={_currentStrat}");
        _currentStrat.AttackFinished();
    }

    private void HandleAttackEnd(bool success)
    {
        Debug.Log($"[FSM] HandleAttackEnd called, success={success}");
        _isActing = false;

        _currentStrat.OnAttackComplete -= HandleAttackEnd; //unsubscribing since this attack is done.
        _nextActionTime = Time.time + actionCooldown; // start cooldown once the attack ends

        if (!success)
        {
            // processes logic when the attack had been interrupted by a strong attack/stun:
            HandleAttackFail();
        }
        // else
        // {
        //     var idle = Animator.StringToHash("Idle");
        //     _mainAnimator.SetBool(idle, true);
        // }
    }

    protected virtual void HandleAttackFail() { }

    public void PlaySFX(string eventPath)
    {
        FMODUnity.RuntimeManager.PlayOneShot(eventPath);
    }

    protected void TriggerAnimation(Animator animController, int animTrig)
    {
        if (animController != null)
            animController.SetTrigger(animTrig);
    }

     #region StatusEffect
    public void ApplyStatusEffect(ActiveStatusEffect effect)
    {
        switch(effect.definition.Type)
        {
            case EStatusEffectType.AttackUp:
            //    enemyAttack.AddDamageMultiplier(effect.definition.Magnitude);
            break;
            case EStatusEffectType.AttackDown:
            //    enemyAttack.ReduceDamageMultiplier(effect.definition.Magnitude);
            break;
            case EStatusEffectType.AttackSpeedUp:
            //    enemyAttack.AddAttackSpeedMultiplier(effect.definition.Magnitude);
            break;
            case EStatusEffectType.AttackSpeedDown:
            //    enemyAttack.ReduceAttackSpeedMultiplier(effect.definition.Magnitude);
            break;
            case EStatusEffectType.MoveSpeedUp:
            //    enemyMove.AddSpeedMultiplier(effect.definition.Magnitude);
            break;
            case EStatusEffectType.Slow:
            //    enemyMove.ReduceSpeedMultiplier(effect.definition.Magnitude);
            break;

            // Not implemented
            case EStatusEffectType.DefenseUp:
            case EStatusEffectType.Shield:
            case EStatusEffectType.DefenseDown:
            break;

            // CC
            // These things make the character not be able to move.
            case EStatusEffectType.Stun:
                Stun(effect.definition.Duration);
            break;
            case EStatusEffectType.Root:
            //    enemyStat.AddCrowdControl(effect.definition.CCType);
            //    enemyMove.SetCouldMove(false);
            break;
            case EStatusEffectType.Knockback:
            //    Vector2 knockbackDir = (transform.position - effect.instigator.transform.position).normalized;
            //    enemyStat.AddCrowdControl(effect.definition.CCType);
            //    enemyMove.ApplyKnockback(knockbackDir, effect.definition.Magnitude);
            break;
            case EStatusEffectType.Fear:
            //    enemyStat.AddCrowdControl(effect.definition.CCType);
            break;
            case EStatusEffectType.Blind:
            //    enemyStat.AddCrowdControl(effect.definition.CCType);
            break;

        }
    }

    //public void RemoveStatusEffect(EStatusEffectType type, float magnitude)
    public void RemoveStatusEffect(ActiveStatusEffect effect)
    {
        switch(effect.definition.Type)
        {
            case EStatusEffectType.AttackUp:
            //    enemyAttack.ReduceDamageMultiplier(effect.definition.Magnitude);
            break;
            case EStatusEffectType.AttackDown:
            //    enemyAttack.AddDamageMultiplier(effect.definition.Magnitude);
            break;
            case EStatusEffectType.AttackSpeedUp:
            //    enemyAttack.ReduceAttackSpeedMultiplier(effect.definition.Magnitude);
            break;
            case EStatusEffectType.AttackSpeedDown:
            //    enemyAttack.AddAttackSpeedMultiplier(effect.definition.Magnitude);
            break;
            case EStatusEffectType.MoveSpeedUp:
            //    enemyMove.ReduceSpeedMultiplier(effect.definition.Magnitude);
            break;
            case EStatusEffectType.Slow:
            //    enemyMove.AddSpeedMultiplier(effect.definition.Magnitude);
            break;

            // Not implemented
            case EStatusEffectType.DefenseUp:
            case EStatusEffectType.Shield:
            case EStatusEffectType.DefenseDown:
            break;
            
            // CC
            case EStatusEffectType.Stun:
                StunFinished();
            break;
            case EStatusEffectType.Root:
            //    enemyStat.RemoveCrowdControl(effect.definition.CCType);
            //    enemyMove.SetCouldMove(true);
            break;
            case EStatusEffectType.Knockback:
            //    enemyStat.RemoveCrowdControl(effect.definition.CCType);
            break;
            case EStatusEffectType.Fear:
            //    enemyStat.RemoveCrowdControl(effect.definition.CCType);
            break;
            case EStatusEffectType.Blind:
            //    enemyStat.RemoveCrowdControl(effect.definition.CCType);
            break;
        }
    }
    #endregion

    public virtual void Stun(float time)
    {
        Debug.Log("[FSM] Stunned");

        _isStunned = true;
        _mainAnimator.SetBool(IsStunned, true);
        _mainAnimator.SetTrigger(StunTrigger);

        _currentStrat?.AttackInturrupted();
    }

    // Called when the status-effect buff timer runs out. Only flips the bool so the
    // Animator can transition Loop -> End; the FSM stays suspended until the End clip's
    // Animation Event calls StunAnimationComplete().
    public virtual void StunFinished()
    {
        Debug.Log("[FSM] Stun Finished (buff expired)");
        _mainAnimator.SetBool(IsStunned, false);
    }

    // Call via Animation Event at the end of the Stun-End clip.
    public virtual void StunAnimationComplete()
    {
        Debug.Log("[FSM] Stun animation complete");
        _isStunned = false;
        _isActing = false;
    }

    public virtual void ResetState()
    {
        bossStat.ResetState();
        bossStat.BuffComp.ResetState();

        ResetEntranceState();
        BeginEntrance();
    }

    // Rewinds the FSM to its pre-fight state so the entrance can replay on level reset.
    // Overridden by bosses with extra animators to clear. Runs before BeginEntrance() so it
    // never wipes the freshly-set Entrance trigger.
    protected virtual void ResetEntranceState()
    {
        _inEntrance = false;
        _fightStarted = false;
        _detectionEnabled = false;
        _isActing = false;
        _isActive = false;
        _isStunned = false;
        _isDeadState = false;
        _currentTarget = null;
        _nextActionTime = 0f;

        if (_mainAnimator != null)
        {
            _mainAnimator.ResetTrigger(EntranceTrigger);
            _mainAnimator.ResetTrigger(StunTrigger);
            _mainAnimator.SetBool(IsDeadHash, false);
            _mainAnimator.SetBool(IsStunned, false);
            _mainAnimator.SetBool(IdleBool, false);
        }
    }
}