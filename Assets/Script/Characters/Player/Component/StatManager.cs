using System;
using UnityEngine;
using UnityEngine.Events;

public class StatManager : MonoBehaviour
{
    // HP
    [Header("HP Settings")]
    [SerializeField] protected float MaxHP;
    [SerializeField] protected float CurrentHP;

    // Event system
    public event Action<float, float> OnHPChanged;
    public event Action OnDeath;
    public event Action OnHeal;
    
    // Is dead
    public bool IsDead {get; protected set;}

    // Is Invincible
    public bool IsBlink{get; protected set;}

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CurrentHP = MaxHP;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// the base method of taking damage
    /// </summary>
    /// <param name="instigator">damage instigator</param>
    /// <param name="damageAmount">the damage amount</param>
    /// <returns>dealt or not</returns>
    public virtual bool TakeDamage(GameObject instigator, float damageAmount)
    {
        if(IsDead || damageAmount <= 0.0f)
            return false;

        CurrentHP = Mathf.Clamp(CurrentHP - damageAmount, 0.0f, MaxHP);
        OnHPChanged?.Invoke(CurrentHP, MaxHP);

        if(CurrentHP == 0.0f)
        {
           Death();
        }

        else
        {

        }

        return true;
    }

    /// <summary>
    /// the base method of healing
    /// </summary>
    /// <param name="healAmount">heal amount</param>
    /// <returns>Healed or not</returns>
    public virtual bool Heal(float healAmount)
    {
        if(IsDead || CurrentHP >= MaxHP || healAmount <= 0.0f)
            return false;

        CurrentHP = Mathf.Clamp(CurrentHP + healAmount, 0.0f, MaxHP);
        OnHPChanged?.Invoke(CurrentHP, MaxHP);
        OnHeal?.Invoke();

        return true;
    }

    /// <summary>
    /// the base method of death
    /// </summary>
    public virtual void Death()
    {
        IsDead = true;
        OnDeath?.Invoke();
    }
}
