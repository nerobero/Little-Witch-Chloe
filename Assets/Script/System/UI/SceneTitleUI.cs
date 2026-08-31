using UnityEngine;
using TMPro;

public class SceneTitleUI : UIBase
{
    [SerializeField] private TextMeshProUGUI _text;

    protected override void Awake()
    {
        base.Awake();
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

    public void SetTitleText(string text)
    {
        _text.text = text;
        
    }

}
