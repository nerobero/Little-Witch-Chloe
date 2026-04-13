using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Captures/caches the raw input values from the user and passes the values 
/// to the appropriate class references for value processing.
/// </summary>
public class PlayerController : MonoBehaviour, PlayerInput.IBaseInputActionActions
{
    // @TODO: Add a serialized private/public PlayerMovement class reference here
    private PlayerMovement _playerMove;
    // @TODO: Add a serialized private/public PlayerAnimControl class reference here

    // Reference to input-event binds
    private PlayerInput _inputContext;
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
    public void OnMoveLeftRight(InputAction.CallbackContext context) { }
    
    public void OnJump(InputAction.CallbackContext context) { }
    
    public void OnAimAttack(InputAction.CallbackContext context) { }
    
    public void OnNormalAttack(InputAction.CallbackContext context) { }
    
    public void OnChargedAttack(InputAction.CallbackContext context) { }

    public void OnBlink(InputAction.CallbackContext context) {}

#endregion
}