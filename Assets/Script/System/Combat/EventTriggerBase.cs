using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class EventTriggerBase : MonoBehaviour
{
    [SerializeField] protected LayerMask playerLayer;
    protected int playerLayerIndex => (int)Mathf.Log(playerLayer.value, 2);

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        
    }
}
