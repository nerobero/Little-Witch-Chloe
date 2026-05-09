using System.Collections;
using UnityEngine;
using Types;

/// <summary>
/// Base projectile objects that deals damage to characters.
/// Instantiation is managed by the pool object manager.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Animator))]
public class ProjectileBase : MonoBehaviour
{
    [SerializeField] private float dealtDamage;
    [SerializeField] private float speed;
    [SerializeField] private float lifeSpan = 10f;
    [SerializeField] private ESpawnType spawnType;
    private Collider2D _collider;
    private Rigidbody2D _projRB;

    private float _firedTimeSnapshot = -1f;
    private bool _isFired = false;

    private Animator _animator;
    private static readonly int CollidedHash = Animator.StringToHash("Collided");
    private static readonly int IsResetHash = Animator.StringToHash("IsReset");

    private void Awake()
    {
        _collider = GetComponent<Collider2D>();
        _projRB = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
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
        2. change the anim state to collided.
        3. call ReturnToPool()
        */

        //1. processing any potential damage:
        var stats = other.gameObject.GetComponent<StatManager>(); 
        if (stats != null)
            stats?.TakeDamage(gameObject, dealtDamage);

        //2. stop movement and disable collider so it doesn't retrigger
        _projRB.linearVelocity = Vector2.zero;
        _collider.enabled = false;

        //3. play collision animation and wait for it to finish before pooling
        _animator.SetBool(IsResetHash, false);
        _animator.SetTrigger(CollidedHash);
        StartCoroutine(ReturnToPoolAfterAnimation());
    }

    private IEnumerator ReturnToPoolAfterAnimation()
    {
        yield return null; // wait one frame for the trigger to take effect
        while (_animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
            yield return null;
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
        // reset the anim state to its default as well before returning it to the pool:
        PoolObjectManager.Instance.Return(spawnType, gameObject);
        _animator.SetBool(IsResetHash, true);
    }

    /// <summary>
    /// Callback function that gets called when the character fires this
    /// projectile object from its attack point
    /// </summary>
    public void OnFired(Transform firePointTransform, float fireAngle)
    {
        _collider.enabled = true;
        this.transform.SetPositionAndRotation(firePointTransform.position, Quaternion.Euler(0f,0f, fireAngle));
        _projRB.AddForce(firePointTransform.up * speed, ForceMode2D.Impulse);
        _firedTimeSnapshot = Time.time; // taking a snapshot of the time at which it was fired
        _isFired = true;
    }
}
