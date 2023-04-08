using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VolumeSliders : MonoBehaviour
{
    public Slider ambienceSlider;
    public Slider musicSlider;
    public Slider sfxSlider;
    private void Start()
    {
        ambienceSlider.value = PlayerData.ambienceVolume;
        musicSlider.value = PlayerData.musicVolume;
        sfxSlider.value = PlayerData.sfxVolume;
    }

    public void SetAmbienceSlider(float value)
    {
        PlayerData.ambienceVolume = value;
        AudioSettings.instance.AmbienceVolume(value);
    }

    public void SetMusicSlider(float value)
    {
        PlayerData.musicVolume = value;
        AudioSettings.instance.MusicVolume(value);
    }

    public void SetSFXSlider(float value)
    {
        PlayerData.sfxVolume = value;
        AudioSettings.instance.SFXVolume(value);
    }
}
