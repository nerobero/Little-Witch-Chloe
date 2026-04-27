using System;
using UnityEngine;

/// <summary>
/// Processes the movement and the physics of the player character
/// given the vector/axis values from the player controller.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(PlayerStatManager))]
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

    private Rigidbody2D _rb; // Physics body for 2D object
    private bool _isBackground = false; //
    private int _playerLayer => gameObject.layer; 
    private int _bgLayerIndex => (int)Mathf.Log(bgLayer.value, 2);
    private int _fgLayerIndex => (int)Mathf.Log(fgLayer.value, 2);

    // stamina related component ref here:
    private PlayerStatManager _statManager;
    public Action OnFlyStopped;

    // @TODO: Add a serialized private/public PlayerAnimControl class reference here

    private void Awake()
    {
        // makes sure that we auto-get the reference for the rigidbody at runtime:
        _rb = GetComponent<Rigidbody2D>();

        // makes sure that we get the reference for the stat manager at runtime:
        _statManager = GetComponent<PlayerStatManager>();


        // ignoring the background platform in the beginning
        Physics2D.IgnoreLayerCollision(_playerLayer, _bgLayerIndex, true);
        Physics2D.IgnoreLayerCollision(_playerLayer, _fgLayerIndex, false);

    }

    private void OnEnable()
    {
        _statManager.OnStaminaOver += StopFlying;
    }

    private void OnDisable()
    {
        _statManager.OnStaminaOver -= StopFlying;
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
        // if (IsGrounded)
        // {
        //     _rb.AddForce(Vector2.up * jumpHeight, ForceMode2D.Impulse);
        // }
        _rb.AddForce(Vector2.up * jumpHeight, ForceMode2D.Impulse);
        // BONUS logic here if needed:
    }

    public void StartFlying()
    {
        // if this function is called when the character is NOT set to fly,
        // then return.
        if (!PlayerController.Instance.IsFlying ||
        _statManager.CurrStamina <= 0) return;

        // flying physics logic here
        _rb.gravityScale = 0.5f; // reducing the gravity by a quarter for more floaty feel 

        // TODO: add the start flying animation state change here:
    }

    public void StopFlying()
    {
        _rb.gravityScale = 1f;

        // TODO: add the stop flying animation state change here:

        OnFlyStopped?.Invoke();
    }

    public void FlyTick()
    {
        _rb.AddForce(Vector2.up * flyForce, ForceMode2D.Force);
        // TODO: add stamina usage logic here
        _statManager.UseStamina(0.1f);
    }

    public void BlinkToOtherPlatform()
    {
        /*
        'Blinking' is basically the term for teleporting between the foreground and background platforms.
        We may need to have our own calculation system for determining where on the platform Chloe should
        teleport to. 
        */

        //1. finding if there is any teleportable platform within the given radius 
        LayerMask layerParam = _isBackground ? bgLayer : fgLayer;
        if (Physics2D.OverlapCircle(transform.position, 15.0f, layerParam) == null)
            return;

        //2. find the surface to get teleport to:
        float camHalfHeight = Camera.main.orthographicSize;
        Vector2 origin = (Vector2)transform.position;
        RaycastHit2D hitresult = Physics2D.Raycast(origin + Vector2.up * camHalfHeight,
                Vector2.down, camHalfHeight * 2f, layerParam);
        if (hitresult.collider == null) return;

        //3. ignoring the colliders of the teleported ground.
        Physics2D.IgnoreLayerCollision(_playerLayer, _bgLayerIndex, !_isBackground);
        Physics2D.IgnoreLayerCollision(_playerLayer, _fgLayerIndex, _isBackground);

        //4. reposition the player character:
        _rb.position = new Vector2(_rb.position.x, hitresult.point.y + 0.1f);



        //5. flip the _isBackground value:
        _isBackground = !_isBackground;
    }
}
