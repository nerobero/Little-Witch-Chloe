using UnityEngine;

/// <summary>
/// Generic class for any singletons that also share functionalities
/// of monobehavior class.
/// </summary>
public class MonoSingletonBase<T> : MonoBehaviour where T : MonoBehaviour
{

    private static T _instance = null;

    private static bool _isApplicationQuitting = false;

    [SerializeField] protected bool dontDestroy = false;

    public static T Instance
    {
        get
        {
            if (_isApplicationQuitting)
                return null;

            if (_instance == null)
            {
                // 1. look through the scene for any instance of this singleton object:
                _instance = FindFirstObjectByType<T>();
                if (_instance == null)
                {
                    // 2a. auto-create a new instance if there is no found instance
                    // loaded on the scene.
                    GameObject newInstance = new GameObject(typeof(T).Name);
                    _instance = newInstance.AddComponent<T>();

                    // 3a. setting the new instance to not be destroyed on load.
                    var mono = _instance as MonoSingletonBase<T>;
                    if (mono != null && mono.dontDestroy)
                        DontDestroyOnLoad(newInstance);
                }
                else
                {
                    // 2b. parse the found instance into a MonoSingleton<T>
                    var mono = _instance as MonoSingletonBase<T>;
                    // 3b. setting the found instance to not be destroyed on load.
                    if (mono != null && mono.dontDestroy)
                        DontDestroyOnLoad(_instance);
                }
            }
            return _instance;
        }
    }

    protected virtual void Awake()
    {
        if (_instance == null)
        {
            _instance = this as T;
            if (dontDestroy)
                DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Prevents another instance from being created while the application
    /// is in the middle of quitting process. 
    /// </summary>
    private void OnApplicationQuit()
    {
        _isApplicationQuitting = true;
    }

    protected virtual void OnDestroy()
    {
        if (_instance == this)
            _instance = null;
    }
}
