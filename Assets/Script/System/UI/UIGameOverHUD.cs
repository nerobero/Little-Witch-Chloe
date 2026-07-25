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
    protected override void Start()
    {
        base.Start();

        //SubscribeEvents();

        //Hide();
    }

    public override void Show()
    {
        Debug.Log("Game Over Show");
        PauseManager.Instance.PauseGame();
        base.Show();
        FMOD.Studio.Bus masterBus = FMODUnity.RuntimeManager.GetBus("bus:/"); 
        masterBus.stopAllEvents(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        FMODUnity.RuntimeManager.PlayOneShot("event:/Game Over");

    }

     #region Button click event
    // Save button click event
    public void OnRetryButtonClicked()
    {
        LevelManager.Instance.RestartCurrentLevel();
        FMODUnity.RuntimeManager.StudioSystem.setParameterByName("Forest Level",0);
        PauseManager.Instance.UnpauseGame();
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
