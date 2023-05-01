using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using System.Linq;

public class SnowmanCamera : MonoBehaviour
{
    [SerializeField] private LayerMask terrainBoundariesMask;
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

    void DisableThirdPersonCam()
    {
        if (currentThirdPersonCam != null)
        {
            currentThirdPersonCam.GetComponent<CinemachineCameraOffset>().m_Offset.z = realOffsetZoom;
            currentThirdPersonCam.gameObject.SetActive(false);
        }
    }

    public void ActivateThirdPersonCam()
    {
        if (currentSnowmanTarget == null)
        {
            SnowmanManager.instance.ActivatePlayerCamera();
            return;
        }
        DisableThirdPersonCam();
        currentThirdPersonCam = currentSnowmanTarget.thirdPersonCam;
        cameraOffset = currentThirdPersonCam.GetComponent<CinemachineCameraOffset>();
        cameraOffset.m_Offset.z = realOffsetZoom;
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
        DisableThirdPersonCam();
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
        DisableThirdPersonCam();
        //cinemachineFreeLook.gameObject.SetActive(false);
    }

    bool rayHit = false;

    private void LateUpdate()
    {
        if (SnowmanManager.instance.cinemachineBrain.IsBlending) return;
        if(cameraOffset != null)
        {
            Vector3 snowmanPos = currentThirdPersonCam.LookAt.position;
            Vector3 rayDir = (SnowmanManager.instance._camera.transform.position - snowmanPos);
            Debug.DrawRay(snowmanPos, rayDir.normalized * rayDir.magnitude);
            RaycastHit[] hits = Physics.RaycastAll(snowmanPos, rayDir.normalized, rayDir.magnitude+0.5f, terrainBoundariesMask);
            Debug.Log("Hits: " + hits.Length);
            if (hits.Length > 0)
            {
                rayHit = true;
                hits = hits.OrderBy(x => Vector3.Distance(x.transform.position, snowmanPos)).ToArray();
                Vector3 pos = hits[hits.Length - 1].point;
                Vector3 cameraPosition = pos - rayDir.normalized * 2;
                cameraOffset.m_Offset.z += (SnowmanManager.instance._camera.transform.position - cameraPosition).magnitude*Time.deltaTime*6 + Time.deltaTime*2;
                cameraOffset.m_Offset.z = Mathf.Clamp(cameraOffset.m_Offset.z, realOffsetZoom, 7.5f);
            } else if(!Physics.Raycast(SnowmanManager.instance._camera.transform.position, -SnowmanManager.instance._camera.transform.forward, 1, terrainBoundariesMask))
            {
                if (rayHit)
                {
                    if (cameraOffset.m_Offset.z > realOffsetZoom)
                    {
                        cameraOffset.m_Offset.z = Mathf.Lerp(cameraOffset.m_Offset.z, realOffsetZoom, Time.deltaTime);
                    }
                    else
                    {
                        rayHit = false;
                    }
                } else
                {
                    cameraOffset.m_Offset.z = realOffsetZoom;
                }
            } else
            {
                cameraOffset.m_Offset.z = Mathf.Clamp(cameraOffset.m_Offset.z, realOffsetZoom, 7.5f);
            }
        }
    }

    [SerializeField] float realOffsetZoom = 3.5f;

    // Update is called once per frame
    void Update()
    {
        float zoomAmount = snowmanZoom.action.ReadValue<float>();
        if(zoomAmount != 0)
        {
            zoomAmount /= Mathf.Abs(zoomAmount);
            realOffsetZoom = Mathf.Clamp(realOffsetZoom + zoomAmount / 2, -10, 6);
            //cameraOffset.m_Offset.z = realOffsetZoom;
        }
        if (cycleSnowmanView.action.WasPerformedThisFrame() && !SnowmanManager.instance.PlayerCameraActive())
        {
            SwapCam();
        }
    }
}
