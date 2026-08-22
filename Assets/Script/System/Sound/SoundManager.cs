using FMOD.Studio;
using FMODUnity;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Singleton class for managing sounds.
/// Mainly controls the changes of BGM tracks.
/// </summary>
public class SoundManager : MonoSingletonBase<SoundManager>
{
    [SerializeField] private EventReference MainMusic;
    [SerializeField] private EventReference BogMusic;
    [SerializeField] private EventReference GameOverMusic;
    private EventInstance _eventInstance;
    private float _currentStateValue;
    private bool _isStarted = false;
    private EventReference _currentTrack;
    protected override void Awake()
    {
        dontDestroy = false;
        base.Awake();
    }

    private void HandleStartManagerEvent()
    {
        if(_isStarted) return;
        Debug.Log("HandleStartManagerEvent Called");
        if (MainMusic.IsNull) return;
        _eventInstance = RuntimeManager.CreateInstance(MainMusic);
        _eventInstance.start();
        _isStarted = true;
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
        _isStarted = false;
    }
    protected override void OnDestroy()
    {
        HandleStopEvent();
        base.OnDestroy();
    }
    private void OnDisable() => HandleStopEvent();

    private void PlayTrack(EventReference track)
    {
        HandleStopEvent();
        if (track.IsNull) return;
        _currentTrack = track;
        _eventInstance = RuntimeManager.CreateInstance(track);
        _eventInstance.start();
        _isStarted = true;
    }

    public bool IsTrackPlaying(EventReference track)
    {
        return _isStarted && _currentTrack.Guid == track.Guid;
    }

    public void PlayMainMusic() => PlayTrack(MainMusic);
    public void PlayBogMusic() => PlayTrack(BogMusic);
    public void PlayGameOver() => PlayTrack(GameOverMusic);
    public void RestartCurrentBGM()
    {
        _eventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        _eventInstance.release();

        HandleStartManagerEvent();
        //RuntimeManager.StudioSystem.setParameterByName("ForestLevel", _currentStateValue);
    }
    public void StopAllMusic(bool allowFadeout = true)
    {
        FMODUnity.RuntimeManager.StudioSystem.getBus("bus:/MUSIC", out var musicBus);
        musicBus.stopAllEvents(allowFadeout
            ? FMOD.Studio.STOP_MODE.ALLOWFADEOUT
            : FMOD.Studio.STOP_MODE.IMMEDIATE);
    }


    #region Volume Setting
    public void SetMasterVolume(float volume)
    {
        RuntimeManager.StudioSystem.setParameterByName("Master Volume", volume);
    }

    public void SetBgmVolume(float volume)
    {
        RuntimeManager.StudioSystem.setParameterByName("MUSIC Volume", volume);
    }

    public void SetSfxVolume(float volume)
    {
        RuntimeManager.StudioSystem.setParameterByName("SFX Volume", volume);
    }
    #endregion
}
