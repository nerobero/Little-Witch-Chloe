using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///
/// </summary>
public class SummonAttackStrategy : IEnemyAttackStrategy, IDisposable
{
    private Queue<GameObject> _summonPool = new();
    private float _timeInterval = 1.5f;
    public event Action<bool> OnAttackComplete;

    // the root position, may add arbitrary alpha values to randomize the position
    private Vector3 _summonPosition;

    private int _poolSize => _summonPool.Count;

    // owner used to run the summon coroutine, since this strategy is a plain C# class
    private MonoBehaviour _owner;
    private Coroutine _summonRoutine;

    // cached from the prefab, since every pooled instance shares the same damage number
    private ISummonable _summonable;

    // summoned instances from this attack that are still out in the world;
    // forced back to the pool once the attack completes
    private readonly List<GameObject> _activeSummons = new();

    public SummonAttackStrategy(MonoBehaviour owner, GameObject summonObj, float timeInterval, int poolSize, Vector3 position)
    {
        if (poolSize <= 0)
        {
            Debug.LogError("[SummonAttackStrategy] Cannot initialize the summon pool with size of 0");
            return;
        }
        if (summonObj == null)
        {
            Debug.LogError("[SummonAttackStrategy] Cannot initialize the summon pool with null object");
            return;
        }

        _owner = owner;
        _timeInterval = timeInterval;

        _summonable = summonObj.GetComponent<ISummonable>();

        // initializing the pool:
        for (int i = 0; i < poolSize; i++)
        {
            var obj = GameObject.Instantiate(summonObj);
            obj.SetActive(false);
            _summonPool.Enqueue(obj);
        }

        _summonPosition = position;

    }
    public bool Attack(GameObject instigator, GameObject target, bool useLastTarget = false)
    {
        if (_poolSize <= 0) return false;
        if (_summonRoutine != null) return false; // already summoning

        _summonRoutine = _owner.StartCoroutine(SummonRoutine());

        return true;
    }

    private IEnumerator SummonRoutine()
    {
        int snapShot = _poolSize;

        for (int i = 0; i < snapShot; i++)
        {
            Summon();
            yield return new WaitForSeconds(_timeInterval);
        }

        _summonRoutine = null;
        AttackFinished();
    }

    private void Summon()
    {
        var obj = Get();
        obj.transform.position = _summonPosition;
        var summonable = obj.GetComponent<ISummonable>();
        summonable?.SetInstigator(_owner.gameObject);
        summonable?.OnSummoned();
        _activeSummons.Add(obj);
    }
    private GameObject Get()
    {
        if (_poolSize > 0)
        {
            var obj = _summonPool.Dequeue();
            obj.SetActive(true);
            return obj;
        }
        return null;
    }

    public bool AttackFinished()
    {
        // force any summons still active from this attack back into the pool
        foreach (var obj in _activeSummons)
        {
            if (obj != null)
            {
                obj.GetComponent<ISummonable>()?.OnReturnedToPool();
                obj.SetActive(false);
                _summonPool.Enqueue(obj);
            }
        }
        _activeSummons.Clear();

        OnAttackComplete?.Invoke(true);
        return true;
    }

    public void Dispose()
    {
        if (_summonRoutine != null && _owner != null)
        {
            _owner.StopCoroutine(_summonRoutine);
            _summonRoutine = null;
        }

        for (int i = 0; i < _poolSize; i++)
        {
            var obj = _summonPool.Dequeue();
            GameObject.Destroy(obj);
        }
        _summonPool.Clear();
    }

    public float GetDamageNumber()
    {
        var score = _summonable?.GetDamageNumber() ?? 0;
        return score * (float) _poolSize;
    }
}
