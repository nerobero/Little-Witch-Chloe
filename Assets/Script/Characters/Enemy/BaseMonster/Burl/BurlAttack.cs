using UnityEngine;
using Types;

public class BurlAttack : EnemyAttack, ISummonable
{
    private GameObject _owner;
    private BurlController _controller;

    protected override void Awake()
    {
        base.Awake();
        _stat = GetComponent<EnemyCharacterBase>();
        _controller = GetComponent<BurlController>();
    }

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

    public void OnReturnedToPool()
    {
        _stat?.ResetState();
        _controller?.ResetTrap();
    }

    public void SetInstigator(GameObject instigator)
    {
        _owner = instigator;
    }
}
