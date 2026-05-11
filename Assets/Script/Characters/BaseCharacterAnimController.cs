using UnityEngine;

/// <summary>
/// Base controller for character animation.
/// All state change logic for the character's animation is handled here
/// for the simplicity of debugging and better readability.
/// </summary>
[RequireComponent(typeof(Animator))]
public class BaseCharacterAnimController : MonoBehaviour
{
    protected static readonly int IdleHash = Animator.StringToHash("IdleHash");
    protected static readonly int WalkingHash = Animator.StringToHash("IsWalking");
    protected static readonly int BlinkingHash = Animator.StringToHash("IsBlinking");

    protected Animator _animator;

    public bool IsFacingRight = true;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    /// <summary>
    /// Flips the owner of this component on the x-axis.
    /// </summary>
    public virtual void FlipCharacter(float moveDir)
    {
        // we only process the character to flip when the 
        if (!IsFacingRight && moveDir > 0f || IsFacingRight && moveDir < 0f)
        {
            IsFacingRight = !IsFacingRight;
            Vector2 localScale2D = transform.localScale;
            localScale2D.x *= -1f;
            transform.localScale = localScale2D;
        }
    }

    public void SetToWalk(bool isWalking)
    {
        _animator.SetBool(WalkingHash, isWalking);
    }
}
