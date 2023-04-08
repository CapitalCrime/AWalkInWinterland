using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioSettings : MonoBehaviour
{
    public static AudioSettings instance;
    [SerializeField] FMOD.Studio.Bus SFXBus;
    FMOD.Studio.Bus MusicBus;
    FMOD.Studio.Bus AmbienceBus;
    float relativeSFXVolume = 1;
    float relativeMusicVolume = 1;
    float relativeAmbienceVolume = 1;
    public void SFXVolume(float value)
    {
        SFXBus.setVolume(value * relativeSFXVolume);
    }

    public void PauseSFX(bool value)
    {
        SFXBus.setPaused(value);
    }

    public void SetMusicVolumeRelative(float valueNormalized)
    {
        relativeMusicVolume = valueNormalized;
        MusicVolume(PlayerData.musicVolume);
    }

    public void MusicVolume(float value)
    {
        MusicBus.setVolume(value * relativeMusicVolume);
    }

    public void PauseMusic(bool value)
    {
        MusicBus.setPaused(value);
    }

    public void SetAmbienceVolumeRelative(float valueNormalized)
    {
        relativeAmbienceVolume = valueNormalized;
        AmbienceVolume(PlayerData.ambienceVolume);
    }

    public void AmbienceVolume(float value)
    {
        AmbienceBus.setVolume(value * relativeAmbienceVolume);
    }

    public void PauseAmbience(bool value)
    {
        AmbienceBus.setPaused(value);
    }

    private void Awake()
    {
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        SFXBus = FMODUnity.RuntimeManager.GetBus("bus:/SFX");
        MusicBus = FMODUnity.RuntimeManager.GetBus("bus:/Music");
        AmbienceBus = FMODUnity.RuntimeManager.GetBus("bus:/Ambience");

        SFXBus.setVolume(PlayerData.sfxVolume);
        MusicBus.setVolume(PlayerData.musicVolume);
        AmbienceBus.setVolume(PlayerData.ambienceVolume);
    }
}
