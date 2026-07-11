using UnityEngine;

public class PauseManager : MonoSingletonBase<PauseManager>
{
    public bool IsPaused { get; private set; }  

    public void PauseGame()
    {
        IsPaused = true;
        
        PlayerController.Instance.InputContext.BaseInputAction.Disable();
        PlayerController.Instance.InputContext.UI.Enable();
        Time.timeScale = 0f;

    }

    public void UnpauseGame()
    {
        IsPaused = false;

        PlayerController.Instance.InputContext.UI.Disable();
        PlayerController.Instance.InputContext.BaseInputAction.Enable();
        Time.timeScale = 1f;
    }
}
