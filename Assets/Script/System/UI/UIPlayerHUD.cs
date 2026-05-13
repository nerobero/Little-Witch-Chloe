using UnityEngine;
using UnityEngine.UI;

public class UIPlayerHUD : UIBase
{
    [SerializeField] private Slider _hpSlider;
    [SerializeField] private Slider _staminaSlider;   
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    #region EventSubscription
    protected override void SubscribeEvents()
    {
        var playerStat = PlayerController.Instance.GetComponent<PlayerStatManager>();

        playerStat.OnHPChanged += UpdateHP;
        playerStat.OnStaminaChanged += UpdateStamina;
        playerStat.OnDeath += OnDeath;

        UpdateHP(playerStat.CurrentHP, playerStat.MaxHP, null);
    }

    protected override void UnsubscribeEvents()
    {
        var playerStat = PlayerController.Instance.GetComponent<PlayerStatManager>();

        playerStat.OnHPChanged -= UpdateHP;
        playerStat.OnStaminaChanged -= UpdateStamina;
        playerStat.OnDeath -= OnDeath;
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
}
