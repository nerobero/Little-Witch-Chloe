using UnityEngine;
using Types;

public class EnemyCharacterBase : StatManager
{
    protected override void Start()
    {
        base.Start();

        UIManager.Instance.Get<EnemyHPWidget>().SetTarget(this);
    }
}
