using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Purely visual telegraph that lives on a child GameObject of a boss (mirrors the
/// setup of Chloe's AttackPoint object on the Player prefab).
///
/// It carries its own <see cref="Animator"/> and <see cref="SpriteRenderer"/>(s) and
/// stays hidden until a telegraph is requested. Rather than playing states by name,
/// it is fed the SAME trigger/bool parameter names the boss FSM already pushes to its
/// other animators (see <c>BaseFSMAIController</c> / <c>JormungandrFSMController</c>:
/// "Melee", "Projectile", "Summon", "StatusEffect", "SwitchSides", "StunTrigger",
/// "IsStunned", "IsDead", ...). Its own AnimatorController just needs matching
/// parameters and a default empty/hidden state.
///
/// Every public method is safe to call from a Unity Animation Event placed on the
/// boss's main animation clips (one argument at most). Alternatively the FSM can
/// forward directly through the int overloads, exactly like it does for its flower
/// animator via <c>TriggerAnimation(Animator, int)</c>.
/// </summary>
[RequireComponent(typeof(Animator))]
public class BossAttackIndicator : MonoBehaviour
{
    [Tooltip("Renderers toggled with the telegraph. Auto-collected from children on Awake when left empty.")]
    [SerializeField] private SpriteRenderer[] _renderers;

    [Tooltip("Seconds after a telegraph is shown before it auto-hides. 0 or less disables the fallback.")]
    [SerializeField] private float _autoHideAfter = 0f;

    [Tooltip("Hide (disable renderers) automatically whenever this GameObject is disabled.")]
    [SerializeField] private bool _hideOnDisable = true;

    // Same names the boss FSM hashes and pushes to its body / flower animators.
    // Kept here so the FSM (or other code) can forward through the int overloads
    // without re-hashing, and so ResetState knows what to clear.
    private static readonly int MeleeHash = Animator.StringToHash("Melee");
    private static readonly int ProjectileHash = Animator.StringToHash("Projectile");
    private static readonly int SummonHash = Animator.StringToHash("Summon");
    private static readonly int StatusEffectHash = Animator.StringToHash("StatusEffect");
    private static readonly int SwitchSidesHash = Animator.StringToHash("SwitchSides");
    private static readonly int StunTriggerHash = Animator.StringToHash("StunTrigger");
    private static readonly int IsStunnedHash = Animator.StringToHash("IsStunned");
    private static readonly int IsDeadHash = Animator.StringToHash("IsDead");

    private static readonly int[] KnownTriggers =
    {
        MeleeHash, ProjectileHash, SummonHash, StatusEffectHash, SwitchSidesHash, StunTriggerHash
    };
    private static readonly int[] KnownBools = { IsStunnedHash, IsDeadHash };

    private Animator _animator;
    private bool _visible;

    private void Awake()
    {
        _animator = GetComponent<Animator>();

        if (_renderers == null || _renderers.Length == 0)
            _renderers = GetComponentsInChildren<SpriteRenderer>(includeInactive: true);

        Hide();
    }

    private void OnDisable()
    {
        CancelInvoke(nameof(Hide));
        if (_hideOnDisable) Hide();
    }

    #region Visibility

    /// <summary>Enables the telegraph renderers without touching the animator.</summary>
    public void Show()
    {
        SetRenderersEnabled(true);
    }

    /// <summary>Disables the telegraph renderers. Safe to wire to an Animation Event at the end of a telegraph clip.</summary>
    public void Hide()
    {
        CancelInvoke(nameof(Hide));
        SetRenderersEnabled(false);
    }

    private void SetRenderersEnabled(bool enabled)
    {
        _visible = enabled;
        if (_renderers == null) return;

        for (int i = 0; i < _renderers.Length; i++)
        {
            if (_renderers[i] != null) _renderers[i].enabled = enabled;
        }
    }

    private void ArmAutoHide()
    {
        CancelInvoke(nameof(Hide));
        if (_autoHideAfter > 0f) Invoke(nameof(Hide), _autoHideAfter);
    }

    #endregion

    #region Animation Event API (string) — call these from the boss's main clips

    /// <summary>
    /// Reveals the telegraph and fires an animator trigger by name. Pass one of the boss
    /// FSM's trigger names (e.g. "Melee", "Projectile", "Summon", "StatusEffect").
    /// </summary>
    public void PlayIndicator(string triggerName)
    {
        if (string.IsNullOrEmpty(triggerName)) return;
        if (_animator == null) _animator = GetComponent<Animator>();

        Show();
        _animator.SetTrigger(triggerName);
        ArmAutoHide();
    }

    /// <summary>
    /// Reveals the telegraph and sets an animator bool by name. A leading '!' sets it to
    /// false instead of true (e.g. "IsStunned" -> true, "!IsStunned" -> false).
    /// </summary>
    public void SetIndicatorFlag(string param)
    {
        if (string.IsNullOrEmpty(param)) return;
        if (_animator == null) _animator = GetComponent<Animator>();

        bool value = true;
        if (param[0] == '!')
        {
            value = false;
            param = param.Substring(1);
            if (param.Length == 0) return;
        }

        Show();
        _animator.SetBool(param, value);
        if (value) ArmAutoHide();
    }

    #endregion

    #region Int overloads — for the FSM to forward directly (parity with TriggerAnimation)

    public void PlayIndicator(int triggerHash)
    {
        if (_animator == null) _animator = GetComponent<Animator>();

        Show();
        _animator.SetTrigger(triggerHash);
        ArmAutoHide();
    }

    public void SetIndicatorFlag(int boolHash, bool value)
    {
        if (_animator == null) _animator = GetComponent<Animator>();

        Show();
        _animator.SetBool(boolHash, value);
        if (value) ArmAutoHide();
    }

    #endregion

    /// <summary>
    /// Clears every known trigger/bool and hides the telegraph. Call from the boss's
    /// own ResetState so pooled / re-fought bosses start clean.
    /// </summary>
    public void ResetState()
    {
        CancelInvoke(nameof(Hide));

        if (_animator == null) _animator = GetComponent<Animator>();

        if (_animator != null)
        {
            for (int i = 0; i < KnownTriggers.Length; i++)
                _animator.ResetTrigger(KnownTriggers[i]);

            for (int i = 0; i < KnownBools.Length; i++)
                _animator.SetBool(KnownBools[i], false);
        }

        Hide();
    }

    public bool IsVisible => _visible;
}
