using UnityEngine;
using UnityEngine.UI;

public class EnemyHPWidget : UIBase
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private EnemyCharacterBase _targetStat;
    [SerializeField] private Slider _hpSlider;

    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Register the target.
    public void SetTarget(EnemyCharacterBase stat)
    {
        UnsubscribeEvents();

        _targetStat = stat;

        // 새로운 대상 구독
        SubscribeEvents();
        
        // 초기 UI 갱신
        UpdateHP(_targetStat.CurrentHP, _targetStat.MaxHP, null);
        
        
    }

    #region EventSubscription
    protected override void SubscribeEvents()
    {
        Debug.Log("This");
        _targetStat.OnHPChanged += UpdateHP;
        _targetStat.OnDeath += OnDeath;

        var Anim = _targetStat.GetComponent<EnemyAnimController>();
        if(Anim == null) return;

        Anim.OnFlipped += Flipped;
    }

    protected override void UnsubscribeEvents()
    {
        Debug.Log("This");
        _targetStat.OnHPChanged -= UpdateHP;
        _targetStat.OnDeath -= OnDeath;

        var Anim = _targetStat.GetComponent<EnemyAnimController>();
        if(Anim == null) return;

        Anim.OnFlipped -= Flipped;
    }
    #endregion

    public void UpdateHP(float current, float max, GameObject instigator)
    {
        _hpSlider.value = current / max;
    }

    public void OnDeath()
    {
        OnDisable();
    }

    public void Flipped()
    {
        Vector2 localPosition2D = transform.position;
        localPosition2D.x *= -1f;
        transform.localPosition = localPosition2D;
    }
}
