using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraSpeedSlider : MonoBehaviour
{
    Slider slider;
    // Start is called before the first frame update
    void Awake()
    {
        if(TryGetComponent(out Slider slider))
        {
            this.slider = slider;
        }
        else
        {
            Debug.Log("Object" + gameObject.name + " is missing Slider component");
        }
        slider.onValueChanged.AddListener(PlayerData.SetCameraSpeed);
    }
}
