using UnityEngine;

/// <summary>
/// Base class for enemy movement and physics.
/// Handles velocity, ground detection, and basic movement.
/// Specific monster types inherit and implement their own AI behavior.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class EnemyMovement : MonoBehaviour
{
    // These values are exposed states for others to read:
    public bool IsGrounded => IsOnGround();
    public float MoveDir { get; protected set; }

    public bool IsEnabled = true;

    [Header("Movement values")]
    [SerializeField] protected float speed;
    [SerializeField] protected float jumpHeight;
    [SerializeField] protected float flyForce;
    [SerializeField] protected float minDistance = 2f; // minimum 2 grid
    [SerializeField] protected float maxDistance = 3f; // maximum 3 grid

    [Header("Ground Detection")]
    [SerializeField] protected float groundCheckDistance = 0.5f;
    [SerializeField] protected float obstacleDistance = 0.5f;
    //[SerializeField] protected LayerMask platformLayer = gameObject.layer;

    [SerializeField] protected LayerMask bgLayer;
    [SerializeField] protected LayerMask fgLayer;

    protected int platformLayer => gameObject.layer;
    protected int _bgLayerIndex => (int)Mathf.Log(bgLayer.value, 2);
    protected int _fgLayerIndex => (int)Mathf.Log(fgLayer.value, 2);
    
    [SerializeField] protected bool _isBackground = false; 
    public bool IsBackground => _isBackground;
    SpriteRenderer sr;
    protected EnemyAnimController _animController;
    protected int orderInLayer;

    [Header("Patrol Settings")]
    public Vector2 targetPosition;
    public bool isChasing;

    // Physics body for 2D object
    protected Rigidbody2D rb;
    // enemy's sprite renderer:
    protected SpriteRenderer _spriteRender;


    // @TODO: Add a serialized private/public PlayerAnimControl class reference here

    protected virtual void Awake()
    {
        // makes sure that we auto-get the reference for the rigidbody at runtime:
        rb = GetComponent<Rigidbody2D>();
        
        Physics2D.IgnoreLayerCollision(platformLayer, _bgLayerIndex, !_isBackground);
        Physics2D.IgnoreLayerCollision(platformLayer, _fgLayerIndex, _isBackground);
        //
        _spriteRender = GetComponent<SpriteRenderer>();
    }

    protected virtual void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        //Invoke("Think", 1);
        GetGroundLayer();
    }

    // Physics is based on time (in seconds), thus we should use FixedUpdate
    // which is not called per-tick.
    protected virtual void FixedUpdate()
    {
        if(IsEnabled)
        {
            // Apply calculated velocity
            rb.linearVelocity = new Vector2(MoveDir * speed, rb.linearVelocity.y); 

            // Check obstacles for jump
            if(IsGrounded)
            {
                CheckObstacles();
            }

            // Check if grounded
            CheckGround();

            // Check if arrived to the target position
            if(isChasing)
                CheckArrived();


        }
    }

    protected void CheckObstacles()
    {
        if(IsGrounded)
        {    
            
            LayerMask layerParam = _isBackground ? bgLayer : fgLayer;
            Vector2 origin = transform.position;
            Vector2 dirVec = Vector2.right * MoveDir;
            Vector2 offset = new Vector2(sr.bounds.extents.x * MoveDir, -sr.bounds.extents.y / 2.0f);

            // 1. Check obstacle (low raycast)
            RaycastHit2D lowHit = Physics2D.Raycast(origin + offset, dirVec, obstacleDistance, layerParam);
            Debug.DrawRay(origin + offset, dirVec * obstacleDistance, new Color(0, 1, 0));

            // 2. Check can vault the obstacle
            RaycastHit2D highHit = Physics2D.Raycast(origin + new Vector2(0, jumpHeight), dirVec, obstacleDistance, layerParam);
            Debug.DrawRay(origin + new Vector2(0, jumpHeight), dirVec * obstacleDistance, new Color(0, 0, 1));

            // Debug.DrawRay(frontVec, Vector3.up, new Color(0, 1, 0));
            // RaycastHit2D rayHit = Physics2D.Raycast(frontVec, Vector2.up, 1.0f, layerParam);

            // If monster detects some obstacles, then jump
            if(lowHit.collider != null)
            {
                float slopeAngle = Vector2.Angle(lowHit.normal, Vector2.up);
                // Debug.Log($"angle: {slopeAngle}");
                if(highHit.collider == null && slopeAngle >= 45.0f)
                {
                    Jump();
                }
                else if(highHit.collider != null)
                {
                    Turn();
                }
            }
            // if(rayHit.collider != null && MoveDir != 0)
            // {
            //     Debug.Log($"Obstacle {rayHit.collider.name} Detected!");
            //     Jump();
            // }
        }
    }

    protected bool IsOnGround()
    {
        LayerMask layerParam = _isBackground ? bgLayer : fgLayer;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 1.0f, layerParam);
        //RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 0.2f, ~(1 << gameObject.layer));
        return hit.collider != null;
    }


    protected int GetGroundLayer()
    {
        LayerMask layerParam = _isBackground ? bgLayer : fgLayer;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 1.0f, layerParam);
        //RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 0.5f, ~(1 << gameObject.layer));
        return hit.collider != null ? hit.collider.gameObject.layer : platformLayer;
    }

    protected virtual void CheckGround()
    {
        LayerMask layerParam = _isBackground ? bgLayer : fgLayer;
        // Platform check
        Vector2 frontVec = new Vector2(rb.position.x + groundCheckDistance * MoveDir * speed,
        rb.position.y);

        Debug.DrawRay(frontVec, Vector3.down * 0.5f, new Color(1, 0, 0));

        RaycastHit2D rayHit = Physics2D.Raycast(frontVec, Vector3.down, 0.5f,
        layerParam);

        // If the next position is cliff, then change its direction
        if(rayHit.collider == null && IsGrounded)
        {
            Turn();
        }
    }

    protected virtual void CheckArrived()
    {
        // Check if the move to the target location is completed
        if(Vector2.Distance(transform.position, targetPosition) <= 0.01f)
        {
            // Cancel all invoke function
            //CancelInvoke();

            // Think next behavior immediately.
            Think();
        }
    }

    public virtual void MoveToTarget()
    {
        isChasing = true;
    }

    public virtual void StopChasing()
    {
        isChasing = false;
    }

    // To change the behavior
    public virtual void Think()
    {
        //Debug.Log("Monster Move: Think");
        MoveDir = Random.Range(-1, 2); // -1 : left, 0: stop, 1: right

        //float nextThinkTime = Random.Range(2.0f, 5.0f);

        //Invoke("Think", nextThinkTime);
    }

    // Change the direction
    protected virtual void Turn()
    {
        //Debug.Log("Monster Move: Turn");
        MoveDir *= -1;
        // Cancel all invoke function
        //CancelInvoke();

        // Think next behavior after 3 seconds.
        //Invoke("Think", 3);
    }

    public virtual void SetMoveDirection(float direction)
    {
        MoveDir = direction;
    }

    public virtual void Jump()
    {
        if (IsGrounded)
        {
            Debug.Log("Jump!");
            rb.AddForce(Vector2.up * jumpHeight, ForceMode2D.Impulse);
        }

        // BONUS logic here if needed:
    }

    public virtual void BlinkToOtherPlatform()
    {
        /*
        'Blinking' is basically the term for teleporting between the foreground and background platforms.
        We may need to have our own calculation system for determining where on the platform Chloe should
        teleport to. 
        */

        Debug.Log("Hello");
        return;
    }
}
