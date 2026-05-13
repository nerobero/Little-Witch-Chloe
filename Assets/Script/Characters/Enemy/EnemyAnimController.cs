using System;
using UnityEngine;

public class EnemyAnimController : BaseCharacterAnimController
{
    protected static readonly int IsAttackingHash = Animator.StringToHash("IsAttacking");
    protected static readonly int IsAttackingTrigHash = Animator.StringToHash("IsAttackingTrig");

    private EnemyHPWidget enemyHPWidget;

    //public event Action OnFlipped;

    protected SpriteRenderer FirePointObj;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        FirePointObj = GetComponent<EnemyAttack>().FirePointObj;
        enemyHPWidget = GetComponent<EnemyHPWidget>();
        
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

        
        if(enemyHPWidget)
        {
            if (IsFacingRight && moveDir > 0f || !IsFacingRight && moveDir < 0f)
            {
                Vector2 localScale2D = transform.localScale;
                localScale2D.x = IsFacingRight ? 1f : -1f;
                enemyHPWidget.HPSlider.transform.localScale = localScale2D;
            }
        }

        // if (!IsFacingRight && moveDir < 0f || IsFacingRight && moveDir > 0f)
        // {
        //     OnFlipped?.Invoke();
        // }
    }
}
