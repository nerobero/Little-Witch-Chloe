using Types;
using UnityEngine;

public class UnlockScrollItem : ScrollItem
{
    [SerializeField] private EAbilityType _unlockType;
    public Sprite _keyIconSprite;

    protected override bool OnInteract(Collider2D other)
    {
        if(LayerMask.LayerToName(other.gameObject.layer).Contains("Player"))
        {
            if(LayerManager.IsSameSide(gameObject, other.gameObject))
            {
                bool unlocked = GameManager.Instance.OnScrollCollected(_unlockType);
                
                FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/Scroll");
                //return OnInteract_HelperImpl(other);
                if(unlocked)
                {
                    UIManager.Instance.Get<UIPlayerHUD>().UpdateSkillList(_unlockType, _keyIconSprite);
                }

                return unlocked;
            }
        }
        // var playerControllerComp = other.gameObject.GetComponent<PlayerController>();
        // if (playerControllerComp == null) 
        // {
        //    // Debug.Log("Player Null");
        //    return false; // cannot get the component, then return false
        // }

        // //int layer = (int)Mathf.Log(isBackground ? bgPlayerLayer : fgPlayeLayer, 2);

        // if(other.gameObject.layer != layer) 
        // {
        //     //Debug.Log("object null");
        //     return false;
        // }

        return false;
    }
}
