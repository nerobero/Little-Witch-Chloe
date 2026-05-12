using Types;
using UnityEngine;

public class UnlockScrollItem : ScrollItem
{
    [SerializeField] private EAbilityType _unlockType;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected override bool OnInteract(Collider2D other)
    {
        var playerControllerComp = other.gameObject.GetComponent<PlayerController>();
        if (playerControllerComp == null) return false; // cannot get the component, then return false

        return GameManager.Instance.OnScrollCollected(_unlockType);
    }
}
