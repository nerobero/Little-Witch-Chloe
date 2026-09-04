using UnityEngine;

public class MessageBoxTrigger : InteractableBase
{

    [SerializeField] private string messageDataKey = "";  
    protected override void Interact_Impl()
    {
        FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/Dialouge");
         UIManager.Instance.Get<UIMessageBox>().ShowMessage(messageDataKey);
    }
}
