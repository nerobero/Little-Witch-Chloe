using System.Collections;
using TMPro;
using Types;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// The on-screen dialogue panel. Listens for <see cref="DialogueSystem.DialogueStarted"/>,
/// builds itself from the chain that is starting (1 speaker -> monologue layout,
/// 2 -> dialogue layout), then renders one line at a time as the player advances.
/// </summary>
public class UIDialoguePanel : UIBase
{
    [Header("Backgrounds")]
    [SerializeField] private GameObject bgDialogue;   // 2-portrait background
    [SerializeField] private GameObject bgMonologue;  // 1-portrait background

    [Header("Speaker slots (index 0 = slot 1, index 1 = slot 2)")]
    [SerializeField] private GameObject[] speakerGroup = new GameObject[2]; // Group_Speaker1 / 2
    [SerializeField] private GameObject[] dialogueBox = new GameObject[2];  // DialogueBox1 / 2
    [SerializeField] private TMP_Text[] speakerName = new TMP_Text[2];
    [SerializeField] private Image[] speakerSprite = new Image[2];
    [SerializeField] private TMP_Text[] dialogueText = new TMP_Text[2];
    [SerializeField] private Image[] speechBubble = new Image[2];

    [Header("Portraits")]
    [SerializeField] private SpeakerRegistry speakerRegistry;

    [Header("Speech bubble colors")]
    [SerializeField] private Color fgColor = Color.white;
    [SerializeField] private Color bgColor = Color.gray;

    // Speaker name occupying each slot; _slotSpeaker[1] is null during a monologue.
    private readonly string[] _slotSpeaker = new string[2];
    // Slot whose speech bubble currently has focus.
    private int _currentSlot;
    private bool isTypeWriting = false;
    private Coroutine typewriting;

    #region EventSubscription
    protected override void SubscribeEvents()
    {
        DialogueSystem.Instance.DialogueStarted += HandleDialogueStarted;
        DialogueSystem.Instance.DialogueEnded += Hide;
    }

    protected override void UnsubscribeEvents()
    {
        if (DialogueSystem.Instance == null)
            return;

        DialogueSystem.Instance.DialogueStarted -= HandleDialogueStarted;
        DialogueSystem.Instance.DialogueEnded -= Hide;
    }
    #endregion

    /// <summary>
    /// While the panel is up, gameplay input is off and the UI action map is on
    /// (so the Next button receives mouse clicks) - same swap the other blocking
    /// panels use. The character keeps its Animator state and settles to idle.
    /// </summary>
    public override void Show()
    {
        PlayerController.Instance.InputContext.UI.Enable();
        PlayerController.Instance.InputContext.BaseInputAction.Disable();
        base.Show();
    }

    public override void Hide()
    {
        PlayerController.Instance.InputContext.UI.Disable();
        PlayerController.Instance.InputContext.BaseInputAction.Enable();
        StopCoroutine(typewriting);
        typewriting = null;
        isTypeWriting = false;
        base.Hide();
    }

    /// <summary>
    /// Configures the panel for the chain that just started, then shows its first line.
    /// </summary>
    private void HandleDialogueStarted()
    {
        var speakers = DialogueSystem.Instance.GetChainSpeakers();
        if (speakers.Count == 0)
        {
            Debug.LogError("[UIDialoguePanel] Dialogue started with no speakers.");
            return;
        }

        bool isMonologue = speakers.Count == 1;

        bgMonologue.SetActive(isMonologue);
        bgDialogue.SetActive(!isMonologue);

        AssignSlot(0, speakers[0]);
        dialogueBox[0].SetActive(true);
        if (isMonologue)
            ClearSlot(1);
        else
        {
            AssignSlot(1, speakers[1]);
            // Speaker 2's box stays hidden until they actually get a line (RenderLine reveals it).
            dialogueBox[1].SetActive(false);
        }

        // Focus starts on slot 0; RenderLine moves it if the first line is slot 1.
        _currentSlot = 0;
        speechBubble[0].color = fgColor;
        if (!isMonologue)
            speechBubble[1].color = bgColor;

        Show();
        RenderLine();
    }

    private void AssignSlot(int slot, string speaker)
    {
        _slotSpeaker[slot] = speaker;
        speakerName[slot].SetText(speaker);
        speakerGroup[slot].SetActive(true);
    }

    private void ClearSlot(int slot)
    {
        _slotSpeaker[slot] = null;
        speakerGroup[slot].SetActive(false);
        dialogueBox[slot].SetActive(false);
    }

    /// <summary>
    /// Renders the line <see cref="DialogueSystem"/> currently points at into the
    /// matching speaker slot. No-ops once the dialogue has ended - the final line
    /// is dismissed by one more Next click, which raises DialogueEnded first.
    /// </summary>
    private void RenderLine()
    {
        if (!DialogueSystem.Instance.IsPlaying)
            return;

        (string speaker, string dialogue, _, EEmotion emotion)
            = DialogueSystem.Instance.ReturnDialogueLine();

        int slot = SlotFor(speaker);
        dialogueBox[slot].SetActive(true);

        if (speakerRegistry != null)
            speakerSprite[slot].sprite = speakerRegistry.Get(speaker, emotion);

        if(isTypeWriting)
        {
            dialogueText[slot].maxVisibleCharacters = dialogue.Length;
        }
        //dialogueText[slot].SetText(dialogue);
        else
        {
            typewriting = StartCoroutine(TypeTextEffect(slot, dialogue));
        }

        if (slot != _currentSlot)
        {
            speechBubble[_currentSlot].color = bgColor;
            _currentSlot = slot;
        }

        speechBubble[slot].color = fgColor;
        speechBubble[slot].transform.SetAsLastSibling();
    }

    IEnumerator TypeTextEffect(int slot, string dialogue)
    {
        isTypeWriting = true;
        dialogueText[slot].SetText(dialogue);

        dialogueText[slot].maxVisibleCharacters = 0;

        for(int i = 0; i <= dialogue.Length; ++i)
        {
            dialogueText[slot].maxVisibleCharacters = i;
            yield return null;
        }

        isTypeWriting = false;
    }

    // Unknown speakers (and every line of a monologue) resolve to slot 0.
    private int SlotFor(string speaker) => speaker == _slotSpeaker[1] ? 1 : 0;

    #region ButtonListeners
    public void OnNextDialogue()
    {
        if(isTypeWriting)
        {
            StopCoroutine(typewriting);
            typewriting = null;
            RenderLine();
            isTypeWriting = false;
        }
        else
        {
            DialogueSystem.Instance.Advance();
            RenderLine();
        }
    }
    #endregion
}
