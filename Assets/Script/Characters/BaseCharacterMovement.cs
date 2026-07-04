using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Processes the movement and the physics of the player character
/// given the vector/axis values from the player controller.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class BaseCharacterMovement : MonoBehaviour
{
    // These values are exposed states for others to read:
    public bool IsGrounded => IsOnGround();
    public float MoveDir { get; protected set; }

    [Header("Movement values")]
    [SerializeField] protected float originalSpeed;
    [SerializeField] protected float jumpHeight;
    [SerializeField] protected LayerMask bgLayer;
    [SerializeField] protected LayerMask fgLayer;
    
    [SerializeField] protected float heightDistance = 2f;
    protected float xOffset = 0.89f * 2f;

    protected int orderInLayer;
    protected float speed;
    protected float curJumpHeight;
    public int OrderInLayer => orderInLayer;
    protected Vector3 originalScale;

    protected Rigidbody2D _rb; // Physics body for 2D object
    [SerializeField] protected bool _isBackground = false; //by default, you're already on 
    public bool IsBackground => _isBackground;
    protected string myLayer = "NPC";

    protected int _characterLayer => gameObject.layer;
    protected int _bgLayerIndex => (int)Mathf.Log(bgLayer.value, 2);
    protected int _fgLayerIndex => (int)Mathf.Log(fgLayer.value, 2);

    // player's sprite renderer:
    protected SpriteRenderer _spriteRender;

    protected virtual void Awake()
    {
        // makes sure that we auto-get the reference for the rigidbody at runtime:
        _rb = GetComponent<Rigidbody2D>();
    

        //
        _spriteRender = GetComponent<SpriteRenderer>();

        originalScale = transform.localScale;
        speed = originalSpeed;
    }

    protected virtual void Start()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 1.0f, _bgLayerIndex | _fgLayerIndex);

        Debug.Log(hit.collider);

        string groundLayerName = LayerMask.LayerToName(hit.collider != null ? hit.collider.gameObject.layer : _characterLayer);

        if (groundLayerName.Contains("_"))
        {
            // 1. Check the ground is Foreground or Background
            string groundPrefix = groundLayerName.Split('_')[0];
            _isBackground = (groundPrefix == "Background");

            // 2. Request a layer number of the "player/enemy" (e.g., "floor") from the manager.
            // 예: 바닥이 Background_Platform이면, 나는 Background_Player 레이어 번호를 가져옴
            int nextMyLayer = LayerManager.Instance.GetLayer(_isBackground, myLayer);
           
            gameObject.layer = nextMyLayer;
            
            Debug.Log($"{GetType().Name}: Because the layer of the platform is {groundLayerName}, change my layer as {LayerMask.LayerToName(nextMyLayer)}.");
        }

        ChangeOrderInLayer();
    }

    /// <summary>
    /// Set the gameobject's orderInLayer -1 or 0 based on whether
    /// the character is in the background or not.
    /// </summary>
    protected virtual void ChangeOrderInLayer()
    {
        orderInLayer = _isBackground ? -1 : 1;
        _spriteRender.sortingOrder = orderInLayer;

        // Change speed, jump height and scale
        speed = _isBackground ? originalSpeed * 0.7f : originalSpeed;
        curJumpHeight = _isBackground ? jumpHeight * 0.7f : jumpHeight;
        transform.localScale = 
            _isBackground ? new Vector3(transform.localScale.x * 0.75f, 0.75f, 1) : 
                new Vector3(Mathf.Sign(transform.localScale.x) * originalScale.x, originalScale.y, 1);
    }

    protected virtual bool IsOnGround()
    {
        LayerMask layerParam = _isBackground ? bgLayer : fgLayer;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, heightDistance, layerParam);
        //RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 1.5f, ~(1 << gameObject.layer));
        return hit.collider != null;
    }

    /// <summary>
    /// Gets the index of the layermask that the player is currently standing on.
    /// </summary>
    /// <returns>the index of the current layermask the player is standing on</returns>
    protected virtual int GetGroundLayer()
    {
        //Debug.DrawRay(transform.position, Vector2.down, new Color(0, 1, 0), 2.0f);
        LayerMask layerParam = _isBackground ? bgLayer : fgLayer;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 1.0f, layerParam);
        //RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 1.0f, ~(1 << gameObject.layer));


        //Debug.Log($"hit.collider: {hit.collider}, playerLayer: {_characterLayer}, hit.layer: {hit.collider.gameObject.layer}");
        return hit.collider != null ? hit.collider.gameObject.layer : _characterLayer;
    }

    /// <summary>
    /// Sets the player's move direction. Also flips the character via anim controller
    /// -1 = left
    /// 0 = idle
    /// 1 = right
    /// </summary>
    /// <param name="direction">the direction value in float</param>
    public virtual void SetMoveDirection(float direction)
    {
        MoveDir = direction;
        //_animController.FlipCharacter(direction);
    }

    public virtual void Jump()
    {
        // BONUS logic here if needed:
    }

    public virtual void ResetState()
    {
        
    }
}
