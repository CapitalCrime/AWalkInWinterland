using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraSpeedSlider : MonoBehaviour
{
    public void SetPlayerCameraSpeed(float value)
    {
        PlayerData.SetCameraSpeed(value);
    }

    public void SetSnowmanCameraSpeed(float value)
    {
        PlayerData.SetSnowmanCameraSpeed(value);
    }
}
