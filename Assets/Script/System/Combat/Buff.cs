using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Types;
using Data;

public class Buff : MonoBehaviour
{
    public StatManager owner { get; private set; }
    //private Dictionary<EStatusEffectType, List<StatusEffect>> m_Buff = new Dictionary<EStatusEffectType, List<StatusEffect>>();
    private Dictionary<ECrowdControlType, ActiveStatusEffect> m_ActiveCCEffects = new();
    //private List<ActiveStatusEffect> m_ActiveEffects = new();

    // This is for VFX(but we do not use maybe)
    //private Dictionary<EStatusEffectType, GameObject> m_Effects = new Dictionary<EStatusEffectType, GameObject>();

    private void Awake()
    {
        owner = GetComponent<StatManager>();
    }

    // public void Update()
    // {
    //     if(m_ActiveEffects.Count == 0) return;

    //     foreach(var effect in m_ActiveEffects)
    //     {
    //         effect.Tick(Time.deltaTime);

    //         if(effect.remainingTime <= 0)
    //         {
    //             Remove(effect);
    //         }
    //     }

    //     foreach(var pair in m_ActiveCCEffects)
    //     {
    //         pair.Value.Tick(Time.deltaTime);

    //         if(pair.Value.remainingTime <= 0)
    //         {
    //             Remove(pair.Value);
    //         }
    //     }

    //     // foreach(var key in m_Buff.Keys.ToList())
    //     // {
    //     //     var data = m_Buff[key];

    //     //     if(owner.IsDead)
    //     //     {
    //     //        data.Expire(); 
    //     //     }

    //     //     data.Tick(owner, Time.deltaTime);

    //     //     // it can't stacked.
    //     //     if(data.remainingTime <= 0)
    //     //     {
    //     //         Remove(key);
    //     //     }
    //     //     else
    //     //     {
    //     //         m_Buff[key] = data;
    //     //     }
    //     // }
    // }

    public void Add(ActiveStatusEffect effect)
    {
        Debug.Log("buff/debuff added");

        switch(effect.definition.Category)
        {
            // CrowdControl
            case EStatusEffectCategory.CrowdControl:
                if(m_ActiveCCEffects.ContainsKey(effect.definition.CCType))
                {
                    float remainTime = m_ActiveCCEffects[effect.definition.CCType].remainingTime;

                    // Apply only if the duration of new one is greater than remaining time of current one.
                    if(remainTime >= effect.definition.Duration)
                    {
                        return;
                    }

                    BuffManager.Instance.Unregister(m_ActiveCCEffects[effect.definition.CCType]);
                    //effect.SetOwner(this);
                    m_ActiveCCEffects[effect.definition.CCType] = effect;
                }
                // there is no
                else
                {
                    effect.SetOwner(this);
                    m_ActiveCCEffects.Add(effect.definition.CCType, effect);

                }
                effect.Apply(owner);
            break;
            // Buff & Debuff
            default:
                effect.SetOwner(this);
                //m_ActiveEffects.Add(effect);
                effect.Apply(owner);
            break;
        }

        BuffManager.Instance.Register(effect);

        // // if the same effect type is already applied
        // if(m_Buff.ContainsKey(effect.definition.Type))
        // {
        //     effect.SetOwner(this.owner);
        //     m_Buff[effect.definition.Type] = effect;
        // }
        // else
        // {
        //     Debug.Log("no effects so add to buff dictionary");
        //     effect.owner = this.owner;
        //     //AttachEffect(type);
        //     m_Buff.Add(effect.Type, effect);
        //     effect.Apply(owner);
        // }
    }

    public void Remove(ActiveStatusEffect effect)
    {
        effect.Remove(owner);

        if(effect.definition.Category == EStatusEffectCategory.CrowdControl)
        {
            ECrowdControlType type = effect.definition.CCType;
            m_ActiveCCEffects.Remove(type);
        }
        // switch(effect.definition.Category)
        // {
        //     case EStatusEffectCategory.CrowdControl:
        //         ECrowdControlType type = effect.definition.CCType;
        //         m_ActiveCCEffects.Remove(type);
        //     break;
        //     default:
        //         m_ActiveEffects.Remove(effect);
        //     break;
        // }
    }

    // public void Remove(EStatusEffectType type)
    // {
    //     if(!m_Buff.ContainsKey(type))
    //     {
    //         return;
    //     }

    //     // This is the VFX effects
    //     // if(m_Effects.ContainsKey(type))
    //     // {
    //     //     if(m_Effects[type] != null)
    //     //     {
    //     //         Destroy(m_Effects[type]);
    //     //     }
    //     //     m_Effects.Remove(type);
    //     // }

    //     m_Buff[type].Remove(owner);
    //     m_Buff.Remove(type);
        
    // }

    public void RemoveAll()
    {
        // foreach(var effect in m_ActiveEffects)
        // {
        //     effect.Remove(owner);
        // }

        // m_ActiveEffects.Clear();

        BuffManager.Instance.RemoveAll(this);

        // foreach(var pair in m_ActiveCCEffects)
        // {
        //     pair.Value.Remove(owner);
        // }

        m_ActiveCCEffects.Clear();
        // foreach(var key in m_Buff.Keys.ToList())
        // {
        //     Remove(key);
        // }
    }

    public List<ECrowdControlType> GetAppliedCCAll()
    {
        //return m_Buff.Keys.ToList();
        return m_ActiveCCEffects.Keys.ToList();
    }

    // public int GetEffectType(EStatusEffectType type)
    // {
    //     return m_ActiveEffectCounts[type];
    // }

    // private void AttachEffect(EStatusEffectType type)
    // {
    //     GameObject buffPrefabs = BuffManager.Instance.GetBuffData(type);
        
    //     if(!m_Effects.ContainsKey(type))
    //     {
    //         m_Effects.Add(type, Instantiate(buffPrefabs));
    //     }
        
    // }

    public void ResetState()
    {
        RemoveAll();
    }
}