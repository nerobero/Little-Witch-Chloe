using UnityEngine;
using Types;
using Data;

public class ActiveStatusEffect
{
    public StatusEffect definition { get; private set; }

    public float remainingTime { get; private set; }

    public MonoBehaviour owner { get; private set; }

    public ActiveStatusEffect()
    {
        
    }

    public ActiveStatusEffect(MonoBehaviour owner, StatusEffect effect)
    {
        this.owner = owner;
        this.definition = effect;
        this.remainingTime = definition.Duration;
    }

    public void SetEffect(StatusEffect effect)
    {
        this.definition= effect;
        this.remainingTime = effect.Duration;
    }

    public void SetOwner(MonoBehaviour owner)
    {
        this.owner = owner;
    }

    public virtual void Apply(StatManager target)
    {
        Debug.Log("Applied");
        definition.Apply(target);
    }

    public void Tick(float deltaTime)
    {
        remainingTime -= deltaTime;
    }

    public virtual void Expire()
    {
        remainingTime = 0.0f; 
    }

    public void Remove(StatManager target)
    {
        definition.Remove(target);

        PoolObjectManager.Instance.ReturnStatusEffect(this);
    }

    public void Reset()
    {
        owner = null;
        remainingTime = 0.0f;
    }
}
