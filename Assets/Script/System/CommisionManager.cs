using System.Collections.Generic;
using Types;
using Data;
using UnityEngine;

public class CommisionManager : MonoSingletonBase<CommisionManager>
{
    private Dictionary<ELevelType, List<ObjectiveData>> objectives;
    // nested Dic < ELEvelType, Dic<Type, amount>> <= Hash Collision <= the bigger data amount the time 

    protected override void Awake()
    {
        base.Awake();
        ReadObjectives();
    }

    private void ReadObjectives()
    {
        objectives = new Dictionary<ELevelType, List<ObjectiveData>>();
        CollectableData[] records = DataTableRegistry.Get<CollectableData>().Records;

        foreach (CollectableData data in records)
        {
            if (!objectives.ContainsKey(data.levelType))
                objectives[data.levelType] = new List<ObjectiveData>();

            objectives[data.levelType].Add(new ObjectiveData(data.collectableType, data.collectedCount));
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
        ObjectiveData data = objectives[currentLevel].Find(x => x.collectableType == herbType);

        return data.collectedCount;
    }

    /// <summary>
    /// Gets the number of collectible types.
    /// </summary>
    /// <param name="currentLevel"></param>
    /// <returns></returns>
    public int GetObjectivesTypeAmount(ELevelType currentLevel)
    {
        return objectives[currentLevel].Count;
    }
}
