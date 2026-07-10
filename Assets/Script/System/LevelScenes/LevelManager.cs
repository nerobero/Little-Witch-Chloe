using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Types;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Processes transitions and loading of levels
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
        if (EventManager.Instance != null)
            EventManager.Instance.OnTransitionLevel += LoadLevelAdditively;
    }

    private void OnDisable()
    {
        if (EventManager.Instance != null)
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
}
