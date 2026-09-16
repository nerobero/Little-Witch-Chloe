using UnityEngine;

public class LovePotionItem : ItemBase
{
    protected override bool OnInteract(Collider2D other)
    {
        if(!_canInteract) return false;
        
        return OnInteract_Helper(other);
    }

    protected bool OnInteract_Helper(Collider2D other)
    {
        if(LayerMask.LayerToName(other.gameObject.layer).Contains("Player"))
        {
            GameManager.Instance.OnLovePotionMade();      
            return true;      
        }

        return false;
    }
}
