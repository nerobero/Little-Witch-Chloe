using UnityEngine;
using Types;

public class MushroomMineAttack : EnemyAttack, ISummonable
{
    private GameObject _owner;

    public override void Attack(GameObject target)
    {
        PlayerStatManager targetStat = target.GetComponent<PlayerStatManager>();
        if(targetStat != null)
        {
            targetStat.TakeDamageHelper(gameObject, damageAmount, _elementType);
        }
    }

    public float GetDamageNumber() => damageAmount;

    public void OnSummoned() { }

    public void OnReturnedToPool() { }

    public void SetInstigator(GameObject instigator)
    {
        _owner = instigator;
    }
}
