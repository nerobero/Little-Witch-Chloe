// using UnityEngine;
// using Types;
// using System.Collections;

public interface IStatusEffect
{
    public void ApplyStatusEffect(ActiveStatusEffect effect);
    public void RemoveStatusEffect(ActiveStatusEffect effect);
    // // duration
    // float duration { get; set; }

    // public void Init();

    // public void Execute();

    // public IEnumerator Activation();

    // public void Deactivation();
}