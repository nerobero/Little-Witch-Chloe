using System.Collections.Generic;
using Types;
using Data;
using UnityEngine;

public class LovePotionManager : MonoSingletonBase<LovePotionManager>
{
    private Dictionary<ELevelType, List<ObjectiveData>> objectives;

    protected override void Awake()
    {
        base.Awake();
        ReadObjectives();
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
}
