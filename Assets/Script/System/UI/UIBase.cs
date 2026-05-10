using UnityEngine;

/// <summary>
/// Base class for any UI that can be shown on the screen.
/// </summary>
public abstract class UIBase : MonoBehaviour
{
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
    public virtual void Show() => gameObject.SetActive(true);

    /// <summary>
    /// Hides this UI panel. 
    /// Left as virtual just in case there is special 
    /// presentation before it is hidden from the screen.
    /// </summary>
    public virtual void Hide() => gameObject.SetActive(false);
}
