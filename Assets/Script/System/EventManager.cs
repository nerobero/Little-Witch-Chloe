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
        OnTransitionLevel?.Invoke(level);
    }

    public void ReqLevelUnlock(ELevelType level)
    {
        OnUnlockLevel?.Invoke(level);
    }
    #endregion
}
