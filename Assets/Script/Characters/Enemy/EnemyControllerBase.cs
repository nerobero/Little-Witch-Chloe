using UnityEngine;
using Types;

[RequireComponent(typeof(SpriteRenderer))]
public class EnemyControllerBase : MonoBehaviour
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

    public bool isAttacking {get; private set;}
    [SerializeField] private float chargeThreshold = 1.5f; // seconds to trigger a charged attack
    [SerializeField] private float maxChargeTime = 3f;
    private float _attackPressTime = -1f;
    [SerializeField] private float loseTargetTime = 4.0f; // time to change idle after missing the target (between 3 and 5 seconds)
    private float _loseTimer = 0f;
    protected bool _hasTarget = false; // Is this monster detected player(target)

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
    [SerializeField] protected float viewHeight;
    [SerializeField] protected float viewAngle;

    [SerializeField] protected float targetTimer = 0f;
    [SerializeField] protected const float FORGET_TIME = 5f;

    #region Setup
    protected virtual void Awake()
    {
        // Caching once, never having to re-fetch again:
        enemyMove = GetComponent<EnemyMovement>();
        enemyAttack = GetComponent<EnemyAttack>();
        enemyStat = GetComponent<EnemyCharacterBase>();
        // _animator = GetComponent<PlayerAnimator>();
    }

    protected virtual void Start()
    {
        enemyState = EMonsterState.Idle;
        enemyMove = GetComponent<EnemyMovement>();
        Invoke("Think", 1);
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
                PlayerDetected(true, hit.gameObject);

                return;
            }
            // if the player is on the same platform
            else
            {
                // 3. Calculate the angle
                // the forward vector
                float cosThreshold = Mathf.Cos(viewAngle * 0.5f * Mathf.Deg2Rad); 

                Vector2 forward = transform.right * enemyMove.MoveDir;
                Vector2 dirToPlayer = (hit.transform.position - transform.position).normalized;

                // Calculate the angle of two vectors
                float dot = Vector2.Dot(forward, dirToPlayer);

                // if the calculated angle is smaller than the half of the view angle
                if(dot > cosThreshold)
                {
                    // the player is in view triangle
                    PlayerDetected(false, hit.gameObject);

                    return;
                }
            }
        }

        // already has target
        if (_hasTarget)
        {
            targetTimer -= Time.deltaTime; // Reduce the timer

            if (targetTimer > 0)
            {
                // remain the timer. => try moving the target location.
                // (필요하다면 여기서 추격 로직을 계속 실행)
                return; 
            }
        }

        // when the timer is over, or there is no target, 
        enemyState = EMonsterState.Patrol;
        if(_hasTarget)
            enemyMove.StopChasing();
        _hasTarget = false;
        return;
    }

    protected void OnDrawGizmos()
    {
        if(enemyMove != null)
        {      
            // 기즈모 색상 설정 (원하는 색으로 변경 가능)
            Gizmos.color = Color.red;

            // OverlapBox와 동일한 위치, 크기, 회전값을 사용하여 박스 그리기
            // eyePoint가 로컬 좌표라면 transform.TransformPoint(eyePoint)를 사용해야 정확합니다.
            Vector3 worldEyePoint = transform.TransformPoint(eyePoint); 
            
            Gizmos.DrawWireCube(worldEyePoint, new Vector3(viewDistance, viewHeight, 0));

            // --- 2. 시야각(FOV) 영역 그리기 (노란 부채꼴) ---
            Gizmos.color = Color.yellow;
            float cosThreshold = Mathf.Cos(viewAngle * 0.5f * Mathf.Deg2Rad); 

            Vector2 forward = transform.right * enemyMove.MoveDir;
        
            // 시야의 양 끝 방향 계산 (Z축 회전)
            Vector3 leftBoundary = Quaternion.Euler(0, 0, viewAngle * 0.5f) * forward;
            Vector3 rightBoundary = Quaternion.Euler(0, 0, -viewAngle * 0.5f) * forward;

            // 양 끝 선 그리기 (길이는 viewDistance 활용)
            Gizmos.DrawRay(worldEyePoint, leftBoundary * viewDistance / 2);
            Gizmos.DrawRay(worldEyePoint, rightBoundary * viewDistance / 2);
            
            // 부채꼴 끝부분 연결 (선택 사항)
            Gizmos.DrawLine(worldEyePoint + leftBoundary * viewDistance / 2, worldEyePoint + rightBoundary * viewDistance / 2);
        }
    }

    protected virtual void PlayerDetected(bool bIsDifferentPlatform, GameObject hit)
    {
        _hasTarget = true;
        targetTimer = FORGET_TIME; // initialize the timer as 5 seconds.
        enemyState = EMonsterState.Chase;
        //enemyMove.targetPosition = hit.transform.position;
        enemyMove.MoveToTarget(hit.transform.position);

        if(bIsDifferentPlatform)
        {

            Debug.Log("Blink");
            enemyMove.BlinkToOtherPlatform();
        }
    }

    // AI behavior
    protected virtual void Think()
    {
        switch(enemyState)
        {
            case EMonsterState.Attack:
                enemyMove.CancelInvoke();

                // HERE ATTACK LOGIC
                FireProjectile();
                Invoke("Think", 2);
            break;
            case EMonsterState.Chase:
                //enemyMove.MoveToTarget();
                Invoke("Think", 2);
            break;
            case EMonsterState.Idle:
                CancelInvoke();
            break;
            default:
                enemyMove.Think();
                Invoke("Think", 2);
            break;

        }
        // // If this enemy's state is attack, then stop move logic and start to attack.
        // if(enemyState == EMonsterState.Attack)
        // {
        // }
        // // else if the state is chase, then start to chase.
        // else if(enemyState == EMonsterState.Chase)
        // {
        // }
        // // else then start to move
        // else
        // {
        // }
    }

    protected virtual void OnBecameVisible()
    {
        Debug.Log("Become Visible");
        enabled = true;
        enemyMove.enabled = true;
        enemyState = EMonsterState.Patrol;
        Think();
    }

    protected virtual void OnBecameInvisible()
    {
        Debug.Log("Become Invisible");
        enabled = false;
        enemyMove.enabled = false;
        enemyState = EMonsterState.Idle;
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

        PlayerDetected(enemyMove.IsBackground == playerMove.IsBackground, instigator);
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
