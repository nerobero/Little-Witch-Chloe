using UnityEngine;
using Types;
using Unity.VisualScripting;
using System.Collections;

[RequireComponent(typeof(SpriteRenderer))]
public class EnemyControllerBase : MonoBehaviour, IResetable
{
    #region ReferenceClasses
    // Handler for enemy's movement
    protected EnemyMovement enemyMove;
    public EnemyMovement EnemyMove => enemyMove;
    
    // Handler for enemy's stat
    protected EnemyCharacterBase enemyStat;

    // Handler for enemy's attack system
    protected EnemyAttack enemyAttack;

    [SerializeField] protected EMonsterState enemyState;
    #endregion

    [Header("Attack")]
    [SerializeField] protected ESpawnType currentSpell = ESpawnType.FireBall;

    public bool isAttacking {get; protected set;}
    [SerializeField] protected float chargeThreshold = 1.5f; // seconds to trigger a charged attack
    [SerializeField] protected float maxChargeTime = 3f;
    protected float _attackPressTime = -1f;
    [SerializeField] protected float loseTargetTime = 4.0f; // time to change idle after missing the target (between 3 and 5 seconds)
    protected float _loseTimer = 0f;
    protected bool _hasTarget = false; // Is this monster detected player(target)
    [SerializeField] protected bool isProjectile; // Does this monster projectile attack?

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
    [SerializeField] protected float viewHeight;
    [SerializeField] protected float viewAngle;

    [SerializeField] protected float targetTimer = 0f;
    [SerializeField] protected const float FORGET_TIME = 5f;

    protected Coroutine thinkRoutine;
    protected WaitForSecondsTracked _interval;

    #region Setup
    protected virtual void Awake()
    {
        // Caching once, never having to re-fetch again:
        enemyMove = GetComponent<EnemyMovement>();
        enemyMove.OnBlinkFinished += ChangeStateToATKorChase;
        enemyAttack = GetComponent<EnemyAttack>();
        enemyStat = GetComponent<EnemyCharacterBase>();
        _interval = new WaitForSecondsTracked(0f);
        // _animator = GetComponent<PlayerAnimator>();
    }

    protected virtual void Start()
    {
        enemyState = EMonsterState.Idle;
        enemyMove = GetComponent<EnemyMovement>();
    }
    #endregion

    protected virtual void FixedUpdate()
    {
            
        // Detect player as target
        DetectPlayer();
    }

    protected virtual void DetectPlayer()
    {
        Collider2D hit;
        PlayerMovement playerMove = null;
        Vector3 worldEyePoint = transform.TransformPoint(eyePoint); 

        // 1. First detect the player to check the current platform layer of player movement
        hit = Physics2D.OverlapBox(worldEyePoint, new Vector2(viewDistance, viewHeight), 0.0f, playerLayer);
        //DrawDebugBox(eyePoint, new Vector2(viewDistance, viewHeight), Color.green);

        if(hit != null)
        {
            //Debug.Log(hit.gameObject);
            playerMove = hit.GetComponent<PlayerMovement>();
        }

        // 2. When player detected
        if(playerMove != null)
        {
            // if the player is on the different platform.
            if(enemyMove.IsBackground != playerMove.IsBackground)
            {
                PlayerDetected(true, hit.gameObject.transform.position);

                return;
            }
            // if the player is on the same platform
            else
            {
                // 3. Calculate the angle
                // the forward vector
                float cosThreshold = Mathf.Cos(viewAngle * 0.5f * Mathf.Deg2Rad); 

                //Vector2 forward = transform.right * enemyMove.MoveDir;
                Vector2 dirToPlayer = (hit.transform.position - transform.position).normalized;
                Vector2 baseForward = transform.right * enemyMove.MoveDir;
                Vector2 forward = baseForward;
                if (Vector2.Dot(baseForward, dirToPlayer) > 0f) 
                {
                    // Aligns the central axis of the field of view with the player (turns head to look).
                    forward = dirToPlayer; 
                }

                // Calculate the angle of two vectors
                float dot = Vector2.Dot(forward, dirToPlayer);

                // if the calculated angle is smaller than the half of the view angle
                if(dot > cosThreshold)
                {
                    // Check the obstacles(wall) between player and this
                    Vector2 dirToTarget = (hit.gameObject.transform.position - worldEyePoint).normalized;
                    float distsToTarget = Vector2.Distance(hit.gameObject.transform.position, transform.position);

                    LayerMask mask = enemyMove.IsBackground ? LayerMask.GetMask("Background") : LayerMask.GetMask("Foreground");

                    // Do raycast for dectect the obstacles
                    RaycastHit2D rayHit = Physics2D.Raycast(worldEyePoint, dirToTarget, distsToTarget, mask);

                    if(rayHit.collider != null)
                    {
                        // the player is in view triangle
                        PlayerDetected(false, hit.gameObject.transform.position);
                        
                        return;
                    }
                }
            }
        }

        // already has target
        if (_hasTarget)
        {
            targetTimer -= Time.deltaTime; // Reduce the timer

            // the timer is over
            if (targetTimer <= 0)
            {
                // when the timer is over, or there is no target, 
                enemyState = EMonsterState.Patrol;
                enemyMove.shouldStop = false;
                if(_hasTarget)
                {
                    enemyMove.StopChasing();
                    //Think();
                }
                _hasTarget = false;
                return;
            }
        }
    }

