using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class EventTriggerBase : MonoBehaviour
{
    [SerializeField] protected LayerMask playerLayer;
    protected int playerLayerIndex => (int)Mathf.Log(playerLayer.value, 2);


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        
    }
}
