using Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIMessageBox : UIBase
{
    [SerializeField] private TMP_Text messageText;
    [SerializeField] private Image messageImage;
    [SerializeField] private Button closeButton;

    private void Awake()
    {
        messageImage.gameObject.SetActive(false);
    }

    protected override void SubscribeEvents()
    {
        closeButton.onClick.AddListener(OnCloseClicked);
    }

    protected override void UnsubscribeEvents()
    {
        closeButton.onClick.RemoveListener(OnCloseClicked);
    }

    /// <summary>
    /// Shows the message box with the system text registered under the given key.
    /// </summary>
    public void ShowMessage(string key)
    {
        SystemTextRow row = DataTableRegistry.Get<SystemTextRow>().GetByKey(key);

        messageText.SetText(row.text);

        Sprite sprite = string.IsNullOrEmpty(row.spriteKey) ? null : Resources.Load<Sprite>(row.spriteKey);
        messageImage.sprite = sprite;
        messageImage.gameObject.SetActive(sprite != null);

        Show();
    }

    private void OnCloseClicked() => Hide();
}
