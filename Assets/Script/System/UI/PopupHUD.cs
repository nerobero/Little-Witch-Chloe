using UnityEngine;
using UnityEngine.UI;

public class PopupHUD : UIBase
{
    [SerializeField] private Slider _masterVolumeSlider;
    [SerializeField] private Slider _bgmVolumeSlider;
    [SerializeField] private Slider _sfxVolumeSlider;  
    [SerializeField] private Image _blinkImg;
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

        playerStat.OnDeath += OnDeath;

        var playerMove = PlayerController.Instance.GetComponent<PlayerMovement>();

        if(playerMove == null) return;

        playerMove.OnBlinkCooldown += UpdateBlinkCooldown;
    }

    protected override void UnsubscribeEvents()
    {
        if(PlayerController.Instance == null) return;

        var playerStat = PlayerController.Instance.GetComponent<PlayerStatManager>();

        playerStat.OnDeath -= OnDeath;

         var playerMove = PlayerController.Instance.GetComponent<PlayerMovement>();

        if(playerMove == null) return;

        playerMove.OnBlinkCooldown -= UpdateBlinkCooldown;
    }
    #endregion

    public void OnDeath()
    {
        OnDisable();
    }

    public void UpdateBlinkCooldown(float cool)
    {
        //_blinkImg.fillAmount = (1.0f / cool);
    }
}
