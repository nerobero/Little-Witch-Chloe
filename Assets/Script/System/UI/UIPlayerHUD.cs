using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Types;
using System.Collections.Generic;

public class UIPlayerHUD : UIBase
{
    [SerializeField] private Slider _hpSlider;
    //[SerializeField] private Slider _staminaSlider;  
    [SerializeField] private Image _staminaImg;  
    [SerializeField] private Image _blinkImg;
    [SerializeField] private List<Sprite> _projImgs, _skillImgs;
    [SerializeField] private TextMeshProUGUI _objectivesText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private bool isInitialized = false;
    [SerializeField] private GameObject _projGroup, _skillGroup;

    [SerializeField]private List<UISlotPanel> _projLists, _skillLists;
    [SerializeField]private int _currentProjIndex, _maxProjIndex = 0;
    [SerializeField]private int _maxSkillIndex = 0;

    protected override void Awake()
    {
        base.Awake();

        _projLists = new List<UISlotPanel>(_projGroup.GetComponentsInChildren<UISlotPanel>());
        _currentProjIndex = 0;

        foreach(var slot in _projLists)
        {
            slot.gameObject.SetActive(false);
        }

        _skillLists = new List<UISlotPanel>(_skillGroup.GetComponentsInChildren<UISlotPanel>());

        foreach(var slot in _skillLists)
        {
            slot.gameObject.SetActive(false);
        }
    }

    public void Initialize()
    {
        ResetState();
        OnEnable();
        PlayerController.Instance.GetComponent<PlayerAttack>().Initialize();
    }

    #region EventSubscription
    protected override void SubscribeEvents()
    {
        Debug.Log("Player HUD Subscribe");

        // prevent re-initialize
        if(isInitialized) return;

        if(PlayerController.Instance == null) return;

        var playerStat = PlayerController.Instance.GetComponent<PlayerStatManager>();

        if(playerStat == null) return;
        Debug.Log("PlayerStat Passed");

        var playerMove = PlayerController.Instance.GetComponent<PlayerMovement>();

        if(playerMove == null) return;

        playerStat.OnHPChanged += UpdateHP;
        playerStat.OnStaminaChanged += UpdateStamina;
        playerStat.OnDeath += OnDeath;

        UpdateHP(playerStat.CurrentHP, playerStat.MaxHP, null);
        UpdateStamina(playerStat.CurrStamina, playerStat.MaxStamina);

        playerMove.OnBlinkCooldown += UpdateBlinkCooldown;

        GameManager.Instance.OnObjectivesCollected += UpdateObjectives;

        isInitialized = true;
    }

    protected override void UnsubscribeEvents()
    {
        if(PlayerController.Instance != null)
        {
            var playerStat = PlayerController.Instance.GetComponent<PlayerStatManager>();

            if(playerStat != null)
            {
                playerStat.OnHPChanged -= UpdateHP;
                playerStat.OnStaminaChanged -= UpdateStamina;
                playerStat.OnDeath -= OnDeath;
            }

            var playerMove = PlayerController.Instance.GetComponent<PlayerMovement>();

            if(playerMove != null)
                playerMove.OnBlinkCooldown -= UpdateBlinkCooldown;
        }

        if(GameManager.Instance != null)
            GameManager.Instance.OnObjectivesCollected -= UpdateObjectives;

        isInitialized = false;
    }
    #endregion

    public void UpdateHP(float current, float max, GameObject instigator)
    {
        _hpSlider.value = current / max;
    }

    public void UpdateStamina(float current, float max)
    {
        //_staminaSlider.value = current / max;
        _staminaImg.fillAmount = current / max;
    }

    public void OnDeath()
    {
        OnDisable();
    }

    public void UpdateBlinkCooldown(float cool)
    {
        //_blinkImg.fillAmount = (1.0f / cool);
    }

    public void UpdateObjectives(ECollectable types, int amount)
    {
        _objectivesText.text = amount.ToString();
    }

    public void UpdateSkillList(EAbilityType abilityType, Sprite keyIcon)
    {
        int unlocked = (int)abilityType;
        _skillLists[_maxSkillIndex].gameObject.SetActive(true);

        _skillLists[_maxSkillIndex].OnSlotUnlocked(abilityType, _skillImgs[unlocked - 1], keyIcon);
        this._maxSkillIndex++;
    }

    public void UpdateProjectileList(ESpawnType projType)
    {
        int unlocked = (int)projType;
        Debug.Log("PlayerHUD _maxProjIndex: "+ _maxProjIndex);
        _projLists[_maxProjIndex].gameObject.SetActive(true);

        _projLists[_maxProjIndex].OnSlotUnlocked(projType, _projImgs[unlocked]);
        this._maxProjIndex++;
        
        Debug.Log("PlayerHUD _maxProjIndex Changed: "+ _maxProjIndex);
    }

    public (ESpawnType, EAbilityType) ProjectileSelected(int slot)
    {
        Debug.Log("PlayerHUD slot: "+ slot);
        (ESpawnType projType, EAbilityType abilityType, bool flag) = _projLists[slot].OnSlotSelected();

        if(flag)
        {
            if(slot == _currentProjIndex) 
            {
                return(projType, abilityType);
            }

            _projLists[_currentProjIndex].OnSlotDeselected();
            _currentProjIndex = slot;
            return (projType, abilityType);
        }

        // slot is not activated
        _projLists[_currentProjIndex].OnSlotSelected();
        Debug.Log("PlayerHUD _currentProjIndex: "+ _currentProjIndex);
        return (_projLists[_currentProjIndex].Type, _projLists[_currentProjIndex].AbilityType);
    }

    public void SlotCooldown(float time, EAbilityType abilityType)
    {
        foreach(var slot in _skillLists)
        {
            if(slot.AbilityType == abilityType)
            {
                slot.OnSlotCooledDown(time);
            }
        }
    }

    public void ResetState()
    {
        for(int i = _maxSkillIndex - 1; i >= 0; --i)
        {
            _skillLists[i].gameObject.SetActive(false);
        }

        for(int i = _maxProjIndex - 1; i >= 0; --i)
        {
            _projLists[i].gameObject.SetActive(false);
        }

        _maxSkillIndex = 0;
        _maxProjIndex = 0;
        _currentProjIndex = 0;
        _objectivesText.text = GameManager.Instance.GetCollectedFrog().ToString();
    }
}
