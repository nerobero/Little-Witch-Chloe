using UnityEngine;
using UnityEngine.UI;
using FMODUnity;
using UnityEngine.EventSystems;


public class MusicVolume : MonoBehaviour, IPointerUpHandler, IPointerEnterHandler
{


    [SerializeField] private Slider slider;
    [SerializeField] private string parameterName = "MUSIC Volume";
    [SerializeField] private EventReference MusicSelect;
    [SerializeField] private EventReference MusicHover;



    void Start()
    {
        UpdateParameter(slider.value);
        slider.onValueChanged.AddListener(_ => UpdateParameter(slider.value));
    }


    void UpdateParameter(float value)
    {
        RuntimeManager.StudioSystem.setParameterByName("MUSIC Volume", value);

    }


    public void OnPointerEnter(PointerEventData eventData)
    {

        RuntimeManager.PlayOneShot(MusicHover);

    }


    public void OnPointerUp(PointerEventData eventData)
    {
        RuntimeManager.PlayOneShot(MusicSelect);
    }
}