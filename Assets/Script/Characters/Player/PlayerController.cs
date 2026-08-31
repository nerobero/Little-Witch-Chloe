using UnityEngine;
using UnityEngine.InputSystem;
using System;
using Types;

/// <summary>
/// Captures/caches the raw input values from the user and passes the values 
/// to the appropriate class references for value processing.
/// </summary>
public class PlayerController : MonoBehaviour, PlayerInput.IBaseInputActionActions, PlayerInput.IUIActions, IResetable, IStatusEffect
{
    private static PlayerController _instance;
    public static PlayerController Instance => _instance;

    #region ReferenceClasses
    // Reference to input-event binds
    public PlayerInput InputContext;

    // Handler for player's movement
    private PlayerMovement _playerMove;

    // Handler for player's attack system
    private PlayerAttack _playerAttack;
    public PlayerAttack PlayerAttack => _playerAttack;

    private InteractionSystem _playerInteract;
    //private StatusEffectController _statusEffects;
    private PlayerStatManager _playerStat;
    public PlayerStatManager PlayerStat => _playerStat;

    private Camera _mainCamera;
    #endregion

    [Header("Attack - Input hold duration")]
    [SerializeField] private float chargeThreshold = 1.5f; // seconds to trigger a charged attack
    [SerializeField] private float maxChargeTime = 3f;
    private float _attackPressTime = -1f;

    // Used Time.time instead of Time.deltaTime because
    // the charge logic is directly related to the actual timestamp,
    // which is not something that accumlates PER frame
    public float CurrentChargeRatio =>
        _attackPressTime < 0f ? 0f : Mathf.Clamp01((Time.time - _attackPressTime / maxChargeTime));


    // Jump => Flying transition related variables:
    [Header("Jump => Flying transition")]
    [SerializeField] private float flyingThreshold = 1.5f;
    public bool IsFlying => _isFlying;
    private bool _isFlying = false;
    private float _jumpPressTime = -1f;
    private bool _optionMenuEnabled = false;

    public event Action<bool> onBlinked;
    public PlayerMovement PlayerMove => _playerMove;
    private Vector3 spawnPosition;
    private Quaternion spawnRotation;

    #region Setup
    private void Awake()
    {
        // assigning the instance:
        _instance = this;

        // create PlayerInput:
        InputContext = new PlayerInput();

        // Caching once, never having to re-fetch again:
        _playerMove = GetComponent<PlayerMovement>();
        _playerAttack = GetComponent<PlayerAttack>();
        //_statusEffects = GetComponent<StatusEffectController>();

        _playerInteract = GetComponent<InteractionSystem>();
        _playerStat = GetComponent<PlayerStatManager>();

        _mainCamera = Camera.main;

        _playerAttack.isBackground = _playerMove.IsBackground;
    }

    private void Start()
    {
        UIManager.Instance.Get<UIPlayerHUD>()?.Initialize(); 
        LevelManager.Instance.RegisterInstance(this);
        SaveManager.Instance.Register(this);

        spawnPosition = gameObject.transform.position;
        spawnRotation = gameObject.transform.rotation;

        _playerStat.OnDeath += Death;
    }

    private void OnEnable()
    {
        //call BaseInputAction.AddCallbacks(this)
        InputContext.BaseInputAction.AddCallbacks(this);

        // enable the input action binding
        InputContext.BaseInputAction.Enable();

        _playerMove.OnFlyStopped += OnFlyStopped;

        InputContext.UI.AddCallbacks(this);
    }

    private void OnDisable()
    {
        //disable the input action binding
        InputContext.BaseInputAction.Disable();

        InputContext.BaseInputAction.RemoveCallbacks(this);

        _playerMove.OnFlyStopped -= OnFlyStopped;
    }

    private void OnDestroy()
    {
        // when the controller is destroyed, we also dispose of the inputcontext
        // preventing any leftover memory references at runtime
        InputContext.Dispose();
    }
    #endregion

    #region InputCallbacks

    // Passes the float value caught from the input callback context
    // directly into the PlayerMovement move direction
    public void OnMoveLeftRight(InputAction.CallbackContext context)
        => _playerMove.SetMoveDirection(context.ReadValue<float>());


