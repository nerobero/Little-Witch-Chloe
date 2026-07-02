using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Types;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 
/// </summary>
public class LevelManager : MonoSingletonBase<LevelManager>
{
    private List<SceneBase> sceneBases = new List<SceneBase>();

    protected override void Awake()
    {
        dontDestroy = true;
        base.Awake();
    }

    public void Register(SceneBase instance)
    {
        sceneBases.Add(instance);
    }

    private void OnEnable()
    {
        EventManager.Instance.OnTransitionLevel += LoadLevelAdditively;
    }

    private void OnDisable()
    {
        EventManager.Instance.OnTransitionLevel -= LoadLevelAdditively;
    }

    private void LoadLevelAdditively(ELevelType levelType)
    {
        SceneManager.LoadSceneAsync((int)levelType, LoadSceneMode.Additive);
    }

    public void RestartCurrentLevel()
    {
        StartCoroutine(RestartLevelCoroutine());
    }

    private IEnumerator RestartLevelCoroutine()
    {
        //int sceneCount = SceneManager.sceneCount;

        //List<string> subScenes = new List<string>(GameManager.Instance.UnlockedLevels);
        //List<ELevelType> subScenes = new List<ELevelType>(GameManager.Instance.UnlockedLevels);
        //string persistentSceneName = SceneManager.GetActiveScene().name;

        foreach(SceneBase scene in sceneBases)
        {
            scene.ResetScene();
            yield return null;
        }

        // for (int i = 0; i < sceneCount; i++)
        // {
        //     Scene scene = SceneManager.GetSceneAt(i);
        //     if (scene.name != persistentSceneName) subScenes.Add(scene.name);
        // }

        // Not use unload => use respawn? object pooling.
        // foreach (ELevelType sceneType in subScenes)
        // {
        //     yield return SceneManager.UnloadSceneAsync((int)sceneType);
        // }
        // IEnumerable<IResetable> interactables = Object.FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None).OfType<IResetable>();

        // yield return Resources.UnloadUnusedAssets();

        bool hasSave = SaveManager.Instance.LoadSaveGame();

        // List<ELevelType> scenesToReload = new List<ELevelType>(GameManager.Instance.UnlockedLevels);

        // // 2. Reload sub scenes.
        // foreach (ELevelType sceneType in scenesToReload)
        // {
        //     yield return SceneManager.LoadSceneAsync((int)sceneType, LoadSceneMode.Additive);
        // }

        if(hasSave)
            SaveManager.Instance.ApplyAllGameData();

        
        //yield return new WaitForSeconds(0.1f);

        UIManager.Instance.Hide<UIGameOverHUD>();
    }
}
