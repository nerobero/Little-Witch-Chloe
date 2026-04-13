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
    [SerializeField] private float velocity;
    [SerializeField] private float jumpHeight;
    // Physics body for 2D object
    private Rigidbody2D _rb;

    // Physics is based on time (in seconds), thus we should use FixedUpdate
    // which is not called per-tick.
    private void FixedUpdate()
    {
        
    } 
}