    // Calls PlayerMovement.Jump if the input has been performed
    public void OnJump(InputAction.CallbackContext context)
    {
        //if (_statusEffects != null && !_statusEffects.CanMove) return;
        if (context.performed) _playerMove.Jump();

        /*
        2026.04.16: Note from Angela -
        Because of the newly added feature 'flying' which shares the same key as jump, 
        I added logic gates that determines whether to jump or to fly depending on the key hold time.
        */
        if (context.started)
        {
            // keeping a snapshot of the time at which the jump was first processed:
            _jumpPressTime = Time.time;
            _playerMove.Jump(); // immediate jump on press
        }
        else if (context.canceled)
        {
            _jumpPressTime = -1f;

            if (_isFlying)
                _playerMove.StopFlying(); // fires OnFlyStopped event → sets _isFlying = false
        }

    }
    private void OnFlyStopped()
    {
        _isFlying = false;
    }

    private void Update()
    {
        // if there is a valid time snapshot for jump start and 
        // the player is not already flying:
        if (_jumpPressTime >= 0f && !_isFlying)
        {
            // elapsed duration of the hold = current time - the jump start time snapshot
            float heldFor = Time.time - _jumpPressTime;
            if (heldFor >= flyingThreshold) // state change
            {
                if (!GameManager.Instance.IsSpellUnlocked(Types.EAbilityType.Flying)) return;
                _isFlying = true;
                _playerMove.StartFlying(); // always called before FlyTick()
            }
        }
    }

    private void FixedUpdate()
    {
        if (_isFlying)
            _playerMove.FlyTick(); // force applied here, every frame after Update()
    }

    public void OnAimAttack(InputAction.CallbackContext context)
    {
        Vector2 mousePosition = context.ReadValue<Vector2>();
        mousePosition = _mainCamera.ScreenToWorldPoint(mousePosition) - transform.position;
        _playerAttack.SetAimDirection(mousePosition);

    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        //if (_statusEffects != null && !_statusEffects.CanAttack) return;
        /*
        2026.04.15: 
        Combined input context catching for normal and charged attacks into one input callback
        function instead of two. 
        'Hold' interaction in the input action is for actions that requires the player to commit
        to a full hold, e.g., a context menu long-press.  
        Thus, we keep track of the duration of which the player holds the input key down and call different
        functions depending on that tracked duration.
        */
        if (context.started)
        {
            _attackPressTime = Time.time; // button down — start tracking
        }
        else if (context.canceled)
        {
            if (_attackPressTime < 0f) return;

            float heldFor = Time.time - _attackPressTime;
            _attackPressTime = -1f;

            if (heldFor >= chargeThreshold)
            {
                float chargeRatio = Mathf.Clamp01(heldFor / maxChargeTime);
                _playerAttack.FireCharged(chargeRatio); // 0.0 = min charge, 1.0 = full
            }
            else
            {
                _playerAttack.FireNormal();
            }
        }

    }

    public void OnBlink(InputAction.CallbackContext context)
    {
        //if (_statusEffects != null && !_statusEffects.CanCast) return;
        if (context.performed)
        {
            _playerMove.BlinkToOtherPlatform();
            _playerAttack.isBackground = _playerMove.IsBackground;
            onBlinked?.Invoke(_playerMove.IsBackground);
        }
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        // Button actions fire started/performed/canceled; only act once per press.
        if (!context.performed) return;
        _playerInteract.Interact();
    }

    public void OnChangeWeapon(InputAction.CallbackContext context)
    {
        //if (_statusEffects != null && !_statusEffects.CanCast) return;
        if (!context.performed) return;

        var controlName = context.control.name;

        if (int.TryParse(controlName, out int slot))
        {
            _playerAttack.SelectWeapon(slot - 1);
        }
    }

