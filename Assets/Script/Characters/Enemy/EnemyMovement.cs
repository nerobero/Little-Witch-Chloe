using UnityEngine;
using System.Collections.Generic;
using System;
using Random = UnityEngine.Random;

/// <summary>
/// Base class for enemy movement and physics.
/// Handles velocity, ground detection, and basic movement.
/// Specific monster types inherit and implement their own AI behavior.
/// </summary>
[RequireComponent(typeof(EnemyAnimController))]
public class EnemyMovement : BaseCharacterMovement
{
    // These values are exposed states for others to read:
    public bool shouldStop;

    public bool IsEnabled = true;

    [Header("Movement values")]
    [SerializeField] protected float minDistance = 2f; // minimum 2 grid
    [SerializeField] protected float maxDistance = 3f; // maximum 3 grid
    [SerializeField] protected float speedIncFactor = 1f;

    [Header("Ground Detection")]
    [SerializeField] protected float groundCheckDistance = 0.5f;
    [SerializeField] protected float obstacleDistance = 0.5f;
    //[SerializeField] protected override float heightDistance = 2f;
    //[SerializeField] protected LayerMask _characterLayer = gameObject.layer;

    [SerializeField] protected LayerMask fgEnemyLayer;
    [SerializeField] protected LayerMask bgEnemyLayer;
    public event Action<Vector2> OnBlinkFinished;
    
    protected EnemyAnimController _animController;
    public EnemyAnimController AnimController => _animController;

    [Header("Patrol Settings")]
    public Vector2 targetPosition;
    public bool isChasing;
    
    protected Vector2 spawnPosition;

    // Physics body for 2D object
    // enemy's sprite renderer:
    protected PolygonCollider2D  myCollider;

    [SerializeField] protected bool isArrived = true;


    // @TODO: Add a serialized private/public PlayerAnimControl class reference here

    protected override void Awake()
    {
        // makes sure that we auto-get the reference for the rigidbody at runtime:
        _rb = GetComponent<Rigidbody2D>();
        myCollider = GetComponent<PolygonCollider2D >();
        
        Physics2D.IgnoreLayerCollision(_characterLayer, _characterLayer, true);

        //
        _spriteRender = GetComponent<SpriteRenderer>();
        _animController = GetComponent<EnemyAnimController>();

        int layerIndex = (int)Mathf.Log(_isBackground ? bgEnemyLayer : fgEnemyLayer, 2);
        gameObject.layer = layerIndex;

        originalScale = transform.localScale;
        speed = originalSpeed;
        heightDistance = 2f;
        myLayer = "Enemy";
        //ChangeOrderInLayer();
    }

    protected override void Start()
    {
        //sr = GetComponent<SpriteRenderer>();
        //Invoke("Think", 1);
        //GetGroundLayer();

        base.Start();
        
        // Memory the spawn point
        spawnPosition = transform.position;
    }
    
    // /// <summary>
    // /// Set the gameobject's orderInLayer -1 or 0 based on whether
    // /// the character is in the background or not.
    // /// </summary>
    // protected override void ChangeOrderInLayer()
    // {

    //     orderInLayer = _isBackground ? -1 : 1;
    //     _spriteRender.sortingOrder = orderInLayer;

    //     // Change speed, jump height and scale
    //     speed = _isBackground ? originalSpeed * 0.7f : originalSpeed;
    //     curJumpHeight = _isBackground ? jumpHeight * 0.7f : jumpHeight;
    //     transform.localScale = 
    //         _isBackground ? new Vector3(transform.localScale.x * 0.75f, 0.75f, 1) : 
    //             new Vector3(Mathf.Sign(transform.localScale.x) * originalScale.x, originalScale.y, 1);
    // }

