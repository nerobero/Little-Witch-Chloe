using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class TitleUI : UIBase
{
    private Button _startButton;
    private Button _loadButton;
    private Button _settingButton;
    private Button _exitButton;
    // [SerializeField] private TextMeshProUGUI _errorText;
    private CanvasGroup _error;
    [SerializeField] private string sceneName = "wip";
    private Coroutine ErrorRoutine;

    protected override void Start()
    {
        base.Start();
        // _error = _errorText.GetComponent<CanvasGroup>();
        // _error.alpha = 0.0f;
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

    public void OnClickStart()
    {
        Debug.Log("Start");
        //SceneManager.LoadSceneAsync(sceneName);
        //UIManager.Instance.Hide<TitleUI>();
        LevelManager.Instance.ChangeScene(Types.ELevelType.MainGame, Types.ELevelType.Overworld, false);
        // EventManager.Instance.ReqLevelLoad(Types.ELevelType.Overworld);

    }

    public void OnClickLoad()
    {
        Debug.Log("Load");
        if (SaveManager.Instance.LoadSaveGame())
        {
            LevelManager.Instance.ChangeScene(Types.ELevelType.MainGame, Types.ELevelType.Overworld, true);
            // SceneManager.LoadSceneAsync(sceneName);
            // UIManager.Instance.Hide<TitleUI>();
        }
        else
        {
            Debug.Log("none save data");
            UIManager.Instance.Get<UIMessageBox>().ShowMessage("NO_SAVE_DATA_TO_LOAD");
            // _errorText.text = "There is no save data";
            // if(ErrorRoutine != null)
            // {
            //     StopCoroutine(ErrorRoutine);
            //     ErrorRoutine = null;
            // }

            // ErrorRoutine = StartCoroutine(Fade(_error, 1.0f, 1.0f, 
            // onStart: () =>
            // {
                
            // },
            // onComplete: () =>
            // {
            //     ErrorRoutine = StartCoroutine(Fade(_error, 0.0f, 3.0f));
            // }));
        }
    }

    public void OnClickSetting()
    {
        Debug.Log("Setting");
        UIManager.Instance.Show<PopupHUD>();
    }

    public void OnClickExit()
    {
        Debug.Log("Exit");
        Application.Quit();
    }

    public void UiHover()
    {

        FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/UI Hover");
    }
    public void UiClick()
    {

        FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/UI Select");
    }

    IEnumerator Fade(CanvasGroup group, float targetAlpha, float duration,
                    System.Action onStart = null, System.Action onComplete = null)
    {
        onStart?.Invoke();

        float elapsedTime = 0f;
        float startAlpha = group.alpha;
        Debug.Log(startAlpha);

        while(elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;

            group.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / duration);

            yield return null;
        }

        group.alpha = targetAlpha;
        onComplete?.Invoke();
    }
}
