using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class SnowmanCamera : MonoBehaviour
{
    [SerializeField] private Cinemachine.CinemachineFreeLook cinemachineFreeLook;
    [SerializeField] private CinemachineCameraOffset cameraOffset;
    [SerializeField] private InputActionReference snowmanZoom;
    [SerializeField] private InputActionReference cycleSnowmanView;
    private Cinemachine.CinemachineVirtualCamera currentFPSCam;
    private Cinemachine.CinemachineFreeLook currentThirdPersonCam;
    Snowman currentSnowmanTarget;
    bool firstPerson = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void SetSnowmanTarget(Snowman snowman)
    {
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
        if(currentSnowmanTarget.thirdPersonCam == null)
        {
            Cinemachine.CinemachineFreeLook camCopy = Instantiate(cinemachineFreeLook.gameObject, currentSnowmanTarget.transform).GetComponent<Cinemachine.CinemachineFreeLook>();
            currentSnowmanTarget.thirdPersonCam = camCopy;
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
        if (currentFPSCam != null)
        {
            currentFPSCam.gameObject.SetActive(false);
        }
    }

    public void ActivateFirstPersonCam()
    {
        if (currentSnowmanTarget == null)
        {
            SnowmanManager.instance.ActivatePlayerCamera();
            return;
        }
        if (currentFPSCam != null)
        {
            currentFPSCam.gameObject.SetActive(false);
        }
        currentFPSCam = currentSnowmanTarget.GetSnowmanFPSCam();
        if(currentFPSCam == null)
        {
            ActivateThirdPersonCam();
            return;
        }
        firstPerson = true;
        currentFPSCam.Follow = currentSnowmanTarget.transform;
        currentFPSCam.gameObject.SetActive(true);
        if(currentThirdPersonCam != null)
        {
            currentThirdPersonCam.gameObject.SetActive(false);
        }
        //cinemachineFreeLook.gameObject.SetActive(false);
    }

    public void DeactivateCameras()
    {
        if (currentFPSCam != null)
        {
            currentFPSCam.gameObject.SetActive(false);
        }
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
            if (firstPerson)
            {
                ActivateThirdPersonCam();
            } else
            {
                ActivateFirstPersonCam();
            }
        }
    }
}
