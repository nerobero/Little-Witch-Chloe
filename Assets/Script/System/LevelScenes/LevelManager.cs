using System.Collections;
using System.Collections.Generic;
using Types;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 
/// </summary>
public class LevelManager : MonoSingletonBase<LevelManager>
{
    protected override void Awake()
    {
        dontDestroy = true;
        base.Awake();
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
        int sceneCount = SceneManager.sceneCount;
        //List<string> subScenes = new List<string>(GameManager.Instance.UnlockedLevels);
        List<ELevelType> subScenes = new List<ELevelType>(GameManager.Instance.UnlockedLevels);
        string persistentSceneName = SceneManager.GetActiveScene().name;

        // for (int i = 0; i < sceneCount; i++)
        // {
        //     Scene scene = SceneManager.GetSceneAt(i);
        //     if (scene.name != persistentSceneName) subScenes.Add(scene.name);
        // }

        foreach (ELevelType sceneType in subScenes)
        {
            yield return SceneManager.UnloadSceneAsync((int)sceneType);
        }

        //System.GC.Collect();
        yield return Resources.UnloadUnusedAssets();

        bool hasSave = SaveManager.Instance.LoadSaveGame();

        List<ELevelType> scenesToReload = new List<ELevelType>(GameManager.Instance.UnlockedLevels);

        // 2. Reload sub scenes.
        foreach (ELevelType sceneType in scenesToReload)
        {
            yield return SceneManager.LoadSceneAsync((int)sceneType, LoadSceneMode.Additive);
        }

        if(hasSave)
            SaveManager.Instance.ApplyAllGameData();
    }
}
