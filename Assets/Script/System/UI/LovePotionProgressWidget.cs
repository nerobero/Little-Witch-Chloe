using UnityEngine;
using UnityEngine.UI;

public class LovePotionProgressWidget : UIBase
{
    [SerializeField] private Slider _progressSlider;

    #region EventSubscription
    protected override void SubscribeEvents()
    {
        if (GameManager.Instance == null) return;

        GameManager.Instance.OnLovePotionProgressChanged += UpdateProgress;
        UpdateProgress(GameManager.Instance.CurrentLovePotionProgress);
    }

    protected override void UnsubscribeEvents()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnLovePotionProgressChanged -= UpdateProgress;
    }
    #endregion

    public void UpdateProgress(float progress)
    {
        _progressSlider.value = progress;
    }
}
