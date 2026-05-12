using System.Data.Common;
using TMPro;
using UnityEditor.U2D;
using UnityEngine;
using UnityEngine.UI;

public class UIDialoguePanel : UIBase
{
    private Image mainPanel;

    [Header("Speaker1")]
    [SerializeField] private TMP_Text speakerName1;
    [SerializeField] private Image speakerSprite1;
    [SerializeField] private TMP_Text dialogueText1;
    [SerializeField] private Image speechBubble1;

    [Header("Speaker2")]
    [SerializeField] private TMP_Text speakerName2;
    [SerializeField] private Image speakerSprite2;
    [SerializeField] private TMP_Text dialogueText2;
    [SerializeField] private Image speechBubble2;
    
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

    public void Initialize(string _speakerName1, string _speakerName2, Sprite sprite1, Sprite sprite2)
    {
        speakerName1.SetText(_speakerName1);
        speakerName2.SetText(_speakerName2);
        speakerSprite1.sprite = sprite1;
        speakerSprite2.sprite = sprite2;

        UpdateUI();
    }

    private void UpdateUI()
    {
        (string speaker, string dialogue, bool isSameSpeaker) = DialogueSystem.Instance.ReturnDialogueLine();


        string name1 = speakerName1.text;
        string name2 = speakerName2.text;

        if (isSameSpeaker)
        {
            
        }

    }
    #region ButtonListeners

    private void OnNextDialogue()
    {
        DialogueSystem.Instance.UpdateLineIndex();
        UpdateUI();
    }
    #endregion
}
