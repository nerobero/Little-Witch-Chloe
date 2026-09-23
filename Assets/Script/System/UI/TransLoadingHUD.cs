using System.Collections;
using UnityEngine;

public class TransLoadingHUD : UIBase
{
    private CanvasGroup _canvasGroup;
    private Coroutine _fadeRoutine;

    protected override void Awake()
    {
        base.Awake();
        _canvasGroup = GetComponent<CanvasGroup>();
        if (_canvasGroup == null)
        {
            _canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
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

    /// <summary>
    /// Fades this HUD's alpha out over <paramref name="duration"/> seconds, then hides it.
    /// </summary>
    public void FadeHide(float duration = 0.5f)
    {
        if (_fadeRoutine != null)
        {
            StopCoroutine(_fadeRoutine);
        }
        _fadeRoutine = StartCoroutine(FadeHideRoutine(duration));
    }

    private IEnumerator FadeHideRoutine(float duration)
    {
        float startAlpha = _canvasGroup.alpha;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            _canvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, elapsedTime / duration);
            yield return null;
        }

        _canvasGroup.alpha = 0f;
        Hide();
        _canvasGroup.alpha = startAlpha;
        _fadeRoutine = null;
    }
}
