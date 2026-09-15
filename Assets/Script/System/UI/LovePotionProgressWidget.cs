using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LovePotionProgressWidget : UIBase
{
    private enum EEmoteType { Sad, Happy, InLove }
    private enum ECharlieProfileState { Neutral, Sad, Happy }

    [SerializeField] private Slider _progressSlider;

    [Header("Emote Icon")]
    [SerializeField] private GameObject _emoteRoot;
    [SerializeField] private Image _emoteIcon;
    [SerializeField] private Sprite _sadEmoteSprite;
    [SerializeField] private Sprite _happyEmoteSprite;
    [SerializeField] private Sprite _inLoveEmoteSprite;
    [SerializeField] private float _emoteDisplayDuration = 1.5f;
    [SerializeField] private float _significantIncreaseThreshold = 0.4f;

    [Header("Charlie Profile")]
    [SerializeField] private Image _charlieProfile;
    [SerializeField] private Sprite _neutralProfileSprite;
    [SerializeField] private Sprite _sadProfileSprite;
    [SerializeField] private Sprite _happyProfileSprite;
    [SerializeField] private int _trendWindowSize = 4;

    private bool _hasLastProgress = false;
    private float _lastProgress = 0f;
    private readonly Queue<float> _recentDeltas = new Queue<float>();
    private Coroutine _emoteRoutine;
    private ECharlieProfileState _currentProfileState = ECharlieProfileState.Neutral;

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

        if (_hasLastProgress)
        {
            float delta = progress - _lastProgress;

            if (!Mathf.Approximately(delta, 0f))
            {
                PlayEmote(delta);
                TrackTrend(delta);
            }
        }

        _lastProgress = progress;
        _hasLastProgress = true;
    }

    private void PlayEmote(float delta)
    {
        EEmoteType emote = delta < 0f
            ? EEmoteType.Sad
            : (delta >= _significantIncreaseThreshold ? EEmoteType.InLove : EEmoteType.Happy);

        Sprite sprite = emote switch
        {
            EEmoteType.Sad => _sadEmoteSprite,
            EEmoteType.InLove => _inLoveEmoteSprite,
            _ => _happyEmoteSprite,
        };

        if (sprite == null || _emoteIcon == null) return;

        _emoteIcon.sprite = sprite;

        if (_emoteRoutine != null)
            StopCoroutine(_emoteRoutine);

        _emoteRoutine = StartCoroutine(ShowEmoteThenHide());
    }

    private IEnumerator ShowEmoteThenHide()
    {
        if (_emoteRoot != null)
            _emoteRoot.SetActive(true);

        yield return new WaitForSeconds(_emoteDisplayDuration);

        if (_emoteRoot != null)
            _emoteRoot.SetActive(false);

        _emoteRoutine = null;
    }

    private void TrackTrend(float delta)
    {
        _recentDeltas.Enqueue(delta);
        while (_recentDeltas.Count > _trendWindowSize)
            _recentDeltas.Dequeue();

        int positiveCount = 0;
        int negativeCount = 0;
        foreach (float d in _recentDeltas)
        {
            if (d > 0f) positiveCount++;
            else if (d < 0f) negativeCount++;
        }

        ECharlieProfileState newState;
        if (_recentDeltas.Count == _trendWindowSize && negativeCount == _trendWindowSize)
            newState = ECharlieProfileState.Sad;
        else if (positiveCount > negativeCount)
            newState = ECharlieProfileState.Happy;
        else
            newState = ECharlieProfileState.Neutral;

        SetCharlieProfile(newState);
    }

    private void SetCharlieProfile(ECharlieProfileState state)
    {
        if (state == _currentProfileState || _charlieProfile == null) return;

        _currentProfileState = state;

        Sprite sprite = state switch
        {
            ECharlieProfileState.Sad => _sadProfileSprite,
            ECharlieProfileState.Happy => _happyProfileSprite,
            _ => _neutralProfileSprite,
        };

        if (sprite != null)
            _charlieProfile.sprite = sprite;
    }
}
