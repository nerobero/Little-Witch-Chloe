using UnityEngine;
using Types;

/// <summary>
/// Collectable moss item. Every 5 collected, grants the player
/// temporary immunity to poison fog.
/// </summary>
public class AntiFogMossPatch : CollectableItemBase
{
    private Animator _animator;
    protected override void Awake()
    {
        _animator = GetComponent<Animator>();
        CollectType = ECollectable.AntiFogMossPatch;
    }

    protected override bool OnInteract_HelperImpl(Collider2D other)
    {
        return GameManager.Instance.OnAntiFogMossCollected();
    }

    protected override void ProcessCollection()
    {
        _animator?.SetTrigger("IsCollected");
        
    }
    public void Deactivate()
    {
        gameObject.SetActive(false);
    }
}
