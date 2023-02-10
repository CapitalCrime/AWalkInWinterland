using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioSettings : MonoBehaviour
{
    public FMOD.Studio.Bus SFXBus;
    public void SFXVolume(Slider slider)
    {
        SFXBus.setVolume(slider.value);
    }
    // Start is called before the first frame update
    void Start()
    {
        SFXBus = FMODUnity.RuntimeManager.GetBus("bus:/SFX");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