    // Show the option pop up
    public void OnPause(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            //if (!_optionMenuEnabled)
            //{
                PauseManager.Instance.PauseGame();
                UIManager.Instance.Show<PopupHUD>();
                //_optionMenuEnabled = true;
            //}
            // else
            // {
            //     PauseManager.Instance.UnpauseGame();
                
            //     InputContext.BaseInputAction.Enable();
            //     InputContext.UI.Disable();
            //     UIManager.Instance.Hide<PopupHUD>();
            //     _optionMenuEnabled = false;
            // }
        }
    }

    #region UI Input
    public void OnUnpause(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            PauseManager.Instance.UnpauseGame();
                    
            // InputContext.BaseInputAction.Enable();
            // InputContext.UI.Disable();
            UIManager.Instance.Hide<PopupHUD>();
            //_optionMenuEnabled = false;
        }
    }

    public void OnNavigate(InputAction.CallbackContext context)
    {
        //
    }

    public void OnSubmit(InputAction.CallbackContext context)
    {
        //
    }

    public void OnCancel(InputAction.CallbackContext context)
    {
        //
    }

    public void OnPoint(InputAction.CallbackContext context)
    {
        //
    }

    public void OnClick(InputAction.CallbackContext context)
    {
        //
    }
    public void OnScrollWheel(InputAction.CallbackContext context)
    {
        //
    }

    public void OnMiddleClick(InputAction.CallbackContext context)
    {
        //
    }

    public void OnRightClick(InputAction.CallbackContext context)
    {
        //
    }

    public void OnTrackedDevicePosition(InputAction.CallbackContext context)
    {
        //
    }

    public void OnTrackedDeviceOrientation(InputAction.CallbackContext context)
    {
        //
    }
    
    #endregion

    public void OnAttackAcross(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            _playerAttack.EnableCrossPlatformAttack(true);
            Debug.Log("[Cross Platform Attack] Enabled");
        }
        else if (context.canceled)
        {
            _playerAttack.EnableCrossPlatformAttack(false);
            Debug.Log("[Cross Platform Attack] Disabled");
        }
    }
