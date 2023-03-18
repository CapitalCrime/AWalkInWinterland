using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VirtualCamSettings : MonoBehaviour
{
    Cinemachine.CinemachinePOV virtualCamera;

    private void Awake()
    {
        if (TryGetComponent(out Cinemachine.CinemachineVirtualCamera virtCam))
        {
            virtualCamera = virtCam.GetCinemachineComponent<Cinemachine.CinemachinePOV>();
        }
        else
        {
            Debug.LogError("Object " + gameObject.name + " is missing CinemachineVirtualCamera component");
        }
    }

    private void OnEnable()
    {
        PlayerData.setPlayerCameraSpeedEvent.AddListener(SetLookSpeed);
        SetLookSpeed(PlayerData.GetPlayerCameraSpeed());
    }

    private void OnDisable()
    {
        PlayerData.setPlayerCameraSpeedEvent.RemoveListener(SetLookSpeed);
    }

    void SetLookSpeed(float trueValue)
    {
        virtualCamera.m_HorizontalAxis.m_MaxSpeed = trueValue;
        virtualCamera.m_VerticalAxis.m_MaxSpeed = trueValue;
    }
}
