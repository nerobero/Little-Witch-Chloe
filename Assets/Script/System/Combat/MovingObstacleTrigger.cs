using UnityEngine;

public class MovingObstacleTrigger : EventTriggerBase
{
    [SerializeField] protected GameObject obstacle;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        // it only reacts if the collider is player
        if(other.gameObject.layer == playerLayerIndex)
        {
            obstacle.SetActive(true);
        }
    }
    
}
