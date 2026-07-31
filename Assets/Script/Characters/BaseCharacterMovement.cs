using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Types;

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
    protected bool couldMove = true;

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


    public float SpeedMultiplier { get; private set; } = 1f;

    public void AddSpeedMultiplier(float amount) => SpeedMultiplier += amount;
    public void ReduceSpeedMultiplier(float amount) => SpeedMultiplier -= amount;

    // player's sprite renderer:
    protected SpriteRenderer _spriteRender;
    //protected StatusEffectController _statusEffects;

    protected Coroutine stunRoutines;
    protected Coroutine rootedRoutines;

    protected WaitForSecondsTracked remainStunTime;
    

    protected Coroutine slowedRoutines;

    protected virtual void Awake()
    {
        // makes sure that we auto-get the reference for the rigidbody at runtime:
        _rb = GetComponent<Rigidbody2D>();
    

        //
        _spriteRender = GetComponent<SpriteRenderer>();
        //_statusEffects = GetComponent<StatusEffectController>();

        originalScale = transform.localScale;
        speed = originalSpeed * SpeedMultiplier;
        remainStunTime = new WaitForSecondsTracked(0f);
    }

    protected virtual void Start()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position - new Vector3(0, _spriteRender.bounds.extents.y / 2f), Vector2.down, 10.0f, bgLayer | fgLayer);
        //Debug.DrawRay(transform.position - new Vector3(0, _spriteRender.bounds.extents.y / 2f), Vector2.down * 10f, new Color(1, 1, 0), 1000f);

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
        speed = _isBackground ? originalSpeed * 0.7f * SpeedMultiplier : originalSpeed * SpeedMultiplier;
        curJumpHeight = _isBackground ? jumpHeight * 0.7f : jumpHeight;
        transform.localScale = 
            _isBackground ? new Vector3(transform.localScale.x * 0.75f, transform.localScale.y * 0.75f, 1) : 
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
        //MoveDir = _statusEffects != null && !_statusEffects.CanMove ? 0f : direction;
        //_animController.FlipCharacter(direction);
    }

    public virtual void Jump()
    {
        // BONUS logic here if needed:
    }

    // protected float GetModifiedMoveSpeed()
    //     => speed * (_statusEffects != null ? _statusEffects.MoveSpeedMultiplier : 1f);

    public virtual void ResetState()
    {
        
    }

    //public virtual void Stun(float duration)
    public virtual void SetCouldMove(bool CouldMove)
    {
        couldMove = CouldMove;

        // if(stunRoutines != null)
        // {
        //     // remaining time is over than duration than refresh stun.
        //     if(remainStunTime.TimeRemaining <= duration)
        //     {
        //         StopCoroutine(stunRoutines);
        //         remainStunTime = null;
        //     }
        //     // else, ignore the stun.
        //     else
        //     {
        //         return;
        //     }
        // }

        // remainStunTime.Reset(duration);
        // stunRoutines = StartCoroutine(StunTimer(duration));
    }

    // protected virtual IEnumerator StunTimer(float duration)
    // {
    //     yield return remainStunTime;

    //     couldMove = true;
    //     stunRoutines = null;
    //     remainStunTime = null;
    // }

    // #region Buff/Debuff Status Effect
    // public void AppliedEffect(EStatusEffectType type, float magnitude)
    // {
    //     switch(type)
    //     {
    //         case EStatusEffectType.MoveSpeedUp:
    //             AddSpeedMultiplier(magnitude);
    //         break;
    //         case EStatusEffectType.Slow:
    //             ReduceSpeedMultiplier(magnitude);
    //         break;
    //         default:
    //         return;
    //     }
    // }

    // public void RemoveEffect(EStatusEffectType type, float magnitude)
    // {
    //     switch(type)
    //     {
    //         case EStatusEffectType.MoveSpeedUp:
    //             ReduceSpeedMultiplier(magnitude);
    //         break;
    //         case EStatusEffectType.Slow:
    //             AddSpeedMultiplier(magnitude);
    //         break;
    //         default:
    //         return;
    //     }
    // }
    // #endregion

    #region Knockedback
    public void ApplyKnockback(Vector2 dir, float force)
    {
        // SetCouldMove(false);
        _rb.linearVelocity = Vector2.zero;
        _rb.AddForce(dir * force, ForceMode2D.Impulse);
    }
    #endregion
}
