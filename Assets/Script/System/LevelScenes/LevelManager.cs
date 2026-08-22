using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Types;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Processes transitions and loading of levels
/// </summary>
public class LevelManager : MonoSingletonBase<LevelManager>
{
    private Dictionary<ELevelType, SceneBase> sceneBases = new Dictionary<ELevelType, SceneBase>();

    [SerializeField] private GameObject _loaderCanvas;
    [SerializeField] private Slider _progressBar;
    public CanvasGroup Fade_img;
    public CanvasGroup SceneTitle_img;
    float fadeDuration = 2;
    private bool _isFirstSceneLoad = true;

    Dictionary<ELevelType, string> sceneName = new Dictionary<ELevelType, string>()
    {
        {ELevelType.Intro, "Title"}, {ELevelType.MainGame, "MainGame"}, {ELevelType.Overworld, "Over World"},
        {ELevelType.BogLevel, "Bog"}
    };

    protected override void Awake()
    {
        dontDestroy = true;
        base.Awake();
        _loaderCanvas.SetActive(false);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public void Register(ELevelType levelType, SceneBase instance)
    {
        sceneBases[levelType] = instance;
        Debug.Log($"Registered SceneBase for {levelType}: {instance}");
    }

    private void OnEnable()
    {
        if (EventManager.Instance != null)
            EventManager.Instance.OnTransitionLevel += LoadLevelAdditively;
    }

    private void OnDisable()
    {
        if (EventManager.Instance != null)
            EventManager.Instance.OnTransitionLevel -= LoadLevelAdditively;
    }

    private async void LoadLevelAdditively(ELevelType levelType)
    {
        await SceneManager.LoadSceneAsync((int)levelType, LoadSceneMode.Additive);
        // _progressBar.value = 0.0f;
        // Debug.Log(levelType);
        // var scene = SceneManager.LoadSceneAsync((int)levelType, LoadSceneMode.Additive);

        // scene.allowSceneActivation = false;
        // _loaderCanvas.SetActive(true);

        // Debug.Log("_loaderCanvas activated");

        // do
        // {
        //     await Task.Delay(100);
        //     _progressBar.value = scene.progress;
        //     Debug.Log(scene.progress);
        // } while(scene.progress < 0.9f);

        // await Task.Delay(1000);

        // scene.allowSceneActivation = true;
        // _loaderCanvas.SetActive(false);
    }

    public void RestartCurrentLevel()
    {
        StartCoroutine(RestartLevelCoroutine());
    }

    private IEnumerator RestartLevelCoroutine()
    {
        foreach(SceneBase scene in sceneBases.Values)
        {
            scene.ResetScene();
            yield return null;
        }

        GameManager.Instance.ResetState();
        
        bool hasSave = SaveManager.Instance.LoadSaveGame();

        if(hasSave)
            SaveManager.Instance.ApplyAllGameData();

        UIManager.Instance.Hide<UIGameOverHUD>();
        
    }

    public void RegisterInstance(MonoBehaviour behaviour)
    {
        ELevelType curLevel = GameManager.Instance.CurrentLevel;

        if(!sceneBases.TryGetValue(curLevel, out SceneBase sceneBase))
        {
            Debug.LogWarning($"No SceneBase registered for level {curLevel}, cannot register {behaviour}");
            return;
        }

        sceneBase.Register(behaviour);
    }

    public async void LoadScene(ELevelType levelType, ELevelType curLevelType)
    {
        _isFirstSceneLoad = true;
        _progressBar.value = 0.0f;
        _loaderCanvas.SetActive(true);

        await SceneManager.LoadSceneAsync((int)levelType);

        var scene = SceneManager.LoadSceneAsync((int)curLevelType, LoadSceneMode.Additive);
        scene.allowSceneActivation = false;

        do
        {
            await Task.Delay(100);
            _progressBar.value = scene.progress;
        } while(scene.progress < 0.9f);

        scene.allowSceneActivation = true;
        GameManager.Instance.SetCurrentLevel(curLevelType);
        //_loaderCanvas.SetActive(false);
    }

    public void ChangeScene(ELevelType permanLevelType, ELevelType loadLevelType)
    {
        // Fade in
        StartCoroutine(Fade(Fade_img, 1f, fadeDuration,
            onStart: () =>
            {
                Fade_img.blocksRaycasts = true;
            },
            onComplete: ()=>
            {
                UIManager.Instance.Hide<TitleUI>();
                LoadScene(permanLevelType, loadLevelType);
            }));
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"_isFirstSceneLoad: {_isFirstSceneLoad}");
        if (_isFirstSceneLoad)
        {
            _isFirstSceneLoad = false;
            return;
        }

        SceneTitle_img.gameObject.SetActive(true);

        Debug.Log($"{sceneName[GameManager.Instance.GetCurrentLevel()]}");
        SceneTitle_img.GetComponent<SceneTitleUI>().SetTitleText(
            sceneName[GameManager.Instance.GetCurrentLevel()]);
        // Fade out
        StartCoroutine(Fade(Fade_img, 0f, fadeDuration,
            onStart: () =>
            {
                _loaderCanvas.SetActive(false);
            },
            onComplete: ()=>
            {
                Fade_img.gameObject.SetActive(false);
                ShowSceneTitle();
            }));
    }

    private void ShowSceneTitle()
    {
        // Fade in
        StartCoroutine(Fade(SceneTitle_img, 1f, fadeDuration,
        onStart: () =>
        {
            
        },
        onComplete: () =>
        {
            StartCoroutine(Fade(SceneTitle_img, 0f, fadeDuration,
            onComplete: () =>
            {
                SceneTitle_img.gameObject.SetActive(false);
            })); 
        }));
        
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
