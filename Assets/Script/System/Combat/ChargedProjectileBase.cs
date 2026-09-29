using System.Collections;
using UnityEngine;
using Types;

/// <summary>
/// Player charged-attack projectile. Unlike <see cref="ProjectileBase"/> (fire-and-forget,
/// returns to pool on first hit), this runs through three phases after being fired:
/// Charge (windup, no damage) -> Execute (deals periodic AoE damage, behavior depends on
/// <see cref="EChargedDeliveryMode"/>) -> Dissipate (plays its dissipate animation, then
/// returns to pool). Instantiation/pooling is managed by <see cref="PoolObjectManager"/>,
/// same as ProjectileBase.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Animator))]
public class ChargedProjectileBase : MonoBehaviour, IResetable
{
    [Header("Delivery")]
    // Projectile: flies for flightDuration, then settles in place and bursts.
    // Stationary: never moves from the cast point, only bursts (may spin cosmetically).
    [SerializeField] private EChargedDeliveryMode deliveryMode = EChargedDeliveryMode.Projectile;

    [SerializeField] private float dealtDamage;
    [SerializeField] private float speed;
    [SerializeField] private ESpawnType spawnType;
    [SerializeField] private EElementType elementType;
    [SerializeField] private string fmodEventName = "";

    [Header("Phase durations (seconds)")]
    [SerializeField] private float chargePhaseDuration = 0.3f;
    // total time spent in the Execute phase - this differs per element type, tune per prefab
    [SerializeField] private float executePhaseDuration = 2f;
    [SerializeField] private float dissipatePhaseDuration = 0.3f;

    [Header("Execute - Projectile mode only")]
    // how long it travels before settling in place to start bursting; ignored in Stationary mode
    [SerializeField] private float flightDuration = 1f;

    [Header("Execute - Stationary mode only")]
    // cosmetic spin while it sits in place bursting; ignored in Projectile mode
    [SerializeField] private float rotationSpeed = 180f;

    [Header("Execute - periodic AoE damage (both modes)")]
    [SerializeField] private float damageTickInterval = 1f;
    [SerializeField] private float damageRadius = 1f;

    private Collider2D _collider;
    private Rigidbody2D _projRB;
    private SpriteRenderer _spriteRenderer;
    private Collider2D _instigatorCollider;
    private Animator _animator;

    private static readonly int ChargeHash = Animator.StringToHash("Charge");
    private static readonly int ExecuteHash = Animator.StringToHash("Execute");
    private static readonly int BurstHash = Animator.StringToHash("Burst");
    private static readonly int DissipateHash = Animator.StringToHash("Dissipate");
    private static readonly int IsResetHash = Animator.StringToHash("IsReset");

    private bool _isFired = false;
    private bool _isBackground;
    private bool _isRotating = false;
    private Coroutine _phaseRoutine;

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

    private void Update()
    {
        if (_isRotating)
            transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);
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

        transform.SetPositionAndRotation(firePointTransform.position, Quaternion.Euler(0f, 0f, fireAngle));

        _isBackground = firedAtBackground;
        instigator = Instigator;
        dealtDamage = damage;
        gameObject.layer = _isBackground ? bgLayer : fgLayer;
        _spriteRenderer.sortingOrder = _isBackground ? -1 : 1;

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

        // ---- Execute: moves (Projectile mode only), then periodically bursts AoE damage ----
        CurrentPhase = EChargedProjectilePhase.Execute;
        _animator.SetTrigger(ExecuteHash);
        FMODUnity.RuntimeManager.PlayOneShot(fmodEventName);

        float burstDuration = executePhaseDuration;

        if (deliveryMode == EChargedDeliveryMode.Projectile)
        {
            float flightTime = Mathf.Min(flightDuration, executePhaseDuration);
            _projRB.linearVelocity = fireDirection * speed;
            yield return new WaitForSeconds(flightTime);
            _projRB.linearVelocity = Vector2.zero;

            burstDuration = executePhaseDuration - flightTime;
        }

        yield return StartCoroutine(BurstRoutine(burstDuration));

        // ---- Dissipate: stop and play the dissipate animation before returning to pool ----
        CurrentPhase = EChargedProjectilePhase.Dissipate;
        _isRotating = false;
        _projRB.linearVelocity = Vector2.zero;
        _animator.SetTrigger(DissipateHash);
        yield return new WaitForSeconds(dissipatePhaseDuration);

        ReturnToPool();
    }

    /// <summary>
    /// Repeatedly deals AoE damage every damageTickInterval for the given duration.
    /// In Stationary mode, also spins the object cosmetically while it runs.
    /// </summary>
    private IEnumerator BurstRoutine(float duration)
    {
        if (deliveryMode == EChargedDeliveryMode.Stationary)
            _isRotating = true;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            DealBurstDamage();
            _animator.SetTrigger(BurstHash);

            float wait = Mathf.Min(damageTickInterval, duration - elapsed);
            yield return new WaitForSeconds(wait);
            elapsed += wait;
        }

        _isRotating = false;
    }

    private void DealBurstDamage()
    {
        string instigatorLayerName = LayerMask.LayerToName(instigator.layer);
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, damageRadius);

        foreach (var hit in hits)
        {
            if (hit == _instigatorCollider) continue;

            // prevent team kill
            string targetLayerName = LayerMask.LayerToName(hit.gameObject.layer);
            if (instigatorLayerName.Contains("Enemy") && targetLayerName.Contains("Enemy")) continue;

            var stats = hit.GetComponent<StatManager>();
            if (stats == null) continue;

            stats.TakeDamageHelper(instigator, dealtDamage, elementType);
        }
    }

    /// <summary>
    /// Helper method that returns this gameobject back to the pool
    /// </summary>
    private void ReturnToPool()
    {
        _isFired = false;
        _isRotating = false;
        _phaseRoutine = null;

        if (_instigatorCollider != null)
        {
            Physics2D.IgnoreCollision(_collider, _instigatorCollider, false);
            _instigatorCollider = null;
        }
        instigator = null;

        PoolObjectManager.Instance.Return(spawnType, gameObject);
        _animator.SetBool(IsResetHash, true);
    }

    public void ResetState()
    {
        if (!_isFired) return;

        StopAllCoroutines();
        _phaseRoutine = null;
        _isRotating = false;
        _projRB.linearVelocity = Vector2.zero;

        ReturnToPool();
        gameObject.SetActive(false);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, damageRadius);
    }
}
