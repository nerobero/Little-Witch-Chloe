using UnityEngine;
using Data;
using FMODUnity;

public class OptionManager : MonoSingletonBase<OptionManager>
{
    [SerializeField] private SettingData settingData;

    protected override void Awake()
    {
        dontDestroy = true;
        base.Awake();

        if(PlayerPrefsExt.HasKey("SettingData"))
        {
            LoadSettingData();
            return;
        }

        float masterVolume, musicVolume, sfxVolume;
        RuntimeManager.StudioSystem.getParameterByName("Master Volume", out masterVolume);
        RuntimeManager.StudioSystem.getParameterByName("MUSIC Volume", out musicVolume);
        RuntimeManager.StudioSystem.getParameterByName("SFX Volume", out sfxVolume);

        SaveSettingData();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void LoadSettingData()
    {
        settingData = PlayerPrefsExt.GetObject<SettingData>("SettingData", new SettingData());

        SoundManager.Instance.SetMasterVolume(settingData.masterVolume);
        SoundManager.Instance.SetBgmVolume(settingData.bgmVolume);
        SoundManager.Instance.SetSfxVolume(settingData.sfxVolume);
    }

    public void ResetSettingData()
    {
        Debug.Log("Create new setting save file");

        settingData = null;
        settingData = new SettingData();

        settingData.language = Application.systemLanguage;
        
        SaveSettingData();
    }

    public void SaveSettingData()
    { 
        PlayerPrefsExt.SetObject<SettingData>("SettingData", settingData);
    }

    public void SetSettingData(SettingData newData)
    {
        settingData = newData;
    }

    public SettingData GetSettingData()
    {
        return settingData;
    }
}
