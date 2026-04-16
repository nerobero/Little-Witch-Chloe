using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Captures/caches the raw input values from the user and passes the values 
/// to the appropriate class references for value processing.
/// </summary>
public class PlayerController : MonoBehaviour, PlayerInput.IBaseInputActionActions
{
    // Reference to input-event binds
    private PlayerInput _inputContext;

    // Handler for player's movement
    private PlayerMovement _playerMove;

    // Handler for player's attack system
    private PlayerAttack _playerAttack;


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
    private float _jumpPressTime = -1f;
    private bool _isFlying = false;


    #region Setup
    private void Awake()
    {
        // create PlayerInput:
        _inputContext = new PlayerInput();

        // Caching once, never having to re-fetch again:
        _playerMove = GetComponent<PlayerMovement>();
        _playerAttack = GetComponent<PlayerAttack>();
        // _animator = GetComponent<PlayerAnimator>();
    }

    private void OnEnable()
    {
        //call BaseInputAction.AddCallbacks(this)
        _inputContext.BaseInputAction.AddCallbacks(this);

        // enable the input action binding
        _inputContext.BaseInputAction.Enable();

    }

    private void OnDisable()
    {
        //disable the input action binding
        _inputContext.BaseInputAction.Disable();

        _inputContext.BaseInputAction.RemoveCallbacks(this);
    }

    private void OnDestroy()
    {
        // when the controller is destroyed, we also dispose of the inputcontext
        // preventing any leftover memory references at runtime
        _inputContext.Dispose();
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
        // if (context.performed) _playerMove.Jump();

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
            // when cancelled, resetting the snapshot to negative (i.e., invalid) value:
            _jumpPressTime = -1f;

            // if the player was flying:
            if (_isFlying)
            {
                _isFlying = false;
                _playerMove.StopFlying();
            }
        }

    }

    private void Update()
    {
        // if there is a valid time snapshot for jump start and 
        // the player is not already flying:
        if (_jumpPressTime >= 0f && !_isFlying)
        {
            // elapsed duration of the hold = current time - the jump start time snapshot
            float heldFor = Time.time - _jumpPressTime;
            if (heldFor >= flyingThreshold)
            {
                _isFlying = true;
                _playerMove.StartFlying();
            }
        }
    }

    public void OnAimAttack(InputAction.CallbackContext context) { }

    public void OnAttack(InputAction.CallbackContext context)
    {
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
        if (context.performed) _playerMove.BlinkToOtherPlatform();
    }

    #endregion
}