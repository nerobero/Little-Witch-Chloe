using UnityEngine;

public class FrogCollection : ItemBase
{
    [Header("Frog Setting")]
    [SerializeField] private float healAmount;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected override bool OnInteract(Collider2D other)
    {
        var stat = other.GetComponent<StatManager>();
        
        return stat.IncreaseMaxHP(healAmount);
    }
}
