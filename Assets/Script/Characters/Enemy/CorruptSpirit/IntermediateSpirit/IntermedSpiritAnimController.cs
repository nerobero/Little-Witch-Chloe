using UnityEngine;

public class IntermedSpiritAnimController : EnemyAnimController
{
    private static readonly int IsFlyingHash = Animator.StringToHash("IsFlying");
    private static readonly int FlyTickHash = Animator.StringToHash("FlyTick");

    private static readonly int IsDeadHash = Animator.StringToHash("IsDead");
    private static readonly int DeadOneShot = Animator.StringToHash("DeadOneShot");

    private static readonly int IsAttackingHash = Animator.StringToHash("IsAttacking");
    private static readonly int IsAttackingTrigHash = Animator.StringToHash("IsAttackingTrig");

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

    public void SetToIsAttacking()
    {
        _animator.SetTrigger(IsAttackingTrigHash);
    }

    public void SetToIsAttacking(bool isAttacking)
    {
        _animator.SetBool(IsAttackingHash, isAttacking);
    }
}
