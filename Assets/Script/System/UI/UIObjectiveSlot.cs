using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class UIObjectiveSlot : UIBase
{
    public Image objectiveIcon;
    [SerializeField] private TextMeshPro _countText;
    [SerializeField] private float _slotDisplayDuration = 2.5f;

    protected override void SubscribeEvents()
    {
        throw new System.NotImplementedException();
    }

    protected override void UnsubscribeEvents()
    {
        throw new System.NotImplementedException();
    }
}
