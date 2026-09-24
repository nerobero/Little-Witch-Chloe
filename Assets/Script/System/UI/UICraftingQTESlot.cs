using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// One crafting QTE slot: pairs a direction-key icon with the ingredient icon it
/// currently represents. Both are set together by <see cref="UICraftingQTE"/> so the
/// pairing stays intact whenever the combo is reshuffled.
/// </summary>
public class UICraftingQTESlot : MonoBehaviour
{
    [SerializeField] private Image _keyIcon;
    [SerializeField] private Image _ingredientIcon;

    public void SetData(Sprite keySprite, Sprite ingredientSprite)
    {
        _keyIcon.sprite = keySprite;
        _ingredientIcon.sprite = ingredientSprite;
    }
}
