using System;
using UnityEngine;
using Types;
using System.Collections;

/// <summary>
/// Base class for managing the character's stats.
/// </summary>
public class StatManager : MonoBehaviour
{
    // HP stats
    [Header("HP Settings")]
    [SerializeField] protected float maxHP;
    [SerializeField] protected float currentHP;
    [SerializeField] protected EElementType mainCharacElement;
    public string OnTakenDamageEvent = "";
    protected const string OnDamageDeflected = "event:/SFX/Reflect";
    protected const string OnCritDamage = "event:/SFX/Crit";
    public EElementType CharacterElement => mainCharacElement;

    public float MaxHP => maxHP;
    public float CurrentHP => currentHP;

    // Event system: can be used for UI changes or animation control
    public event Action<float, float, GameObject> OnHPChanged;
    public event Action OnDeath;
    public event Action OnHeal;
    public event Action OnTakeDamage;

    protected Coroutine DOTRoutine;

    // Is dead
    public bool IsDead { get; protected set; }

    // When blinking, the character is Invincible
    public bool IsBlink { get; protected set; }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected virtual void Start()
    {
        currentHP = maxHP;
    }


    #region initialize
    public virtual void ApplyAllGameData(float savedMaxHP, float savedCurrentHP)
    {
        this.maxHP = savedMaxHP;
        this.currentHP = savedCurrentHP;
    }
    #endregion

    // Update is called once per frame
    // void Update()
    // {

    // }

    /// <summary>
    /// the base method to help managing take damage logic.
    /// this function has double ways to takedamage and dot damage.
    /// </summary>
    /// <param name="instigator"></param>
    /// <param name="damageAmount"></param>
    /// <param name="damageElement"></param>
    /// <param name="isDOT"></param>
    /// <param name="duration"></param>
    /// <param name="interval"></param>
    /// <returns></returns>
    public virtual bool TakeDamageHelper(GameObject instigator, float damageAmount, EElementType damageElement,
                                            bool isDOT = false, float duration = 0f, float interval = 0f)
    {
        if(isDOT)
        {
            return TakeDOTDamage(instigator, damageAmount, duration, interval, damageElement);
        }
        else
        {
            return TakeDamage(instigator, damageAmount, damageElement);
        }
    }

    /// <summary>
    /// the base method of taking damage
    /// </summary>
    /// <param name="instigator">damage instigator</param>
    /// <param name="damageAmount">the damage amount</param>
    /// <param name="element">damage element</param>
    /// <returns>dealt or not</returns>
    public virtual bool TakeDamage(GameObject instigator, float damageAmount, EElementType damageElement)
    {
        Debug.Log(IsDead);
        float actualDamage = CalculateActualDamage(damageAmount, damageElement, mainCharacElement);

        if (IsDead || actualDamage <= 0.0f)
        {
            FMODUnity.RuntimeManager.PlayOneShot(OnDamageDeflected);
            return false;
        }

        currentHP = Mathf.Clamp(currentHP - actualDamage, 0.0f, maxHP);
        FMODUnity.RuntimeManager.PlayOneShot(damageAmount < actualDamage ? OnCritDamage : OnTakenDamageEvent);
        this.OnHPChanged?.Invoke(currentHP, maxHP, instigator);
        OnTakeDamage?.Invoke();

        if (currentHP == 0.0f)
        {
            Death();
        }

        return true;
    }

