using TMPro;
using UnityEngine;

public class UIDialoguePanel : UIBase
{
    [SerializeField] private TMP_Text speakerName1;
    
    #region EventSubscription
    protected override void SubscribeEvents()
    {
        DialogueSystem.Instance.DialogueEnded += base.Hide;
    }

    protected override void UnsubscribeEvents()
    {
        DialogueSystem.Instance.DialogueEnded -= base.Hide;
    }
    #endregion

    private void UpdateUI()
    {

    }
    #region ButtonListeners

    private void OnNextDialogue()
    {

    }
    #endregion
}
