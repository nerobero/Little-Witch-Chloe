using UnityEngine;

/// <summary>
/// Interface for the pure contract of 
/// interaction logic for all interactable objects
/// </summary>
public interface IInteractable
{
    bool CanInteract();
    void Interact();
}

/// <summary>
/// Abstract class that has 
/// </summary>
public abstract class InteractableBase : MonoBehaviour, IInteractable
{
    // the child classes can override this function based on
    // its functionality:
    public virtual bool CanInteract() => true;

    protected abstract void Interact_Impl();

    /// <summary>
    /// Main function for processing interaction.
    /// The actual logic for interaction sits in Interact_Impl().
    /// </summary>
    public void Interact()
    {
        if (CanInteract())
            Interact_Impl();
    }

}
