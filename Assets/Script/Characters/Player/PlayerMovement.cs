using System;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Processes the movement and the physics of the player character
/// given the vector/axis values from the player controller.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(PlayerStatManager))]
[RequireComponent(typeof(PlayerAnimController))]
public class PlayerMovement : MonoBehaviour
{

    // These values are exposed states for others to read:
    public bool IsGrounded => IsOnGround();
    public float MoveDir { get; private set; }
    public event Action OnFlyStopped;

    [Header("Movement values")]
    [SerializeField] private float speed;
    [SerializeField] private float jumpHeight;
    [SerializeField] private float flyForce;
    [SerializeField] private LayerMask bgLayer;
    [SerializeField] private LayerMask fgLayer;
    private int orderInLayer;

    private Rigidbody2D _rb; // Physics body for 2D object
    private bool _isBackground = false; //by default, you're already on 
    public bool IsBackground => _isBackground;

    private int _playerLayer => gameObject.layer;
    private int _bgLayerIndex => (int)Mathf.Log(bgLayer.value, 2);
    private int _fgLayerIndex => (int)Mathf.Log(fgLayer.value, 2);

    // stamina related component ref here:
    private PlayerStatManager _statManager;

    // player's animation controller:
    private PlayerAnimController _animController;

    // player's sprite renderer:
    [SerializeField] private SpriteRenderer _childSpriteRender;
    private SpriteRenderer _spriteRender;
    

    private void Awake()
    {
        // makes sure that we auto-get the reference for the rigidbody at runtime:
        _rb = GetComponent<Rigidbody2D>();

        // makes sure that we get the reference for the stat manager at runtime:
        _statManager = GetComponent<PlayerStatManager>();

        // makes sure that we get the reference for the player anim controller at runtime:
        _animController = GetComponent<PlayerAnimController>();

        _spriteRender = GetComponent<SpriteRenderer>();

        // ignoring the background platform in the beginning
        Physics2D.IgnoreLayerCollision(_playerLayer, _bgLayerIndex, !_isBackground);
        Physics2D.IgnoreLayerCollision(_playerLayer, _fgLayerIndex, _isBackground);

    }

    private void Start()
    {
        ChangeOrderInLayer();
    }

    /// <summary>
    /// Set the gameobject's orderInLayer -1 or 0 based on whether
    /// the character is in the background or not.
    /// </summary>
    private void ChangeOrderInLayer()
    {

        orderInLayer = _isBackground ? -1 : 0;
        _spriteRender.sortingOrder = orderInLayer;
        _childSpriteRender.sortingOrder = orderInLayer;
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
        // if MoveDir != 0, it means that the player is moving in either direction:
        _animController.SetToWalk(_rb.linearVelocity.x != 0f);
    }

