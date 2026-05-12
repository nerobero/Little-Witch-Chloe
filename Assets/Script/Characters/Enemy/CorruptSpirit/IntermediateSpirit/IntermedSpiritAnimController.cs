using UnityEngine;

public class IntermedSpiritAnimController : EnemyAnimController
{

    private static readonly int IsSeenHash = Animator.StringToHash("IsSeen");
    private static readonly int IsSeenTrigHash = Animator.StringToHash("IsSeenTrigg");
    private static readonly int TransAttackTrigHash = Animator.StringToHash("TransAttackTrigg");
    private static readonly int IsDeadHash = Animator.StringToHash("IsDead");
    private static readonly int DeadOneShot = Animator.StringToHash("DeadOneShot");

    public void SetToDead()
    {
        _animator.SetBool(IsDeadHash, true);
        _animator.SetTrigger(DeadOneShot);
    }


    public void SetToSeenTrans()
    {
        _animator.SetTrigger(IsSeenTrigHash);
        _animator.SetBool(IsSeenHash, true);
    }

    public void SetToIdle()
    {
        // _animator.SetBool(IsSeenHash, false);
        _animator.SetBool(IdleHash, true);
    }

    public override void SetToIsAttacking(bool isAttacking)
    {
        Debug.Log("Intermediate's called");
        _animator.SetTrigger(TransAttackTrigHash);
        base.SetToIsAttacking(isAttacking);
    }
}
