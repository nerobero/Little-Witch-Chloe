using UnityEngine;
using Types;

/// <summary>
/// Class for respawnable HP items.
/// Heals n HP upon collision.
/// </summary>
public class RegenHPItem : RegenItemBase
{
    // the amount to heal upon interaction:
    [SerializeField] private float _healAmount = 0f;

    private void Awake()
    {
        base.spawnType = ESpawnType.HPItem;    
    }

    protected override bool OnInteractHelper(Collider2D other)
    {
        var statManagerComp = other.gameObject.GetComponent<StatManager>();
        if (statManagerComp == null) return false;
        
        int layer = (int)Mathf.Log(isBackground ? bgPlayerLayer : fgPlayeLayer, 2);

        if(other.gameObject.layer != layer) return false;

        return statManagerComp.Heal(_healAmount);
    }
}