    protected void OnDrawGizmos()
    {
        if(enemyMove != null)
        {      
            // Gizmo color settings (can be changed to desired color)
            Gizmos.color = Color.red;

            // Draw a box using the same position, size, and rotation values ​​as the OverlapBox
            // If eyePoint is a local coordinate, you must use transform.TransformPoint(eyePoint) for accuracy.
            Vector3 worldEyePoint = transform.TransformPoint(eyePoint); 
            
            Gizmos.DrawWireCube(worldEyePoint, new Vector3(viewDistance, viewHeight, 0));

            // --- 2. Drawing the Field of View (FOV) area (yellow fan) ---
            Gizmos.color = Color.yellow;
            float cosThreshold = Mathf.Cos(viewAngle * 0.5f * Mathf.Deg2Rad); 

            //Vector2 forward = transform.right * enemyMove.MoveDir;
            Vector2 baseForward = transform.right * enemyMove.MoveDir;
            Vector2 dirToPlayer = ((Vector3)enemyMove.targetPosition - transform.position).normalized;
            Vector2 forward = baseForward;
            if (Vector2.Dot(baseForward, dirToPlayer) > 0f) 
            {
                // Aligns the central axis of the field of view with the player (turns head to look).
                forward = dirToPlayer; 
            }
        
            // Calculate the direction of both ends of the field of view (Rotate Z axis)
            Vector3 leftBoundary = Quaternion.Euler(0, 0, viewAngle * 0.5f) * forward;
            Vector3 rightBoundary = Quaternion.Euler(0, 0, -viewAngle * 0.5f) * forward;

            // Draw lines at both ends (Use viewDistance for length)
            Gizmos.DrawRay(worldEyePoint, leftBoundary * viewDistance / 2);
            Gizmos.DrawRay(worldEyePoint, rightBoundary * viewDistance / 2);
            
            // Connecting the ends of the fan shape
            Gizmos.DrawLine(worldEyePoint + leftBoundary * viewDistance / 2, worldEyePoint + rightBoundary * viewDistance / 2);
        }
    }

    protected virtual void PlayerDetected(bool bIsDifferentPlatform, Vector2 hitPosition)
    {
        _hasTarget = true;
        targetTimer = FORGET_TIME; // initialize the timer as 5 seconds.

        // Remove the delayed function call(think)
        CancelInvoke();

        if(bIsDifferentPlatform)
        {
            enemyMove.targetPosition = hitPosition;
            enemyMove.OnBlinkCallback();
        }
        else
        {
            ChangeStateToATKorChase(hitPosition);
        }
    }

    protected virtual void ChangeStateToATKorChase(Vector2 position)
    {
        enemyMove.AnimController.SetToIsAttacking(true);
        
        if(isProjectile)
        {
            enemyState = EMonsterState.Attack;
            enemyMove.targetPosition = position;
            enemyMove.shouldStop = true;
        }
        else
        {
            enemyState = EMonsterState.Chase;
            enemyMove.MoveToTarget(position);
        }
        
        //Think();
    }

    protected virtual void onAttack()
    {
        if(enemyStat.IsDead) return;
        
        bool shouldFacingLeft = enemyMove.targetPosition.x < transform.position.x;
        // IsFacingRight of enemy is same as is facing left
        if (shouldFacingLeft != enemyMove.AnimController.IsFacingRight)
        {
            Debug.Log("Turn?");
            
            enemyMove.SetMoveDirection(enemyMove.MoveDir * -1);
        }
    }

    protected IEnumerator ThinkRoutine()
    {
        while(!enemyStat.IsDead)
        {
            Think();
            yield return _interval;
        }
    }

    // AI behavior
    protected virtual void Think()
    {
        switch(enemyState)
        {
            case EMonsterState.Attack:
                // HERE ATTACK LOGIC
                FireProjectile();
                //Invoke("Think", 0.5f);
            break;
            case EMonsterState.Chase:
                //enemyMove.MoveToTarget();
                //Invoke("Think", 0.5f);
            break;
            case EMonsterState.Idle:
                //CancelInvoke();
            break;
            default:
                enemyMove.Think();
                //Invoke("Think", 0.5f);
            break;

        }
    }

    protected virtual void OnBecameVisible()
    {
        //Debug.Log("Become Visible");
        enabled = true;
        enemyMove.enabled = true;
        enemyState = EMonsterState.Patrol;
        _interval.Reset(0.5f);
        thinkRoutine = StartCoroutine(ThinkRoutine());
        //Think();
    }

    protected virtual void OnBecameInvisible()
    {
        //Debug.Log("Become Invisible");
        enabled = false;
        enemyMove.enabled = false;
        enemyState = EMonsterState.Idle;
        StopCoroutine(thinkRoutine);
    }

    void OnEnable()
    {
        // subscribe the event: OnHit
        enemyStat.OnHPChanged += OnHit;
    }

    void OnDisable()
    {
        // release subscription for escaping memory leak
        enemyStat.OnHPChanged -= OnHit;
    }

    void OnHit(float currentHP, float maxHP, GameObject instigator)
    {
        // 여기서 타겟을 플레이어로 설정하고 타이머를 5초로 세팅
        var playerMove = instigator.GetComponent<PlayerMovement>();

        if(playerMove == null)
        {
            return;
        }

        bool shouldFacingLeft = instigator.transform.position.x < transform.position.x;
        // IsFacingRight of enemy is same as is facing left
        if (shouldFacingLeft != enemyMove.AnimController.IsFacingRight)
        {
            enemyMove.SetMoveDirection(enemyMove.MoveDir * -1);
        }

        PlayerDetected(enemyMove.IsBackground != playerMove.IsBackground, instigator.transform.position);
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

    public virtual void ResetState()
    {
        gameObject.SetActive(true);
        enemyMove.ResetState();
        enemyStat.ResetState();
        _interval.Reset(0.5f);
        thinkRoutine = StartCoroutine(ThinkRoutine());
    }
}
