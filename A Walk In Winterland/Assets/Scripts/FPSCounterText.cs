using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FPSCounterText : MonoBehaviour
{
    TextMeshProUGUI fpsText;
    [SerializeField] float refreshRateSeconds;
    float timer;
    // Start is called before the first frame update
    void Awake()
    {
        if (TryGetComponent(out TextMeshProUGUI text))
        {
            fpsText = text;
        } else
        {
            Debug.LogError("Object " + gameObject.name + " is missing TextMesh text component");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(Time.unscaledTime > timer)
        {
            int fps = (int)(1f / Time.unscaledDeltaTime);
            fpsText.text = "FPS: " + fps;
            timer = Time.unscaledTime + refreshRateSeconds;
        }
    }
}
