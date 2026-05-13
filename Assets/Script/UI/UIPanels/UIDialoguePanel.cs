using System.Data.Common;
using TMPro;
using UnityEditor.U2D;
using UnityEngine;
using UnityEngine.UI;

public class UIDialoguePanel : UIBase
{
    private Image mainPanel;

    [Header("Speaker")]
    [SerializeField] private TMP_Text[] speakerName = new TMP_Text[2];
    [SerializeField] private Image[] speakerSprite = new Image[2];
    [SerializeField] private TMP_Text[] dialogueText = new TMP_Text[2];
    [SerializeField] private Image[] speechBubble = new Image[2];

    [Header("Color Settings")]
    [SerializeField] private Color fgColor;
    [SerializeField] private Color bgColor;
    // To memory speaker index
    private int currentSpeakerIndex; // 0 : left, 1 : right
    
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
        speakerName[0].SetText(_speakerName1);
        speakerName[1].SetText(_speakerName2);
        speakerSprite[0].sprite = sprite1;
        speakerSprite[1].sprite = sprite2;

        SetFirstLine();
    }

    private void SetFirstLine()
{
    (string firstSpeaker, string dialogue, _) = DialogueSystem.Instance.ReturnDialogueLine();
    
    // Check the first speaker
    if (firstSpeaker.Equals(speakerName[0].text))
    {
        currentSpeakerIndex = 0;
    }
    else if (firstSpeaker.Equals(speakerName[1].text))
    {
        currentSpeakerIndex = 1;
    }
    else
    {
        // default. (error exception)
        currentSpeakerIndex = 0;
    }
    
    // Set current speaker's speechbubble to be on the top
    speechBubble[currentSpeakerIndex].color = fgColor;
    speechBubble[currentSpeakerIndex].transform.SetAsFirstSibling();
    
    dialogueText[currentSpeakerIndex].SetText(dialogue);

    // Set non speaker's speechbubble to be on the bottom
    int otherIndex = currentSpeakerIndex == 0 ? 1 : 0;
    speechBubble[otherIndex].color = bgColor;
}

    private void UpdateUI()
    {
        (string speaker, string dialogue, bool isSameSpeaker) = DialogueSystem.Instance.ReturnDialogueLine();

        if (isSameSpeaker)
        {
            dialogueText[currentSpeakerIndex].SetText(dialogue);

            return;
        }

        // Change the prev speaker bubble's color to be background color
        speechBubble[currentSpeakerIndex].color = bgColor;

        // Change the index
        currentSpeakerIndex += 1;

        if(currentSpeakerIndex >= 2)
        {
            currentSpeakerIndex = 0;
        }

        // Change the current speaker bubble's color to be foreground color
        speechBubble[currentSpeakerIndex].color = fgColor;
        // And Set the bubble to be on the top.
        speechBubble[currentSpeakerIndex].transform.SetAsFirstSibling();
    }
    #region ButtonListeners

    private void OnNextDialogue()
    {
        DialogueSystem.Instance.UpdateLineIndex();
        UpdateUI();
    }
    #endregion
}
