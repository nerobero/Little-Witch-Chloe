using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using Data;

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
        base.Show();
    }

     #region Button click event
    // Save button click event
    public void OnRetryButtonClicked()
    {
        LevelManager.Instance.RestartCurrentLevel();
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
        
    }

    #endregion
}
