using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreeCamSettings : MonoBehaviour
{
    Cinemachine.CinemachineFreeLook freeLookCam;

    private void Awake()
    {
        if (TryGetComponent(out Cinemachine.CinemachineFreeLook freeCam))
        {
            freeLookCam = freeCam;
        }
        else
        {
            Debug.LogError("Object " + gameObject.name + " is missing CinemachineFreeLook component");
        }
    }

    private void OnEnable()
    {
        PlayerData.setSnowmanCameraSpeedEvent.AddListener(SetLookSpeed);
        PlayerData.GetSnowmanCameraSpeed();
    }

    private void OnDisable()
    {
        PlayerData.setSnowmanCameraSpeedEvent.RemoveListener(SetLookSpeed);
    }

    void SetLookSpeed(float trueValue)
    {
        freeLookCam.m_XAxis.m_MaxSpeed = trueValue;
        freeLookCam.m_YAxis.m_MaxSpeed = trueValue / 100;
    }
}
