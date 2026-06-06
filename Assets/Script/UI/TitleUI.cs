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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnClickStart()
    {
        Debug.Log("Start");
        SceneManager.LoadSceneAsync(sceneName);
    }

    public void OnClickLoad()
    {
        Debug.Log("Load");
        if (SaveManager.Instance.LoadSaveGame())
            SceneManager.LoadSceneAsync(sceneName);
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
