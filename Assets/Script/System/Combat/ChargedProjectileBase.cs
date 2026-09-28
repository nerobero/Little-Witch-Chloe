using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Types;

/// <summary>
/// Player charged-attack projectile. Unlike <see cref="ProjectileBase"/> (fire-and-forget,
/// returns to pool on first hit), this runs through three phases after being fired:
/// Charge (windup, no damage) -> Execute (deals damage, persists for a per-type duration
/// regardless of hits) -> Dissipate (plays its dissipate animation, then returns to pool).
/// Instantiation/pooling is managed by <see cref="PoolObjectManager"/>, same as ProjectileBase.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Animator))]
public class ChargedProjectileBase : MonoBehaviour, IResetable
{
    [SerializeField] private float dealtDamage;
    [SerializeField] private float speed;
    [SerializeField] private ESpawnType spawnType;
    [SerializeField] private EElementType elementType;
    [SerializeField] private string fmodEventName = "";

    [Header("Phase durations (seconds)")]
    [SerializeField] private float chargePhaseDuration = 0.3f;
    // this differs per element type - tune per prefab
    [SerializeField] private float executePhaseDuration = 2f;
    [SerializeField] private float dissipatePhaseDuration = 0.3f;

    private Collider2D _collider;
    private Rigidbody2D _projRB;
    private SpriteRenderer _spriteRenderer;
    private Collider2D _instigatorCollider;
    private Animator _animator;

    private static readonly int ChargeHash = Animator.StringToHash("Charge");
    private static readonly int ExecuteHash = Animator.StringToHash("Execute");
    private static readonly int DissipateHash = Animator.StringToHash("Dissipate");
    private static readonly int IsResetHash = Animator.StringToHash("IsReset");

    private Vector2 _startPoint = Vector2.zero;
    private bool _isFired = false;
    private bool _isBackground;
    private Coroutine _phaseRoutine;
    private readonly HashSet<Collider2D> _alreadyHit = new HashSet<Collider2D>();

    protected int fgLayer;
    protected int bgLayer;

    protected GameObject instigator;
    protected StatManager instigatorStat;

    public EChargedProjectilePhase CurrentPhase { get; private set; }

    private void Awake()
    {
        _collider = GetComponent<Collider2D>();
        _projRB = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        _projRB.gravityScale = 0f;

        fgLayer = LayerMask.NameToLayer("Foreground_Projectiles");
        bgLayer = LayerMask.NameToLayer("Background_Projectiles");
    }

    /// <summary>
    /// Callback function that gets called when the player fires this charged
    /// projectile from its attack point.
    /// </summary>
    /// <param name="chargeRatio">how fully the attack was charged, 0.0 (min) - 1.0 (full)</param>
    public void OnFired(Transform firePointTransform, float fireAngle, float damage, bool firedAtBackground, GameObject Instigator, StatManager instigatorStat, float chargeRatio)
    {
        _instigatorCollider = Instigator.GetComponent<Collider2D>();
        this.instigatorStat = instigatorStat;

        if (_collider != null && _instigatorCollider != null)
            Physics2D.IgnoreCollision(_collider, _instigatorCollider, true);

        _collider.enabled = false; // no damage yet - still charging
        transform.SetPositionAndRotation(firePointTransform.position, Quaternion.Euler(0f, 0f, fireAngle));
        _startPoint = firePointTransform.position;

        _isBackground = firedAtBackground;
        instigator = Instigator;
        dealtDamage = damage;
        gameObject.layer = _isBackground ? bgLayer : fgLayer;
        _spriteRenderer.sortingOrder = _isBackground ? -1 : 1;

        _alreadyHit.Clear();
        _isFired = true;

        if (_phaseRoutine != null) StopCoroutine(_phaseRoutine);
        _phaseRoutine = StartCoroutine(PhaseRoutine(firePointTransform.up));
    }

    private IEnumerator PhaseRoutine(Vector2 fireDirection)
    {
        // ---- Charge: windup, no movement, no damage ----
        CurrentPhase = EChargedProjectilePhase.Charge;
        _animator.SetTrigger(ChargeHash);
        yield return new WaitForSeconds(chargePhaseDuration);

        // ---- Execute: launches and deals damage; persists for its full duration ----
        CurrentPhase = EChargedProjectilePhase.Execute;
        _animator.SetTrigger(ExecuteHash);
        _collider.enabled = true;
        FMODUnity.RuntimeManager.PlayOneShot(fmodEventName);
        _projRB.AddForce(fireDirection * speed, ForceMode2D.Impulse);
        yield return new WaitForSeconds(executePhaseDuration);

        // ---- Dissipate: stop and play the dissipate animation before returning to pool ----
        CurrentPhase = EChargedProjectilePhase.Dissipate;
        _projRB.linearVelocity = Vector2.zero;
        _collider.enabled = false;
        _animator.SetTrigger(DissipateHash);
        yield return new WaitForSeconds(dissipatePhaseDuration);

        ReturnToPool();
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (CurrentPhase != EChargedProjectilePhase.Execute) return;

        string instigatorLayerName = LayerMask.LayerToName(instigator.layer);
        string targetLayerName = LayerMask.LayerToName(other.gameObject.layer);

        // prevent team kill
        if (instigatorLayerName.Contains("Enemy") && targetLayerName.Contains("Enemy")) return;

        var stats = other.gameObject.GetComponent<StatManager>();
        if (stats != null && !_alreadyHit.Contains(other.collider))
        {
            if (stats.TakeDamageHelper(instigator, dealtDamage, elementType))
                _alreadyHit.Add(other.collider);
        }

        // whether it hit terrain, or a target it already damaged this execute phase,
        // it stops moving and lingers in place until the execute duration elapses
        _projRB.linearVelocity = Vector2.zero;
    }

    /// <summary>
    /// Helper method that returns this gameobject back to the pool
    /// </summary>
    private void ReturnToPool()
    {
        _isFired = false;
        _phaseRoutine = null;

        if (_instigatorCollider != null)
        {
            Physics2D.IgnoreCollision(_collider, _instigatorCollider, false);
            _instigatorCollider = null;
        }
        instigator = null;
        _alreadyHit.Clear();

        PoolObjectManager.Instance.Return(spawnType, gameObject);
        _animator.SetBool(IsResetHash, true);
    }

    public void ResetState()
    {
        if (!_isFired) return;

        StopAllCoroutines();
        _phaseRoutine = null;
        _projRB.linearVelocity = Vector2.zero;
        _collider.enabled = false;

        ReturnToPool();
        gameObject.SetActive(false);
    }
}
