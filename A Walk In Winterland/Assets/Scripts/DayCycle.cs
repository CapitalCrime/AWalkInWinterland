using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DayCycle : MonoBehaviour
{
    public LightingPreset lightPreset;
    public static event UnityAction dayActions;
    public static event UnityAction nightActions;
    Light sun;
    static DayCycle instance;
    bool currentlyDay;
    private void Awake()
    {
        instance = this;
        if(TryGetComponent(out Light light))
        {
            sun = light;
        } else
        {
            Debug.LogError("DayCycle not attatched to light object");
        }
        currentlyDay = IsSunUp();
    }

    static float SunDirection()
    {
        return Vector3.Dot(Vector3.up, instance.transform.forward);
    }

    float normalizeValue(float value, float min, float max)
    {
        return (value - min) / (max - min);
    }

    public static bool IsSunUp()
    {
        return SunDirection() < 0.3f;
    }

    void Update()
    {
        transform.Rotate(Vector3.right * Time.deltaTime * (IsSunUp() ? 0.5f : 2));
        if(IsSunUp() && !currentlyDay)
        {
            currentlyDay = true;
            dayActions?.Invoke();
        }
        if(!IsSunUp() && currentlyDay)
        {
            currentlyDay = false;
            nightActions?.Invoke();
        }
        float daylightLerp = Mathf.Clamp(normalizeValue(SunDirection(), -0.3f, 0.3f),0,1);
        RenderSettings.ambientSkyColor = lightPreset.skyGradient.Evaluate(daylightLerp);
        RenderSettings.ambientEquatorColor = lightPreset.equatorGradient.Evaluate(daylightLerp);
        RenderSettings.reflectionIntensity = Mathf.Clamp(1-(daylightLerp/1.5f),0,1);
        sun.intensity = Mathf.Clamp(1-daylightLerp, 0, 1);
    }
}
