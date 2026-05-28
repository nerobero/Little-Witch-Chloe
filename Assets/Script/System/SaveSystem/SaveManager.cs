using System.IO;
using System.Collections.Generic;
using UnityEngine;
using Data;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance {get; private set;}

    [SerializeField] private PlayerController playerController;
    [SerializeField] private PlayerStatManager playerState;
    [SerializeField] private PlayerAttack playerAttack;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private SettingData settingData;
    
    private string savePlayerPath = "savePlayerData.json";

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
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadSaveGame()
    {
        LoadPlayerData();
    }

    public virtual void SavePlayerData()
    {
        Debug.Log("Start Save");

        SavePlayerData savePlayerData = CollectAllPlayerData();

        string json = JsonUtility.ToJson(savePlayerData);
        string filePath = Application.persistentDataPath + savePlayerPath;

        File.WriteAllText(filePath, json);

        Debug.Log("Save Finish");
    }

    private SavePlayerData CollectAllPlayerData()
    {
        SavePlayerData savePlayerData = new SavePlayerData();

        if(playerAttack)
        {
            savePlayerData.savedTransform = playerAttack.gameObject.transform;
            savePlayerData.unlockedSpell = gameManager.GetUnlockedSpell;
            savePlayerData.spellList = playerAttack.GetUnlockedSpell();

            savePlayerData.currentTime = System.DateTime.Now;
        }

        return savePlayerData;
    }

    private void LoadPlayerData()
    {
        string filePath = Application.persistentDataPath + savePlayerPath;

        // If there is saved file
        if(File.Exists(filePath))
        {
            Debug.Log("Load saved player data");

            string FromJsonFile = File.ReadAllText(filePath);
            SavePlayerData savePlayerData = JsonUtility.FromJson<SavePlayerData>(FromJsonFile);
            
            if(settingData == null)
            {
                Debug.Log("There is no saved setting");
            }

            ApplyAllGameData(savePlayerData);

        }
        // // if not
        // else
        // {
        //     ResetPlayerData();
        // }
    }

    private void ApplyAllGameData(SavePlayerData savePlayerData)
    {
        if(playerAttack)
        {
            playerAttack.gameObject.transform.position = savePlayerData.savedTransform.position;
            playerAttack.gameObject.transform.rotation = savePlayerData.savedTransform.rotation;
            
            foreach(Types.EAbilityType unlocked in savePlayerData.unlockedSpell)
            {
                gameManager.OnScrollCollected(unlocked);
            }

            foreach(Types.ESpawnType spell in savePlayerData.spellList)
            {   
                // if already collected spell, unlock
                playerAttack.UnlockSpell(spell);
            }
        }
    }
}
