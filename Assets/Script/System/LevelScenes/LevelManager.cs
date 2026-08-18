using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Types;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Processes transitions and loading of levels
/// </summary>
public class LevelManager : MonoSingletonBase<LevelManager>
{
    private List<SceneBase> sceneBases = new List<SceneBase>();

    [SerializeField] private GameObject _loaderCanvas;
    [SerializeField] private Slider _progressBar;

    protected override void Awake()
    {
        dontDestroy = true;
        base.Awake();
        _loaderCanvas.SetActive(false);
    }

    public void Register(SceneBase instance)
    {
        sceneBases.Add(instance);
        Debug.Log(sceneBases.Last());
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
        foreach(SceneBase scene in sceneBases)
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
        int curlevel = (int)GameManager.Instance.CurrentLevel;

        if(sceneBases.Count <= 0)
        {
            return;
        }
        
        sceneBases[curlevel].Register(behaviour);
    }

    public async void LoadScene(ELevelType levelType, ELevelType curLevelType)
    {
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
        _loaderCanvas.SetActive(false);
    }
}
