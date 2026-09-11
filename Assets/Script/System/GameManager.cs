using System;
using System.Collections.Generic;
using Data;
using NUnit.Framework.Constraints;
using System.Linq;
using Types;
using UnityEngine;
public class GameManager : MonoSingletonBase<GameManager>
{
    [Header("Anti-Fog Moss Settings")]
    [SerializeField] private StatusEffect antiPoisonFogEffect;
    private const int AntiFogMossCountPerActivation = 5;
    private int _antiFogMossBatchCount = 0;
    private ActiveStatusEffect _activeAntiPoisonFogEffect;

    // Unlocked levels during gameplay (excluding Intro and MainGame)
    private HashSet<ELevelType> unlockedLevels = new HashSet<ELevelType>();
    private ELevelType _currentLevel;
    public ELevelType CurrentLevel => _currentLevel;
    public HashSet<ELevelType> UnlockedLevels => unlockedLevels;

    // Activated spells by scroll.
    private HashSet<EAbilityType> _unlockedAbilities = new HashSet<EAbilityType>();
    public HashSet<EAbilityType> GetUnlockedAbilities => _unlockedAbilities;
    public List<EAbilityType> GetUnlockedAbilitiesList => new List<EAbilityType>(_unlockedAbilities);

    // Objectives related fields
    public event Action<ECollectable, int> OnObjectivesCollected;
    private HashSet<string> _defeatedBosses = new HashSet<string>();

    private Dictionary<ECollectable, int> _objectives = new Dictionary<ECollectable, int>();
    private int allCollectedObjectivesCounts = 0; 

    private List<string> defeatedBosses = new List<string>();

    // Love potion progress-bar related fields
    [Header("Love Potion Progress Weights")]
    [SerializeField] private float ingredientProgressWeight = 0.2f;
    [SerializeField] private float benchmarkProgressWeight = 0.5f;
    [SerializeField] private float damageProgressWeight = 0.5f;
    [SerializeField] private float potionMadeBonus = 0.3f;

    public event Action<float> OnLovePotionProgressChanged;

    private Dictionary<ECollectable, int> _loveIngredientObjectives = new Dictionary<ECollectable, int>();
    private float _benchmarkProgress = 0f; // 0..1, accumulated via OnLevelBenchmarkPassed
    private float _damageProgressPenalty = 0f; // 0..1, accumulated as a fraction of max HP lost
    private bool _lovePotionMade = false;
    private float _currentLovePotionProgress = 0f;
    private StatManager _subscribedPlayerStat;

    public float CurrentLovePotionProgress => _currentLovePotionProgress;


    protected override void Awake()
    {
        unlockedLevels.Add(ELevelType.Intro);
        unlockedLevels.Add(ELevelType.Overworld);
        dontDestroy = true;
        base.Awake();
    }

    private void OnEnable()
    {
        EventManager.Instance.OnUnlockLevel += OnUnlockLevel;
    }

    private void OnDisable()
    {
        if(EventManager.Instance != null)
        {
            EventManager.Instance.OnUnlockLevel -= OnUnlockLevel;
        }
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
        if (_objectives.ContainsKey(ECollectable.FrogCollectible))
        {
            _objectives[ECollectable.FrogCollectible]++;
        }
        else
        {
            _objectives.Add(ECollectable.FrogCollectible, 1);
        }

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

        _antiFogMossBatchCount++;

        if (_antiFogMossBatchCount >= AntiFogMossCountPerActivation)
        {
            _antiFogMossBatchCount = 0;
            ActivateAntiPoisonFogBuff();
        }

        return true;
    }

    private void ActivateAntiPoisonFogBuff()
    {
        PlayerStatManager playerStat = PlayerController.Instance?.GetComponent<PlayerStatManager>();

        if (playerStat == null) return;

        // if the buff is still active, extend the remaining time instead of stacking a new one.
        if (_activeAntiPoisonFogEffect != null && _activeAntiPoisonFogEffect.remainingTime > 0f)
        {
            _activeAntiPoisonFogEffect.ExtendDuration(antiPoisonFogEffect.Duration);
            return;
        }

        ActiveStatusEffect pooledEffect = PoolObjectManager.Instance.GetStatusEffect();
        pooledEffect.SetEffect(antiPoisonFogEffect);

        playerStat.BuffComp.Add(pooledEffect);
        _activeAntiPoisonFogEffect = pooledEffect;
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

        int targetAmount = CommisionManager.Instance.GetTargetCount(_currentLevel, type);

        if(targetAmount == _objectives[type])
        {
            allCollectedObjectivesCounts++;
            CheckAllObjectives();
        }

        return true;
    }

    public void CheckAllObjectives()
    {
        if(CommisionManager.Instance.GetObjectivesTypeAmount(_currentLevel) == allCollectedObjectivesCounts)
        {
            onLevelUnlocked(_currentLevel + 1);
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

            if(CommisionManager.Instance != null)
            {
                if(data.collectedCount == CommisionManager.Instance.GetTargetCount(_currentLevel, data.collectableType))
                {
                    allCollectedObjectivesCounts++;
                }
            }
        }
    }


    public void RestoreObjective(ECollectable type, int count)
    {
        if (_objectives.ContainsKey(type))
        {
            _objectives[type] = count;
            OnObjectivesCollected?.Invoke(type, count);
        }
    }
    #endregion

    #region LovePotionProgress

    public bool OnLoveIngredientCollected(ECollectable type)
    {
        if (_loveIngredientObjectives.ContainsKey(type))
        {
            _loveIngredientObjectives[type]++;
        }
        else
        {
            _loveIngredientObjectives.Add(type, 1);
        }

        RecalculateLovePotionProgress();
        return true;
    }

