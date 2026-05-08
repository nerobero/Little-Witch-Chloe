using UnityEngine;

/// <summary>
/// Controller for character's animation.
/// All state change logic for the character's animation is handled here
/// for the simplicity of debugging and better readability.
/// </summary>
[RequireComponent(typeof(Animator))]
public class PlayerAnimController : BaseCharacterAnimController
{
    private static readonly int IsFlyingHash = Animator.StringToHash("IsFlying");
    private static readonly int FlyTickHash = Animator.StringToHash("FlyTick");

    private static readonly int IsDeadHash = Animator.StringToHash("IsDead");
    private static readonly int DeadOneShot = Animator.StringToHash("DeadOneShot");

    public bool _isFacingRight = true;

    public void SetToStartFlying()
    {
        _animator.SetBool(IsFlyingHash, true);
        _animator.SetTrigger(FlyTickHash);
    }

    public void SetToStopFlying()
    {
        _animator.SetBool(IsFlyingHash, false);
    }

    public void SetToDead()
    {
        _animator.SetBool(IsDeadHash, true);
        _animator.SetTrigger(DeadOneShot);
    }
}