#endregion

    public void Death()
    {
        Instance.InputContext.UI.Enable();
    }

    #region Reset
    public void ResetState()
    {
        UIManager.Instance.Get<UIPlayerHUD>().Initialize(); 
        _playerAttack.ResetState();
        _playerMove.ResetState();
        _playerStat.ResetState();

        gameObject.transform.position = spawnPosition + new Vector3(0f, 1f, 0f);
        gameObject.transform.rotation = spawnRotation;
    }

    #endregion

    #region StatusEffect
    //public void ApplyStatusEffect(EStatusEffectType type, float magnitude, GameObject instigator)
    public void ApplyStatusEffect(ActiveStatusEffect effect)
    {
        switch(effect.definition.Type)
        {
            case EStatusEffectType.AttackUp:
                _playerAttack.AddDamageMultiplier(effect.definition.Magnitude);
            break;
            case EStatusEffectType.AttackDown:
                _playerAttack.ReduceDamageMultiplier(effect.definition.Magnitude);
            break;
            case EStatusEffectType.AttackSpeedUp:
                _playerAttack.AddAttackSpeedMultiplier(effect.definition.Magnitude);
            break;
            case EStatusEffectType.AttackSpeedDown:
                _playerAttack.ReduceAttackSpeedMultiplier(effect.definition.Magnitude);
            break;
            case EStatusEffectType.MoveSpeedUp:
                _playerMove.AddSpeedMultiplier(effect.definition.Magnitude);
            break;
            case EStatusEffectType.Slow:
                _playerMove.ReduceSpeedMultiplier(effect.definition.Magnitude);
            break;
            case EStatusEffectType.AntiPoisonFog:
                _playerStat.SetPoisonImmune(true);
            break;

            // Not implemented
            case EStatusEffectType.DefenseUp:
            case EStatusEffectType.Shield:
            case EStatusEffectType.DefenseDown:
            break;

            // CC
            // These things make the character not be able to move.
            case EStatusEffectType.Stun:
                Stun();
                _playerStat.AddCrowdControl(effect.definition.CCType);
                _playerMove.SetCouldMove(false);
            break;
            case EStatusEffectType.Root:
                Rooted();
                _playerStat.AddCrowdControl(effect.definition.CCType);
                _playerMove.SetCouldMove(false);
            break;
            case EStatusEffectType.Knockback:
                Stun();
                _playerStat.AddCrowdControl(effect.definition.CCType);
                Vector2 knockbackDir = (transform.position - effect.instigator.transform.position).normalized;
                _playerMove.ApplyKnockback(knockbackDir, effect.definition.Magnitude);
            break;
            case EStatusEffectType.Fear:
                Stun();
                _playerStat.AddCrowdControl(effect.definition.CCType);
            break;
            case EStatusEffectType.Blind:
                _playerStat.AddCrowdControl(effect.definition.CCType);
            break;

        }

        UIManager.Instance.Get<StatusEffectSlotContainer>().AddNewItem(effect);
    }

    //public void RemoveStatusEffect(EStatusEffectType type, float magnitude)
    public void RemoveStatusEffect(ActiveStatusEffect effect)
    {
        switch(effect.definition.Type)
        {
            case EStatusEffectType.AttackUp:
                _playerAttack.ReduceDamageMultiplier(effect.definition.Magnitude);
            break;
            case EStatusEffectType.AttackDown:
                _playerAttack.AddDamageMultiplier(effect.definition.Magnitude);
            break;
            case EStatusEffectType.AttackSpeedUp:
                _playerAttack.ReduceAttackSpeedMultiplier(effect.definition.Magnitude);
            break;
            case EStatusEffectType.AttackSpeedDown:
                _playerAttack.AddAttackSpeedMultiplier(effect.definition.Magnitude);
            break;
            case EStatusEffectType.MoveSpeedUp:
                _playerMove.ReduceSpeedMultiplier(effect.definition.Magnitude);
            break;
            case EStatusEffectType.Slow:
                _playerMove.AddSpeedMultiplier(effect.definition.Magnitude);
            break;
            case EStatusEffectType.AntiPoisonFog:
                _playerStat.SetPoisonImmune(false);
            break;

            // Not implemented
            case EStatusEffectType.DefenseUp:
            case EStatusEffectType.Shield:
            case EStatusEffectType.DefenseDown:
            break;

            // CC
            case EStatusEffectType.Stun:
                StunFinished();
                _playerStat.RemoveCrowdControl(effect.definition.CCType);
                _playerMove.SetCouldMove(true);
            break;
            case EStatusEffectType.Root:
                _playerMove.SetCouldMove(true);
                _playerStat.RemoveCrowdControl(effect.definition.CCType);
                RootedFinished();
            break;
            case EStatusEffectType.Knockback:
                _playerStat.RemoveCrowdControl(effect.definition.CCType);
                StunFinished();
            break;
            case EStatusEffectType.Fear:
                _playerStat.RemoveCrowdControl(effect.definition.CCType);
                StunFinished();
            break;
            case EStatusEffectType.Blind:
                _playerStat.RemoveCrowdControl(effect.definition.CCType);
            break;
        }
    }
    #endregion

    #region Stun
    private void Stun()
    {
        InputContext.FindAction("MoveLeftRight").Disable();
        InputContext.FindAction("Jump").Disable();
        InputContext.FindAction("AimAttack").Disable();
        InputContext.FindAction("Attack").Disable();
        InputContext.FindAction("Blink").Disable();
        InputContext.FindAction("Interact").Disable();
        InputContext.FindAction("ChangeWeapon").Disable();
        InputContext.FindAction("AttackAcross").Disable();
    }

    private void StunFinished()
    {
        InputContext.FindAction("MoveLeftRight").Enable();
        InputContext.FindAction("Jump").Enable();
        InputContext.FindAction("AimAttack").Enable();
        InputContext.FindAction("Attack").Enable();
        InputContext.FindAction("Blink").Enable();
        InputContext.FindAction("Interact").Enable();
        InputContext.FindAction("ChangeWeapon").Enable();
        InputContext.FindAction("AttackAcross").Enable();
    }
    #endregion

    #region Rooted
    private void Rooted()
    {
        InputContext.FindAction("MoveLeftRight").Disable();
        InputContext.FindAction("Jump").Disable();
        InputContext.FindAction("Blink").Disable();
    }

    private void RootedFinished()
    {
        InputContext.FindAction("MoveLeftRight").Enable();
        InputContext.FindAction("Jump").Enable();
        InputContext.FindAction("Blink").Enable();
    }
    #endregion
}
