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
    [SerializeField] float playerMoveBounds;
    [SerializeField] float playerMoveHeightBounds;
    [SerializeField] Vector3 centerOfWorld;
    public LayerMask snowmanMask;
    Outline currentOutline;
    Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        provider.XYAxis.action.started += MousePressed;
        provider.XYAxis.action.canceled += MouseReleased;
    }

    void MousePressed(InputAction.CallbackContext context)
    {
        Cursor.lockState = CursorLockMode.Confined;
    }

    void MouseReleased(InputAction.CallbackContext context)
    {
        Cursor.lockState = CursorLockMode.None;
    }

    private void FixedUpdate()
    {
        rb.velocity = _camera.transform.rotation * movementAxis * Time.fixedDeltaTime * (fasterCam ? 30 : 10) * 60;
        //rb.MovePosition(rb.position + _camera.transform.rotation * movementAxis * Time.fixedDeltaTime * (fasterCam ? 30 : 10));

        //Vector3 pos = transform.position + _camera.transform.rotation * movementAxis * Time.deltaTime * (fasterCam ? 30 : 10);
        //pos.x = Mathf.Clamp(pos.x, -playerMoveBounds+centerOfWorld.x, playerMoveBounds + centerOfWorld.x);
        //pos.y = Mathf.Clamp(pos.y, 1+centerOfWorld.y, playerMoveHeightBounds + centerOfWorld.y);
        //pos.z = Mathf.Clamp(pos.z, -playerMoveBounds + centerOfWorld.z, playerMoveBounds + centerOfWorld.z);
        //transform.position = pos;
    }

    Vector3 movementAxis;
    bool fasterCam = false;
    private void Update()
    {
        movementAxis = movement.action.ReadValue<Vector3>();
        fasterCam = fasterCamAction.action.ReadValue<float>() > 0.5f;
        //transform.position += _camera.transform.rotation * movementAxis * Time.deltaTime * (fasterCam ? 30 : 10);
    }
}
