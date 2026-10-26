using System;
using UnityEngine;

public class EnemyAnimController : BaseCharacterAnimController
{
    //protected static readonly int IsAttackingHash = Animator.StringToHash("IsAttacking");
    //protected static readonly int IsAttackingTrigHash = Animator.StringToHash("IsAttackingTrig");

    //protected static readonly int IsDeadHash = Animator.StringToHash("IsDead");
    
    protected static readonly int HurtHash = Animator.StringToHash("IsHurt");

    private EnemyHPWidget enemyHPWidget;

    //public event Action OnFlipped;

    protected SpriteRenderer FirePointObj;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        FirePointObj = GetComponent<EnemyAttack>().FirePointObj;
        enemyHPWidget = GetComponent<EnemyHPWidget>();
        EnemyCharacterBase enemyStat = GetComponent<EnemyCharacterBase>();
        
        enemyStat.OnDeath += SetToDead;
    }

    public virtual void SetToDead()
    {
        _animator.SetBool(IsDeadHash, true);

        EnemyCharacterBase enemyStat = GetComponent<EnemyCharacterBase>();
        
        enemyStat.OnDeath -= SetToDead;
    }

    public virtual void OnDeathFinished()
    {
        gameObject.SetActive(false);
    }

    public override void ResetState()
    {
        base.ResetState();

        EnemyCharacterBase enemyStat = GetComponent<EnemyCharacterBase>();
        
        enemyStat.OnDeath += SetToDead;
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
            if(enemyHPWidget != null && enemyHPWidget.HPSlider != null)
            {
                Transform bar = enemyHPWidget.HPSlider.transform;
                Vector3 scale = bar.localScale;
                scale.x = Mathf.Abs(scale.x) * (transform.localScale.x < 0f ? -1f : 1f);
                bar.localScale = scale;
            }
        }

        // if (!IsFacingRight && moveDir < 0f || IsFacingRight && moveDir > 0f)
        // {
        //     OnFlipped?.Invoke();
        // }
    }

    public virtual void Hurt()
    {
        _animator.SetTrigger(HurtHash);
    }
}
