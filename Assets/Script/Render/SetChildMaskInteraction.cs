using UnityEngine;

public class SetChildMaskInteraction : MonoBehaviour
{
    public SpriteMaskInteraction maskInteractionSetting;

    [ContextMenu("Apply Mask Interaction to Children")]
    void ApplyMaskInteraction()
    {
        SpriteRenderer[] renderers = GetComponentsInChildren<SpriteRenderer>(true);

        foreach (SpriteRenderer sr in renderers)
        {
            // Skip the parent if it also has a SpriteRenderer you don't want to change
            if (sr.gameObject == this.gameObject) continue;

            sr.maskInteraction = maskInteractionSetting;
        }
        
        Debug.Log($"Updated mask interaction for {renderers.Length} child sprites.");
    }
}
