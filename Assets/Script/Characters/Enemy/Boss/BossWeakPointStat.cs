using UnityEngine;
using Types;
using System.Collections;

/// <summary>
/// Separate stat manager for the boss NPCs. (weakpoint Stats)
/// </summary>
public class BossWeakPointStat : EnemyCharacterBase
{
    private static readonly int IsDeadHash = Animator.StringToHash("IsDead");

    protected Animator _animator;
    private Collider2D _collider;

    // set by BossEnemyStatManager.KnockedDown() to match the body's stun duration
    private float _reactivateDelay = 10f;

    public void SetReactivateDelay(float delay)
    {
        _reactivateDelay = delay;
    }

    protected override void Start()
    {
        base.Start();
        _animator = GetComponent<Animator>();
        _collider = GetComponent<Collider2D>();
    }

    public override bool TakeDamage(GameObject instigator, float damageAmount, EElementType damageElement)
    {
        if (IsDead || damageAmount <= 0.0f)
            return false;

        float actualDamage = CalculateActualDamage(damageAmount, damageElement, mainCharacElement);

        currentHP = Mathf.Clamp(currentHP - actualDamage, 0.0f, maxHP);
        FMODUnity.RuntimeManager.PlayOneShot(OnTakenDamageEvent);
        InvokeOnHPChanged(currentHP, maxHP, instigator);

        if (currentHP == 0.0f)
        {
            Death();
        }

        return true;
    }

    // Called when the weak point is knocked down. This is temporary - it stuns the body and
    // plays the flower's Stunned animation via the shared IsStunned bool (the GameObject stays
    // active so that animation can actually play). The Dead animation is reserved for
    // FinalizeDeath(), which only fires once the boss itself actually dies.
    public override void Death()
    {
        base.Death();

        if (_collider != null) _collider.enabled = false;

        Invoke("Reactivate", _reactivateDelay);
    }

    private void Reactivate()
    {
        if(!IsDead) return;
        if (_collider != null) _collider.enabled = true;
        IsDead = false;
        currentHP = maxHP;
    }

    // Called by BossEnemyStatManager.Death() once the boss's own HP hits 0. Unlike a regular
    // knockdown, this is permanent: plays the flower's actual Dead animation and cancels any
    // pending reactivation.
    public void FinalizeDeath()
    {
        CancelInvoke();
        if (_collider != null) _collider.enabled = false;
        if (_animator != null) _animator.SetBool(IsDeadHash, true);
    }

    public override void ResetState()
    {
        CancelInvoke();
        if (_collider != null) _collider.enabled = true;
        if (_animator != null) _animator.SetBool(IsDeadHash, false);
        base.ResetState();
    }
}