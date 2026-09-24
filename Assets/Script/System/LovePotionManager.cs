using System;
using System.Collections.Generic;
using Types;
using Data;
using UnityEngine;

/// <summary>
/// Owns the love potion's own recipe state: ingredients collected so far and whether
/// the potion has been crafted. Cross-cutting inputs to the overall progress bar
/// (level benchmarks, player damage taken) and the combined total live in
/// <see cref="GameManager"/> instead, since they don't have a more specific owner.
/// </summary>
public class LovePotionManager : MonoSingletonBase<LovePotionManager>
{
    private Dictionary<ELevelType, List<ObjectiveData>> _levelObjectives;

    private Dictionary<ECollectable, int> _collectedIngredients = new Dictionary<ECollectable, int>();
    private bool _lovePotionMade = false;

    /// <summary>Fired whenever ingredient collection or the crafted flag changes, so GameManager can recompute the total.</summary>
    public event Action OnLovePotionStateChanged;

    protected override void Awake()
    {
        dontDestroy = true;
        base.Awake();
        ReadObjectives();
    }

    private void ReadObjectives()
    {
        _levelObjectives = new Dictionary<ELevelType, List<ObjectiveData>>();
        LovePotionIngredientData[] records = DataTableRegistry.Get<LovePotionIngredientData>().Records;

        foreach (LovePotionIngredientData data in records)
        {
            if (!_levelObjectives.ContainsKey(data.levelType))
                _levelObjectives[data.levelType] = new List<ObjectiveData>();

            _levelObjectives[data.levelType].Add(new ObjectiveData(data.collectableType, data.requiredCount));
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
        ObjectiveData data = _levelObjectives[currentLevel].Find(x => x.collectableType == ingredientType);

        return data.collectedCount;
    }

    /// <summary>
    /// Gets the number of ingredient types required for a level.
    /// </summary>
    /// <param name="currentLevel"></param>
    /// <returns></returns>
    public int GetObjectivesTypeAmount(ELevelType currentLevel)
    {
        return _levelObjectives[currentLevel].Count;
    }

    /// <summary>
    /// Gets the ingredient types required for a level, e.g. for the crafting QTE to pair
    /// each direction slot with the ingredient icon it represents.
    /// </summary>
    /// <param name="currentLevel"></param>
    /// <returns></returns>
    public List<ECollectable> GetObjectiveIngredients(ELevelType currentLevel)
    {
        if (!_levelObjectives.ContainsKey(currentLevel))
            return new List<ECollectable>();

        List<ECollectable> ingredients = new List<ECollectable>();
        foreach (ObjectiveData data in _levelObjectives[currentLevel])
            ingredients.Add(data.collectableType);

        return ingredients;
    }

    /// <summary>
    /// Gets the total required ingredient amount across all types for a level,
    /// used to compute the ingredient-collection share of the level's progress bar.
    /// </summary>
    /// <param name="currentLevel"></param>
    /// <returns></returns>
    public int GetTotalRequiredCount(ELevelType currentLevel)
    {
        if (!_levelObjectives.ContainsKey(currentLevel))
            return 0;

        int total = 0;
        foreach (ObjectiveData data in _levelObjectives[currentLevel])
            total += data.collectedCount;

        return total;
    }

    #region LovePotionState

    public bool OnLoveIngredientCollected(ECollectable type)
    {
        if (_collectedIngredients.ContainsKey(type))
        {
            _collectedIngredients[type]++;
        }
        else
        {
            _collectedIngredients.Add(type, 1);
        }

        OnLovePotionStateChanged?.Invoke();
        return true;
    }

    public void OnLovePotionMade()
    {
        _lovePotionMade = true;
        OnLovePotionStateChanged?.Invoke();
    }

    /// <summary>
    /// Checks whether enough of every required ingredient has been collected for the given level.
    /// </summary>
    /// <param name="currentLevel"></param>
    /// <returns></returns>
    public bool HasEnoughIngredients(ELevelType currentLevel)
    {
        if (!_levelObjectives.ContainsKey(currentLevel))
            return true;

        foreach (ObjectiveData data in _levelObjectives[currentLevel])
        {
            int collected = _collectedIngredients.TryGetValue(data.collectableType, out int count) ? count : 0;

            if (collected < data.collectedCount)
                return false;
        }

        return true;
    }

    /// <summary>
    /// Deducts the required amount of every ingredient for the given level. Call only after
    /// <see cref="HasEnoughIngredients"/> has confirmed there's enough to consume.
    /// </summary>
    /// <param name="currentLevel"></param>
    public void ConsumeIngredients(ELevelType currentLevel)
    {
        if (!_levelObjectives.ContainsKey(currentLevel))
            return;

        foreach (ObjectiveData data in _levelObjectives[currentLevel])
        {
            if (_collectedIngredients.TryGetValue(data.collectableType, out int count))
                _collectedIngredients[data.collectableType] = Mathf.Max(0, count - data.collectedCount);
        }

        OnLovePotionStateChanged?.Invoke();
    }

    /// <summary>
    /// Ratio (0..1) of ingredients collected so far against the level's total requirement.
    /// </summary>
    /// <param name="currentLevel"></param>
    /// <returns></returns>
    public float GetIngredientProgressRatio(ELevelType currentLevel)
    {
        int totalCollected = 0;
        foreach (int count in _collectedIngredients.Values)
            totalCollected += count;

        int totalRequired = GetTotalRequiredCount(currentLevel);

        return totalRequired > 0 ? Mathf.Clamp01((float)totalCollected / totalRequired) : 0f;
    }

    /// <summary>
    /// Resets per-level ingredient collection and the crafted flag. Call when freshly
    /// entering a level (not when restoring one from a save).
    /// </summary>
    public void ResetIngredientState()
    {
        _collectedIngredients.Clear();
        _lovePotionMade = false;
        OnLovePotionStateChanged?.Invoke();
    }

    public List<SavedObjectiveData> GetLoveIngredientObjectiveData()
    {
        List<SavedObjectiveData> data = new List<SavedObjectiveData>();
        foreach (KeyValuePair<ECollectable, int> objective in _collectedIngredients)
        {
            data.Add(new SavedObjectiveData(objective.Key, objective.Value));
        }

        return data;
    }

    public void LoadLoveIngredientObjectiveData(List<SavedObjectiveData> savedData)
    {
        _collectedIngredients.Clear();

        if (savedData != null)
        {
            foreach (SavedObjectiveData data in savedData)
            {
                _collectedIngredients[data.collectableType] = data.collectedCount;
            }
        }

        OnLovePotionStateChanged?.Invoke();
    }

    public bool GetLovePotionMade() => _lovePotionMade;

    public void LoadLovePotionMade(bool value)
    {
        _lovePotionMade = value;
        OnLovePotionStateChanged?.Invoke();
    }

    #endregion
}
