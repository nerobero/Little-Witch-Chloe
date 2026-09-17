using System;
using System.Collections.Generic;
using Types;
using Data;
using UnityEngine;

public class LovePotionManager : MonoSingletonBase<LovePotionManager>
{
    private Dictionary<ELevelType, List<ObjectiveData>> objectives;

    [Header("Love Potion Progress Weights")]
    [SerializeField] private float ingredientProgressWeight = 0.25f;
    [SerializeField] private float benchmarkProgressWeight = 0.25f;
    [SerializeField] private float damageProgressWeight = 0.1f;
    [SerializeField] private float potionMadeBonus = 0.5f;

    public event Action<float> OnLovePotionProgressChanged;

    private Dictionary<ECollectable, int> _loveIngredientObjectives = new Dictionary<ECollectable, int>();
    private float _benchmarkProgress = 0f; // 0..1, accumulated via OnLevelBenchmarkPassed
    private float _damageProgressPenalty = 0f; // cached mirror of GameManager.DamageProgressPenalty
    private bool _lovePotionMade = false;
    private float _currentLovePotionProgress = 0f;

    public float CurrentLovePotionProgress => _currentLovePotionProgress;

    protected override void Awake()
    {
        dontDestroy = true;
        base.Awake();
        ReadObjectives();
    }

    private void OnEnable()
    {
        if (GameManager.Instance == null) return;

        GameManager.Instance.OnDamageProgressPenaltyChanged += HandleDamageProgressPenaltyChanged;
        _damageProgressPenalty = GameManager.Instance.DamageProgressPenalty;
    }

    private void OnDisable()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnDamageProgressPenaltyChanged -= HandleDamageProgressPenaltyChanged;
    }

    private void HandleDamageProgressPenaltyChanged(float value)
    {
        _damageProgressPenalty = value;
        RecalculateLovePotionProgress();
    }

    private void ReadObjectives()
    {
        objectives = new Dictionary<ELevelType, List<ObjectiveData>>();
        LovePotionIngredientData[] records = DataTableRegistry.Get<LovePotionIngredientData>().Records;

        foreach (LovePotionIngredientData data in records)
        {
            if (!objectives.ContainsKey(data.levelType))
                objectives[data.levelType] = new List<ObjectiveData>();

            objectives[data.levelType].Add(new ObjectiveData(data.collectableType, data.requiredCount));
        }
    }

    /// <summary>
    /// Get the target amount for a specific ingredient type.
    /// </summary>
    /// <param name="currentLevel"></param>
    /// <param name="ingredientType"></param>
    /// <returns></returns>
    public int GetTargetCount(ELevelType currentLevel, ECollectable ingredientType)
    {
        ObjectiveData data = objectives[currentLevel].Find(x => x.collectableType == ingredientType);

        return data.collectedCount;
    }

    /// <summary>
    /// Gets the number of ingredient types required for a level.
    /// </summary>
    /// <param name="currentLevel"></param>
    /// <returns></returns>
    public int GetObjectivesTypeAmount(ELevelType currentLevel)
    {
        return objectives[currentLevel].Count;
    }

    /// <summary>
    /// Gets the total required ingredient amount across all types for a level,
    /// used to compute the ingredient-collection share of the level's progress bar.
    /// </summary>
    /// <param name="currentLevel"></param>
    /// <returns></returns>
    public int GetTotalRequiredCount(ELevelType currentLevel)
    {
        if (!objectives.ContainsKey(currentLevel))
            return 0;

        int total = 0;
        foreach (ObjectiveData data in objectives[currentLevel])
            total += data.collectedCount;

        return total;
    }

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

    // Stub: no caller yet. Wire this up once the level benchmark/checkpoint system exists;
    // it should pass the normalized (0..1) share of the benchmark budget that passing this
    // benchmark represents (e.g. 1 / totalBenchmarksForLevel).
    public void OnLevelBenchmarkPassed(float normalizedIncrement)
    {
        _benchmarkProgress = Mathf.Clamp01(_benchmarkProgress + normalizedIncrement);
        RecalculateLovePotionProgress();
    }

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

        int totalRequired = GameManager.Instance != null
            ? GetTotalRequiredCount(GameManager.Instance.CurrentLevel)
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
        _lovePotionMade = false;
        GameManager.Instance?.ResetDamageProgressPenalty();
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
}
