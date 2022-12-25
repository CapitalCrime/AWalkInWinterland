using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class SnowmanCamera : MonoBehaviour
{
    [SerializeField] private CameraControls playerCamera;
    [SerializeField] private Cinemachine.CinemachineFreeLook cinemachineFreeLook;
    [SerializeField] private CinemachineCameraOffset cameraOffset;
    [SerializeField] private InputActionReference performAction;
    [SerializeField] private InputActionReference snowmanZoom;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void ActivateCamera(Transform snowman)
    {
        cinemachineFreeLook.Follow = snowman;
        cinemachineFreeLook.LookAt = snowman;
        gameObject.SetActive(true);
        playerCamera.gameObject.SetActive(false);
    }

    void DeactivateCamera()
    {
        playerCamera.gameObject.SetActive(true);
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        float zoomAmount = snowmanZoom.action.ReadValue<float>();
        if(zoomAmount != 0)
        {
            zoomAmount /= Mathf.Abs(zoomAmount);
            cameraOffset.m_Offset = new Vector3(0, 0, Mathf.Clamp(cameraOffset.m_Offset.z + zoomAmount/2, -10, 5));
        }
        if (performAction.action.triggered && !EventSystem.current.IsPointerOverGameObject())
        {
            DeactivateCamera();
        }
    }
}
