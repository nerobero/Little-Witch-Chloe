using System;
using UnityEngine;

public class EnemyAnimController : BaseCharacterAnimController
{
    protected static readonly int IsAttackingHash = Animator.StringToHash("IsAttacking");
    protected static readonly int IsAttackingTrigHash = Animator.StringToHash("IsAttackingTrig");

    public event Action OnFlipped;

    protected SpriteRenderer FirePointObj;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        FirePointObj = GetComponent<EnemyAttack>().FirePointObj;
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

    public override void FlipCharacter(float moveDir)
    {
        base.FlipCharacter(moveDir);

        // we only process the character to flip when the 
        if (!IsFacingRight && moveDir > 0f || IsFacingRight && moveDir < 0f)
        {
            Vector2 localPosition2D = FirePointObj.transform.position;
            localPosition2D.x *= -1f;
            FirePointObj.transform.localPosition = localPosition2D;
            OnFlipped?.Invoke();
        }
    }
}
