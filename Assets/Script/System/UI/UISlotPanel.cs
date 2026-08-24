using System.Collections;
using System.Collections.Generic;
using Types;
using UnityEngine;
using UnityEngine.UI;

public class UISlotPanel : UIBase
{
    public Image _iconImage;
    public Image _selectedImage;
    public Image _cooldownImg;
    public Image _keyIconImg;

    private ESpawnType _type = ESpawnType.None;
    private EAbilityType _abilityType = EAbilityType.None;
    private bool _activated = false;
    private bool _isCooleddown = false;
    private float cooltime = 0.0f, baseCooltime = 0.0f;

    public ESpawnType Type => _type;
    public EAbilityType AbilityType => _abilityType;

    protected void FixedUpdate()
    {
        if(_isCooleddown)
        {
            cooltime -= Time.deltaTime;
            
            _cooldownImg.fillAmount = cooltime / baseCooltime;

            if(cooltime <= 0.0f)
            {
                _isCooleddown = false;
                cooltime = baseCooltime;
                _cooldownImg.gameObject.SetActive(false);
            }
        }
    }
    

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
        _keyIconImg.gameObject.SetActive(true);
    }

    public virtual void OnSlotUnlocked(ESpawnType type, Sprite icon)
    {
        _type = type;
        _iconImage.gameObject.SetActive(true);
        _keyIconImg.gameObject.SetActive(true);
        _iconImage.sprite = icon;
        _cooldownImg.sprite = icon;
        _activated = true;
    }

    public virtual void OnSlotUnlocked(EAbilityType type, Sprite icon, Sprite keyIcon)
    {
        _type = ESpawnType.ScrollItem;
        _abilityType = type;
        _iconImage.sprite = icon;
        _cooldownImg.sprite = icon;
        _iconImage.gameObject.SetActive(true);
        _keyIconImg.sprite = keyIcon;
        _keyIconImg.gameObject.SetActive(true);
        _activated = true;
    }

    public virtual void OnSlotCooledDown(float time)
    {
        Debug.Log("Cooled down");
        _cooldownImg.gameObject.SetActive(true);
        cooltime = time;
        baseCooltime = time;
        _isCooleddown = true;
    }
}
