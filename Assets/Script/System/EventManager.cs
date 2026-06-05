using System;
using Types;
using UnityEngine;

/// <summary>
/// Event Manager that processes the global events
/// such as level loading, story unlocks, commission progress.
/// </summary>
public class EventManager : MonoSingletonBase<EventManager>
{
    #region SubscribableEvents
    public event Action<ELevelType> OnTransitionLevel;

    public event Action OnLevelLoadFailed;

    public event Action<ELevelType> OnUnlockLevel;

    #endregion


    protected override void Awake()
    {
        dontDestroy = true;
        base.Awake();
    }

    #region RequestFunctions
    public void ReqLevelLoad(ELevelType level)
    {
        if (GameManager.Instance.IsLevelUnlocked(level))
            OnTransitionLevel?.Invoke(level);
        else OnLevelLoadFailed?.Invoke();
    }

    public void ReqLevelUnlock(ELevelType level)
    {
        if (!GameManager.Instance.IsLevelUnlocked(level))
            OnUnlockLevel?.Invoke(level);
    }
    #endregion
}
