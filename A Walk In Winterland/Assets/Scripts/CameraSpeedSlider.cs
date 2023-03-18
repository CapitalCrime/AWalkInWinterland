using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraSpeedSlider : MonoBehaviour
{
    enum CameraType
    {
        SnowmanCamera,
        PlayerCamera
    }

    [SerializeField] CameraType cameraType;
    private void Awake()
    {
        if(gameObject.TryGetComponent(out Slider slider))
        {
            if(cameraType == CameraType.SnowmanCamera)
            {
                slider.SetValueWithoutNotify(PlayerData.getNormalizedSnowmanCameraSpeed());
            } else
            {
                slider.SetValueWithoutNotify(PlayerData.getNormalizedCameraSpeed());
            }
        }
    }

    public void SetPlayerCameraSpeed(float value)
    {
        PlayerData.SetCameraSpeed(value);
    }

    public void SetSnowmanCameraSpeed(float value)
    {
        PlayerData.SetSnowmanCameraSpeed(value);
    }
}
