using UnityEngine;

public class TransLoadingHUD : UIBase
{
    protected override void Start()
    {
        base.Start();
        Hide();
    }
    /// <summary>
    /// Subscribes events from the related systems.  
    /// </summary>
    protected override void SubscribeEvents()
    {
        
    }

    /// <summary>
    /// Unsubscribes events from the related systems.
    /// </summary>
    protected override void UnsubscribeEvents()
    {
        
    }
}
