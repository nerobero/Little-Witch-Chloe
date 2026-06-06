using UnityEngine;
using Data;
using System.IO;

public class OptionManager : MonoBehaviour
{
    public static OptionManager Instance {get; private set;}
    
    [SerializeField] private SettingData settingData;
    
    private string saveSettingPath = "saveSettingData.json";

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            // maintain this instance even if the scene changed.
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        LoadSettingData();
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
        string filePath = Application.persistentDataPath + saveSettingPath;
        settingData = PlayerPrefsExt.GetObject<SettingData>("SettingData", new SettingData());
        // settingData.masterVolume = PlayerPrefsExt.GetObject<float>("masterVolume", 100.0f);
        // settingData.bgmVolume = PlayerPrefsExt.GetObject<float>("bgmVolume", 100.0f);
        // settingData.sfxVolume = PlayerPrefsExt.GetObject<float>("sfxVolume", 100.0f);

        SoundManager.Instance.SetMasterVolume(settingData.masterVolume);
        SoundManager.Instance.SetBgmVolume(settingData.bgmVolume);
        SoundManager.Instance.SetSfxVolume(settingData.sfxVolume);

        // // If there is saved file
        // if(File.Exists(filePath))
        // {
        //     Debug.Log("Load saved setting");

        //     string FromJsonFile = File.ReadAllText(filePath);
        //     settingData = JsonUtility.FromJson<SettingData>(FromJsonFile);
            
        //     if(settingData == null)
        //     {
        //         Debug.Log("There is no saved setting");
        //     }

        //     SoundManager.Instance.SetMasterVolume(settingData.masterVolume);
        //     SoundManager.Instance.SetBgmVolume(settingData.bgmVolume);
        //     SoundManager.Instance.SetSfxVolume(settingData.sfxVolume);
        // }
        // // if not
        // else
        // {
        //     ResetSettingData();
        // }
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
        // string ToJsonData = JsonUtility.ToJson(settingData);
        // string filePath = Application.persistentDataPath + saveSettingPath;

        // // overwrite the save file
        // File.WriteAllText(filePath, ToJsonData);   
        PlayerPrefsExt.SetObject<SettingData>("SettingData", settingData);

        // PlayerPrefsExt.SetObject<float>("masterVolume", settingData.masterVolume);
        // PlayerPrefsExt.SetObject<float>("bgmVolume", settingData.bgmVolume);
        // PlayerPrefsExt.SetObject<float>("sfxVolume", settingData.sfxVolume);
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
