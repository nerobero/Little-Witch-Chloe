using UnityEngine;
using System.Collections.Generic;
using Types;

/// <summary>
/// Struct that groups the information needed for
/// any poolable objects.
/// </summary>
[System.Serializable]
public struct PoolEntry
{
    public ESpawnType type;
    public GameObject spawnPrefab;
    public int initSpawnSize; // initial spawn limit before having to increase upon demand
}

/// <summary>
/// Manages different types of poolable objects as a dictionary.
/// </summary>
public class PoolObjectManager : MonoSingletonBase<PoolObjectManager>
{
    // private static PoolObjectManager _instance;
    // public static PoolObjectManager Instance => _instance;
    [SerializeField] PoolEntry[] poolEntries; //initialized at compile time, for authoring

    private Dictionary<ESpawnType, GameObject> prefabMap = new(); // for easier runtime access
    private Dictionary<ESpawnType, Queue<GameObject>> pools = new(); // key = pool object type, value = pool

    private Dictionary<GameObject, ESpawnType> activeObjects = new(); // objects currently handed out by Get

    private Stack<ActiveStatusEffect> statusPools = new();

    private Stack<GameObject> shadowPools = new();
    [SerializeField] private GameObject shadowObj;

    protected override void Awake()
    {
        dontDestroy = true;
        base.Awake();

        // initializes the pools:
        foreach (var entry in poolEntries)
        {
            prefabMap[entry.type] = entry.spawnPrefab;
            pools[entry.type] = new Queue<GameObject>();

            for (int i = 0; i < entry.initSpawnSize; i++)
                pools[entry.type].Enqueue(CreateNew(entry.type));
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        
    }

    public GameObject Get(ESpawnType type)
    {
        var pool = pools[type];

        // pooled objects parented to a level object are destroyed with its scene, leaving nulls in the queue:
        GameObject obj = null;
        while(pool.Count > 0 && obj == null)
        {
            obj = pool.Dequeue();
        }

        if(obj == null)
        {
            obj = CreateNew(type); // when we run out of pooled objects, we increase the size of the pool by creating new
        }

        activeObjects[obj] = type;
        obj.SetActive(true);
        return obj;
    }

    /// <summary>
    /// Returns every object still handed out by Get to its pool.
    /// Call before unloading a level so in-flight objects are not left behind.
    /// </summary>
    public void ReturnAll()
    {
        // Return() modifies activeObjects, so iterate over a copy
        var snapshot = new List<KeyValuePair<GameObject, ESpawnType>>(activeObjects);
        activeObjects.Clear();

        foreach(var pair in snapshot)
        {
            GameObject obj = pair.Key;
            if(obj == null) continue; // already destroyed along with its scene

            // detach from any level object so it survives the scene unload
            if(obj.transform.parent != transform)
            {
                obj.transform.SetParent(transform, false);
            }

            Return(pair.Value, obj);
        }
    }

    public ActiveStatusEffect GetStatusEffect()
    {
        if(statusPools.Count > 0)
        {
            return statusPools.Pop();
        }

        return new ActiveStatusEffect();
    }

    public void Return(ESpawnType type, GameObject obj)
    {
        if(!pools.ContainsKey(type))
        {
            pools[type] = new Queue<GameObject>();
        }

        activeObjects.Remove(obj);
        obj.SetActive(false);
        pools[type].Enqueue(obj);
    }

    public void ReturnStatusEffect(ActiveStatusEffect effect)
    {
        effect.Reset();

        statusPools.Push(effect);
    }

    GameObject CreateNew(ESpawnType type)
    {
        var obj = Instantiate(prefabMap[type]);
        obj.SetActive(false);

        var behaviour = obj.GetComponent<MonoBehaviour>();

        LevelManager.Instance.RegisterInstance(behaviour);

        return obj;
    }

    public GameObject GetShadowObject()
    {
        while(shadowPools.Count > 0)
        {
            var pooled = shadowPools.Pop();

            // a pooled shadow stays parented to its last owner, so it is destroyed if that owner was destroyed
            if(pooled != null)
            {
                return pooled;
            }
        }

        var obj = Instantiate(shadowObj);
        obj.SetActive(false);

        return obj;
    }

    public void ReturnShadowObject(GameObject shadowObj)
    {
        // Don't reparent here: this is called from OnDisable, and Unity forbids
        // changing the parent while it is being deactivated. The next owner
        // reparents the shadow in its OnEnable.
        shadowObj.SetActive(false);
        shadowPools.Push(shadowObj);
    }
}
