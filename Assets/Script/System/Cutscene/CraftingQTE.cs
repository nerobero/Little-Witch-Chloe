using System;
using System.Collections.Generic;
using Types;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Tracks a directional combo during the crafting cutscene. The combo is reshuffled
/// from the four cardinal directions at the start of every attempt.
/// </summary>
public class CraftingQTE : MonoBehaviour
{
    private static readonly Vector2[] CardinalDirections =
    {
        Vector2.left,
        Vector2.right,
        Vector2.up,
        Vector2.down,
    };

    private readonly List<Vector2> _sequence = new List<Vector2>(CardinalDirections);

    private int _comboIndex;
    private bool _isActive;
    private ELevelType _currentLevel;

    /// <summary>Current combo index and total sequence length, so UI can render progress.</summary>
    public event Action<int, int> OnComboProgress;
    /// <summary>Fired when a wrong input resets combo progress back to 0.</summary>
    public event Action OnComboReset;
    /// <summary>Fired instead of starting the QTE when ingredients are insufficient.</summary>
    public event Action OnQTEBlocked;
    public event Action OnQTESucceeded;

    private void OnEnable()
    {
        PlayerController.Instance.InputContext.CraftQTE.CraftingPotion.performed += HandleInput;
    }

    private void OnDisable()
    {
        PlayerController.Instance.InputContext.CraftQTE.CraftingPotion.performed -= HandleInput;
    }

    /// <summary>
    /// Triggered via Timeline Signal Emitter once the crafting loop starts.
    /// </summary>
    public void StartQTE()
    {
        _currentLevel = GameManager.Instance.GetCurrentLevel();

        if (!LovePotionManager.Instance.HasEnoughIngredients(_currentLevel))
        {
            Debug.LogWarning("[CraftingQTE] Missing required ingredients! Skipping QTE sequence.");
            OnQTEBlocked?.Invoke();
            return;
        }

        ShuffleSequence();
        _comboIndex = 0;
        _isActive = true;

        PlayerController.Instance.InputContext.BaseInputAction.Disable();
        PlayerController.Instance.InputContext.CraftQTE.Enable();
    }

    /// <summary>
    /// Fisher-Yates shuffle of the four cardinal directions, done fresh each attempt.
    /// </summary>
    private void ShuffleSequence()
    {
        for (int i = _sequence.Count - 1; i > 0; i--)
        {
            int swapIndex = UnityEngine.Random.Range(0, i + 1);
            (_sequence[i], _sequence[swapIndex]) = (_sequence[swapIndex], _sequence[i]);
        }
    }

    private void HandleInput(InputAction.CallbackContext context)
    {
        if (!_isActive)
            return;

        Vector2 raw = context.ReadValue<Vector2>();
        if (raw.magnitude < 0.5f)
            return;

        Vector2 input = SnapToCardinal(raw);

        if (input == _sequence[_comboIndex])
        {
            _comboIndex++;

            if (_comboIndex >= _sequence.Count)
                CompleteQTE();
            else
                OnComboProgress?.Invoke(_comboIndex, _sequence.Count);
        }
        else
        {
            _comboIndex = 0;
            OnComboReset?.Invoke();
        }
    }

    private void CompleteQTE()
    {
        _isActive = false;

        PlayerController.Instance.InputContext.CraftQTE.Disable();
        PlayerController.Instance.InputContext.BaseInputAction.Enable();

        LovePotionManager.Instance.ConsumeIngredients(_currentLevel);
        LovePotionManager.Instance.OnLovePotionMade();

        OnQTESucceeded?.Invoke();
    }

    private static Vector2 SnapToCardinal(Vector2 input)
    {
        return Mathf.Abs(input.x) > Mathf.Abs(input.y)
            ? new Vector2(Mathf.Sign(input.x), 0)
            : new Vector2(0, Mathf.Sign(input.y));
    }
}
