using UnityEngine;
using Types;

public class JormungandrTail : MonoBehaviour, ISummonable
{
    [SerializeField] private float dealtDamage;
    [SerializeField] private ESpawnType spawnType;
    [SerializeField] private EElementType elementType;
    [SerializeField] private string fmodEventName = "";
    private bool _isActive = true;
    
    private Collider2D _collider;
    private Rigidbody2D _projRB;
    private SpriteRenderer _spriteRenderer;
    private Collider2D _instigatorCollider;
    protected GameObject _instigator;

    private void Awake()
    {
        _collider = GetComponent<Collider2D>();
        _projRB = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        _projRB.gravityScale = 0f;
    }

    // private void OnTriggerEnter2D(Collider2D other) 
    // {
    //     /*
    //     1. call the TakeDamage(gameObject, dealtDamage) interface function 
    //     2. change the anim state to collided.
    //     3. call ReturnToPool()
    //     */

    //     Debug.Log($"Jormungandr tail Collided: {other.gameObject}");

       

    // }

     // Call at the animation event
    public void CheckHit()
    {
        if(!_isActive) return;

        Collider2D[] results = Physics2D.OverlapBoxAll(
            (Vector2)transform.position + GetComponent<BoxCollider2D>().offset,
            GetComponent<BoxCollider2D>().size,
            0f
        );
        
        foreach (var col in results)
        {
            // Get instigator's layer name and collided object's layer name
            string instigatorLayerName = LayerMask.LayerToName(_instigator.layer);

            string targetLayerName = LayerMask.LayerToName(col.gameObject.layer);

            // // prevent team kill 
            if (instigatorLayerName.Contains("Enemy"))
            {
                if (targetLayerName.Contains("Enemy"))
                {
                    Debug.Log("?");
                    return;
                }
            }

            //1. processing any potential damage:
            var stats = col.gameObject.GetComponent<StatManager>();
            if (stats != null)
            {
                if (stats.TakeDamageHelper(_instigator, dealtDamage, elementType))
                {
                    //2. stop movement and disable collider so it doesn't retrigger
                    _projRB.linearVelocity = Vector2.zero;
                    _isActive = false;
                }
            }
        }
    }

    public void OnCollisionFinished()
    {
        gameObject.SetActive(false);
    }


    public void StartAttack(Vector3 attackPosition, GameObject Instigator)
    {
        _instigatorCollider = Instigator.GetComponent<Collider2D>();
        _instigator = Instigator;
        _isActive = true;

        if (_collider != null && _instigatorCollider != null)
        {
            // Set to ignore collisions between the projectile collider and the owner collider
            Physics2D.IgnoreCollision(_collider, _instigatorCollider, false);
        }

        transform.position = attackPosition;

        //_projRB.AddForce(fireDirection * speed, ForceMode2D.Impulse);
    }

    public float GetDamageNumber()
    {
        return dealtDamage;
    }

    public void OnSummoned()
    {
        //if (_collider != null) _collider.enabled = true;
        _isActive = true;
    }

    public void OnReturnedToPool()
    {
    }

    public void SetInstigator(GameObject instigator)
    {
        _instigator = instigator;
    }

}
