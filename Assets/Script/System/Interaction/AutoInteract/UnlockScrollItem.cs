using Types;
using UnityEngine;

public class UnlockScrollItem : ScrollItem
{
    [SerializeField] private EAbilityType _unlockType;

    protected override bool OnInteract(Collider2D other)
    {
        var playerControllerComp = other.gameObject.GetComponent<PlayerController>();
        if (playerControllerComp == null) 
        {
            Debug.Log("Player Null");
           return false; // cannot get the component, then return false
        }

        int layer = (int)Mathf.Log(isBackground ? bgPlayerLayer : fgPlayeLayer, 2);

        if(other.gameObject.layer != layer) 
        {
            Debug.Log("object null");
            return false;
        }


        return GameManager.Instance.OnScrollCollected(_unlockType);
    }
}
