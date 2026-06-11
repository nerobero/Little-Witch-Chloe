using UnityEngine;
using UnityEngine.UI;
using FMODUnity;
using UnityEngine.EventSystems;
using Data;
using System.Collections;


public class MasterVolume : MonoBehaviour, IPointerUpHandler, IPointerEnterHandler
{


    [SerializeField] private Slider slider;
    [SerializeField] private string parameterName = "Master Volume";
    [SerializeField] private EventReference MasterSelect;
    [SerializeField] private EventReference MasterHover;

    void Start()
    {
        StartCoroutine(Initialize());
    }
    private IEnumerator Initialize()
    {
        try
        {
            var data = OptionManager.Instance.GetSettingData() ?? new SettingData { masterVolume = slider.maxValue };
            slider.value = data.masterVolume;
            slider.onValueChanged.AddListener(_ => UpdateParameter(slider.value));
        }
        catch (System.Exception e)
        {
            Debug.LogError("Initialize failed: " + e.Message);
        }
        yield return null;
    }

    void UpdateParameter(float value)
    {
        RuntimeManager.StudioSystem.setParameterByName(parameterName, value);
        
        SettingData settingData = OptionManager.Instance.GetSettingData();
        settingData.masterVolume = value;

        OptionManager.Instance.SaveSettingData();
    }


    public void OnPointerEnter(PointerEventData eventData)
    {

        RuntimeManager.PlayOneShot(MasterHover);

    }


    public void OnPointerUp(PointerEventData eventData) 
    {
    RuntimeManager.PlayOneShot(MasterSelect);
    }


}