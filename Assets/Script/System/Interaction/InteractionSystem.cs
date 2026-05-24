using UnityEngine;

/// <summary>
/// Component class that handles 
/// </summary>
public class InteractionSystem : MonoBehaviour
{

    private void Awake()
    {
        
    }

    /// <summary>
    /// Wrapper function that can be publically called. 
    /// </summary>
    public void Interact()
    {
        if (CanInteractWith())
        {
            
        }
    }

    private bool CanInteractWith()
    {
        return false;
    }
}
