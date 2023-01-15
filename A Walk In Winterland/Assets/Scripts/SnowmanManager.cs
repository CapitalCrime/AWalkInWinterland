using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class SnowmanManager : MonoBehaviour
{
    [SerializeField] private Camera _camera;
    Outline currentOutline;
    [SerializeField] private Cinemachine.CinemachineFreeLook cinemachineFreeLook;
    [SerializeField] private SnowmanCamera snowmanCamera;
    [SerializeField] private CameraControls playerCamera;
    [SerializeField] private InputActionReference performAction;
    [SerializeField] private InputActionReference changeSnowmanCam;
    public LayerMask snowmanMask;

    public static SnowmanManager instance;
    List<Snowman> snowmen;
    int index = 0;
    Snowman currentViewSnowman;
    void Awake()
    {
        Snowman.snowmanCreatedEvent += AddSnowman;
        snowmen = new List<Snowman>();
        instance = this;
    }

    public void AddSnowman(Snowman snowman)
    {
        snowmen.Add(snowman);
    }

    public void setCurrentLookIndex(int index)
    {
        this.index = Mathf.Clamp(index, 0, snowmen.Count-1);
    }

    public void increaseLookIndex(int amount)
    {
        index += amount;
        if(index < 0)
        {
            index = snowmen.Count - 1;
        } else if(index > snowmen.Count - 1)
        {
            index = 0;
        }
    }

    public Snowman GetCurrentSnowman()
    {
        if(snowmen.Count != 0 && index < snowmen.Count)
        {
            return snowmen[index];
        } else
        {
            return null;
        }
    }

    int GetSnowmanIndex(Snowman snowman)
    {
        return snowmen.IndexOf(snowman);
    }

    void RemoveOutline()
    {
        if (currentOutline != null)
        {
            currentOutline.enabled = false;
            currentOutline = null;
        }
    }

    void HoverSnowman()
    {
        if (EventSystem.current.IsPointerOverGameObject()) { RemoveOutline(); return; }
        RaycastHit info;
        Vector3 mousePos = Mouse.current.position.ReadValue();
        mousePos.z = _camera.farClipPlane;
        Ray ray = new Ray(_camera.transform.position, _camera.ScreenToWorldPoint(mousePos));
        Debug.DrawLine(_camera.transform.position, _camera.ScreenToWorldPoint(mousePos));
        if (Physics.Raycast(_camera.transform.position, _camera.ScreenToWorldPoint(mousePos) - _camera.transform.position, out info, _camera.farClipPlane, snowmanMask))
        {
            if (currentViewSnowman == null || info.transform != currentViewSnowman.transform)
            {
                if (info.transform.TryGetComponent(out Outline outline))
                {
                    if (currentOutline != outline)
                    {
                        if (currentOutline != null) currentOutline.enabled = false;
                        currentOutline = outline;
                        currentOutline.enabled = true;
                    }
                }
            } else
            {
                RemoveOutline();
            }
        }
        else
        {
            RemoveOutline();
        }
    }

    public void SwitchCamera()
    {
        if (EventSystem.current.IsPointerOverGameObject()) { RemoveOutline(); return; }
        if (currentOutline != null)
        {
            if (currentOutline.transform.TryGetComponent(out Snowman snowman))
            {
                index = snowmen.IndexOf(snowman);
                ActivateSnowmanCamera(snowman);
            }
            else
            {
                ActivatePlayerCamera();
            }
            RemoveOutline();
        } else
        {
            ActivatePlayerCamera();
        }
    }

    public void ActivateSnowmanCamera(Snowman snowman)
    {
        if (snowman == null) return;
        if (snowman == currentViewSnowman) { ActivatePlayerCamera(); return; }

        index = GetSnowmanIndex(snowman);
        currentViewSnowman = snowman;
        snowmanCamera.SetSnowmanTarget(snowman);
        snowmanCamera.ActivateCamera();
        playerCamera.gameObject.SetActive(false);
    }

    public void ActivatePlayerCamera()
    {
        if (playerCamera.gameObject.activeSelf) return;
        currentViewSnowman = null;
        playerCamera.transform.position = _camera.transform.position - _camera.transform.forward*2 + Vector3.up;
        playerCamera.gameObject.SetActive(true);
        snowmanCamera.DeactivateCameras();
    }

    void ViewNextSnowman(float direction)
    {
        if (!snowmanCamera.gameObject.activeSelf)
        {
            setCurrentLookIndex(0);
        }
        else
        {
            increaseLookIndex(direction < 0 ? -1 : 1);
        }

        ActivateSnowmanCamera(GetCurrentSnowman());
    }

    // Update is called once per frame
    float changeSnowmanIndex;
    void Update()
    {
        changeSnowmanIndex = changeSnowmanCam.action.ReadValue<float>();

        HoverSnowman();
        if (performAction.action.triggered)
        {
            SwitchCamera();
        }
        if (changeSnowmanCam.action.WasPressedThisFrame() && changeSnowmanIndex != 0)
        {
            ViewNextSnowman(changeSnowmanIndex);
        }
    }
}
