using UnityEngine;
using UnityEngine.Playables;

public class LovePotionItem : ItemBase
{
    public PlayableDirector _director;
    [SerializeField] private float threshold = 0.01f;

    private void Awake()
    {
        _director.stopped += ReqGameEnded;
        gameObject.SetActive(false);
    }

    protected override bool OnInteract(Collider2D other)
    {
        if(!_canInteract) return false;

        if(OnInteract_Helper(other))
        {
            if(_director)
            {
                _director.time = 0;
                _director?.Play();
                _canInteract = false;
            }
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

    
    public void ReqGameEnded(PlayableDirector director)
    {
        if(director == _director)
        {
            Debug.Log("[Ending] Potion: ReqEndingScene" +
                $"directorMatch={director == _director}, " +
                $"time={director.time:F3}, duration={director.duration:F3}, " +
                $"threshold={threshold}, gameManager={GameManager.Instance != null}");
            // for onCompleted Notify
            if(director.time <= threshold)
            {
                if(GameManager.Instance)
                {
                    GameManager.Instance.ShowEndingScreen();
                    Debug.Log("[Ending] Potion: Succeed");
                    return;
                }
            } 
        }
        Debug.Log("[Ending] Potion: Error");
    }
}
