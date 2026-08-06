using System;
using UnityEngine;

// Attached to pooled summon instances so SummonAttackStrategy is notified
// when the object deactivates (e.g. after its death animation finishes),
// regardless of which controller triggered the deactivation.
public class PooledSummonReturner : MonoBehaviour
{
    public event Action OnReturned;

    private void OnDisable()
    {
        OnReturned?.Invoke();
    }
}
