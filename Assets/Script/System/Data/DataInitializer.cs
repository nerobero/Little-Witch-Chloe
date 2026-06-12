using Data;
using UnityEngine;

/// <summary>
/// Registers all binary data tables before any scene loads.
/// When adding a new CSV data type, add a Register call here.
/// </summary>
public static class DataInitializer
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Initialize()
    {
        DataTableRegistry.Register<DialogueRow>("DialogueLinesData", DialogueRow.Deserialize);
        DataTableRegistry.Register<CollectableData>("CommissionData", CollectableData.Deserialize);
    }
}
