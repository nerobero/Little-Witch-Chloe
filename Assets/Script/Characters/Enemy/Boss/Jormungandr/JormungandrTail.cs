using UnityEngine;
using Types;

public class JormungandrTail : MonoBehaviour
{
    [SerializeField] private float dealtDamage;
    [SerializeField] private ESpawnType spawnType;
    [SerializeField] private EElementType elementType;
    [SerializeField] private string fmodEventName = "";
    
    private Collider2D _collider;
    private Rigidbody2D _projRB;
    private SpriteRenderer _spriteRenderer;
    private Collider2D _instigatorCollider;
    protected GameObject instigator;

    private void Awake()
    {
        _collider = GetComponent<Collider2D>();
        _projRB = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        _projRB.gravityScale = 0f;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        /*
        1. call the TakeDamage(gameObject, dealtDamage) interface function 
        2. change the anim state to collided.
        3. call ReturnToPool()
        */

        Debug.Log($"Collided: {other.gameObject}");

        // Get instigator's layer name and collided object's layer name
        string instigatorLayerName = LayerMask.LayerToName(instigator.layer);

        string targetLayerName = LayerMask.LayerToName(other.gameObject.layer);

        // prevent team kill 
        if (instigatorLayerName.Contains("Enemy"))
        {
            if (targetLayerName.Contains("Enemy"))
            {
                Debug.Log("?");
                return;
            }
        }

        //1. processing any potential damage:
        var stats = other.gameObject.GetComponent<StatManager>();
        if (stats != null)
        {
            if (stats.TakeDamageHelper(instigator, dealtDamage, elementType))
            {
                //2. stop movement and disable collider so it doesn't retrigger
                _projRB.linearVelocity = Vector2.zero;
                _collider.enabled = false;

                //3. play collision animation and wait for it to finish before pooling
            }
        }

    }
    
    /// <summary>
    /// Helper method that returns this gameobject to back to the pool
    /// </summary>
    private void ReturnToPool()
    {
        //resetting the snapshot values back to their default before returning it to the pool
        //_firedTimeSnapshot = -1f;
        //_isFired = false;

        // clear the per-collider ignore so the next instigator isn't affected by stale pairs
        if (_instigatorCollider != null)
        {
            Physics2D.IgnoreCollision(_collider, _instigatorCollider, false);
            _instigatorCollider = null;
        }
        instigator = null;

        // reset the anim state to its default as well before returning it to the pool:
        PoolObjectManager.Instance.Return(spawnType, gameObject);
        //_animator.SetBool(IsResetHash, true);
    }


    public void StartAttack(Vector3 attackPosition, GameObject Instigator)
    {
        _instigatorCollider = Instigator.GetComponent<Collider2D>();
        instigator = Instigator;

        if (_collider != null && _instigatorCollider != null)
        {
            // Set to ignore collisions between the projectile collider and the owner collider
            Physics2D.IgnoreCollision(_collider, _instigatorCollider, false);
        }

        transform.position = attackPosition;

        //_projRB.AddForce(fireDirection * speed, ForceMode2D.Impulse);
    }

    public void StartAttack()
    {
        
    }
}
