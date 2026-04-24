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
    public bool IsGrounded { get; private set; }
    public float MoveDir { get; private set; }

    [Header("Movement values")]
    [SerializeField] private float speed;
    [SerializeField] private float jumpHeight;
    [SerializeField] private float flyForce;

    [Header("Ground Detection")]
    [SerializeField] protected float groundCheckDistance = 0.5f;
    [SerializeField] protected LayerMask platformLayer;

    // Physics body for 2D object
    private Rigidbody2D rb;

    // @TODO: Add a serialized private/public PlayerAnimControl class reference here

    private void Awake()
    {
        // makes sure that we auto-get the reference for the rigidbody at runtime:
        rb = GetComponent<Rigidbody2D>();
        
        //
        Invoke("Think", 5);
    }

    // Physics is based on time (in seconds), thus we should use FixedUpdate
    // which is not called per-tick.
    private void FixedUpdate()
    {
        // Apply calculated velocity
        rb.linearVelocity = new Vector2(MoveDir * speed, rb.linearVelocity.y); 

        // Check if grounded
        CheckGround();
    }

    protected void CheckGround()
    {
        // Platform check
        Vector2 frontVec = new Vector2(rb.position.x + 0.5f * MoveDir * speed,
        rb.position.y);

        Debug.DrawRay(frontVec, Vector3.down, new Color(0, 1, 0));

        RaycastHit2D rayHit = Physics2D.Raycast(frontVec, Vector3.down, 1,
        platformLayer);

        // If the next position is cliff, then change its direction
        if(rayHit.collider == null)
        {
            MoveDir *= -1;

            // Cancel all invoke function
            CancelInvoke();

            // Think next behavior after 3 seconds.
            Invoke("Think", 3);
        }
    }

    // To change the behavior
    protected void Think()
    {
        MoveDir = Random.Range(-1, 2);

        float nextThinkTime = Random.Range(2f, 5f);

        Invoke("Think", nextThinkTime);
    }

    // Change the direction
    protected void Turn()
    {
        MoveDir *= -1;
        
    }

    public void SetMoveDirection(float direction)
    {
        MoveDir = direction;
    }

    public void Jump()
    {
        if (IsGrounded)
        {
            rb.AddForce(Vector2.up * jumpHeight, ForceMode2D.Impulse);
        }

        // BONUS logic here if needed:
    }

    public void StartFlying()
    {
        // if this function is called when the character is NOT set to fly,
        // then return.
        if (!PlayerController.Instance.IsFlying) return;

        // flying physics logic here
        rb.gravityScale = 0.75f; // reducing the gravity by a quarter for more floaty feel 

        // TODO: add the start flying animation state change here:
    }

    public void StopFlying()
    {
        // if this function is called when the character is STILL set to fly,
        // then return.
        if (PlayerController.Instance.IsFlying) return;

        // stop flying physics logic here
        rb.gravityScale = 1f; // restoring the default gravity value

        // TODO: add the stop flying animation state change here:
    }

    public void FlyTick()
    {
        rb.AddForce(Vector2.up * flyForce, ForceMode2D.Force);
    }

    public void BlinkToOtherPlatform()
    {
        /*
        'Blinking' is basically the term for teleporting between the foreground and background platforms.
        We may need to have our own calculation system for determining where on the platform Chloe should
        teleport to. 
        */
    }
}
