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
        PlayerData.setCameraSpeedEvent.AddListener(SetLookSpeed);
        SetLookSpeed(PlayerData.GetCameraSpeed());
    }

    private void OnDisable()
    {
        PlayerData.setCameraSpeedEvent.RemoveListener(SetLookSpeed);
    }

    void SetLookSpeed(float trueValue)
    {
        freeLookCam.m_XAxis.m_MaxSpeed = trueValue;
        freeLookCam.m_YAxis.m_MaxSpeed = trueValue / 100;
    }
}
