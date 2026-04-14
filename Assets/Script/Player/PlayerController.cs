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
    
    private PlayerMovement _playerMove;
#region Setup
    private void Awake()
    {
        // create PlayerInput:
        _inputContext = new PlayerInput();

        //call BaseInputAction.AddCallbacks(this)
        _inputContext.BaseInputAction.AddCallbacks(this);
    }

    private void OnEnable()
    {
        // enable the input action binding
        _inputContext.BaseInputAction.Enable();
    }

    private void OnDisable()
    {
        //disable the input action binding
        _inputContext.BaseInputAction.Disable();
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
        if (context.performed) _playerMove.Jump();
    }
    
    public void OnAimAttack(InputAction.CallbackContext context) { }
    
    public void OnNormalAttack(InputAction.CallbackContext context) { }
    
    public void OnChargedAttack(InputAction.CallbackContext context) { }

    public void OnBlink(InputAction.CallbackContext context)
    {
        if (context.performed) _playerMove.BlinkToOtherPlatform();
    }

#endregion
}