    // Physics is based on time (in seconds), thus we should use FixedUpdate
    // which is not called per-tick.
    protected virtual void FixedUpdate()
    {
        if(IsEnabled)
        {
            if(shouldStop) return;

            // Apply calculated velocity
            _rb.linearVelocity = new Vector2(MoveDir * speed, _rb.linearVelocity.y); 

            // Check obstacles for jump
            if(IsGrounded)
            {
                CheckObstacles();
            }

            // Check if grounded
            CheckGround();

            // Check if arrived to the target position
            //if(isChasing)
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
            Vector2 offset = new Vector2(_spriteRender.bounds.extents.x * MoveDir, -_spriteRender.bounds.extents.y / 2.0f);

            // 1. Check obstacle (low raycast)
            RaycastHit2D lowHit = Physics2D.Raycast(origin + offset, dirVec, obstacleDistance, layerParam);
            Debug.DrawRay(origin + offset, dirVec * obstacleDistance, new Color(0, 1, 0));

            // 2. Check can vault the obstacle
            RaycastHit2D highHit = Physics2D.Raycast(origin + new Vector2(0, curJumpHeight), dirVec, obstacleDistance, layerParam);
            Debug.DrawRay(origin + new Vector2(0, curJumpHeight), dirVec * obstacleDistance, new Color(0, 0, 1));

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

    // protected bool IsOnGround()
    // {
    //     LayerMask layerParam = _isBackground ? bgLayer : fgLayer;
    //     RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, heightDistance, layerParam);
    //     //RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 0.2f, ~(1 << gameObject.layer));
    //     return hit.collider != null;
    // }


    // protected int GetGroundLayer()
    // {
    //     LayerMask layerParam = _isBackground ? bgLayer : fgLayer;
    //     RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 1.0f, layerParam);
    //     //RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 0.5f, ~(1 << gameObject.layer));
    //     return hit.collider != null ? hit.collider.gameObject.layer : _characterLayer;
    // }

    protected virtual void CheckGround()
    {
        LayerMask layerParam = _isBackground ? bgLayer : fgLayer;
        // Platform check
        Vector2 frontVec = new Vector2(_rb.position.x + groundCheckDistance * MoveDir * speed,
        _rb.position.y);

        Debug.DrawRay(frontVec, Vector3.down * heightDistance, new Color(1, 0, 0));

        RaycastHit2D rayHit = Physics2D.Raycast(frontVec, Vector3.down, heightDistance,
        layerParam);

        // If the next position is cliff, then change its direction
        if(rayHit.collider == null && IsGrounded)
        {
            Turn();
        }
    }

    protected virtual void CheckArrived()
    {
        if(isChasing)
        {
            // Check if the move to the target location is completed
            if(Vector2.Distance(transform.position, targetPosition) <= 0.01f)
            {
                isArrived = true;
                // reset the spawn position to current position
                //spawnPosition = transform.position;
                // Cancel all invoke function
                //CancelInvoke();

                // Think next behavior immediately.
                //Think();
            }
            
        }
        // if it is not chasing
        else
        {
            // compare the x values.
            if(Mathf.Abs(transform.position.x - targetPosition.x) <= 0.01f || MoveDir == 0f)
            {
                isArrived = true;
            }
        }
    }

    public virtual void SeeTarget(Vector2 target)
    {
        isChasing = true;
        targetPosition = target;

        // Set move direction
        SetMoveDirection(Mathf.Sign((targetPosition - (Vector2)transform.position).normalized.x));
    }

    public virtual void MoveToTarget(Vector2 target)
    {
        if(!isChasing)
            speed *= speedIncFactor;
        isChasing = true;
        targetPosition = target;

        // Set move direction
        SetMoveDirection(Mathf.Sign((targetPosition - (Vector2)transform.position).normalized.x));

    }

    public virtual void StopChasing()
    {
        if(isChasing)
            speed = originalSpeed;
        isChasing = false;
        //speed /= 1.5f;
        Think();
    }

    // To change the behavior
    public virtual void Think()
    {
        //Debug.Log("Monster Move: Think");
        if(isArrived)
        {
            SetMoveDirection(Random.Range(-1, 2)); // -1 : left, 0: stop, 1: right
            //isArrived = false;
        }

        //float nextThinkTime = Random.Range(2.0f, 5.0f);

        //Invoke("Think", nextThinkTime);
    }

    // Change the direction
    protected virtual void Turn()
    {
        //Debug.Log("Monster Move: Turn");
        SetMoveDirection(MoveDir * -1f);
        // Cancel all invoke function
        //CancelInvoke();

        // Think next behavior after 3 seconds.
        //Invoke("Think", 3);
    }

    public override void SetMoveDirection(float direction)
    {
        base.SetMoveDirection(direction);
        _animController.FlipCharacter(-MoveDir);
    }

    public override void Jump()
    {
        if (IsGrounded)
        {
            _rb.AddForce(Vector2.up * curJumpHeight, ForceMode2D.Impulse);
        }

        // BONUS logic here if needed:
    }

    public virtual void OnBlinkCallback()
    {
        // change the state of the _animController.SetToSeen()
    }


    public virtual void BlinkToOtherPlatform()
    {
        /*
        'Blinking' is basically the term for teleporting between the foreground and background platforms.
        We may need to have our own calculation system for determining where on the platform Chloe should
        teleport to. 
        */

        OnBlinkFinished.Invoke(targetPosition);
    }

    public override void ResetState()
    {
        transform.position = spawnPosition;
        AnimController.ResetState();
    }
}
