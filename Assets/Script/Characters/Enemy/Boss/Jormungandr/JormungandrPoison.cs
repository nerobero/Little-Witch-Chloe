using UnityEngine;
using Types;
using Data;

public class JormungandrPoison : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float dealtDamage;
    [SerializeField] private float duration;
    [SerializeField] private float interval;
    [SerializeField] private ESpawnType spawnType;
    [SerializeField] private EElementType elementType;
    [SerializeField] private string fmodEventName = "";

    // For now, the effect is used by hard coded.
    [SerializeField] private StatusEffect effect;

    private Collider2D _collider;
    private SpriteRenderer _spriteRenderer;
    private Collider2D _instigatorCollider;
    protected GameObject instigator;
    

    private void Awake()
    {
        _collider = GetComponent<Collider2D>();
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        /*
        1. call the TakeDamage(gameObject, dealtDamage) interface function 
        2. change the anim state to collided.
        3. call ReturnToPool()
        */

        Debug.Log($"Collided: {other.gameObject}");

        // Get instigator's layer name and collided object's layer name
        // string instigatorLayerName = LayerMask.LayerToName(instigator.layer);

        // string targetLayerName = LayerMask.LayerToName(other.gameObject.layer);

        // // prevent team kill 
        // if (instigatorLayerName.Contains("Enemy"))
        // {
        //     if (targetLayerName.Contains("Enemy"))
        //     {
        //         Debug.Log("?");
        //         return;
        //     }
        // }

        //1. processing any potential damage:
        var stats = other.gameObject.GetComponent<StatManager>();
        if (stats != null)
        {
            if (stats.TakeDamageHelper(instigator, dealtDamage, elementType, true, duration, interval))
            {
                // 2. Adjust the debuff(blind)
                stats.BuffComp.Add(effect);

                //3. play collision animation and wait for it to finish before pooling
                gameObject.SetActive(false);
            }
        }

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
}
