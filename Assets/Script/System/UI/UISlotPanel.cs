using Types;
using UnityEngine;
using UnityEngine.UI;

public class UISlotPanel : UIBase
{
    [SerializeField] private Image _iconImage;
    [SerializeField] private Image _selectedImage;
    private ESpawnType _type = ESpawnType.None;
    private EAbilityType _abilityType = EAbilityType.None;
    private bool _activated = false;

    public ESpawnType Type => _type;
    public EAbilityType AbilityType => _abilityType;
    

    /// <summary>
    /// Subscribes events from the related systems.  
    /// </summary>
    protected override void SubscribeEvents()
    {
        
    }

    /// <summary>
    /// Unsubscribes events from the related systems.
    /// </summary>
    protected override void UnsubscribeEvents()
    {
        
    }


    public virtual (ESpawnType, EAbilityType, bool) OnSlotSelected()
    {
        if(!_activated)
        {
            return (ESpawnType.None, EAbilityType.None, _activated);
        }

        if(_selectedImage)
        {
            _selectedImage.gameObject.SetActive(true);

            return (_type, _abilityType, _activated);
        }

        return (ESpawnType.None, EAbilityType.None, _activated);
    }

    public virtual void OnSlotDeselected()
    {
        if(_selectedImage)
        {
            _selectedImage.gameObject.SetActive(false);
        }
    }

    public virtual void OnSlotUnlocked()
    {
        _iconImage.gameObject.SetActive(true);
    }

    public virtual void OnSlotUnlocked(ESpawnType type, Sprite icon)
    {
        _type = type;
        _iconImage.gameObject.SetActive(true);
        _iconImage.sprite = icon;
        _activated = true;
    }

    public virtual void OnSlotUnlocked(EAbilityType type, Sprite icon)
    {
        _type = ESpawnType.ScrollItem;
        _abilityType = type;
        _iconImage.sprite = icon;
        _iconImage.gameObject.SetActive(true);
        _activated = true;
    }
}
