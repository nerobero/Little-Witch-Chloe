using UnityEngine;
using UnityEngine.UI;
using FMODUnity;
using UnityEngine.EventSystems;
using Data;
using System.Collections;

public class MusicVolume : MonoBehaviour, IPointerUpHandler, IPointerEnterHandler
{


    [SerializeField] private Slider slider;
    [SerializeField] private string parameterName = "MUSIC Volume";
    [SerializeField] private EventReference MusicSelect;
    [SerializeField] private EventReference MusicHover;



    void Start()
    {
        StartCoroutine(Initialize());
    }
    private IEnumerator Initialize()
    {
        try
        {
            var data = OptionManager.Instance.GetSettingData() ?? new SettingData { bgmVolume = slider.maxValue };
            slider.value = data.bgmVolume;
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
        settingData.bgmVolume = value;

        OptionManager.Instance.SaveSettingData();
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