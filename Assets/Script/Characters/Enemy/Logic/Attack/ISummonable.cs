using UnityEngine;


/// <summary>
/// Contract for objects that can be spawned/pooled by a summon-style attack
/// (e.g. SummonAttackStrategy), so the summoner can query their damage number
/// and notify them of pool lifecycle transitions.
/// </summary>
public interface ISummonable
{
    void SetInstigator(GameObject instigator);
    float GetDamageNumber();
    void OnSummoned();
    void OnReturnedToPool();
}