    /// <summary>
    /// Is the character on the ground platform?
    /// </summary>
    /// <returns>true if the raycast hits an object</returns>
    private bool IsOnGround()
    {
        LayerMask layerParam = _isBackground ? bgLayer : fgLayer;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 1.5f, layerParam);
        //RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 1.5f, ~(1 << gameObject.layer));
        return hit.collider != null;
    }

    /// <summary>
    /// Gets the index of the layermask that the player is currently standing on.
    /// </summary>
    /// <returns>the index of the current layermask the player is standing on</returns>
    private int GetGroundLayer()
    {
        //Debug.DrawRay(transform.position, Vector2.down, new Color(0, 1, 0), 2.0f);
        LayerMask layerParam = _isBackground ? bgLayer : fgLayer;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 1.0f, layerParam);
        //RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 1.0f, ~(1 << gameObject.layer));


        //Debug.Log($"hit.collider: {hit.collider}, playerLayer: {_playerLayer}, hit.layer: {hit.collider.gameObject.layer}");
        return hit.collider != null ? hit.collider.gameObject.layer : _playerLayer;
    }

    /// <summary>
    /// Sets the player's move direction. Also flips the character via anim controller
    /// -1 = left
    /// 0 = idle
    /// 1 = right
    /// </summary>
    /// <param name="direction">the direction value in float</param>
    public void SetMoveDirection(float direction)
    {
        MoveDir = direction;
        _animController.FlipCharacter(direction);
    }

    /// <summary>
    /// Adds force to the character to have it jump.
    /// Only works if the character is currently grounded.
    /// </summary>
    public void Jump()
    {
        if (IsGrounded)
        {
            _rb.AddForce(Vector2.up * jumpHeight, ForceMode2D.Impulse);
        }
        // BONUS logic here if needed:
    }

    /// <summary>
    /// Prepares the character before flying. 
    /// - If the character is already flying OR the stamina is below 0, then early return
    /// - Modifies the gravity for flying that looks more floaty 
    /// - Sets the anim state to start flying
    /// </summary>
    public void StartFlying()
    {
        // if this function is called when the character is NOT set to fly,
        // then return.
        if (!PlayerController.Instance.IsFlying ||
        _statManager.CurrStamina <= 0) return;

        // flying physics logic here
        _rb.gravityScale = 0.5f; // reducing the gravity by a quarter for more floaty feel 

        // add the start flying animation state change here:
        _animController.SetToStartFlying();
    }

    /// <summary>
    /// Stops the character from flying.
    /// - Resets the gravity back to 1f
    /// - Invokes OnFlyStopped event
    /// - Changes the anim state to Stop Flying
    /// </summary>
    public void StopFlying()
    {
        _rb.gravityScale = 1f;

        // add the stop flying animation state change here:
        OnFlyStopped?.Invoke();
        _animController.SetToStopFlying();
    }

    /// <summary>
    /// Applies flight force to the character's rigidbody per tick.
    /// - Also uses 10% of stamina per tick.
    /// </summary>
    public void FlyTick()
    {
        _rb.AddForce(Vector2.up * flyForce, ForceMode2D.Force);
        _statManager.UseStamina(0.1f);
    }

    /// <summary>
    /// Checks if the player can 'blink' to another platform
    /// and performs the action if so. 
    /// </summary>
    public void BlinkToOtherPlatform()
    {
        /*
        'Blinking' is basically the term for teleporting between the foreground and background platforms.
        We may need to have our own calculation system for determining where on the platform Chloe should
        teleport to. 
        */

        //1. finding if there is any teleportable platform within the given radius 
        LayerMask layerParam = _isBackground ? fgLayer : bgLayer;
        Collider2D collided = Physics2D.OverlapCircle(transform.position, 15.0f, layerParam);
        int currLayer = GetGroundLayer();
        if (collided == null || collided.gameObject.layer == currLayer)
        {
            Debug.LogWarning("cannot teleport.");
            return;
        }
        //Debug.Log($"Collided: {collided.gameObject.layer}, Player: {currLayer}");

        //2. find the surface to get teleport to:
        float camHalfHeight = Camera.main.orthographicSize;
        float xOffset = 0.89f * 2f;

        // 2a. flipping the xOffset based on the character's move direction
        // and whether the character is in the background or not:
        if (_animController._isFacingRight)
            xOffset = _isBackground ? -xOffset : xOffset;
        else
            xOffset = _isBackground ? xOffset : -xOffset;

        // 2b. using raycast to determine where on the surface the character can 'blink' to:
        Vector2 origin = new Vector2(_rb.position.x + xOffset, _rb.position.y);
        // Debug.DrawRay(origin + Vector2.up * camHalfHeight, Vector2.down * camHalfHeight * 2f, new Color(0, 0, 1), 2.0f);
        RaycastHit2D hitresult = Physics2D.Raycast(origin + Vector2.up * camHalfHeight,
                Vector2.down, camHalfHeight * 2f, layerParam);
        if (hitresult.collider == null)
        {
            Debug.Log("Null");
            return;
        }

        //3. flip the _isBackground value before we reposition the character:
        _isBackground = !_isBackground;

        //4. ignoring the colliders of the source ground
        // and enabling the colliders for the destination ground:
        Physics2D.IgnoreLayerCollision(_playerLayer, _bgLayerIndex, !_isBackground);
        Physics2D.IgnoreLayerCollision(_playerLayer, _fgLayerIndex, _isBackground);

        //5. reposition the player character:
        _rb.position = new Vector2(hitresult.point.x, hitresult.point.y + 1.0f);

        //6. changing the order in layer:
        ChangeOrderInLayer();
    }

    public int GetCurrentLayer()
    {
        return IsBackground ? _bgLayerIndex : _fgLayerIndex;
    }
}
