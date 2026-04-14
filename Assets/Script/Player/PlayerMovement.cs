using UnityEngine;

/// <summary>
/// Processes the movement and the physics of the player character
/// given the vector/axis values from the player controller.
/// </summary>
public class PlayerMovement : MonoBehaviour
{

    // These values are exposed states for others to read:
    public bool IsGrounded {get; private set;}
    public float MoveDir {get; private set;}

    [Header("Movement values")]
    [SerializeField] private float speed;
    [SerializeField] private float jumpHeight;
    // Physics body for 2D object
    private Rigidbody2D _rb;

    // @TODO: Add a serialized private/public PlayerAnimControl class reference here

    // Physics is based on time (in seconds), thus we should use FixedUpdate
    // which is not called per-tick.
    private void FixedUpdate()
    {
        // fixing horizontal drift:
        if (MoveDir > -0.3f && MoveDir < 0.3f) 
            MoveDir = 0f;
        
        // moving the rigidbody:
        // the y-axis remains constant here.
        _rb.linearVelocity = new Vector2(MoveDir * speed, _rb.linearVelocity.y);
    }

    public void SetMoveDirection(float direction)
    {
        MoveDir = direction;
    }

    public void Jump()
    {
        if (IsGrounded)
        {
            _rb.AddForce(Vector2.up * jumpHeight, ForceMode2D.Impulse);
        }

        // BONUS logic here if needed:
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
