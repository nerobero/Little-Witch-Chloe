using UnityEngine;
using UnityEngine.UI;

public class EnemyHPWidget : UIBase
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private EnemyCharacterBase _targetStat;
    //private EnemyAnimController _targetAnim;
    [SerializeField] private Slider _hpSlider;
    public Slider HPSlider => _hpSlider;
    

    protected override void Start()
    {
        base.Start();

        // _targetStat = GetComponentInParent<EnemyCharacterBase>();

        // if(_targetStat == null)
        // {
        //     Debug.Log("Why");
        // }

        // SetTarget(_targetStat);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Register the target.
    public void SetTarget(EnemyCharacterBase stat)
    {
        _targetStat = stat;
        Show();

        UpdateHP(_targetStat.CurrentHP, _targetStat.MaxHP, null);

        SubscribeEvents();
    }

    #region EventSubscription
    protected override void SubscribeEvents()
    {
        _targetStat.OnHPChanged += UpdateHP;
        _targetStat.OnDeath += OnDeath;

        // _targetAnim = _targetStat.GetComponent<EnemyAnimController>();
        // if(_targetAnim == null) return;

        // _targetAnim.OnFlipped += Flipped;
    }

    protected override void UnsubscribeEvents()
    {
        _targetStat.OnHPChanged -= UpdateHP;
        _targetStat.OnDeath -= OnDeath;

        //_targetAnim.OnFlipped -= Flipped;

        _targetStat = null;
        //_targetAnim = null;
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

    // public void Flipped()
    // {
    //     Debug.Log("Hmm");
    //     // Vector2 localScale2D = _targetStat.transform.localScale;
    //     // localScale2D.x *= -1f;

    //     if(_targetAnim.IsFacingRight)
    //     {
    //         Vector2 localScale2D = Vector2.one;
    //         localScale2D.x = 1f;
    //         transform.localScale = localScale2D;
    //     }
    //     else
    //     {
    //         Vector2 localScale2D = Vector2.one;
    //         localScale2D.x = -1f;
    //         transform.localScale = localScale2D;
    //     }
    // }
}
