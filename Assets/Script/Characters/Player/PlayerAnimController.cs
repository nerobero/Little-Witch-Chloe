using UnityEngine;

/// <summary>
/// Controller for character's animation.
/// All state change logic for the character's animation is handled here
/// for the simplicity of debugging and better readability.
/// </summary>
[RequireComponent(typeof(Animator))]
public class PlayerAnimController : MonoBehaviour
{
    private static readonly int IdleHash = Animator.StringToHash("IdleHash");
    private static readonly int WalkingHash = Animator.StringToHash("IsWalking");
    private static readonly int IsFlyingHash = Animator.StringToHash("IsFlying");
    private static readonly int FlyTickHash = Animator.StringToHash("FlyTick");

    private Animator _animator;
    public bool _isFacingRight = true;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    /// <summary>
    /// Flips the owner of this component on the x-axis.
    /// </summary>
    public void FlipCharacter(float moveDir)
    {
        // we only process the character to flip when the 
        if (!_isFacingRight && moveDir > 0f || _isFacingRight && moveDir < 0f)
        {
            _isFacingRight = !_isFacingRight;
            Vector2 localScale2D = transform.localScale;
            localScale2D.x *= -1f;
            transform.localScale = localScale2D;
        }
    }

    public void SetToWalk(bool isWalking)
    {
        _animator.SetBool(WalkingHash, isWalking);
    }

    public void SetToStartFlying()
    {
        _animator.SetBool(IsFlyingHash, true);
        _animator.SetTrigger(FlyTickHash);
    }

    public void SetToStopFlying()
    {
        _animator.SetBool(IsFlyingHash, false);
    }
}
