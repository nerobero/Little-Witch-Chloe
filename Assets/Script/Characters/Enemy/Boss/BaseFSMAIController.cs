using System.Collections.Generic;
using UnityEngine;

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
public class BaseFSMAIController : MonoBehaviour
{
    // key = attack strat type
    // value = attack entry (anim hash, strategy)
    protected Dictionary<string, AttackEntry> attacklist = new();

    protected GameObject _currentTarget;

    private IEnemyAttackStrategy _currentStrat;

    [Header("Animator - Main body")]
    public Animator _mainAnimator;
    public Transform _eyePoint;

    private bool _isActing = false;

    private bool _isActive = false;

    protected int _currentAnimHash;

    [Header("Cooldown time between action turn")]
    [SerializeField] protected float actionCooldown = 1f;
    private float _nextActionTime = 0f;

    [Header("Detection")]
    public LayerMask playerLayer;
    [SerializeField] protected float viewDistance;
    [SerializeField] protected float viewHeight;

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

    protected void Update()
    {
        // Debug.Log($"[FSM] Update tick, isActing={_isActing}");
        if (_isActing || !_isActive) return; // if already acting, then no need to process the rest of logic.
        if (Time.time < _nextActionTime) return; //if in cooldown for this action turn, return
        if (_currentTarget == null) return;

        StartAttackStrat();
    }

    protected void FixedUpdate()
    {
        if (!_isActive || _currentTarget != null) return;

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

    public void AttackEnd()
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
}