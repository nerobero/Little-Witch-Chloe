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

    // the fixed root position this strategy was configured with; never mutated after construction
    private Vector3 _basePosition;

    // the position computed for the current/most recent attack, derived fresh from _basePosition each time
    private Vector3 _summonPosition;

    // radius around _summonPosition within which each summon's spawn point is randomized
    private float _orbitRadius = 0f;

    private int _poolSize => _summonPool.Count;

    // owner used to run the summon coroutine, since this strategy is a plain C# class
    private MonoBehaviour _owner;
    private Coroutine _summonRoutine;
    private float _summonRange;

    // cached from the prefab, since every pooled instance shares the same damage number
    private ISummonable _summonable;

    // summoned instances from this attack that are still out in the world;
    // forced back to the pool once the attack completes
    private readonly List<GameObject> _activeSummons = new();

    private WaitForSecondsTracked _waitTime;

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
        _waitTime = new WaitForSecondsTracked(_timeInterval);

        _summonable = summonObj.GetComponent<ISummonable>();

        // initializing the pool:
        for (int i = 0; i < poolSize; i++)
        {
            var obj = GameObject.Instantiate(summonObj);
            obj.SetActive(false);
            _summonPool.Enqueue(obj);
        }

        _basePosition = position;
        _summonPosition = position;

    }

    public SummonAttackStrategy(MonoBehaviour owner, GameObject summonObj, float timeInterval, int poolSize, Vector3 position, float orbitRadius = 0.0f, float summonRange = 0.0f)
        : this(owner, summonObj, timeInterval, poolSize, position)
    {
        _orbitRadius = orbitRadius;
        _summonRange = summonRange;
    }

    public bool Attack(GameObject instigator, GameObject target, bool useLastTarget = false)
    {
        if (_poolSize <= 0) return false;
        if (_summonRoutine != null) return false; // already summoning

        BaseFSMAIController ai = (BaseFSMAIController)_owner;

        if(_orbitRadius == 0.0f)
        {
            float randomOffset = UnityEngine.Random.Range(-_summonRange, _summonRange);
            float x = target.transform.position.x + randomOffset;
            float y = _basePosition.y;

            if(ai.CurrentSideIsRight)
            {
                x = Mathf.Clamp(x, ai.ForegroundMinX, ai.ForegroundMaxX);
            }
            // Left side
            else
            {
                x = Mathf.Clamp(x, ai.BackgroundMinX, ai.BackgroundMaxX);
                y += Mathf.Abs(ai.BackgroundY - ai.ForegroundY);
            }

            // recomputed fresh from _basePosition every attack, rather than mutated in place,
            // so repeated background-side attacks don't keep stacking the Y offset
            _summonPosition = new Vector3(x, y, _basePosition.z);
        }

        _summonRoutine = _owner.StartCoroutine(SummonRoutine());

        return true;
    }

    private IEnumerator SummonRoutine()
    {
        int snapShot = _poolSize;

        for (int i = 0; i < snapShot; i++)
        {
            Summon();
            yield return _waitTime;
        }

        _summonRoutine = null;
        AttackFinished();
    }

    private void Summon()
    {
        var obj = Get();
        Vector2 randomOffset = UnityEngine.Random.insideUnitCircle * _orbitRadius;
        obj.transform.position = _summonPosition + new Vector3(randomOffset.x, randomOffset.y, 0f);

        // match the summoned object's layer to whichever side the instigator is currently on
        bool isBackground = LayerMask.LayerToName(_owner.gameObject.layer).StartsWith("Background");
        obj.layer = LayerManager.Instance.GetLayer(isBackground, "Enemy");

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

    public bool AttackInturrupted()
    {
        if(_summonRoutine != null)
        {
            _owner.StopCoroutine(_summonRoutine);
            _summonRoutine = null;
        }

        OnAttackComplete?.Invoke(false);
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
