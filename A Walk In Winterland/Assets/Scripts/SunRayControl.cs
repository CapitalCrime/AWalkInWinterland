using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(VolumetricFogAndMist2.VolumetricFog))]
public class SunRayControl : MonoBehaviour
{
    public Transform camera;
    public Transform sun;
    VolumetricFogAndMist2.VolumetricFog fog;
    public float maxDensity = 0.3f;
    public float minDensity = 0.02f;
    public float baseDensity = 0.01f;
    [Range(0,1)]
    public float startFadeSunAngle = 0.5f;
    [Range(0, 1)]
    public float endFadeSunAngle = 0.3f;

    private void Awake()
    {
        fog = GetComponent<VolumetricFogAndMist2.VolumetricFog>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float alignedUnclamp = Mathf.Pow((Vector3.Dot(camera.forward, (sun.position - camera.position).normalized) - 0.75f) * 4f, 3);
        float alignedMax = Mathf.Clamp(alignedUnclamp, 0, 1);
        float alignedMin = Mathf.Clamp(alignedUnclamp/40, -1, 0);
        float sunUpwards = Vector3.Dot(Vector3.up, sun.position.normalized);
        Debug.Log("Sun upwards: "+sunUpwards);
        if(sunUpwards < startFadeSunAngle)
        {
            sunUpwards = Mathf.Clamp(sunUpwards - endFadeSunAngle, 0, startFadeSunAngle) / (startFadeSunAngle - endFadeSunAngle);
        } else
        {
            sunUpwards = 1;
        }
        fog.settings.density = baseDensity + ( ((minDensity-baseDensity) * (1+alignedMin)) + (alignedMax) * (maxDensity - minDensity) )*sunUpwards;
    }
}
