using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class CameraControls : MonoBehaviour
{
    [SerializeField] private InputActionReference movement;
    [SerializeField] private InputActionReference fasterCamAction;
    [SerializeField] private InputActionReference performAction;
    [SerializeField] private Camera _camera;
    [SerializeField] private SnowmanCamera snowmanCamera;
    public LayerMask snowmanMask;
    Outline currentOutline;
    
    private void Awake()
    {
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
        Ray ray = new Ray(transform.position, _camera.ScreenToWorldPoint(mousePos));
        Debug.DrawLine(transform.position, _camera.ScreenToWorldPoint(mousePos));
        if (Physics.Raycast(transform.position, _camera.ScreenToWorldPoint(mousePos) - transform.position, out info, _camera.farClipPlane, snowmanMask))
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
        }
        else
        {
            RemoveOutline();
        }
    }

    bool followSnowman = false;
    void SwitchCamera()
    {
        if (EventSystem.current.IsPointerOverGameObject()) { RemoveOutline(); return; }
        if (currentOutline != null)
        {
            if (currentOutline.transform.GetComponent<Snowman>())
            {
                snowmanCamera.ActivateCamera(currentOutline.transform);
            }
            RemoveOutline();
        }
    }

    Vector3 movementAxis;
    bool fasterCam = false;
    private void Update()
    {
        movementAxis = movement.action.ReadValue<Vector3>();
        fasterCam = fasterCamAction.action.ReadValue<float>() > 0.5f;
        transform.position += _camera.transform.rotation * movementAxis * Time.deltaTime * (fasterCam ? 30 : 10);
        HoverSnowman();
        if (performAction.action.triggered)
        {
            SwitchCamera();
        }
    }
}
