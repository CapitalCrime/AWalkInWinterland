using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using System.Runtime.InteropServices;

public class CameraControls : MonoBehaviour
{
    public static CameraControls current;
    [SerializeField] private InputActionReference movement;
    [SerializeField] private InputActionReference fasterCamAction;
    [SerializeField] private Camera _camera;
    [SerializeField] private Collider roughCameraBounds;
    [SerializeField] private SnowmanCamera snowmanCamera;
    [SerializeField] private Cinemachine.CinemachineInputProvider provider;
    [SerializeField] Texture2D gamepadCursor;
    Cinemachine.CinemachinePOV virtualCamera;
    public LayerMask snowmanMask;
    Outline currentOutline;
    Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        GameManager.OnControllerChange += CheckForController;
    }

    public Collider GetCameraRoughBounds()
    {
        return roughCameraBounds;
    }

    public Cinemachine.CinemachineVirtualCamera GetVirtualCamera()
    {
        return provider.GetComponent<Cinemachine.CinemachineVirtualCamera>();
    }

    private void Start()
    {
        virtualCamera = GetComponentInChildren<Cinemachine.CinemachinePOV>();
    }

    void CheckForController(ControllerType controller)
    {
        switch (controller)
        {
            case ControllerType.Controller:
                mouseLocked = false;
                Cursor.SetCursor(gamepadCursor, Vector2.one*50, CursorMode.Auto);
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = true;
                break;
            case ControllerType.Keyboard:
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                break;
        }
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
        if (mouseLocked)
        {
            if (PlayerData.lockMouse)
            {
                Cursor.lockState = CursorLockMode.None;
                Mouse.current.WarpCursorPosition(mouseLockPosition);
            } else
            {
                Cursor.lockState = CursorLockMode.Confined;
            }
        }
        rb.velocity = _camera.transform.rotation * movementAxis * Time.fixedDeltaTime * (fasterCam ? 30 : 10) * 60;
        //rb.MovePosition(rb.position + _camera.transform.rotation * movementAxis * Time.fixedDeltaTime * (fasterCam ? 30 : 10));

        //Vector3 pos = transform.position + _camera.transform.rotation * movementAxis * Time.deltaTime * (fasterCam ? 30 : 10);
        //pos.x = Mathf.Clamp(pos.x, -playerMoveBounds+centerOfWorld.x, playerMoveBounds + centerOfWorld.x);
        //pos.y = Mathf.Clamp(pos.y, 1+centerOfWorld.y, playerMoveHeightBounds + centerOfWorld.y);
        //pos.z = Mathf.Clamp(pos.z, -playerMoveBounds + centerOfWorld.z, playerMoveBounds + centerOfWorld.z);
        //transform.position = pos;
    }

    private void OnEnable()
    {
        if (!Mouse.current.rightButton.isPressed)
        {
            Cursor.lockState = CursorLockMode.None;
            mouseLocked = false;
        }
    }

    Vector3 movementAxis;
    bool fasterCam = false;
    static Vector2 mouseLockPosition;
    static bool mouseLocked = false;
    private void Update()
    {
        if(PlayerData.controller == ControllerType.Keyboard)
        {
            if(virtualCamera != null)
            {
                if(Mouse.current.rightButton.wasPressedThisFrame)
                {
                    mouseLockPosition = Mouse.current.position.ReadValue();
                    mouseLocked = true;
                }
                if(Mouse.current.rightButton.wasReleasedThisFrame)
                {
                    Cursor.lockState = CursorLockMode.None;
                    mouseLocked = false;
                }
            }
        }

        if(PlayerData.controller == ControllerType.Controller)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.Locked;
        }
        movementAxis = movement.action.ReadValue<Vector3>();
        fasterCam = fasterCamAction.action.ReadValue<float>() > 0.5f;
        //transform.position += _camera.transform.rotation * movementAxis * Time.deltaTime * (fasterCam ? 30 : 10);
    }
}
