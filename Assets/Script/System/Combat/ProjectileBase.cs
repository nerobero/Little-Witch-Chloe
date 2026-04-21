using System.ComponentModel;
using UnityEngine;

/// <summary>
/// Base projectile objects that deals damage to characters.
/// Instantiation is managed by the pool object manager.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class ProjectileBase : MonoBehaviour
{
    [SerializeField] private float dealtDamage;
    [SerializeField] private float speed;
    [SerializeField] private float lifeSpan = 10f;
    [SerializeField] private SpawnType spawnType;
    private Collider2D _collider;
    private Rigidbody2D _projRB;
    private float _firedTimeSnapshot = -1f;
    private bool _isFired = false;

    private void Awake()
    {
        _collider = GetComponent<Collider2D>();
        _projRB = GetComponent<Rigidbody2D>();
        _projRB.gravityScale = 0f;
    }

    private void Update()
    {
        // checking if its lifetime is expired:
        if (_firedTimeSnapshot > 0f && _isFired)
        {
            float lifetime = Time.time - _firedTimeSnapshot;
            if (lifetime >= lifeSpan) 
                ReturnToPool();
        }
    }

    private void OnTriggerEnter2D(Collider2D other) 
    {
        /*
        1. call the TakeDamage(gameObject, dealtDamage) interface function 
        2. call ReturnToPool()
        */

        //1. processing any potential damage:
        

        //2. returning this to the pool:
        ReturnToPool();
    }

    /// <summary>
    /// Helper method that returns this gameobject to back to the pool
    /// </summary>
    private void ReturnToPool()
    {
        //resetting the snapshot values back to their default before returning it to the pool
        _firedTimeSnapshot = -1f;
        _isFired = false;
        PoolObjectManager.Instance.Return(spawnType, gameObject);
    }

    /// <summary>
    /// Callback function that gets called when the character fires this
    /// projectile object from its attack point
    /// </summary>
    public void OnFired(Transform firePointTransform)
    {
        this.transform.SetPositionAndRotation(firePointTransform.position, firePointTransform.rotation);
        _projRB.linearVelocity = firePointTransform.up * speed; // setting its travel velocity
        _firedTimeSnapshot = Time.time; // taking a snapshot of the time at which it was fired
        _isFired = true;
    }
}
