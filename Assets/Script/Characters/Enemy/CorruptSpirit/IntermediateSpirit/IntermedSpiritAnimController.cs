using UnityEngine;

public class IntermedSpiritAnimController : EnemyAnimController
{

    private static readonly int IsSeenHash = Animator.StringToHash("IsSeen");
    private static readonly int IsSeenTrigHash = Animator.StringToHash("IsSeenTrigg");
    private static readonly int TransAttackTrigHash = Animator.StringToHash("TransAttackTrigg");
    private static readonly int DeadOneShot = Animator.StringToHash("DeadOneShot");

    public override void SetToDead()
    {
        //_animator.SetBool(IsDeadHash, true);
        _animator.SetTrigger(DeadOneShot);
        base.SetToDead();
    }


    // True once the PassiveIdle => SeenTrans => SeenIdle chain has been triggered.
    // The attack transition only exists from SeenIdle, so this must be true before attacking.
    public bool HasBeenSeen { get; private set; }

    public void SetToSeenTrans()
    {
        HasBeenSeen = true;
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
        _animator.SetTrigger(TransAttackTrigHash);
        base.SetToIsAttacking(isAttacking);
    }
}
