using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SnowmanDropSlider : MonoBehaviour
{
    public Slider slider;

    // Update is called once per frame
    void Update()
    {
        slider.value = SnowmanManager.instance.TimeSinceDrop() / SnowmanManager.instance.snowmanDropTime;
    }
}
