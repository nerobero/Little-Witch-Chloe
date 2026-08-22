using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TitleUI : UIBase
{
    private Button _startButton;
    private Button _loadButton;
    private Button _settingButton;
    private Button _exitButton;
    [SerializeField] private string sceneName = "wip";

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
        LevelManager.Instance.ChangeScene(Types.ELevelType.MainGame, Types.ELevelType.Overworld);
        // EventManager.Instance.ReqLevelLoad(Types.ELevelType.Overworld);

    }

    public void OnClickLoad()
    {
        Debug.Log("Load");
        if (SaveManager.Instance.LoadSaveGame())
        {
            SceneManager.LoadSceneAsync(sceneName);
            UIManager.Instance.Hide<TitleUI>();
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

        public void UiHover()
    {

        FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/UI Hover");


    }
    public void UiClick()
    {

        FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/UI Select");


    }
}
