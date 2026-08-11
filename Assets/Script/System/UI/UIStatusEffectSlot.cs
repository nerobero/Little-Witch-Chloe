using TMPro;
using Types;
using UnityEngine;
using UnityEngine.UI;

public class UIStatusEffectSlot : UIBase
{
    [Header("Setting")]
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private Image icon;
    private ActiveStatusEffect effectRef;

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

    protected void Update()
    {
        float time = effectRef.remainingTime;
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);
        timerText.text = $"{minutes:00}:{seconds:00}";
    }

    public void Init(ActiveStatusEffect effect)
    {
        effectRef = effect;
        nameText.text = effect.definition.EffectName;
        icon.sprite = effect.definition.Icon;
        effect.OnEffectExpired += Expired;
    }

    public void Expired()
    {
        Debug.Log("UI Destroyed");
        effectRef.OnEffectExpired -= Expired;
        this.Hide();
    }
}
