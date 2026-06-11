using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using FMODUnity;
using UnityEngine.EventSystems;
using Data;
using Unity.VisualScripting;

public class SFXVolume : MonoBehaviour, IPointerUpHandler, IPointerEnterHandler
{


    [SerializeField] private Slider slider;
    [SerializeField] private string parameterName = "SFX Volume";
    [SerializeField] private EventReference SFXSelect;
    [SerializeField] private EventReference SFXHover;



    void Start()
    {
        StartCoroutine(Initialize());
    }
    private IEnumerator Initialize()
    {
        try
        {
            var data = OptionManager.Instance.GetSettingData() ?? new SettingData { sfxVolume = slider.maxValue };
            slider.value = data.sfxVolume;
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
        var result = RuntimeManager.StudioSystem.setParameterByName(parameterName, value);
        Debug.Log($"FMOD result: {result} | parameterName: {parameterName} | value: {value}");

        SoundManager.Instance.SetSfxVolume(value);
        SettingData settingData = OptionManager.Instance.GetSettingData();
        settingData.sfxVolume = value;

        OptionManager.Instance.SaveSettingData();
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