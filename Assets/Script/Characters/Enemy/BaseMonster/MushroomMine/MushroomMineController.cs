using UnityEngine;
using Types;

public class MushroomMineController : BaseMonsterController
{
    [Header("Trap Setting")]
    [SerializeField] protected float explosionDist = 1.2f;
    protected bool isExploded = false;

    // Activated Is Trigger, to detect player
    BoxCollider2D triggerCollider;
    


    protected override void Start()
    {
        base.Start();

        triggerCollider = GetComponent<BoxCollider2D>();

        if(enemyMove.AnimController is MushroomMineAnimController anim)
        {
            enemyStat.OnDeath += anim.SetToStartDead;
        }
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

    }

    // Box collider enter logic => growing ;; Think about after growing, using blink but inside the collider(trigger); it should shrink
    // => Use Event 
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isExploded) return;

        // Check the player layer (use LayerMask)
        if (((1 << collision.gameObject.layer) & playerLayer) != 0)
        {
            var PlayerMove = collision.GetComponent<PlayerMovement>();
            var Player = collision.GetComponent<PlayerController>();
            //bool isSamePlatform = false;
            if(Player != null)
            {
                Player.onBlinked += AdjustScale;
            }

            if(PlayerMove != null)
            {
            //    isSamePlatform = PlayerMove.IsBackground == enemyMove.IsBackground;
                AdjustScale(PlayerMove.IsBackground);
            }
        }
    }

    private void AdjustScale(bool isBackground)
    {
        var anim = enemyMove.AnimController as MushroomMineAnimController;
        if(!_hasTarget)
        {
            // Grow only if it is in the same platform.
            if(isBackground == enemyMove.IsBackground)
            {
                _hasTarget = true;
                // if(enemyMove.AnimController is MushroomMineAnimController anim)
                // {
                    anim.SetToStartGrowing();
                // }

                return;
            }    
        }

        _hasTarget = false;

        //if(enemyMove.AnimController is MushroomMineAnimController anim)
        //{
            anim.SetToStartShrinking();
        //}
    }

    // Box collider exit logic => shrink
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (isExploded) return;

        // Check the player layer (use LayerMask)
        if (((1 << collision.gameObject.layer) & playerLayer) != 0)
        {
            //AdjustScale(false);

            var Player = collision.GetComponent<PlayerController>();
            //bool isSamePlatform = false;
            if(Player != null)
            {
                Player.onBlinked -= AdjustScale;
            }

            _hasTarget = false;

            if(enemyMove.AnimController is MushroomMineAnimController anim)
            {
                anim.SetToStartShrinking();
            }
        }
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
        enemyStat.TakeDamage(this.gameObject, enemyStat.MaxHP, EElementType.Water);
        
    }

    protected override void Think()
    {
        switch(enemyState)
        {
            case EMonsterState.Attack:
                // HERE ATTACK LOGIC
                FireProjectile();
                Invoke("Think", 2);
            break;
            case EMonsterState.Chase:
                //enemyMove.MoveToTarget();
                Invoke("Think", 2);
            break;
            case EMonsterState.Idle:
                CancelInvoke();
            break;

        }
    }

    // override detect player of the enemy controller base, but don't use base.DetectPlayer().
    protected override void DetectPlayer()
    {
        
    }

    protected virtual void PlayerDetected(bool bIsDifferentPlatform, GameObject hit)
    {
        
    }

    /// <summary>
    /// This is the event driven function which will be called at death animation.
    /// </summary>
    public void OnDeathFinished()
    {
        // Remove bound function of OnDeath event dispatcher
        if(enemyMove.AnimController is MushroomMineAnimController anim)
        {
            enemyStat.OnDeath -= anim.SetToStartDead;
        }

        // Return to pool
        gameObject.SetActive(false);
        
    }
}
