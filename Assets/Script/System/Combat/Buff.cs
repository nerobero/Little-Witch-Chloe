using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Types;
using Data;

public class Buff : MonoBehaviour
{
    private StatManager owner;
    private Dictionary<EStatusEffectType, StatusEffect> m_Buff = new Dictionary<EStatusEffectType, StatusEffect>();

    // This is for VFX(but we do not use maybe)
    //private Dictionary<EStatusEffectType, GameObject> m_Effects = new Dictionary<EStatusEffectType, GameObject>();

    private void Awake()
    {
        owner = GetComponent<StatManager>();
    }

    public void Update()
    {
        if(m_Buff.Count == 0) return;

        foreach(var key in m_Buff.Keys.ToList())
        {
            var data = m_Buff[key];

            if(owner.IsDead)
            {
               data.Expire(); 
            }

            data.Tick(owner, Time.deltaTime);
            if(data.remainingTime <= 0)
            {
                Remove(key);
            }
            else
            {
                m_Buff[key] = data;
            }
        }
    }

    public void Add(StatusEffect effect)
    {
        Debug.Log("buff/debuff added");
        if(m_Buff.ContainsKey(effect.Type))
        {
            effect.owner = this.owner;
            m_Buff[effect.Type] = effect;
        }
        else
        {
            Debug.Log("no effects so add to buff dictionary");
            effect.owner = this.owner;
            //AttachEffect(type);
            m_Buff.Add(effect.Type, effect);
            effect.Apply(owner);
        }
    }

    public void Remove(EStatusEffectType type)
    {
        if(!m_Buff.ContainsKey(type))
        {
            return;
        }

        // This is the VFX effects
        // if(m_Effects.ContainsKey(type))
        // {
        //     if(m_Effects[type] != null)
        //     {
        //         Destroy(m_Effects[type]);
        //     }
        //     m_Effects.Remove(type);
        // }

        m_Buff[type].Remove(owner);
        m_Buff.Remove(type);
        
    }

    public void RemoveAll()
    {
        foreach(var key in m_Buff.Keys.ToList())
        {
            Remove(key);
        }
    }

    public List<EStatusEffectType> GetBuffAll()
    {
        return m_Buff.Keys.ToList();
    }

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