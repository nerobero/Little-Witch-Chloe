using UnityEngine;
using UnityEngine.SceneManagement;

public class EndingScreen : MonoBehaviour
{
    public void OnClick_OpenURL()
    {
        Application.OpenURL("https://neroberovero.itch.io/little-witch-chloe");
    }
    
    public void OnClick_OpenURL(string url)
    {
        Application.OpenURL(url);
    }

    public void BackToTitle()
    {
        PlayerController.Instance.InputContext.UI.Disable();
        PlayerController.Instance.InputContext.BaseInputAction.Enable();
        LevelManager.Instance.GoToTitle();
        PauseManager.Instance.UnpauseGame();
        SceneManager.LoadScene("TitleScene");
    }
}
