using UnityEngine;

/// <summary>
/// Component class that handles interaction attempts 
/// by the player via player controller.
/// </summary>
[RequireComponent(typeof(InteractiveDetector))]
public class InteractionSystem : MonoBehaviour
{
    private InteractiveDetector _detector;

    private void Awake()
    {
        _detector = GetComponent<InteractiveDetector>();
    }

    /// <summary>
    /// Attempts to interact with the current interactable.
    /// </summary>
    public void Interact()
    {
        if (CanInteractWith())
        {
            _detector.CurrentInteractable.Interact();
        }
    }

    /// <summary>
    /// Checks if there is a valid interactable near the owner's vicinity.
    /// </summary>
    /// <returns>true if the current interactable is not null and interactable</returns>
    private bool CanInteractWith()
        => _detector.CurrentInteractable != null &&
        _detector.CurrentInteractable.CanInteract();
}
