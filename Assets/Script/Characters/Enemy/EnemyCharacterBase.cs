using UnityEngine;
using Types;


public class EnemyCharacterBase : StatManager
{
    [SerializeField] private EMonsterType monsterType;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void Death()
    {
        base.Death();

        // Return to pool
        gameObject.SetActive(false);
    }
}
