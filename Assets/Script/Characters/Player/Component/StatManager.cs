using System;
using UnityEngine;
using Types;
using System.Collections;
using Data;

/// <summary>
/// Base class for managing the character's stats.
/// </summary>
//[RequireComponent(typeof(StatusEffectController))]
public class StatManager : MonoBehaviour, IDamageable
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
    [SerializeField] protected ECrowdControlType CrowdControl;

    public float MaxHP => maxHP;
    public float CurrentHP => currentHP;
    //public StatusEffectController StatusEffects { get; private set; }

    // Event system: can be used for UI changes or animation control
    public event Action<float, float, GameObject> OnHPChanged;
    public event Action OnDeath;
    public event Action OnHeal;
    public event Action OnTakeDamage;

    protected Coroutine DOTRoutine;

    public Buff BuffComp { get; protected set; }

    // Is dead
    public bool IsDead { get; protected set; }

    // When blinking, the character is Invincible
    public bool IsBlink { get; protected set; }

    // Immune to poison damage while the anti-poison fog buff is active
    public bool IsPoisonImmune { get; protected set; }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected virtual void Awake()
    {
        BuffComp = GetComponent<Buff>();
    }

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
        if (IsPoisonImmune && damageElement == EElementType.Poison)
        {
            FMODUnity.RuntimeManager.PlayOneShot(OnDamageDeflected);
            return false;
        }

        float actualDamage = CalculateActualDamage(damageAmount, damageElement, mainCharacElement);

        //Debug.Log($"TakeDamage(): {actualDamage}");
        if(IsDead)
        {
            return true;
        }

        if (IsBlink || actualDamage <= 0.0f)
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
    public virtual void Buff(StatusEffect effect, GameObject instigator)
    {
        switch(effect.Category)
        {
            case EStatusEffectCategory.Buff:
            AddBuff(effect.Type, effect.Magnitude, instigator);
            break;
            case EStatusEffectCategory.Debuff:
            AddDebuff(effect.Type, effect.Magnitude, instigator);
            break;
            case EStatusEffectCategory.CrowdControl:
            AddCrowdControl(effect.CCType);
            AddDebuff(effect.Type, effect.Magnitude, instigator);
            break;
        }
    }

    public virtual void RemoveBuff(StatusEffect effect)
    {
        switch(effect.Category)
        {
            case EStatusEffectCategory.Buff:
            DispelBuff(effect.Type, effect.Magnitude);
            break;
            case EStatusEffectCategory.Debuff:
            RemoveDebuff(effect.Type, effect.Magnitude);
            break;
            case EStatusEffectCategory.CrowdControl:
            RemoveCrowdControl(effect.CCType);
            RemoveDebuff(effect.Type, effect.Magnitude);
            break;
        }
    }

    public virtual void AddBuff(EStatusEffectType type, float magnitude, GameObject instigator)
    {
        switch(type)
        {
            case EStatusEffectType.AttackUp:
            // Adjust the magnitude to attack component

                break;
            case EStatusEffectType.DefenseUp:
                break;
            case EStatusEffectType.MoveSpeedUp:
                
                break;
            case EStatusEffectType.AttackSpeedUp:
                break;
            case EStatusEffectType.Shield:
                break;
        }
    }

    public virtual void DispelBuff(EStatusEffectType type, float magnitude)
    {
        switch(type)
        {
            case EStatusEffectType.AttackUp:

            break;
            case EStatusEffectType.DefenseUp:
            break;
            case EStatusEffectType.MoveSpeedUp:
            break;
            case EStatusEffectType.AttackSpeedUp:
            break;
            case EStatusEffectType.Shield:
            break;
        }
    }

    public void SetPoisonImmune(bool isImmune)
    {
        IsPoisonImmune = isImmune;
    }

    public virtual void AddDebuff(EStatusEffectType type, float magnitude, GameObject instigator)
    {
        switch(type)
        {
            case EStatusEffectType.AttackDown:
            
            break;
            case EStatusEffectType.DefenseDown:
            break;
            case EStatusEffectType.Slow:
            break;
            case EStatusEffectType.AttackSpeedDown:
            break;
        }
    }

    public virtual void RemoveDebuff(EStatusEffectType type, float magnitude)
    {
        switch(type)
        {
            case EStatusEffectType.AttackDown:
            
            break;
            case EStatusEffectType.DefenseDown:
            break;
            case EStatusEffectType.Slow:
            break;
            case EStatusEffectType.AttackSpeedDown:
            break;
        }
    }

    public virtual void AddCrowdControl(ECrowdControlType cc)
    {
        CrowdControl |= cc;
        Debug.Log(CrowdControl + " and is applied the current " + cc + "? " + (HasCrowdControl(cc)? "yes" : "no"));
    }

    public virtual void RemoveCrowdControl(ECrowdControlType cc)
    {
        CrowdControl &= ~cc;
    }

    public virtual bool HasCrowdControl(ECrowdControlType cc)
    {
        return (CrowdControl & cc) != 0;
    }

    /// <summary>Applies a data-driven buff, debuff or crowd-control effect to this character.</summary>
    //public virtual bool ApplyStatusEffect(StatusEffectData effect, GameObject source)
    //    => StatusEffects != null && StatusEffects.Apply(effect, source);

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

        // if(damageElement == EElementType.Poison)
        // {
        //     if(IsPoisonImmune) return true;
        // }

        // Use coroutine for apply dot damage.
        StartCoroutine(ApplyDOT(instigator, damageAmount, duration, interval, damageElement));

        return true;
    }

    protected IEnumerator ApplyDOT(GameObject instigator, float totalDamage, float duration, float interval, EElementType damageElement)
    {
        WaitForSecondsTracked waitSeconds = new WaitForSecondsTracked(interval);
        float damagePerTick = totalDamage / (duration / interval);
        float elapsedTime = 0.0f;

        while(elapsedTime < duration)
        {
            TakeDamage(instigator, damagePerTick, damageElement);
            
            yield return waitSeconds;

            elapsedTime += interval;
        }
    }

    public virtual void ResetState()
    {
        IsDead = false;
        this.currentHP = this.maxHP;
        BuffComp?.ResetState();
        OnHPChanged?.Invoke(currentHP, maxHP, gameObject);
    }
}