    // Calculate actual damage amount
    protected virtual float CalculateActualDamage(float damageAmount, EElementType damageElement, EElementType CharacElement)
    {
        float actualDamage = damageAmount;

        switch (CharacElement)
        {
            // if the character's element is fire
            case EElementType.Fire:
                // if the damage element is water => more damage
                if (damageElement == EElementType.Water || damageElement == EElementType.Ice)
                {
                    actualDamage *= 1.5f;
                }
                // if the damage element is fire or electricity => no damage
                else if (damageElement == CharacElement)
                {
                    actualDamage = 0.0f;
                }
                break;
            case EElementType.Water:
                if (damageElement == EElementType.Electricity)
                    actualDamage *= 1.5f;
                else if (damageElement == CharacElement || damageElement == EElementType.Fire)
                    actualDamage = 0.0f;
                break;
            // if the character's element is plant
            case EElementType.Plant:
                // if the damage element is fire or poision => more damage
                if (damageElement == EElementType.Fire || damageElement == EElementType.Poison)
                {
                    actualDamage *= 1.5f;
                }
                // if the damage element is water or electricity => no damage
                else if (damageElement == EElementType.Water || damageElement == EElementType.Ice)
                {
                    actualDamage = 0.0f;
                }
                break;
            // if the character's element is poison
            case EElementType.Poison:
                // if the damage element is water => more damage
                if (damageElement == EElementType.Water || damageElement == EElementType.Ice)
                {
                    actualDamage *= 1.5f;
                }
                // if the damage element is poison or fire => no damage
                else if (damageElement == CharacElement || damageElement == EElementType.Fire)
                {
                    actualDamage = 0.0f;
                }
                break;
            // if the character's element is darkness
            case EElementType.Darkness:
                // if the damage element is light or electricity => more damage
                if (damageElement == EElementType.Light)
                {
                    actualDamage *= 1.5f;
                }
                // if the damage element is water or poison => no damage
                else if (damageElement == EElementType.Water || damageElement == EElementType.Poison
                || damageElement == EElementType.Ice)
                {
                    actualDamage = 0.0f;
                }
                break;
            // if the character's element is ice
            case EElementType.Ice:
                // if the damage element is fire => more damage
                if (damageElement == EElementType.Fire)
                {
                    actualDamage *= 1.5f;
                }
                // if the damage element is water or poison => no damage
                else if (damageElement == EElementType.Water || damageElement == EElementType.Poison
                || damageElement == EElementType.Ice)
                {
                    actualDamage = 0.0f;
                }
                break;
        }

        return actualDamage;
    }

    /// <summary>
    /// the base method of healing
    /// </summary>
    /// <param name="healAmount">heal amount</param>
    /// <returns>Healed or not</returns>
    public virtual bool Heal(float healAmount)
    {
        if (IsDead || currentHP >= maxHP || healAmount <= 0.0f)
            return false;

        currentHP = Mathf.Clamp(currentHP + healAmount, 0.0f, maxHP);
        this.OnHPChanged?.Invoke(currentHP, maxHP, null);
        this.OnHeal?.Invoke();

        return true;
    }

    public virtual bool IncreaseMaxHP(float amount)
    {
        if (IsDead || amount <= 0.0f)
            return false;

        // Increase the max hp and heal
        maxHP += amount;
        return Heal(amount); // this is same with increase current hp
    }

    /// <summary>
    /// Buff system but not used yet.
    /// </summary>
    /// <param name="buffAmount"></param>
    /// <returns></returns>
    public virtual bool Buff(float buffAmount)
    {
        return false;
    }

    /// <summary>
    /// the base method of death
    /// </summary>
    public virtual void Death()
    {
        IsDead = true;
        this.OnDeath?.Invoke();
    }

    protected void InvokeOnHPChanged(float current, float max, GameObject instigator)
        => OnHPChanged?.Invoke(current, max, instigator);


    public virtual bool TakeDOTDamage(GameObject instigator, float damageAmount, float duration, float interval, EElementType damageElement)
    {
        if(IsDead) return false;

        // Use coroutine for apply dot damage.
        StartCoroutine(ApplyDOT(instigator, damageAmount, duration, interval, damageElement));

        return true;
    }

    protected IEnumerator ApplyDOT(GameObject instigator, float totalDamage, float duration, float interval, EElementType damageElement)
    {
        float damagePerTick = totalDamage / (duration / interval);
        float elapsedTime = 0.0f;

        while(elapsedTime < duration)
        {
            TakeDamage(instigator, damagePerTick, damageElement);
            
            yield return new WaitForSeconds(interval);

            elapsedTime += interval;
        }
    }
}
