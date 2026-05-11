using FMOD.Studio;
using FMODUnity;
using UnityEngine;

/// <summary>
/// Singleton class for managing sounds.
/// Mainly controls the changes of BGM tracks.
/// </summary>
public class SoundManager : MonoBehaviour
{
    private static SoundManager _instance;
    public static SoundManager Instance => _instance;

    [SerializeField] private EventReference MainMusic;
    private EventInstance _eventInstance;
    private float _currentStateValue;

    private void Awake()
    {
        if (_instance != this && _instance != null)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        
    }
    private void HandleStartManagerEvent()
    {
        if (MainMusic.IsNull) return;
        _eventInstance = RuntimeManager.CreateInstance(MainMusic);
        _eventInstance.start();
    }

    private void Start() => HandleStartManagerEvent();
    private void OnEnable() => HandleStartManagerEvent();

    /// <summary>
    /// Update Param on Trigger
    /// </summary>
    /// <param name="state"></param>
    public void SetGlobalValue(float state)
    {
        _currentStateValue = state;
        RuntimeManager.StudioSystem.setParameterByName("ForestLevel", state);
    }

    /// <summary>
    /// Updates the parameter value upon trigger
    /// </summary>
    /// <param name="paramName">the name of the parameter to change</param>
    /// <param name="stateParam">the state value to set the param to</param>
    public void SetGlobalValue(string paramName, float stateParam)
    {
        if (string.IsNullOrEmpty(paramName))
        {
            Debug.LogError("The FMOD parameter name is null or empty!");
            return;
        }

        _currentStateValue = stateParam;
        RuntimeManager.StudioSystem.setParameterByName(paramName, stateParam);
    }

    /// <summary>
    /// Gets the current state value.
    /// </summary>
    /// <returns>the current state value in float</returns>
    public float GetGlobalValue()
    {
        return _currentStateValue;
    }

    private void HandleStopEvent()
    {
        _eventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        _eventInstance.release();
    }
    private void OnDestroy() => HandleStopEvent();
    private void OnDisable() => HandleStopEvent();
}
