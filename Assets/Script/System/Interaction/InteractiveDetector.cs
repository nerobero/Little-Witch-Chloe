using UnityEngine;

/// <summary>
/// Component dedicated to detecting any interactable objects near
/// the owner of this component.
/// Requires a Circle Collider 2D.
/// </summary>
[RequireComponent(typeof(CircleCollider2D))]
public class InteractiveDetector : MonoBehaviour
{
    public IInteractable CurrentInteractable { get; private set; }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<IInteractable>(out var interactable))
        {
            CurrentInteractable = interactable;
            CurrentInteractable.ShowInteractUI(true);
        }

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent<IInteractable>(out var interactable)
           && CurrentInteractable == interactable)
        {
            CurrentInteractable = null;
            CurrentInteractable.ShowInteractUI(false);
        }
    }
}
