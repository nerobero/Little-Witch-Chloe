using System;
using UnityEngine;
using UnityEngine.Events;

public class PlayerStatManager : StatManager
{
    // Stamina
    [Header("Stamina Settings")]
    [SerializeField] protected float MaxStamina;
    [SerializeField] protected float CurrentStamina;
    public float CurrStamina => CurrentStamina;

    // Event system
    public event Action<float, float> OnStaminaChanged;
    public event Action OnStaminaOver;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CurrentStamina = MaxStamina;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// the base method of use stamina
    /// </summary>
    /// <param name="staminaAmount">the stamina amount to use</param>
    /// <returns></returns>
    public virtual bool UseStamina(float staminaAmount)
    {
        if(CurrentStamina <= 0.0f || staminaAmount <= 0.0f)
            return false;

        CurrentStamina = Mathf.Clamp(CurrentStamina - staminaAmount, 0.0f, MaxStamina);
        OnStaminaChanged?.Invoke(CurrentStamina, MaxStamina);

        if(CurrentStamina <= 0.0f)
        {
            OnStaminaOver?.Invoke();
        }

        else
        {

        }

        return true;
    }

    /// <summary>
    /// the base method of replenishing stamina
    /// </summary>
    /// <param name="staminaAmount">stamina amount to replenish</param>
    /// <returns></returns>
    public virtual bool ReplenishStamina(float staminaAmount)
    {
        if(CurrentStamina >= MaxStamina || staminaAmount <= 0.0f)
            return false;

        CurrentStamina = Mathf.Clamp(CurrentStamina + staminaAmount, 0.0f, MaxStamina);
        OnStaminaChanged?.Invoke(CurrentStamina, MaxStamina);

        return true;
    }
}
