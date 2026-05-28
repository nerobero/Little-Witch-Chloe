using UnityEngine;

/// <summary>
/// Base class for any items that can be regenerated
/// few seconds after the interaction.
/// </summary>
public abstract class RegenItemBase : ItemBase
{
    // Cooldown Time before this item regenerates
    [SerializeField] private float _regenCDTime = 0f;

    protected override bool OnInteract(Collider2D other)
    {
        // if the interaction went successful and this item has been regenerated 
        // successfully, return true.
        if (OnInteractHelper(other))
        {
            FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/Healing Berry");
            Invoke("Regenerate", _regenCDTime);
            return true;
        }
        return false;
    }

    /// <summary>
    /// Helper function for processing the interaction.
    /// Created so that OnInteract(other) can be abstracted out a bit more.
    /// </summary>
    /// <param name="other">the collided gameObject</param>
    /// <returns>true if the helper function successfully processes interaction logic</returns>
    protected abstract bool OnInteractHelper(Collider2D other);

    /// <summary>
    /// Universal logic for item regeneration.
    /// Is to be invoked after _regenCDTime expires.
    /// </summary>
    private void Regenerate()
    {
        PoolObjectManager.Instance.Get(base.spawnType);
    }
}
