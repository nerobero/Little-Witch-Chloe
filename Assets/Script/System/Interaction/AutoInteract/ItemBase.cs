using UnityEngine;
using Types;
/// <summary>
/// Base class for any interactable items.
/// Utilizes the pool object manager to handle potential respawning
/// of the items after interaction.
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Collider2D))]
public abstract class ItemBase : MonoBehaviour
{
    protected ESpawnType spawnType;
    
    [SerializeField] protected LayerMask bgPlayerLayer;
    [SerializeField] protected LayerMask fgPlayeLayer;
    [SerializeField] protected bool isBackground;

    /// <summary>
    /// Handles interaction logic. Can be overriden by the child classes.
    /// </summary>
    /// <param name="other">the object this item has collided with</param>
    /// <returns>true if the interaction has been processed successfully</returns>
    protected abstract bool OnInteract(Collider2D other);


    protected void OnTriggerEnter2D(Collider2D collision)
    {
        if (OnInteract(collision)) 
            PoolObjectManager.Instance.Return(spawnType, this.gameObject);
    }
}
