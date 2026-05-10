using System;
using UnityEngine;
using Types;

/// <summary>
/// Base class for managing the character's stats.
/// </summary>
public class StatManager : MonoBehaviour
{
    // HP stats
    [Header("HP Settings")]
    [SerializeField] protected float maxHP;
    [SerializeField] protected float currentHP;
    [SerializeField] protected EElementType characterElement;
    public EElementType CharacterElement => characterElement;

    public float MaxHP => maxHP;
    public float CurrentHP => currentHP;

    // Event system: can be used for UI changes or animation control
    public event Action<float, float, GameObject> OnHPChanged;
    public event Action OnDeath;
    public event Action OnHeal;
    
    // Is dead
    public bool IsDead {get; protected set;}

    // When blinking, the character is Invincible
    public bool IsBlink{get; protected set;}

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentHP = maxHP;
    }

    // Update is called once per frame
    // void Update()
    // {
        
    // }

    /// <summary>
    /// the base method of taking damage
    /// </summary>
    /// <param name="instigator">damage instigator</param>
    /// <param name="damageAmount">the damage amount</param>
    /// <param name="element">damage element</param>
    /// <returns>dealt or not</returns>
    public virtual bool TakeDamage(GameObject instigator, float damageAmount, ESpawnType damageElement)
    {
        Debug.Log(IsDead);

        if(IsDead || damageAmount <= 0.0f)
            return false;

        float actualDamage = damageAmount;

        switch(characterElement)
        {
            // if the character's element is fire
            case EElementType.Fire:
                // if the damage element is water => more damage
                if(damageElement == ESpawnType.WaterBall || damageElement == ESpawnType.IceShard
                    || damageElement == ESpawnType.IceBall || damageElement == ESpawnType.IceGroundShards)
                {
                    actualDamage *= 1.5f;
                }
                // if the damage element is fire or electricity => no damage
                else if(damageElement == ESpawnType.FireBall || damageElement == ESpawnType.FirePillar
                    || damageElement == ESpawnType.FirePillar)
                {
                    actualDamage = 0.0f;
                }
            break;
            // if the character's element is plant
            case EElementType.Plant:
                // if the damage element is fire or poision => more damage
                if(damageElement == ESpawnType.FireBall || damageElement == ESpawnType.FirePillar
                    || damageElement == ESpawnType.FirePillar || damageElement == ESpawnType.PoisonBall
                    || damageElement == ESpawnType.PoisonPool || damageElement == ESpawnType.PoisonSplash)
                {
                    actualDamage *= 1.5f;
                }
                // if the damage element is water or electricity => no damage
                else if(damageElement == ESpawnType.WaterBall || damageElement == ESpawnType.IceShard
                    || damageElement == ESpawnType.IceBall || damageElement == ESpawnType.IceGroundShards)
                {
                    actualDamage = 0.0f;
                }
            break;
            // if the character's element is poison
            case EElementType.Posion:
                // if the damage element is water => more damage
                if(damageElement == ESpawnType.WaterBall || damageElement == ESpawnType.IceShard
                    || damageElement == ESpawnType.IceBall || damageElement == ESpawnType.IceGroundShards)
                {
                    actualDamage *= 1.5f;
                }
                // if the damage element is poison or fire => no damage
                else if(damageElement == ESpawnType.FireBall || damageElement == ESpawnType.FirePillar
                    || damageElement == ESpawnType.FirePillar || damageElement == ESpawnType.PoisonBall
                    || damageElement == ESpawnType.PoisonPool || damageElement == ESpawnType.PoisonSplash)
                {
                    actualDamage = 0.0f;
                }
            break;
            // if the character's element is darkness
            case EElementType.Darkness:
                // if the damage element is light or electricity => more damage
                if(damageElement == ESpawnType.LightBall)
                {
                    actualDamage *= 1.5f;
                }
                // if the damage element is water or poison => no damage
                else if(damageElement == ESpawnType.WaterBall || damageElement == ESpawnType.IceShard
                    || damageElement == ESpawnType.IceBall || damageElement == ESpawnType.IceGroundShards
                    || damageElement == ESpawnType.PoisonBall || damageElement == ESpawnType.PoisonPool
                    || damageElement == ESpawnType.PoisonSplash)
                {
                    actualDamage = 0.0f;
                }
            break;
            // if the character's element is ice
            case EElementType.Ice:
                // if the damage element is fire => more damage
                if(damageElement == ESpawnType.FireBall || damageElement == ESpawnType.FirePillar
                    || damageElement == ESpawnType.FirePillar)
                {
                    actualDamage *= 1.5f;
                }
                // if the damage element is water or poison => no damage
                else if(damageElement == ESpawnType.WaterBall || damageElement == ESpawnType.IceShard
                    || damageElement == ESpawnType.IceBall || damageElement == ESpawnType.IceGroundShards
                    || damageElement == ESpawnType.PoisonBall || damageElement == ESpawnType.PoisonPool
                    || damageElement == ESpawnType.PoisonSplash)
                {
                    actualDamage = 0.0f;
                }
            break;
        }

        currentHP = Mathf.Clamp(currentHP - actualDamage, 0.0f, maxHP);
        OnHPChanged?.Invoke(currentHP, maxHP, instigator);

        if(currentHP == 0.0f)
        {
           Death();
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
        if(IsDead || currentHP >= maxHP || healAmount <= 0.0f)
            return false;

        currentHP = Mathf.Clamp(currentHP + healAmount, 0.0f, maxHP);
        OnHPChanged?.Invoke(currentHP, maxHP, null);
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
