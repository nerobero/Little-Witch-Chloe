using UnityEngine;
using UnityEngine.UI;
using FMODUnity;
using UnityEngine.EventSystems;


public class MasterVolume : MonoBehaviour, IPointerUpHandler, IPointerEnterHandler
{

    
        [SerializeField] private Slider slider;
        [SerializeField] private string parameterName = "Master Volume";
        [SerializeField] private EventReference MasterSelect;
        [SerializeField] private EventReference MasterHover;

        

        void Start()
        {
            UpdateParameter(slider.value);
            slider.onValueChanged.AddListener(_ => UpdateParameter(slider.value));   
        }
        
        
        void UpdateParameter(float value)
        {
            RuntimeManager.StudioSystem.setParameterByName("Master Volume", value);
           
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