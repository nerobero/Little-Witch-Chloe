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

    public bool enabled = true;

    [Header("Movement values")]
    [SerializeField] protected float speed;
    [SerializeField] protected float jumpHeight;
    [SerializeField] protected float flyForce;
    [SerializeField] protected float minDistance = 2f; // minimum 2 grid
    [SerializeField] protected float maxDistance = 3f; // maximum 3 grid

    [Header("Ground Detection")]
    [SerializeField] protected float groundCheckDistance = 0.5f;
    [SerializeField] protected LayerMask platformLayer;

    [SerializeField] protected LayerMask bgLayer;
    [SerializeField] protected LayerMask fgLayer;

    protected int _bgLayerIndex => (int)Mathf.Log(bgLayer.value, 2);
    protected int _fgLayerIndex => (int)Mathf.Log(fgLayer.value, 2);
    
    private bool _isBackground = false; 
    public bool IsBackground => _isBackground;


    [Header("Patrol Settings")]
    public Vector2 targetPosition;
    public bool isChasing;

    // Physics body for 2D object
    protected Rigidbody2D rb;

    // @TODO: Add a serialized private/public PlayerAnimControl class reference here

    protected virtual void Awake()
    {
        // makes sure that we auto-get the reference for the rigidbody at runtime:
        rb = GetComponent<Rigidbody2D>();
        
        Physics2D.IgnoreLayerCollision(platformLayer, _bgLayerIndex, !_isBackground);
        Physics2D.IgnoreLayerCollision(platformLayer, _fgLayerIndex, _isBackground);
        //
        Invoke("Think", 5);
    }

    protected virtual void Start()
    {
        GetGroundLayer();
    }

    // Physics is based on time (in seconds), thus we should use FixedUpdate
    // which is not called per-tick.
    protected virtual void FixedUpdate()
    {
        if(enabled)
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
            float detectionDist = MoveDir == 1.0f? 0.5f : -0.5f;
            Vector2 rayOrigin = transform.position;

            Vector2 frontVec = new Vector2(rayOrigin.x + detectionDist, rayOrigin.y + 0.5f);

            Debug.DrawRay(frontVec, Vector3.up, new Color(0, 1, 0));
            RaycastHit2D rayHit = Physics2D.Raycast(frontVec, Vector2.up, 1.0f, layerParam);

            // If monster detects some obstacles, then jump
            if(rayHit != null && MoveDir != 0)
            {
                Debug.Log("Obstacle Detected!");
                Jump();
            }
        }
    }

    protected bool IsOnGround()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 1.5f, ~(1 << gameObject.layer));
        return hit.collider != null;
    }


    protected int GetGroundLayer()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 0.5f, ~(1 << gameObject.layer));
        return hit.collider != null ? hit.collider.gameObject.layer : platformLayer;
    }

    protected virtual void CheckGround()
    {
        // Platform check
        Vector2 frontVec = new Vector2(rb.position.x + 0.5f * MoveDir * speed,
        rb.position.y);

        //Debug.DrawRay(frontVec, Vector3.down, new Color(0, 1, 0));

        RaycastHit2D rayHit = Physics2D.Raycast(frontVec, Vector3.down, 1,
        platformLayer);

        // If the next position is cliff, then change its direction
        if(rayHit.collider == null)
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
            CancelInvoke();

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
        MoveDir = Random.Range(-1, 2); // -1 : left, 0: stop, 1: right

        float nextThinkTime = Random.Range(2.0f, 5.0f);

        Invoke("Think", nextThinkTime);
    }

    // Change the direction
    protected virtual void Turn()
    {
        MoveDir *= -1;
        // Cancel all invoke function
        CancelInvoke();

        // Think next behavior after 3 seconds.
        Invoke("Think", 3);
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
    }
}
