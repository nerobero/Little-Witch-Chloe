using UnityEngine;
using Types;
using Data;

public class ActiveStatusEffect
{
    public StatusEffect definition { get; private set; }

    public float remainingTime { get; private set; }

    public MonoBehaviour owner { get; private set; }
    public GameObject instigator { get; private set; }

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

    public void SetInstigator(GameObject instigator)
    {
        this.instigator = instigator;
    }

    public virtual void Apply(MonoBehaviour target)
    {
        Debug.Log("Applied");
        //definition.Apply(target, instigator);

        var receiver = target.GetComponent<IStatusEffect>();
        receiver?.ApplyStatusEffect(this);
    }

    public void Tick(float deltaTime)
    {
        remainingTime -= deltaTime;
    }

    public void ExtendDuration(float amount)
    {
        remainingTime += amount;
    }

    public virtual void Expire()
    {
        remainingTime = 0.0f; 
    }

    public void Remove(StatManager target)
    {
        Debug.Log("Removed");
        var receiver = target.GetComponent<IStatusEffect>();
        receiver?.RemoveStatusEffect(this);

        PoolObjectManager.Instance.ReturnStatusEffect(this);
    }

    public void Reset()
    {
        owner = null;
        remainingTime = 0.0f;
    }
}
