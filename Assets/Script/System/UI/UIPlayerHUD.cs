using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Types;
using System.Collections.Generic;

public class UIPlayerHUD : UIBase
{
    [SerializeField] private Slider _hpSlider;
    [SerializeField] private Slider _staminaSlider;  
    [SerializeField] private Image _blinkImg;
    [SerializeField] private List<Sprite> _projImgs;
    [SerializeField] private TextMeshProUGUI _objectivesText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private bool isInitialized = false;

    protected override void Start()
    {
        base.Start();
        //SubscribeEvents();
    }

    public void Initialize()
    {
        OnEnable();
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
        _staminaSlider.value = current / max;
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

    public void UpdateProjectileList(ESpawnType projType)
    {
        
    }

    public void ProjectileSelected(int slot)
    {
        
    }
}
