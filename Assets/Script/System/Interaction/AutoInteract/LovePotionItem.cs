using UnityEngine;
using UnityEngine.Playables;

public class LovePotionItem : ItemBase
{
    public PlayableDirector _director;

    protected override bool OnInteract(Collider2D other)
    {
        if(!_canInteract) return false;

        if(OnInteract_Helper(other))
        {
            _director?.Play();
            _canInteract = false;
        }
        
        return false;
    }

    protected bool OnInteract_Helper(Collider2D other)
    {
        if(LayerMask.LayerToName(other.gameObject.layer).Contains("Player"))
        {
            LovePotionManager.Instance.OnLovePotionMade();
            return true;      
        }

        return false;
    }
}
