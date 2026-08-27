using UnityEngine;

public class MessageBoxTrigger : InteractableBase
{

    [SerializeField] private string messageDataKey = "";  
    protected override void Interact_Impl()
    {
         UIManager.Instance.Get<UIMessageBox>().ShowMessage(messageDataKey);
    }
}
