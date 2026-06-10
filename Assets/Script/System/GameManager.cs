using System;
using System.Collections.Generic;
using Data;
using NUnit.Framework.Constraints;
using Types;
using UnityEngine;
public class GameManager : MonoSingletonBase<GameManager>
{

    // Unlocked levels during gameplay (excluding Intro and MainGame)
    private HashSet<ELevelType> unlockedLevels = new HashSet<ELevelType>();
    private ELevelType currentLevel;

    // Activated spells by scroll. (flying, blink)
    private HashSet<EAbilityType> unlockedSpell = new HashSet<EAbilityType>();
    public HashSet<EAbilityType> GetUnlockedSpell => unlockedSpell;

    public event Action<ECollectable, int> OnObjectivesCollected;

    private Dictionary<ECollectable, int> _objectives = new Dictionary<ECollectable, int>();
    private int allCollectedObjectivesCounts = 0;

    private List<string> defeatedBosses = new List<string>();

    protected override void Awake()
    {
        dontDestroy = true;
        base.Awake();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //SaveManager.Instance.LoadSaveGame();
        foreach (KeyValuePair<ECollectable, int> collection in _objectives)
        {
            OnObjectivesCollected?.Invoke(collection.Key, collection.Value);
        }
    }

    // #region Save&Load
    // public void ApplyAllGameData()
    // {
        
    // }
    // #endregion

    #region CollectableCounter
    public void OnFrogCollected()
    {
        if(_objectives.ContainsKey(ECollectable.FrogCollectible))
        {
            _objectives[ECollectable.FrogCollectible]++;
        }
        else
        {
            _objectives.Add(ECollectable.FrogCollectible, 1);
        }

        FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/Frog");
        OnObjectivesCollected?.Invoke(ECollectable.FrogCollectible, _objectives[ECollectable.FrogCollectible]);
    }

    public int GetCollectedFrog()
    {
        if (!_objectives.ContainsKey(ECollectable.FrogCollectible))
            return 0;
        return _objectives[ECollectable.FrogCollectible];
    }

    public bool OnAntiFogMossCollected()
    {
        if (_objectives.ContainsKey(ECollectable.AntiFogMossPatch))
            _objectives[ECollectable.AntiFogMossPatch]++;
        else _objectives.Add(ECollectable.AntiFogMossPatch, 1);
        
        return true;
    }

    public bool OnCommHerbCollected(ECollectable type)
    {
        if (type == ECollectable.FrogCollectible
            || type == ECollectable.AntiFogMossPatch)
            return false;

        if (_objectives.ContainsKey(type))
        {
            _objectives[type]++;
        }
        else
        {
            _objectives.Add(type, 1);
        }

        int targetAmount = CommisionManager.Instance.GetTargetCount(currentLevel, type);

        if(targetAmount == _objectives[type])
        {
            allCollectedObjectivesCounts++;
            CheckAllObjectives();
        }

        return true;
    }

    public void CheckAllObjectives()
    {
        if(CommisionManager.Instance.GetObjectivesAmount(currentLevel) == allCollectedObjectivesCounts)
        {
            onLevelUnlocked(currentLevel + 1);
        }
    }

    public List<SavedObjectiveData> GetObjectiveData()
    {
        List<SavedObjectiveData> data = new List<SavedObjectiveData>();
        foreach(KeyValuePair<ECollectable, int> objective in _objectives)
        {
            data.Add(new SavedObjectiveData(objective.Key, objective.Value));
        }

        return data;
    }

    public void LoadObjectiveData(List<SavedObjectiveData> savedData)
    {
        foreach(SavedObjectiveData data in savedData)
        {
            _objectives.Add(data.collectableType, data.collectedCount);

            if(data.collectedCount == CommisionManager.Instance.GetTargetCount(currentLevel, data.collectableType))
            {
                allCollectedObjectivesCounts++;
            }
        }
    }


    #endregion

    #region LevelLoad

    public bool IsLevelUnlocked(ELevelType level)
    {
        return unlockedLevels.Contains(level);
    }

    public bool onLevelUnlocked(ELevelType level)
    {
        if(level == ELevelType.Count)
        {
            // Game END
            Debug.Log("Game END");
            return false;
        }

        return unlockedLevels.Add(level);
    }

    public List<ELevelType> GetUnlockedLevel()
    {
        List<ELevelType> levelList = new List<ELevelType>();

        foreach(ELevelType level in unlockedLevels)
        {
            levelList.Add(level);
        }

        return levelList;
    }

    public void SetCurrentLevel(ELevelType level)
    {
        currentLevel = level;
    }

    public ELevelType GetCurrentLevel()
    {
        return currentLevel;
    }

    public void onBossDefeated(string bossName)
    {
        defeatedBosses.Add(bossName);
    }

    public List<string> GetDefeatedBosses()
    {
        return defeatedBosses;
    }

    public void LoadDefeatedBosses(List<string> bosses)
    {
        defeatedBosses = new List<string>(bosses);
    }

    // @TODO: implement a function that actually processes the unlocking (i.e., adding a new level) to the hash set:

    #endregion

    #region ScrollCollection

    /// <summary>
    /// Manage the unlock ability(current blink and flying)
    /// </summary>
    /// <param name="scrollType">the ability to unlock</param>
    /// <returns>Does ability unlocked succeed</returns>
    public bool OnScrollCollected(EAbilityType scrollType)
    {
        return unlockedSpell.Add(scrollType);
    }

    /// <summary>
    /// Check the unlock ability(current blink and flying)
    /// </summary>
    /// <param name="spell">the ability to find<</param>
    /// <returns>Is ability unlocked</returns>
    public bool IsSpellUnlocked(EAbilityType spell)
    {
        return unlockedSpell.Contains(spell);
    }

    #endregion
}
