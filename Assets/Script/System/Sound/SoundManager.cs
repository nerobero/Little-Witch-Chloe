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
    [SerializeField] private EventReference Overworld;
    private EventInstance _eventInstance;
    private float _currentStateValue;
    private bool _isStarted = false;
    private EventReference _currentTrack;

    // This manager lives in the level scene and is recreated on a level restart,
    // so what the BGM was doing has to be kept outside the instance.
    private static float s_lastStateValue = 0f;
    private static bool s_lastTrackWasMain = false;

    /// <summary>
    /// Forgets the BGM progress kept for level restarts. Call when starting a new session from the title.
    /// </summary>
    public static void ResetPersistentState()
    {
        s_lastStateValue = 0f;
        s_lastTrackWasMain = false;
    }

    // The restart handling below only applies to playable levels, not the Title or Main Game scenes.
    private bool IsPlayableLevelScene
    {
        get
        {
            string sceneName = gameObject.scene.name;
            return sceneName != "TitleScene" && sceneName != "MainScene";
        }
    }

    protected override void Awake()
    {
        dontDestroy = false;
        base.Awake();

        // FMOD global parameters outlive this object, so re-apply the last phase instead of leaving a stale value
        if (IsPlayableLevelScene)
        {
            _currentStateValue = s_lastStateValue;
            RuntimeManager.StudioSystem.setParameterByName("ForestLevel", s_lastStateValue);
        }

        RuntimeManager.StudioSystem.setParameterByName("Master Volume", 0.5f);
        RuntimeManager.StudioSystem.setParameterByName("MUSIC Volume", 0.5f);
        RuntimeManager.StudioSystem.setParameterByName("SFX Volume", 0.5f);
    }

    private void HandleStartManagerEvent()
    {
        if (_isStarted) return;
        // after a level restart, resume the main music if it was playing before
        EventReference startTrack = (IsPlayableLevelScene && s_lastTrackWasMain && !MainMusic.IsNull) ? MainMusic : Overworld;
        if (startTrack.IsNull) return;
        Debug.Log("HandleStartManagerEvent Called");
        _currentTrack = startTrack;
        _eventInstance = RuntimeManager.CreateInstance(startTrack);
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
        if (IsPlayableLevelScene) s_lastStateValue = state;
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
        if (IsPlayableLevelScene && track.Guid != GameOverMusic.Guid) // the game over jingle must not decide what resumes after a restart
            s_lastTrackWasMain = track.Guid == MainMusic.Guid;
        _currentTrack = track;
        _eventInstance = RuntimeManager.CreateInstance(track);
        _eventInstance.start();
        _isStarted = true;
    }

    public bool IsTrackPlaying(EventReference track)
    {
        return _isStarted && _currentTrack.Guid == track.Guid;
    }

    /// <summary>
    /// Starts the main music unless it, or other special music such as a boss track, is already playing.
    /// Used by the BGM phase triggers, which can be reached without ever passing the Ambience trigger (e.g. after a restart).
    /// </summary>
    public void EnsureMainMusic()
    {
        if (!_isStarted || IsTrackPlaying(Overworld))
            PlayMainMusic();
    }

    public void PlayMainMusic() => PlayTrack(MainMusic);
    public void PlayBogMusic() => PlayTrack(BogMusic);
    public void PlayGameOver() => PlayTrack(GameOverMusic);
    public void PlayOverworld() => PlayTrack(Overworld);
    public bool PlayMusic(EventReference track)
    {
        if (track.IsNull) return false;
        _currentTrack = track;
        _eventInstance = RuntimeManager.CreateInstance(track);
        _eventInstance.start();
        _isStarted = true;
        return _isStarted;
    }

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
        _isStarted = false;
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
