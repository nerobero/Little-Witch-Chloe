using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Defines a type of pool object that can be spawned
/// by the PC or NPC.
/// </summary>
public enum SpawnType
{
    // PC normal projectiles (excluding the charged attacks).
    // NPCs can also spawn these projectiles.
    FireBall = 0,
    WaterBall,
    ElectricBall,
    PoisonBall,
    LightBall,
    // NPC projectiles
    FireSlash,
    FirePillar,
    PlantBall,
    PoisonPool,
    PoisonSplash,
    DarknessBall,
    DarknessCloud,
    IceShard,
    IceBall,
    IceGroundShards,
}

/// <summary>
/// Struct that groups the information needed for
/// any poolable objects.
/// </summary>
[System.Serializable]
public struct PoolEntry
{
    public SpawnType type;
    public GameObject spawnPrefab;
    public int initSpawnSize; // initial spawn limit before having to increase upon demand
}

/// <summary>
/// Manages different types of poolable objects as a dictionary.
/// </summary>
public class PoolObjectManager : MonoBehaviour
{
    private static PoolObjectManager _instance;
    public static PoolObjectManager Instance => _instance;
    [SerializeField] PoolEntry[] poolEntries; //initialized at compile time, for authoring

    private Dictionary<SpawnType, GameObject> prefabMap = new(); // for easier runtime access
    private Dictionary<SpawnType, Queue<GameObject>> pools = new(); // key = pool object type, value = pool

    private void Awake()
    {
        _instance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        // initializes the pools:
        foreach (var entry in poolEntries)
        {
            prefabMap[entry.type] = entry.spawnPrefab;
            pools[entry.type] = new Queue<GameObject>();

            for (int i = 0; i < entry.initSpawnSize; i++)
                pools[entry.type].Enqueue(CreateNew(entry.type));
        }
    }

    public GameObject Get(SpawnType type)
    {
        var pool = pools[type];
        var obj = pool.Count > 0 ? pool.Dequeue() : CreateNew(type); // when we run out of pooled objects, we increase the size of the pool by creating new
        obj.SetActive(true);
        return obj;
    }

    public void Return(SpawnType type, GameObject obj)
    {
        obj.SetActive(false);
        pools[type].Enqueue(obj);
    }

    GameObject CreateNew(SpawnType type)
    {
        var obj = Instantiate(prefabMap[type]);
        obj.SetActive(false);
        return obj;
    }
}
