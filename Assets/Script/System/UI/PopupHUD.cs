using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using Data;

public class PopupHUD : UIBase
{
    [SerializeField] private MasterVolume _masterVolumeObject;
    [SerializeField] private MusicVolume _bgmVolumeObject;
    [SerializeField] private SFXVolume _sfxVolumeObject;
    // [SerializeField] private TMP_Dropdown _languages;
    // List<string> languageLists;

    protected override void Start()
    {
        base.Start();

        // _languages?.ClearOptions();

        // languageLists.Add("English");
        // languageLists.Add("한국어");
        // // This will be added if we plan to add jp/cn(tw)
        // // languageLists.Add("日本語"); // japanese
        // // languageLists.Add("简体中文"); // chinese
        // // languageLists.Add("繁體中文"); // taiwan chinese

        // _languages.AddOptions(languageLists);

        SubscribeEvents();

        Hide();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    #region Button click event
    // Save button click event
    public void OnSaveButtonClicked()
    {
        SaveManager.Instance.SavePlayerData();
    }

    // Return to the menu button click event
    public void OnReturnMenuButtonClicked()
    {
        // Change Scene(title scene)
        SceneManager.LoadScene("TitleScene");
    }

    public void OnClosePopupButtonClicked()
    {
        Time.timeScale = 1.0f;
        UIManager.Instance.Hide<PopupHUD>();
    }

    public void OnQuitClicked()
    {
        Application.Quit(0);
    }
    
    #endregion

    #region EventSubscription
    protected override void SubscribeEvents()
    {
        Debug.Log("Option HUD Subscribe");
        
    }

    protected override void UnsubscribeEvents()
    {
        
    }

    #endregion
}
