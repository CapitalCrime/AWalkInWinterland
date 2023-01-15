using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class CameraControls : MonoBehaviour
{
    [SerializeField] private InputActionReference movement;
    [SerializeField] private InputActionReference fasterCamAction;
    [SerializeField] private Camera _camera;
    [SerializeField] private SnowmanCamera snowmanCamera;
    [SerializeField] private Cinemachine.CinemachineInputProvider provider;
    public LayerMask snowmanMask;
    Outline currentOutline;

    private void Awake()
    {
    }

    Vector3 movementAxis;
    bool fasterCam = false;
    float changeSnowmanIndex;
    private void Update()
    {
        movementAxis = movement.action.ReadValue<Vector3>();
        fasterCam = fasterCamAction.action.ReadValue<float>() > 0.5f;
        transform.position += _camera.transform.rotation * movementAxis * Time.deltaTime * (fasterCam ? 30 : 10);
        Debug.Log(provider.XYAxis.action.bindings[0]);
        Cursor.lockState = CursorLockMode.Confined;
    }
}
