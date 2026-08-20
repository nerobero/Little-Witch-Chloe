using UnityEngine;
using Types;

public class BurlController : BaseMonsterController
{
    [Header("Trap Setting")]
    [SerializeField] protected float explosionDist = 1.2f;
    protected bool isExploded = false;

    // Activated Is Trigger, to detect player
    BoxCollider2D triggerCollider;
    private BurlAnimController _animController;
    //private int playerLayerIndex => (int)Mathf.Log(playerLayer.value, 2);

    //protected new MushroomMineAttack enemyAttack;

    protected override void Start()
    {
        base.Start();

        triggerCollider = GetComponent<BoxCollider2D>();
        _animController = GetComponent<BurlAnimController>();

        enemyStat.OnDeath += _animController.SetToStartDead;
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

    }

    /// <summary>
    /// Resets the trap so it can detonate again once respawned from the pool.
    /// </summary>
    public void ResetTrap()
    {
        isExploded = false;
        enemyState = EMonsterState.Idle;
    }

    // Polygon collider(default: Physics) collision event => explode
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isExploded || enemyState == EMonsterState.Dead) return;

        // Check the player layer (use LayerMask)
        if (((1 << collision.gameObject.layer) & playerLayer) != 0)
        {
            Explode(collision.gameObject);
        }
    }

    // Damage logic
    private void Explode(GameObject target)
    {
        isExploded = true;
        enemyState = EMonsterState.Dead;

        // 1. deal damage to player
        enemyAttack.Attack(target);

        // 2. Play animation => this can be called by Death method of the enemy stat.
        // if(enemyMove.AnimController is MushroomMineAnimController anim)
        // {
        //     anim.SetToStartDead();
        // }

        // 
        Debug.Log("Collide with player. Deal explosion damage");

        // 3. take damage to itself for calling on death event dispatcher
        enemyStat.TakeDamageHelper(this.gameObject, enemyStat.MaxHP, EElementType.Fire);
        
    }

    protected override void Think()
    {
        
    }

    // override detect player of the enemy controller base, but don't use base.DetectPlayer().
    protected override void DetectPlayer()
    {
        
    }

    protected override void PlayerDetected(bool bIsDifferentPlatform, Vector2 hitPosition)
    {
        
    }

    /// <summary>
    /// This is the event driven function which will be called at death animation.
    /// </summary>
    public void OnDeathFinished()
    {
        enemyStat.OnDeath -= _animController.SetToStartDead;

        // Return to pool
        gameObject.SetActive(false);

    }
}
