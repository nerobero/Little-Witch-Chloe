using System;
using System.Collections.Generic;
using Types;
using UnityEngine;

public class UICraftingQTE : UIBase
{
    [Serializable]
    private struct DirectionKeySprite
    {
        public Vector2 direction;
        public Sprite keySprite;
    }

    [SerializeField] private GameObject _slotGroup;
    [SerializeField] private ItemIconDatabase _itemIconDatabase;
    [SerializeField] private List<DirectionKeySprite> _directionKeySprites;

    private readonly List<UICraftingQTESlot> _uiSlots = new List<UICraftingQTESlot>();

    /// <summary>
    /// Lays out one slot per (direction, ingredient) pair. Directions and ingredients are
    /// already paired index-for-index by <see cref="CraftingQTE"/>, so slot i just renders
    /// pair i. Any slots beyond the pair count (fewer required ingredients than 4) are hidden.
    /// </summary>
    public void Initialize(List<Vector2> sequence, List<ECollectable> ingredients)
    {
        EnsureSlotsCached();

        for (int i = 0; i < _uiSlots.Count; i++)
        {
            bool isUsed = i < sequence.Count;
            _uiSlots[i].gameObject.SetActive(isUsed);

            if (isUsed)
            {
                Sprite keySprite = GetKeySprite(sequence[i]);
                Sprite ingredientSprite = _itemIconDatabase.GetItemIcon(ingredients[i]);
                _uiSlots[i].SetData(keySprite, ingredientSprite);
            }
        }
    }

    private void EnsureSlotsCached()
    {
        if (_uiSlots.Count > 0)
            return;

        _uiSlots.AddRange(_slotGroup.GetComponentsInChildren<UICraftingQTESlot>(true));
    }

    private Sprite GetKeySprite(Vector2 direction)
    {
        foreach (DirectionKeySprite entry in _directionKeySprites)
        {
            if (entry.direction == direction)
                return entry.keySprite;
        }

        Debug.LogWarning($"[UICraftingQTE] No key sprite mapped for direction {direction}.");
        return null;
    }

    protected override void Awake()
    {
        base.Awake();
        Hide();
    }


    protected override void SubscribeEvents()
    {
    }

    protected override void UnsubscribeEvents()
    {
    }


}
