using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class SnowmanCamera : MonoBehaviour
{
    [SerializeField] private InputActionReference snowmanZoom;
    [SerializeField] private InputActionReference cycleSnowmanView;
    private CinemachineCameraOffset cameraOffset;
    private Cinemachine.CinemachineVirtualCamera currentFPSCam;
    private Cinemachine.CinemachineFreeLook currentThirdPersonCam;
    Snowman currentSnowmanTarget;
    bool firstPerson = false;

    public void SetSnowmanTarget(Snowman snowman)
    {
        EnableFPSCam(false);
        currentSnowmanTarget = snowman;
    }

    public void ActivateCamera()
    {
        if (firstPerson)
        {
            ActivateFirstPersonCam();
        } else
        {
            ActivateThirdPersonCam();
        }
    }

    void EnableFPSCam(bool value)
    {
        if(currentFPSCam != null)
        {
            currentFPSCam.gameObject.SetActive(value);
        }

        if (currentSnowmanTarget != null)
        {
            if (value)
            {
                currentSnowmanTarget.OnViewFPSEvent();
            }
            else
            {
                currentSnowmanTarget.OnLeaveFPSEvent();
            }
        }
    }

    public void ActivateThirdPersonCam()
    {
        if (currentSnowmanTarget == null)
        {
            SnowmanManager.instance.ActivatePlayerCamera();
            return;
        }
        if(currentThirdPersonCam != null)
        {
            currentThirdPersonCam.gameObject.SetActive(false);
        }
        currentThirdPersonCam = currentSnowmanTarget.thirdPersonCam;
        cameraOffset = currentThirdPersonCam.GetComponent<CinemachineCameraOffset>();
        firstPerson = false;
        currentThirdPersonCam.Follow = currentSnowmanTarget.transform;
        currentThirdPersonCam.LookAt = currentSnowmanTarget.transform;
        currentThirdPersonCam.gameObject.SetActive(true);
        //cinemachineFreeLook.Follow = currentSnowmanTarget.transform;
        //cinemachineFreeLook.LookAt = currentSnowmanTarget.transform;
        //cinemachineFreeLook.gameObject.SetActive(true);
        EnableFPSCam(false);
    }

    public void ActivateFirstPersonCam()
    {
        if (currentSnowmanTarget == null)
        {
            SnowmanManager.instance.ActivatePlayerCamera();
            return;
        }
        EnableFPSCam(false);
        currentFPSCam = currentSnowmanTarget.GetSnowmanFPSCam();
        if(currentFPSCam == null)
        {
            ActivateThirdPersonCam();
            return;
        }
        firstPerson = true;
        currentFPSCam.Follow = currentSnowmanTarget.transform;
        EnableFPSCam(true);
        if (currentThirdPersonCam != null)
        {
            currentThirdPersonCam.gameObject.SetActive(false);
        }
        //cinemachineFreeLook.gameObject.SetActive(false);
    }

    public void SwapCam()
    {
        if (firstPerson)
        {
            ActivateThirdPersonCam();
        }
        else
        {
            ActivateFirstPersonCam();
        }
    }

    public void DeactivateCameras()
    {
        EnableFPSCam(false);
        if(currentThirdPersonCam != null)
        {
            currentThirdPersonCam.gameObject.SetActive(false);
        }
        //cinemachineFreeLook.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        float zoomAmount = snowmanZoom.action.ReadValue<float>();
        if(zoomAmount != 0)
        {
            zoomAmount /= Mathf.Abs(zoomAmount);
            cameraOffset.m_Offset = new Vector3(0, 0, Mathf.Clamp(cameraOffset.m_Offset.z + zoomAmount/2, -10, 6));
        }
        if (cycleSnowmanView.action.WasPerformedThisFrame() && !SnowmanManager.instance.PlayerCameraActive())
        {
            SwapCam();
        }
    }
}
