using UnityEngine;
using Types;

public class EnemyCharacterBase : StatManager
{
    protected override void Start()
    {
        base.Start();

        var enemyHP = GetComponent<EnemyHPWidget>();

        if(enemyHP == null)
        {
            return;
        }
        
        enemyHP.SetTarget(this);
    }
}