    public void SubscribeToPlayerDamage(StatManager playerStat)
    {
        if (_subscribedPlayerStat == playerStat) return;

        if (_subscribedPlayerStat != null)
            _subscribedPlayerStat.OnTakeDamage -= OnPlayerDamageTaken;

        _subscribedPlayerStat = playerStat;

        if (_subscribedPlayerStat != null)
            _subscribedPlayerStat.OnTakeDamage += OnPlayerDamageTaken;
    }

    private void OnPlayerDamageTaken(float damageAmount)
    {
        if (_subscribedPlayerStat == null || _subscribedPlayerStat.MaxHP <= 0f) return;

        _damageProgressPenalty = Mathf.Clamp01(_damageProgressPenalty + damageAmount / _subscribedPlayerStat.MaxHP);
        RecalculateLovePotionProgress();
    }

    // Stub: no caller yet. Wire this up once the level benchmark/checkpoint system exists;
    // it should pass the normalized (0..1) share of the benchmark budget that passing this
    // benchmark represents (e.g. 1 / totalBenchmarksForLevel).
    public void OnLevelBenchmarkPassed(float normalizedIncrement)
    {
        _benchmarkProgress = Mathf.Clamp01(_benchmarkProgress + normalizedIncrement);
        RecalculateLovePotionProgress();
    }

    // Stub: no caller yet. Wire this up once the love potion crafting/brewing system exists.
    public void OnLovePotionMade()
    {
        _lovePotionMade = true;
        RecalculateLovePotionProgress();
    }

    private void RecalculateLovePotionProgress()
    {
        int totalCollected = 0;
        foreach (int count in _loveIngredientObjectives.Values)
            totalCollected += count;

        int totalRequired = LovePotionManager.Instance != null
            ? LovePotionManager.Instance.GetTotalRequiredCount(_currentLevel)
            : 0;

        float ingredientRatio = totalRequired > 0 ? Mathf.Clamp01((float)totalCollected / totalRequired) : 0f;

        float progress = ingredientRatio * ingredientProgressWeight
            + _benchmarkProgress * benchmarkProgressWeight
            + (_lovePotionMade ? potionMadeBonus : 0f)
            - _damageProgressPenalty * damageProgressWeight;

        _currentLovePotionProgress = Mathf.Clamp01(progress);
        OnLovePotionProgressChanged?.Invoke(_currentLovePotionProgress);
    }

    /// <summary>
    /// Resets per-level love potion progress. Call when freshly entering a level
    /// (not when restoring one from a save).
    /// </summary>
    public void ResetLovePotionProgressForLevel()
    {
        _loveIngredientObjectives.Clear();
        _benchmarkProgress = 0f;
        _damageProgressPenalty = 0f;
        _lovePotionMade = false;
        RecalculateLovePotionProgress();
    }

    public List<SavedObjectiveData> GetLoveIngredientObjectiveData()
    {
        List<SavedObjectiveData> data = new List<SavedObjectiveData>();
        foreach (KeyValuePair<ECollectable, int> objective in _loveIngredientObjectives)
        {
            data.Add(new SavedObjectiveData(objective.Key, objective.Value));
        }

        return data;
    }

    public void LoadLoveIngredientObjectiveData(List<SavedObjectiveData> savedData)
    {
        _loveIngredientObjectives.Clear();

        if (savedData == null) return;

        foreach (SavedObjectiveData data in savedData)
        {
            _loveIngredientObjectives[data.collectableType] = data.collectedCount;
        }
    }

    public float GetLoveBenchmarkProgress() => _benchmarkProgress;
    public void LoadLoveBenchmarkProgress(float value) => _benchmarkProgress = value;

    public float GetLoveDamagePenalty() => _damageProgressPenalty;
    public void LoadLoveDamagePenalty(float value) => _damageProgressPenalty = value;

    public bool GetLovePotionMade() => _lovePotionMade;
    public void LoadLovePotionMade(bool value) => _lovePotionMade = value;

    public float GetLovePotionProgress() => _currentLovePotionProgress;

    // Restores the cached progress value directly (rather than recomputing) so the
    // UI reflects the exact saved value immediately on load.
    public void LoadLovePotionProgress(float value)
    {
        _currentLovePotionProgress = value;
        OnLovePotionProgressChanged?.Invoke(_currentLovePotionProgress);
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

    public ELevelType GetCurrentLevel()
    {
        return _currentLevel;
    }

    public void onBossDefeated(string bossName)
    {
        defeatedBosses.Add(bossName); // event dispatcher? GameManger.Instance.onBossDefeated?
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

    public void OnUnlockLevel(ELevelType type)
    {
        if (!unlockedLevels.Contains(type))
            unlockedLevels.Add(type);
    }

    public void SetCurrentLevel(ELevelType type) => _currentLevel = type;

    #endregion

    #region ScrollCollection

    /// <summary>
    /// Manage the unlock ability(current blink and flying)
    /// </summary>
    /// <param name="scrollType">the ability to unlock</param>
    /// <returns>Does ability unlocked succeed</returns>
    public bool OnScrollCollected(EAbilityType scrollType)
    {
        return _unlockedAbilities.Add(scrollType);
    }

    /// <summary>
    /// Check the unlock ability(current blink and flying)
    /// </summary>
    /// <param name="spell">the ability to find<</param>
    /// <returns>Is ability unlocked</returns>
    public bool IsSpellUnlocked(EAbilityType spell)
    {
        return _unlockedAbilities.Contains(spell);
    }

    #endregion

    public void ResetState()
    {
        _unlockedAbilities.Clear();
        _objectives.Clear();
        ResetLovePotionProgressForLevel();
    }
}
