using UnityEngine;

public class MovingObstacleTrigger : EventTriggerBase
{
    [SerializeField] protected GameObject obstacle;
    [SerializeField] protected bool isBackground;

    [SerializeField] protected LayerMask bgPlayerLayer;
    [SerializeField] protected LayerMask fgPlayeLayer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    MovingObstacleBase obsScript;

    void Start()
    {
        obsScript = obstacle.GetComponent<MovingObstacleBase>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected override void OnTriggerEnter2D(Collider2D other)
    {
         int layer = (int)Mathf.Log(isBackground ? bgPlayerLayer : fgPlayeLayer, 2);

        // it only reacts if the collider is player
        if(other.gameObject.layer != layer)
        {
            obsScript.SetIsBackground(isBackground);
            obstacle.SetActive(true);
        }
    }
    
}
