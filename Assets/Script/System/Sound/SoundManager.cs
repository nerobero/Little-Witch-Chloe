using FMOD.Studio;
using FMODUnity;
using UnityEngine;

/// <summary>
/// Singleton class for managing sounds.
/// Mainly controls the changes of BGM tracks.
/// </summary>
public class SoundManager : MonoSingletonBase<SoundManager>
{
    [SerializeField] private EventReference MainMusic;
    private EventInstance _eventInstance;
    private float _currentStateValue;

    private Bus _masterBus;
    private Bus _bgmBus;
    private Bus _sfxBus;

    private const string MASTER_KEY = "Volume_Master";
    private const string BGM_KEY = "Volume_BGM";
    private const string SFX_KEY = "Volume_SFX";

    protected override void Awake()
    {
        dontDestroy = true;
        base.Awake();

        _masterBus = RuntimeManager.GetBus("bus:/");
        _bgmBus = RuntimeManager.GetBus("bus:/BGM"); 
        _sfxBus = RuntimeManager.GetBus("bus:/SFX"); 
        
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
    protected override void OnDestroy()
    {
        HandleStopEvent();
        base.OnDestroy();
    }
    private void OnDisable() => HandleStopEvent();

    #region Volume Setting
    public void SetMasterVolume(float volume)
    {
        volume = Mathf.Clamp01(volume);
        _masterBus.setVolume(volume);
        PlayerPrefs.SetFloat(MASTER_KEY, volume); // 기기에 볼륨 저장
    }

    public void SetBgmVolume(float volume)
    {
        volume = Mathf.Clamp01(volume);
        _bgmBus.setVolume(volume);
        PlayerPrefs.SetFloat(BGM_KEY, volume);
    }

    public void SetSfxVolume(float volume)
    {
        volume = Mathf.Clamp01(volume);
        _sfxBus.setVolume(volume);
        PlayerPrefs.SetFloat(SFX_KEY, volume);
    }
    #endregion
}
