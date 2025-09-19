using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SaveSlotUI : MonoBehaviour
{
    //Slider music
    [SerializeField] private Slider sfxSlider;
    
    void Start()
    {
        sfxSlider.value = PlayerPrefs.GetFloat("SFXVolume");
    }
    public void SetVolumePref()
    {
        PlayerPrefs.SetFloat("SFXVolume", sfxSlider.value);
    }
    
}
