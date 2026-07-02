using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TitleUI : MonoBehaviour
{
    private Button _startButton;
    private Button _loadButton;
    private Button _settingButton;
    private Button _exitButton;
    [SerializeField] private string sceneName = "wip";

    public void OnClickStart()
    {
        Debug.Log("Start");
        //SceneManager.LoadSceneAsync(sceneName);
        EventManager.Instance.ReqLevelLoad(Types.ELevelType.MainGame);
    }

    public void OnClickLoad()
    {
        Debug.Log("Load");
        if (SaveManager.Instance.LoadSaveGame())
        {
            SceneManager.LoadSceneAsync(sceneName);
            
        }
        else
            Debug.Log("none save data");
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
}
