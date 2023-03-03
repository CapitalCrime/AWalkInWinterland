using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioSettings : MonoBehaviour
{
    public static AudioSettings instance;
    [SerializeField] FMOD.Studio.Bus SFXBus;
    public void SFXVolume(float value)
    {
        SFXBus.setVolume(value);
    }

    public void PauseSFX(bool value)
    {
        SFXBus.setPaused(value);
    }

    private void Awake()
    {
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        SFXBus = FMODUnity.RuntimeManager.GetBus("bus:/SFX");
    }
}
