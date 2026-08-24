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

    private static readonly int DeadOneShot = Animator.StringToHash("DeadOneShot");
    private static readonly int HurtOneShot = Animator.StringToHash("IsHurtTrig");


    //private static readonly int IsAttackingHash = Animator.StringToHash("IsAttacking");
    //private static readonly int IsAttackingTrigHash = Animator.StringToHash("IsAttackingTrig");

    private static readonly int IsBlinkStartTrigHash = Animator.StringToHash("IsBlinkStartTrig");

    public bool _isFacingRight = true;

    private PlayerAttack playerAttack = null;

    void Start()
    {
        PlayerStatManager playerStat = GetComponent<PlayerStatManager>();
        playerAttack = GetComponent<PlayerAttack>();

        playerStat.OnDeath += SetToDead;
        playerStat.OnTakeDamage += SetToIsHurt;
    }

    /// <summary>
    /// Flips the owner of this component on the x-axis.
    /// </summary>
    public override void FlipCharacter(float moveDir)
    {
        base.FlipCharacter(moveDir);

        playerAttack.SetAimDirection(playerAttack.AimDirection);
    }

    public virtual void OnDeathFinished()
    {
        SoundManager.Instance.PlayGameOver();
        PlayerStatManager playerStat = GetComponent<PlayerStatManager>();
        
        playerStat.OnDeath -= SetToDead;
        playerStat.OnTakeDamage -= SetToIsHurt;
        //gameObject.SetActive(false);
        Debug.Log("DeathFinish");
        UIManager.Instance.Show<UIGameOverHUD>();
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

    public void SetToDead()
    {
        _animator.SetBool(IsDeadHash, true);
        _animator.SetTrigger(DeadOneShot);
        PlayerController.Instance.InputContext.BaseInputAction.Disable();
    }

    public void SetToIsHurt()
    {
        _animator.SetTrigger(HurtOneShot);
    }

    public void SetToIsAttacking()
    {
        _animator.SetTrigger(IsAttackingTrigHash);
    }

    public void SetToIsAttacking(bool isAttacking)
    {
        _animator.SetBool(IsAttackingHash, isAttacking);
    }

    public void SetToIsBlinkingStartTrig()
    {
        _animator.SetTrigger(IsBlinkStartTrigHash);
    }

    public override void ResetState()
    {
        PlayerStatManager playerStat = GetComponent<PlayerStatManager>();
        playerStat.OnDeath += SetToDead;
        playerStat.OnTakeDamage += SetToIsHurt;

        base.ResetState();
    }
}