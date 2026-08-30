using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Processes the movement and the physics of the player character
/// given the vector/axis values from the player controller.
/// </summary>
[RequireComponent(typeof(PlayerStatManager))]
[RequireComponent(typeof(PlayerAnimController))]
public class PlayerMovement : BaseCharacterMovement
{

    // These values are exposed states for others to read:
    public event Action OnFlyStopped;

    [Header("Movement values")]
    [SerializeField] private float flyForce;
    [SerializeField] private LayerMask bgPlayerLayer;
    [SerializeField] private LayerMask fgPlayerLayer;

    // stamina related component ref here:
    private PlayerStatManager _statManager;

    // player's animation controller:
    private PlayerAnimController _animController;

    // player's sprite renderer:
    [SerializeField] private SpriteRenderer _childSpriteRender;
    
    [SerializeField] private float blinkCooldownTime = 3.0f;
    private bool canBlink = true;
    public event Action<float> OnBlinkCooldown;
    private IBlinkStrategy _blinkStrat;

    WaitForFixedUpdate blinkCooldown = new WaitForFixedUpdate();

    protected override void Awake()
    {
        // makes sure that we auto-get the reference for the rigidbody at runtime:
        _rb = GetComponent<Rigidbody2D>();

        // makes sure that we get the reference for the stat manager at runtime:
        _statManager = GetComponent<PlayerStatManager>();

        // makes sure that we get the reference for the player anim controller at runtime:
        _animController = GetComponent<PlayerAnimController>();

        _spriteRender = GetComponent<SpriteRenderer>();

        // ignoring the background platform in the beginning
        int layerIndex = (int)Mathf.Log(_isBackground ? bgPlayerLayer : fgPlayerLayer, 2);
        gameObject.layer = layerIndex;
        _blinkStrat = new NormalBlinkStrategy();

        originalScale = transform.localScale;
        heightDistance = 1.5f;
        myLayer = "Player";
        
        remainStunTime = new WaitForSecondsTracked(0f);
    }

    public void ApplyAllGameData(bool isBackground, Vector3 savedPosition, Quaternion savedRotation, Vector3 savedScale)
    {

        gameObject.transform.position = savedPosition;
        gameObject.transform.rotation = savedRotation;
        
        _isBackground = isBackground;
        int layerIndex = (int)Mathf.Log(_isBackground ? bgPlayerLayer : fgPlayerLayer, 2);
        gameObject.layer = layerIndex;
        ChangeOrderInLayer();
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
        // if (_statusEffects != null && !_statusEffects.CanMove)
        //     MoveDir = 0f;

        // _rb.linearVelocity = new Vector2(MoveDir * GetModifiedMoveSpeed(), _rb.linearVelocity.y);
        
        // if MoveDir != 0, it means that the player is moving in either direction:
        _animController.SetToWalk(_rb.linearVelocity.x != 0f);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="newStrat"></param>
    public void SetBlinkStrat(IBlinkStrategy newStrat)
    {
        _blinkStrat = newStrat;
    }

    /// <summary>
    /// Sets the player's move direction. Also flips the character via anim controller
    /// -1 = left
    /// 0 = idle
    /// 1 = right
    /// </summary>
    /// <param name="direction">the direction value in float</param>
    public override void SetMoveDirection(float direction)
    {
        base.SetMoveDirection(direction);
        _animController.FlipCharacter(direction);
    }

    /// <summary>
    /// Adds force to the character to have it jump.
    /// Only works if the character is currently grounded.
    /// </summary>
    public override void Jump()
    {
        //if (_statusEffects != null && !_statusEffects.CanMove) return;
        if (IsGrounded)
        {
            _rb.AddForce(Vector2.up * curJumpHeight, ForceMode2D.Impulse);
            FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/Jump");
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
        //if (_statusEffects != null && !_statusEffects.CanMove) return;
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
        //if (_statusEffects != null && !_statusEffects.CanMove) return;
        _rb.AddForce(Vector2.up * flyForce, ForceMode2D.Force);
        _statManager.UseStamina(0.01f);
    }

    /// <summary>
    /// Checks if the player can 'blink' to another platform
    /// and performs the action if so. 
    /// </summary>
    public void BlinkToOtherPlatform()
    {
        //if (_statusEffects != null && !_statusEffects.CanCast) return;
        /*
        'Blinking' is basically the term for teleporting between the foreground and background platforms.
        We may need to have our own calculation system for determining where on the platform Chloe should
        teleport to. 
        */

        // If blink is unlocked
        if(!GameManager.Instance.IsSpellUnlocked(Types.EAbilityType.Blink)) return;

        
        if(!canBlink) return;

        canBlink = false;


        bool enableBlink = true;
        Vector2 teleportLocation;
        
        // 3. reposition the player character:
        (enableBlink, teleportLocation) = _blinkStrat.ProcessTeleport(1.5f * 3f, _isBackground, 
            _animController.IsFacingRight, transform); //new Vector2(hitresult.point.x, hitresult.point.y + 1.0f);

        if(enableBlink == false)
        {
            Debug.Log("failed to teleport");
            StartCoroutine(BlinkCooltimeChk(blinkCooldownTime));
            return;
        }
        
        _rb.position = teleportLocation;

        _animController.SetToIsBlinkingStartTrig();
        
        FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/Blink");

        // 1. flip the _isBackground value before we reposition the character:
        _isBackground = !_isBackground;
        
        // 2. ignoring the colliders of the source ground
        // and enabling the colliders for the destination ground:
        int layerIndex = (int)Mathf.Log(_isBackground ? bgPlayerLayer : fgPlayerLayer, 2);
        gameObject.layer = layerIndex;

        // 4. changing the order in layer:
        ChangeOrderInLayer();

        
        StartCoroutine(BlinkCooltimeChk(blinkCooldownTime));
    }

    public override void ForceToBeOnForeground()
    {
        base.ForceToBeOnForeground();
        int layerIndex = (int)Mathf.Log(_isBackground ? bgPlayerLayer : fgPlayerLayer, 2);
        gameObject.layer = layerIndex;
        ChangeOrderInLayer();
    }

    public int GetCurrentLayer()
    {
        return IsBackground ? _bgLayerIndex : _fgLayerIndex;
    }

    #region Cooltime
    IEnumerator BlinkCooltimeChk(float cool)
    {
        Debug.Log("Cooltime start");
        UIManager.Instance.Get<UIPlayerHUD>().SlotCooldown(cool, Types.EAbilityType.Blink);

        while (cool > 1.0f)
        {
            cool -= Time.deltaTime;
            OnBlinkCooldown?.Invoke(cool);
            yield return blinkCooldown;
        }

        canBlink = true;
    }
    #endregion

    public override void ResetState()
    {
        _animController.ResetState();
        _isBackground = false;
        int layerIndex = (int)Mathf.Log(_isBackground ? bgPlayerLayer : fgPlayerLayer, 2);
        gameObject.layer = layerIndex;
        ChangeOrderInLayer();
    }
}
