using TMPro;
using Types;
using UnityEngine;
using UnityEngine.UI;

public class LWCIngredientCheckbox : UIBase
{
    // the objective types => should be allocated at the prefab.
    [SerializeField] private ECollectable _objTypes;
    [SerializeField] private Image _objCheckbox;
    [SerializeField] private Image _objIcon;
    private TextMeshProUGUI _objName;
    private int _objAmount;

    protected override void SubscribeEvents()
    {
        
    }

    protected override void UnsubscribeEvents()
    {
        
    }

    public override void Show()
    {
        GetObjectives();
        base.Show();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Awake()
    {
        //_objCheckbox.gameObject.SetActive(false);
        //_objAmount = LovePotionManager.Instance.GetTargetCount(GameManager.Instance.CurrentLevel, _objTypes);
        _objName = GetComponent<TextMeshProUGUI>();

        //_objIcon.sprite = _objIconDataBase.GetIconSprite(_objTypes);
    }

    public void Initialize(ECollectable type, LovePotionIconDatabase iconDatabase)
    {
        _objTypes = type;

        _objAmount = LovePotionManager.Instance.GetTargetCount(
            GameManager.Instance.CurrentLevel, _objTypes);

        _objIcon.sprite = iconDatabase.GetIconSprite(_objTypes);
        _objCheckbox.gameObject.SetActive(false);
    }

    private void GetObjectives()
    {
        int ingredients = LovePotionManager.Instance.GetLoveIngredientCollectedAmount(_objTypes);
        
        if(ingredients >= _objAmount)
        {
            _objCheckbox.gameObject.SetActive(true);
        }
    }
}
