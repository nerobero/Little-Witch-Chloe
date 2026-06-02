using Types;
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
}
