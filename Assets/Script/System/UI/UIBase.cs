using UnityEngine;

/// <summary>
/// Base class for any UI that can be shown on the screen.
/// </summary>
public abstract class UIBase : MonoBehaviour
{
    public GameObject root;
    public bool isStackable = false;
    public bool isPauseable = false;
    public bool isInputDisable = false;

    protected virtual void Awake()
    {
        // self-registering to UIManager's panel registry:
        UIManager.Instance.Register(this);
    }

    /// <summary>
    /// Subscribes events from the related systems.  
    /// </summary>
    protected abstract void SubscribeEvents();

    /// <summary>
    /// Unsubscribes events from the related systems.
    /// </summary>
    protected abstract void UnsubscribeEvents();

    protected virtual void OnEnable() => SubscribeEvents();
    protected virtual void OnDisable() => UnsubscribeEvents();

    /// <summary>
    /// Shows this UI panel. 
    /// Left as virtual just in case there is special 
    /// presentation before it is shown on screen.
    /// </summary>
    public virtual void Show()
    {
        if (!root.activeSelf)
        {
            Canvas canvas = GetComponent<Canvas>();
            canvas.overrideSorting = true;

            canvas.sortingOrder = UIManager.Instance.currentSortOrder;
            UIManager.Instance.currentSortOrder++;

            if (isStackable)
            {
                UIManager.Instance.shownUI.Push(this);
            }
        }

        root.SetActive(true);

    }

    /// <summary>
    /// Hides this UI panel. 
    /// Left as virtual just in case there is special 
    /// presentation before it is hidden from the screen.
    /// </summary>
    public virtual void Hide()
    {
        if (root.activeSelf)
        {
            Canvas canvas = GetComponent<Canvas>();
            if (canvas != null)
            {
                canvas.overrideSorting = false;

                canvas.sortingOrder = 0;
                UIManager.Instance.currentSortOrder--;
                if (UIManager.Instance.currentSortOrder <= 0)
                {
                    UIManager.Instance.currentSortOrder = 0;
                }

                if (isStackable)
                {
                    if (UIManager.Instance.shownUI.Count > 0)
                    {
                        UIManager.Instance.shownUI.Pop();
                    }

                    if (UIManager.Instance.shownUI.Count == 0)
                    {
                        if (isInputDisable)
                        {
                            if (PlayerController.Instance != null)
                            {
                                PlayerController.Instance.InputContext.UI.Disable();
                                PlayerController.Instance.InputContext.BaseInputAction.Enable();
                            }
                        }
                        PauseManager.Instance.UnpauseGame();
                    }
                }
            }

            root.SetActive(false);
        }

    }
}
