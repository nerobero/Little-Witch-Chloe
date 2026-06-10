using System.Collections.Generic;
using Types;
using Data;
using UnityEngine;

[System.Serializable]
public class CollectableData
{
    public ELevelType levelType;
    public ECollectable collectableType;
    public int collectedCount = 0;
}

public class CommisionManager : MonoSingletonBase<CommisionManager>
{
    private Dictionary<ELevelType, List<SavedObjectiveData>> objectives;
    [SerializeField] private static readonly TextAsset _commisionCSV;

    protected override void Awake()
    {
        base.Awake();
        
        ReadObjectives();
    }

    private void ReadObjectives()
    {
        List<CollectableData> objectivesLine = CSVParser.Parse<CollectableData>(_commisionCSV, cols => new CollectableData
        {
            levelType = (ELevelType)(uint.TryParse(cols[0], out uint lidx) ? lidx : 0),
            collectableType = (ECollectable)(uint.TryParse(cols[1], out uint tidx) ? tidx : 0),
            collectedCount = int.TryParse(cols[2], out int amount) ? amount : 0,
        });

        foreach(CollectableData data in objectivesLine)
        {
            if(!objectives.ContainsKey(data.levelType))
            {
                objectives[data.levelType] = new List<SavedObjectiveData>();
            }

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
