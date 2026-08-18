using UnityEngine;

public class TintChildren : MonoBehaviour
{
    public Color tintColor = Color.white;

    void Start()
    {
        ApplyTint(tintColor);
    }

    public void ApplyTint(Color newColor)
    {
        SpriteRenderer[] spriteChildren = GetComponentsInChildren<SpriteRenderer>();

        foreach (SpriteRenderer child in spriteChildren)
        {
            child.color *= newColor;
        }
    }
}
