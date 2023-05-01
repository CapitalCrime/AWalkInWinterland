using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class FreeCamSettings : MonoBehaviour
{
    [SerializeField] private LayerMask terrainBoundariesMask;
    Cinemachine.CinemachineFreeLook freeLookCam;
    [SerializeField] Vector3[] rigSettings = new Vector3[3];

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
        SetLookSpeed(PlayerData.GetSnowmanCameraSpeed());
    }

    private void LateUpdate()
    {
    }

    private void OnDisable()
    {
        PlayerData.setSnowmanCameraSpeedEvent.RemoveListener(SetLookSpeed);
    }

    void SetLookSpeed(float trueValue)
    {
        freeLookCam.m_XAxis.m_MaxSpeed = trueValue;
        freeLookCam.m_YAxis.m_MaxSpeed = trueValue / 75;
    }
}
