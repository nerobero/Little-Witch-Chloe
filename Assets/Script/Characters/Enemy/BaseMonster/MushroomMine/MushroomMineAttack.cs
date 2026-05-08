using UnityEngine;

public class MushroomMineAttack : EnemyAttack
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void Attack(GameObject target)
    {
        PlayerStatManager targetStat = target.GetComponent<PlayerStatManager>();
        if(targetStat != null)
        {
            targetStat.TakeDamage(gameObject, damageAmount);
        }
    }
}
