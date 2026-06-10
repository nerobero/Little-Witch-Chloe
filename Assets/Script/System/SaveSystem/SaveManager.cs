using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Data;
using Types;
using System.Linq;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance {get; private set;}

    [SerializeField] private PlayerController playerController;
    [SerializeField] private PlayerStatManager playerState;
    [SerializeField] private PlayerAttack playerAttack;
    [SerializeField] private PlayerMovement playerMove;
    [SerializeField] private GameManager gameManager;
    private SavePlayerData _pendingData;
    
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
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public bool LoadSaveGame()
    {
        if (!PlayerPrefs.HasKey("PlayerData")) return false;
    
        _pendingData = PlayerPrefsExt.GetObject<SavePlayerData>("PlayerData", null);
        return _pendingData != null;
        //LoadPlayerData();
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
            savePlayerData.savedPosition = playerAttack.gameObject.transform.position;
            savePlayerData.savedRotation = playerAttack.gameObject.transform.rotation;
            savePlayerData.savedScale = playerAttack.gameObject.transform.localScale;
            savePlayerData.unlockedAbility = gameManager.GetUnlockedSpell.ToList<EAbilityType>();
            savePlayerData.spellList = playerAttack.GetUnlockedSpell();

        }

        if(playerState)
        {
            savePlayerData.currentHP = playerState.CurrentHP;
            savePlayerData.currentMaxHP = playerState.MaxHP;
            savePlayerData.currentStamina = playerState.CurrStamina;
        }

        if(playerMove)
        {
            savePlayerData.isInBackground = playerMove.IsBackground;
        }

        savePlayerData.objectives = GameManager.Instance.GetObjectiveData();
        savePlayerData.currentLevel = GameManager.Instance.GetCurrentLevel();
        savePlayerData.unlockedLevels = GameManager.Instance.GetUnlockedLevel();
        savePlayerData.defeatedBosses = GameManager.Instance.GetDefeatedBosses();

        savePlayerData.currentTime = System.DateTime.Now;

        return savePlayerData;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (_pendingData == null) return;
        
        ApplyAllGameData(_pendingData);
        _pendingData = null; // initialize after applied
    }

    private void ApplyAllGameData(SavePlayerData savePlayerData)
    {
        if(playerAttack)
        {
            playerAttack.gameObject.transform.position = savePlayerData.savedPosition;
            playerAttack.gameObject.transform.rotation = savePlayerData.savedRotation;
            playerAttack.gameObject.transform.localScale = savePlayerData.savedScale;
            
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

        if(playerMove)
        {
            playerMove.ApplyAllGameData(savePlayerData.isInBackground);
        }

        if(playerState)
        {
            playerState.ApplyAllGameData(savePlayerData.currentMaxHP, savePlayerData.currentHP, savePlayerData.currentStamina);
        }

        foreach(ELevelType unlockedLevel in savePlayerData.unlockedLevels)
        {
            GameManager.Instance.onLevelUnlocked(unlockedLevel);
        }

        GameManager.Instance.LoadObjectiveData(savePlayerData.objectives);
        GameManager.Instance.SetCurrentLevel(savePlayerData.currentLevel);    
        GameManager.Instance.LoadDefeatedBosses(savePlayerData.defeatedBosses); 
    }
}
