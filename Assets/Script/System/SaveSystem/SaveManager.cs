using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Data;
using Types;
using System.Linq;

public class SaveManager : MonoSingletonBase<SaveManager>
{

    [SerializeField] private PlayerController playerController;
    [SerializeField] private PlayerStatManager playerState;
    [SerializeField] private PlayerAttack playerAttack;
    [SerializeField] private PlayerMovement playerMove;

    private SavePlayerData _pendingData;
    
    private string savePlayerPath = "savePlayerData.json";

    protected override void Awake()
    {
        dontDestroy = true;
        base.Awake();
    }

    // private void OnEnable()
    // {
    //     SceneManager.sceneLoaded += OnSceneLoaded;
    // }

    // private void OnDisable()
    // {
    //     SceneManager.sceneLoaded -= OnSceneLoaded;
    // }
    
    public void Register(PlayerController controller)
    {
        playerController = controller;
        playerState = controller.PlayerStat;
        playerAttack = controller.PlayerAttack;
        playerMove = controller.PlayerMove;
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
            Transform transform = playerAttack.gameObject.transform;
            savePlayerData.savedPosition = transform.position;
            savePlayerData.savedRotation = transform.rotation;
            savePlayerData.savedScale = transform.localScale;

            savePlayerData.unlockedAbility = GameManager.Instance.GetUnlockedAbilities.ToList<EAbilityType>(); 
            savePlayerData.spellList = playerAttack.GetUnlockedSpell();
            savePlayerData.unlockedAbility = GameManager.Instance.GetUnlockedAbilitiesList;
            savePlayerData.spellList = new List<ESpawnType>(playerAttack.GetUnlockedSpell());
            savePlayerData.objectives = new List<SavedObjectiveData>
            {
                new() { collectableType = ECollectable.FrogCollectible, collectedCount = GameManager.Instance.GetCollectedFrog() }
            };
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

    // private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    // {
    //     if (_pendingData == null) return;
        
    //     ApplyAllGameData(_pendingData);
    //     _pendingData = null; // initialize after applied
    // }

    public void ApplyAllGameData()
    {
        if (_pendingData == null) return;
        
        ApplyAllGameData(_pendingData);
        _pendingData = null; // initialize after applied
    }

    private void ApplyAllGameData(SavePlayerData savePlayerData)
    {
        if(playerAttack)
        {
            // playerAttack.gameObject.transform.position = savePlayerData.savedPosition;
            // playerAttack.gameObject.transform.rotation = savePlayerData.savedRotation;
            // playerAttack.gameObject.transform.localScale = savePlayerData.savedScale;
            
            foreach(Types.EAbilityType unlocked in savePlayerData.unlockedAbility)
            {
                GameManager.Instance.OnScrollCollected(unlocked);
            }

            foreach(Types.ESpawnType spell in savePlayerData.spellList)
            {   
                // if already collected spell, unlock
                playerAttack.UnlockSpell(spell);
            }
        }

        if(playerMove)
        {
            playerMove.ApplyAllGameData(savePlayerData.isInBackground,
                                        savePlayerData.savedPosition, savePlayerData.savedRotation, savePlayerData.savedScale);

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
