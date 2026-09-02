using UnityEngine;
using Types;

/// <summary>
/// Class for respawnable stamina items.
/// Replenishes n amount of stamina upon collision.
/// </summary>
public class RegenStaminaItem : RegenItemBase
{
    // the amount to replenish upon interaction:
    [SerializeField] private float _replenishAmount = 0f;
    private void Awake()
    {
        base.spawnType = ESpawnType.StaminaItem;
    }

    protected override bool OnInteractHelper(Collider2D other)
    {

        //int layer = (int)Mathf.Log(isBackground ? bgPlayerLayer : fgPlayeLayer, 2);
        if(LayerManager.IsSameSide(gameObject, other.gameObject))
        {
            var playerStatMgrComp = other.gameObject.GetComponent<PlayerStatManager>();
            if (playerStatMgrComp == null) return false;
            
            return playerStatMgrComp.ReplenishStamina(_replenishAmount);
        }
        return false;

        //if(other.gameObject.layer != layer) return false;
    }
}
