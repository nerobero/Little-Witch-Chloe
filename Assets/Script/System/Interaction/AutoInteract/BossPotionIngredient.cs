using UnityEngine;

/// <summary>
/// Love potion ingredient dropped by a boss. Once collected, moves the player into the
/// crafting QTE area. The spawn point is a scene object, so the boss injects it on drop.
/// </summary>
public class BossPotionIngredient : LovePotionIngredient
{
    private Transform craftingQTESpawnPoint;

    public void SetCraftingQTESpawnPoint(Transform spawnPoint)
    {
        craftingQTESpawnPoint = spawnPoint;
    }

    protected override bool OnInteract_HelperImpl(Collider2D other)
    {
        // collect first so the ingredient is counted before the QTE area's trigger can fire
        bool collected = base.OnInteract_HelperImpl(other);

        if (collected && craftingQTESpawnPoint != null)
            PlayerController.Instance.PlayerMove.TeleportTo(craftingQTESpawnPoint.position);

        return collected;
    }
}
