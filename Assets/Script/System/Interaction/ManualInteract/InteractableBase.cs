using UnityEngine;

/// <summary>
/// Interface for the pure contract of 
/// interaction logic for all interactable objects
/// </summary>
public interface IInteractable
{
    bool CanInteract();
    void Interact();
    void ShowInteractUI(bool isShown);
}

/// <summary>
/// Abstract class that implements the IInteractable interface
/// as well as shared Monobehavior functions.
/// </summary>
public abstract class InteractableBase : MonoBehaviour, IInteractable
{
    [SerializeField] private Sprite interactIcon;
    [SerializeField] private Vector2 interactIconOffset = new Vector2(0f, 1f);

    private static Sprite s_placeholderIconSprite;

    private SpriteRenderer _iconRenderer;

    // the child classes can override this function based on
    // its functionality:
    public virtual bool CanInteract() => true;

    protected abstract void Interact_Impl();

    /// <summary>
    /// Main function for processing interaction.
    /// The actual logic for interaction sits in Interact_Impl().
    /// </summary>
    public void Interact()
    {
        if (CanInteract())
            Interact_Impl();
    }

    public void ShowInteractUI(bool isShown)
    {
        if (_iconRenderer == null)
            _iconRenderer = CreateIcon();

        _iconRenderer.gameObject.SetActive(isShown);
    }

    private SpriteRenderer CreateIcon()
    {
        var iconObject = new GameObject("InteractIcon");
        iconObject.transform.SetParent(transform, false);
        iconObject.transform.localPosition = interactIconOffset;
        iconObject.SetActive(false);

        var renderer = iconObject.AddComponent<SpriteRenderer>();
        renderer.sprite = interactIcon != null ? interactIcon : GetPlaceholderIconSprite();
        renderer.sortingOrder = 100;

        return renderer;
    }

    private static Sprite GetPlaceholderIconSprite()
    {
        if (s_placeholderIconSprite != null)
            return s_placeholderIconSprite;

        const int size = 32;
        var texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
        var center = new Vector2(size - 1, size - 1) * 0.5f;
        var radius = size * 0.5f;

        for (var y = 0; y < size; y++)
        {
            for (var x = 0; x < size; x++)
            {
                var dist = Vector2.Distance(new Vector2(x, y), center);
                var inside = dist <= radius;
                texture.SetPixel(x, y, inside ? Color.yellow : Color.clear);
            }
        }
        texture.Apply();

        s_placeholderIconSprite = Sprite.Create(
            texture,
            new Rect(0, 0, size, size),
            new Vector2(0.5f, 0.5f),
            size);

        return s_placeholderIconSprite;
    }
}
