using UnityEngine;

public class EnemyAnimController : BaseCharacterAnimController
{
    protected static readonly int IsAttackingHash = Animator.StringToHash("IsAttacking");
    protected static readonly int IsAttackingTrigHash = Animator.StringToHash("IsAttackingTrig");
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public virtual void onDead()
    {
        
    }

    public virtual void SetToIsAttacking(bool isAttacking)
    {
        _animator.SetBool(IsAttackingHash, isAttacking);
    }

    public void SetToIsAttacking()
    {
        _animator.SetTrigger(IsAttackingTrigHash);
    }

}
