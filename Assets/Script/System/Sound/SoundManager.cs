using FMOD.Studio;
using FMODUnity;
using UnityEngine;
public class SoundManager : MonoBehaviour
{
    public EventReference MainMusic;
    private EventInstance instance;
    private float currentState;
    void Start()
    {
        instance = RuntimeManager.CreateInstance(MainMusic);
        instance.start();
    }
    public void SetGlobalValue(float state)
    {
        //Update Param on Trigger
        currentState= state;
        RuntimeManager.StudioSystem.setParameterByName("ForestLevel", state);

    }
    public float GetGlobalValue()
    {
        return currentState;
    }
    void OnDestroy()
    {
        instance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        instance.release();
    }
}
