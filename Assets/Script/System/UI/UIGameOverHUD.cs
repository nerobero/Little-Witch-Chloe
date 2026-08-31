using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using Data;
using Unity.VisualScripting;
using FMODUnity;

public class UIGameOverHUD : UIBase
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Awake()
    {
        base.Awake();

        //SubscribeEvents();

        //Hide();
    }

    public override void Show()
    {
        Debug.Log("Game Over Show");
        PauseManager.Instance.PauseGame();
        base.Show();
        // FMOD.Studio.Bus masterBus = FMODUnity.RuntimeManager.GetBus("bus:/"); 
        // masterBus.stopAllEvents(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        // FMODUnity.RuntimeManager.PlayOneShot("event:/Game Over");
    }

     #region Button click event
    // Save button click event
    public void OnRetryButtonClicked()
    {
        PauseManager.Instance.UnpauseGame();
        LevelManager.Instance.RestartCurrentLevel();
        SoundManager.Instance.RestartCurrentBGM();
        //FMODUnity.RuntimeManager.StudioSystem.setParameterByName("Forest Level",0);
    }

    public void OnExitClicked()
    {
        SceneManager.LoadScene("TitleScene");
    }
    
    #endregion

    #region EventSubscription
    protected override void SubscribeEvents()
    {
        Debug.Log("Game Over HUD Subscribe");
        
    }

    protected override void UnsubscribeEvents()
    {
        Debug.Log("Game Over HUD unsubscribe");
    }

    #endregion
}
