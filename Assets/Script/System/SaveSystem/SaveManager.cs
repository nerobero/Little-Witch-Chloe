using System.IO;
using System.Collections.Generic;
using UnityEngine;
using Data;
using Types;

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

        // Prototypes it can be changed
        //string filePath = Application.persistentDataPath + savePlayerPath;
        //File.WriteAllText(filePath, json);

        PlayerPrefsExt.SetObject<SavePlayerData>("PlayerData", savePlayerData);
        

        Debug.Log("Save Finish"); 
    }

    private SavePlayerData CollectAllPlayerData()
    {
        SavePlayerData savePlayerData = new SavePlayerData();

        if(playerAttack)
        {
            savePlayerData.savedTransform = playerAttack.gameObject.transform;
            savePlayerData.unlockedAbility = gameManager.GetUnlockedSpell;
            savePlayerData.spellList = playerAttack.GetUnlockedSpell();
            savePlayerData.objectives[ECollectibleType.Frog] = GameManager.Instance.GetCollectedFrog();

            savePlayerData.currentTime = System.DateTime.Now;
        }

        return savePlayerData;
    }

    private void LoadPlayerData()
    {
        SavePlayerData savedPlayerData = PlayerPrefsExt.GetObject<SavePlayerData>("PlayerData", new SavePlayerData());
        ApplyAllGameData(savedPlayerData);
        
        // string filePath = Application.persistentDataPath + savePlayerPath;

        // // If there is saved file
        // if(File.Exists(filePath))
        // {
        //     Debug.Log("Load saved player data");

        //     string FromJsonFile = File.ReadAllText(filePath);
        //     SavePlayerData savePlayerData = JsonUtility.FromJson<SavePlayerData>(FromJsonFile);
            
        //     if(settingData == null)
        //     {
        //         Debug.Log("There is no saved setting");
        //     }

        //     ApplyAllGameData(savePlayerData);

        // }
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
            playerAttack.gameObject.transform.localScale = savePlayerData.savedTransform.localScale;
            
            foreach(Types.EAbilityType unlocked in savePlayerData.unlockedAbility)
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
