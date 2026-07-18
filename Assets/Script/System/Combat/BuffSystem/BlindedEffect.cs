using UnityEngine;
using Types;
using Data;

[System.Serializable]
public class BlindedEffect : StatusEffect
{
    public BlindedEffect(MonoBehaviour owner, float magnitude, float duration)
        : base(owner, EStatusEffectType.Blind, EStatusEffectCategory.CrowdControl, null, magnitude, duration)
    {}


    public override void Apply(StatManager target)
    {
        Debug.Log("Blind Applied");
        target.Buff(category, type, ECrowdControlType.Blinded);
    }

    public override void Remove(StatManager target)
    {
        target.RemoveBuff(category, type, ECrowdControlType.Blinded);
    }
}
