using UnityEngine;

public class EnemyControllerBase : MonoBehaviour
{
    #region ReferenceClasses
    // Reference to input-event binds

    // Handler for player's movement
    private EnemyMovement enemyMove;
    #endregion

    [Header("Attack - Input hold duration")]
    [SerializeField] private float chargeThreshold = 1.5f; // seconds to trigger a charged attack
    [SerializeField] private float maxChargeTime = 3f;
    private float _attackPressTime = -1f;

    // Used Time.time instead of Time.deltaTime because
    // the charge logic is directly related to the actual timestamp,
    // which is not something that accumlates PER frame
    public float CurrentChargeRatio =>
        _attackPressTime < 0f ? 0f : Mathf.Clamp01((Time.time - _attackPressTime / maxChargeTime));

    // Jump => Flying transition related variables:
    [Header("Jump => Flying transition")]
    [SerializeField] private float flyingThreshold = 1.5f;
    public bool IsFlying => _isFlying;
    private bool _isFlying = false;
    private float _jumpPressTime = -1f;


    #region Setup
    private void Awake()
    {
        // Caching once, never having to re-fetch again:
        enemyMove = GetComponent<EnemyMovement>();
        //_playerAttack = GetComponent<PlayerAttack>();
        // _animator = GetComponent<PlayerAnimator>();
    }
    #endregion


    private void Update()
    {
        // if there is a valid time snapshot for jump start and 
        // the enemy is not already flying:
        if (_jumpPressTime >= 0f && !_isFlying)
        {
            // elapsed duration of the hold = current time - the jump start time snapshot
            float heldFor = Time.time - _jumpPressTime;
            if (heldFor >= flyingThreshold) // state change
            {
                _isFlying = true;
                enemyMove.StartFlying(); // always called before FlyTick()
            }
        }
    }

    private void FixedUpdate()
    {
        if (_isFlying)
            enemyMove.FlyTick(); // force applied here, every frame after Update()
    }
}
