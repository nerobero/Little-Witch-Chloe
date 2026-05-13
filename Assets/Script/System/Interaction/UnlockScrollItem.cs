using Types;
using UnityEngine;

public class UnlockScrollItem : ScrollItem
{
    [SerializeField] private EAbilityType _unlockType;

    protected override bool OnInteract(Collider2D other)
    {
        var playerControllerComp = other.gameObject.GetComponent<PlayerController>();
        if (playerControllerComp == null) return false; // cannot get the component, then return false

        return GameManager.Instance.OnScrollCollected(_unlockType);
    }
}
