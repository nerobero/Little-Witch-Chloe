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
        OnAttackComplete?.Invoke(true);
    }

    private void Summon()
    {
        var obj = Get();
        obj.transform.position = _summonPosition;
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

    private void Return(GameObject toReturn)
    {
        toReturn.SetActive(false);
        _summonPool.Enqueue(toReturn);
    }

    public bool AttackFinished()
    {
        throw new NotImplementedException();
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
        return 0;
    }
}
