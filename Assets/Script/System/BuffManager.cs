using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class BuffManager : MonoSingletonBase<BuffManager>
{
    private List<ActiveStatusEffect> m_AllEffects = new();

    public void Register(ActiveStatusEffect effect)
    {
        //effect.SetOwner(owner);
        //effect.Apply(owner.owner);

        m_AllEffects.Add(effect);
    }

    public void Unregister(ActiveStatusEffect effect)
    {
        // effect.SetOwner(owner);
        var effectOwner = (Buff)effect.owner;
        effectOwner.Remove(effect);

        m_AllEffects.Remove(effect);
    }

    // Update is called once per frame
    private void Update()
    {
        for(int i = m_AllEffects.Count - 1; i >= 0; --i)
        {
            m_AllEffects[i].Tick(Time.deltaTime);

            if(m_AllEffects[i].remainingTime <= 0)
            {
                Unregister(m_AllEffects[i]);
            }
        }
    }

    public void RemoveAll(Buff owner)
    {
        for (int i = m_AllEffects.Count - 1; i >= 0; i--)
        {
            if (m_AllEffects[i].owner == owner)
            {
                Unregister(m_AllEffects[i]);   
            }
        }
    }
}
