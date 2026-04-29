using UnityEngine;
using UnityEngine.UI;
using FMODUnity;
using UnityEngine.EventSystems;


public class SFXVolume : MonoBehaviour, IPointerUpHandler, IPointerEnterHandler
{


    [SerializeField] private Slider slider;
    [SerializeField] private string parameterName = "SFX Volume";
    [SerializeField] private EventReference SFXSelect;
    [SerializeField] private EventReference SFXHover;



    void Start()
    {
        UpdateParameter(slider.value);
        slider.onValueChanged.AddListener(_ => UpdateParameter(slider.value));
    }


    void UpdateParameter(float value)
    {
        RuntimeManager.StudioSystem.setParameterByName("SFX Volume", value);

    }


    public void OnPointerEnter(PointerEventData eventData)
    {

        RuntimeManager.PlayOneShot(SFXHover);

    }


    public void OnPointerUp(PointerEventData eventData)
    {
        RuntimeManager.PlayOneShot(SFXSelect);
    }
}