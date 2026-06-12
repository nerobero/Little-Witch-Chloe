using System.Collections.Generic;
using Types;
using Data;
using UnityEngine;

public class CommisionManager : MonoSingletonBase<CommisionManager>
{
    private Dictionary<ELevelType, List<SavedObjectiveData>> objectives;

    protected override void Awake()
    {
        base.Awake();
        ReadObjectives();
    }

    private void ReadObjectives()
    {
        objectives = new Dictionary<ELevelType, List<SavedObjectiveData>>();
        CollectableData[] records = DataTableRegistry.Get<CollectableData>().Records;

        foreach (CollectableData data in records)
        {
            if (!objectives.ContainsKey(data.levelType))
                objectives[data.levelType] = new List<SavedObjectiveData>();

            objectives[data.levelType].Add(new SavedObjectiveData(data.collectableType, data.collectedCount));
        }
    }

    /// <summary>
    /// Get the target amount for a specific type of collectible.
    /// </summary>
    /// <param name="currentLevel"></param>
    /// <param name="herbType"></param>
    /// <returns></returns>
    public int GetTargetCount(ELevelType currentLevel, ECollectable herbType)
    {
        SavedObjectiveData data = objectives[currentLevel].Find(x => x.collectableType == herbType);

        return data.collectedCount;
    }

    /// <summary>
    /// Gets the number of collectible types.
    /// </summary>
    /// <param name="currentLevel"></param>
    /// <returns></returns>
    public int GetObjectivesAmount(ELevelType currentLevel)
    {
        return objectives[currentLevel].Count;
    }
}
