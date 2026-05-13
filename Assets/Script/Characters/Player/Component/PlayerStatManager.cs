using System;
using UnityEngine;

public class PlayerStatManager : StatManager
{
    // Stamina
    [Header("Stamina Settings")]
    [SerializeField] protected float maxStamina;
    [SerializeField] protected float currentStamina;
    public float CurrStamina => currentStamina;
    public float MaxStamina => maxStamina;

    // Event system
    public event Action<float, float> OnStaminaChanged;
    public event Action OnStaminaOver;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        base.Start();
        currentStamina = maxStamina;
        
    }

    // Update is called once per frame
    // void Update()
    // {
        
    // }

    /// <summary>
    /// the base method of use stamina
    /// </summary>
    /// <param name="staminaAmount">the stamina amount to use</param>
    /// <returns></returns>
    public virtual bool UseStamina(float staminaAmount)
    {
        if(currentStamina <= 0.0f || staminaAmount <= 0.0f)
            return false;

        currentStamina = Mathf.Clamp(currentStamina - staminaAmount, 0.0f, maxStamina);
        OnStaminaChanged?.Invoke(currentStamina, maxStamina);

        if(currentStamina <= 0.0f)
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
        if(currentStamina >= maxStamina || staminaAmount <= 0.0f)
            return false;

        currentStamina = Mathf.Clamp(currentStamina + staminaAmount, 0.0f, maxStamina);
        OnStaminaChanged?.Invoke(currentStamina, maxStamina);

        return true;
    }
}
