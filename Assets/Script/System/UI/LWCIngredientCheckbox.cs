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
    private int _objAmount = 0;
    private int _objTargetAmount = 0;

    protected override void SubscribeEvents()
    {
        if (LovePotionManager.Instance != null)
        {
            LovePotionManager.Instance.OnLovePotionStateChanged += UpdateUI;
        }
    }

    protected override void UnsubscribeEvents()
    {
        if (LovePotionManager.Instance != null)
        {
            LovePotionManager.Instance.OnLovePotionStateChanged -= UpdateUI;
        }
    }

    public override void Show()
    {
        // GetObjectives();
        base.Show();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Awake()
    {
        // Text sits on child "CollectableName", not on the root.
        _objName = GetComponentInChildren<TextMeshProUGUI>(true);
    }

    public void Initialize(ECollectable type, LovePotionIconDatabase iconDatabase)
    {
        // Awake may not have run yet if the parent HUD is inactive.
        if (_objName == null)
        {
            _objName = GetComponentInChildren<TextMeshProUGUI>(true);
        }

        _objTypes = type;
        _objTargetAmount = LovePotionManager.Instance.GetTargetCount(
            GameManager.Instance.CurrentLevel, type);

        // Initializing the text:
        UpdateCountText(_objTypes);

        // Initializing the sprite and the checkbox:
        _objIcon.sprite = iconDatabase.GetIconSprite(_objTypes);
        _objCheckbox.gameObject.SetActive(false);
    }

    private void UpdateUI()
    {
        // Updating the text and the collected count:
        UpdateCountText(_objTypes);
        // Updating the objective collected toggle:
        GetObjectives(_objAmount);
    }

    private void UpdateCountText(ECollectable type)
    {
        // Initializing the text:
        _objAmount = LovePotionManager.Instance.GetLoveIngredientCollectedAmount(type);

        _objName.text = $"{_objAmount}/{_objTargetAmount}";
    }

    private void GetObjectives()
    {
        int ingredients = LovePotionManager.Instance.GetLoveIngredientCollectedAmount(_objTypes);
        
        if(ingredients >= _objAmount)
        {
            _objCheckbox.gameObject.SetActive(true);
        }
    }
    private void GetObjectives(int count)
    {   
        if(count >= _objTargetAmount)
        {
            _objCheckbox.gameObject.SetActive(true);
        }
    }
}
