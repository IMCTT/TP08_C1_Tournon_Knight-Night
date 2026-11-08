using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeSettings : MonoBehaviour
{
    [SerializeField] private AudioMixer mixer;
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider fxSlider;
    [SerializeField] private Slider uiSlider;

    
    void Start()
    {
        SetupSlider(masterSlider, "MasterVolume");
        SetupSlider(musicSlider, "MusicVolume");
        SetupSlider(fxSlider, "FXVolume");
        SetupSlider(uiSlider, "UIVolume");
    }

    private void SetupSlider(Slider slider, string parameter)
    {
        
        float savedValue = PlayerPrefs.GetFloat(parameter, 0.75f);
        slider.value = savedValue;
        SetVolume(parameter, savedValue);

        
        slider.onValueChanged.AddListener(value => SetVolume(parameter, value));
    }

    private void SetVolume(string parameter, float value)
    {
        
        mixer.SetFloat(parameter, Mathf.Log10(value) * 20f); //IA 
        PlayerPrefs.SetFloat(parameter, value); 
    }
}