using UnityEngine;
using UnityEngine.UI;

public class UIPlayerHUD : UIBase
{
    [SerializeField] private Slider _hpSlider;
    [SerializeField] private Slider _staminaSlider;  
    [SerializeField] private Image _blinkImg;  
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        base.Start();
        SubscribeEvents();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    #region EventSubscription
    protected override void SubscribeEvents()
    {
        if(PlayerController.Instance == null) return;

        var playerStat = PlayerController.Instance.GetComponent<PlayerStatManager>();

        if(playerStat == null) return;

        playerStat.OnHPChanged += UpdateHP;
        playerStat.OnStaminaChanged += UpdateStamina;
        playerStat.OnDeath += OnDeath;

        UpdateHP(playerStat.CurrentHP, playerStat.MaxHP, null);
        UpdateStamina(playerStat.CurrStamina, playerStat.MaxStamina);

        var playerMove = PlayerController.Instance.GetComponent<PlayerMovement>();

        if(playerMove == null) return;

        playerMove.OnBlinkCooldown += UpdateBlinkCooldown;
    }

    protected override void UnsubscribeEvents()
    {
        if(PlayerController.Instance == null) return;

        var playerStat = PlayerController.Instance.GetComponent<PlayerStatManager>();

        playerStat.OnHPChanged -= UpdateHP;
        playerStat.OnStaminaChanged -= UpdateStamina;
        playerStat.OnDeath -= OnDeath;

         var playerMove = PlayerController.Instance.GetComponent<PlayerMovement>();

        if(playerMove == null) return;

        playerMove.OnBlinkCooldown -= UpdateBlinkCooldown;
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
}
