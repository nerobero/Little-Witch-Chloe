using System;
using Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIMessageBox : UIBase
{
    [SerializeField] private TMP_Text messageText;
    [SerializeField] private Image messageImage;

    private Action _onClosed;

    protected override void Awake()
    {
        base.Awake();
        messageImage.gameObject.SetActive(false);
    }

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
        base.Hide();
    }

    protected override void SubscribeEvents()
    {
        // closeButton.onClick.AddListener(OnCloseClicked);
    }

    protected override void UnsubscribeEvents()
    {
        // closeButton.onClick.RemoveListener(OnCloseClicked);
    }

    /// <summary>
    /// Shows the message box with the system text registered under the given key.
    /// </summary>
    /// <param name="onClosed">Invoked once, when the box is closed via the close button.</param>
    public void ShowMessage(string key, Action onClosed = null)
    {
        SystemTextRow row = DataTableRegistry.Get<SystemTextRow>().GetByKey(key);

        messageText.SetText(row.text);

        Sprite sprite = string.IsNullOrEmpty(row.spriteKey) ? null : Resources.Load<Sprite>(row.spriteKey);
        messageImage.sprite = sprite;
        messageImage.gameObject.SetActive(sprite != null);

        _onClosed = onClosed;
        Show();
    }

    public void OnCloseClicked()
    {
        Hide();
        _onClosed?.Invoke();
        _onClosed = null;
    }
}
