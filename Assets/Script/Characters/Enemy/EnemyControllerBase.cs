using UnityEngine;
using Types;

[RequireComponent(typeof(SpriteRenderer))]
public class EnemyControllerBase : MonoBehaviour
{
    #region ReferenceClasses
    // Reference to input-event binds

    // Handler for player's movement
    protected EnemyMovement enemyMove;

    [SerializeField] protected EMonsterState enemyState;
    #endregion

    [Header("Attack - Input hold duration")]
    [SerializeField] protected ESpawnType currentSpell = ESpawnType.FireBall;

    public bool isAttacking {get; private set;}
    [SerializeField] private float chargeThreshold = 1.5f; // seconds to trigger a charged attack
    [SerializeField] private float maxChargeTime = 3f;
    private float _attackPressTime = -1f;
    [SerializeField] private float loseTargetTime = 4.0f; // time to change idle after missing the target (between 3 and 5 seconds)
    private float _loseTimer = 0f;
    private bool _hasTarget = false; // Is this monster detected player(target)

    public bool enabled = true;

    // Used Time.time instead of Time.deltaTime because
    // the charge logic is directly related to the actual timestamp,
    // which is not something that accumlates PER frame
    public float CurrentChargeRatio =>
        _attackPressTime < 0f ? 0f : Mathf.Clamp01((Time.time - _attackPressTime / maxChargeTime));

    // Jump => Flying transition related variables:
    // [Header("Jump => Flying transition")]
    // [SerializeField] private float flyingThreshold = 1.5f;
    // public bool IsFlying => _isFlying;
    // private bool _isFlying = false;
    // private float _jumpPressTime = -1f;

    [Header("Detection")]
    [SerializeField] protected Vector3 eyePoint;
    [SerializeField] protected LayerMask playerLayer;
    [SerializeField] protected float viewDistance;
    [SerializeField] protected float viewAngle;

    #region Setup
    protected virtual void Awake()
    {
        // Caching once, never having to re-fetch again:
        enemyMove = GetComponent<EnemyMovement>();
        //_playerAttack = GetComponent<PlayerAttack>();
        // _animator = GetComponent<PlayerAnimator>();
    }

    protected virtual void Start()
    {
        enemyState = EMonsterState.Idle;
        Invoke("Think", 1);
    }
    #endregion

    protected void FixedUpdate()
    {
            
        // Detect player as target
        DetectPlayer();
    }

    protected virtual void DetectPlayer()
    {
        Collider2D hit;

        // if the enemy is in background, the detection range is box => different layer with player.
        if(enemyMove.IsBackground)
        {
            hit = Physics2D.OverlapBox(eyePoint, new Vector2(5.0f, 3.0f), 0.0f, playerLayer);

            // Player detected
            if(hit != null)
            {
                _hasTarget = true;
                enemyState = EMonsterState.Chase;
                enemyMove.targetPosition = hit.transform.position;
                enemyMove.BlinkToOtherPlatform();

                return;
            }
        }
        // else if it is in foreground, the range is triangle
        else
        {
            hit = Physics2D.OverlapCircle(eyePoint, viewDistance, playerLayer);

            if(hit != null)
            {
                // 2. Calculate the angle
                // the forward vector
                Vector2 forward = transform.right * enemyMove.MoveDir;
                Vector2 dirToPlayer = (hit.transform.position - transform.position).normalized;

                // Calculate the angle of two vectors
                float angle = Vector2.Angle(forward, dirToPlayer);

                // if the calculated angle is smaller than the half of the view angle
                if(angle < viewAngle * 0.5f)
                {
                    // the player is in view triangle
                    _hasTarget = true;
                    enemyState = EMonsterState.Chase;
                    enemyMove.targetPosition = hit.transform.position;

                    return;
                }
            }
        }

        enemyState = EMonsterState.Patrol;
        _hasTarget = false;
        return;
    }

    // AI behavior
    protected virtual void Think()
    {
        Debug.Log("Monster Cont: Think");
        // If this enemy's state is attack, then stop move logic and start to attack.
        if(enemyState == EMonsterState.Attack)
        {
            enemyMove.CancelInvoke();

            // HERE ATTACK LOGIC
            FireProjectile();
        }
        // else if the state is chase, then start to chase.
        else if(enemyState == EMonsterState.Chase)
        {
            enemyMove.MoveToTarget();
        }
        // else then start to move
        else
        {
            enemyMove.Think();
        }

        Invoke("Think", 2);
    }

    protected virtual void OnBecomeVisible()
    {
        enabled = true;
        enemyMove.IsEnabled = true;
        enemyState = EMonsterState.Patrol;
    }

    protected virtual void OnBecameInvisible()
    {
        enabled = false;
        enemyMove.IsEnabled = false;
        enemyState = EMonsterState.Idle;
    }

    protected virtual void FireProjectile()
    {
        float probability = (float)Random.Range(0, 100) / 100.0f;

        if(probability >= 0.7f)
        {
            FireNormal();
        }
        else
        {
            // Temp
            FireCharged(0.0f);
        }
    }

    protected virtual void FireNormal()
    {
        PoolObjectManager.Instance.Get(currentSpell);
        
    }

    protected virtual void FireCharged(float chargeRatio)
    {
        
    }
}
