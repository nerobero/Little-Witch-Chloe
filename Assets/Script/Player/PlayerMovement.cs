using System;
using UnityEngine;

/// <summary>
/// Processes the movement and the physics of the player character
/// given the vector/axis values from the player controller.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{

    // These values are exposed states for others to read:
    public bool IsGrounded { get; private set; }
    public float MoveDir { get; private set; }

    [Header("Movement values")]
    [SerializeField] private float speed;
    [SerializeField] private float jumpHeight;
    [SerializeField] private float flyForce;
    [SerializeField] private LayerMask bgLayer;
    [SerializeField] private LayerMask fgLayer;

    [Header("Debugging, Testing toggler")]
    [SerializeField] private bool Is3D;

    // Physics body for 2D object
    private Rigidbody2D _rb;
    private bool _isBackground = false;

    // @TODO: Add a serialized private/public PlayerAnimControl class reference here

    private void Awake()
    {
        // makes sure that we auto-get the reference for the rigidbody at runtime:
        _rb = GetComponent<Rigidbody2D>();
    }

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

    public void StartFlying()
    {
        // if this function is called when the character is NOT set to fly,
        // then return.
        if (!PlayerController.Instance.IsFlying) return;

        // flying physics logic here
        _rb.gravityScale = 0.75f; // reducing the gravity by a quarter for more floaty feel 

        // TODO: add the start flying animation state change here:
    }

    public void StopFlying()
    {
        // if this function is called when the character is STILL set to fly,
        // then return.
        if (PlayerController.Instance.IsFlying) return;

        // stop flying physics logic here
        _rb.gravityScale = 1f; // restoring the default gravity value

        // TODO: add the stop flying animation state change here:
    }

    public void FlyTick()
    {
        _rb.AddForce(Vector2.up * flyForce, ForceMode2D.Force);
        // TODO: add stamina usage logic here
    }

    public void BlinkToOtherPlatform()
    {
        /*
        'Blinking' is basically the term for teleporting between the foreground and background platforms.
        We may need to have our own calculation system for determining where on the platform Chloe should
        teleport to. 
        */
        if (Is3D)
        {

        }
        else
        {
            //1. finding if there is any teleportable platform within the given radius 
            LayerMask layerParam = _isBackground ? bgLayer : fgLayer;
            if (Physics2D.OverlapCircle(transform.position, 15.0f, layerParam) == null)
                return;

            //2. find the surface to get teleport to:
            float camHalfHeight = Camera.main.orthographicSize;
            // TODO: may need to change this to another position vector
            Vector2 camOrigin = (Vector2)Camera.main.transform.position;
            RaycastHit2D hit2D = Physics2D.Raycast(camOrigin + Vector2.up * camHalfHeight,
                    Vector2.down, camHalfHeight * 2f, layerParam);
            if (hit2D.collider == null) return;

            //3. reposition the player character:
            _rb.position = new Vector2(_rb.position.x, hit2D.point.y);
        }
    }
}
