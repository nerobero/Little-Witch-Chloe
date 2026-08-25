using UnityEngine;

public class MessageBoxCinematicTrigger : CinematicTrigger
{
    public string messageKey = "";

    protected override void Interact_Impl()
    {
        UIManager.Instance.Get<UIMessageBox>().ShowMessage(messageKey, () => _director?.Play());
    }
}
