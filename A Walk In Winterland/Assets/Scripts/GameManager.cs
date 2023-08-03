using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public static event UnityAction<ControllerType> OnControllerChange;
    public int startingQuality = 1;

    private void Awake()
    {
        //QualitySettings.vSyncCount = 1;
        Application.targetFrameRate = 60;
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        QualitySettings.SetQualityLevel(startingQuality);
        InputSystem.onDeviceChange += HandleDeviceChange;
    }

    void HandleDeviceChange(InputDevice device, InputDeviceChange change)
    {
        switch (change)
        {
            case InputDeviceChange.Added:
                if (device is Gamepad)
                {
                    PlayerData.SetCurrentController(ControllerType.Controller);
                    InputSystem.DisableDevice(Keyboard.current);
                    InputSystem.DisableDevice(Mouse.current);
                    InputSystem.EnableDevice(Gamepad.current);
                }
                else
                {
                    PlayerData.SetCurrentController(ControllerType.Keyboard);
                    InputSystem.EnableDevice(Keyboard.current);
                    InputSystem.EnableDevice(Mouse.current);
                    InputSystem.DisableDevice(Gamepad.current);
                }
                break;
            case InputDeviceChange.Removed:
                if (device is Gamepad)
                {
                    PlayerData.SetCurrentController(ControllerType.Keyboard);
                    InputSystem.EnableDevice(Keyboard.current);
                    InputSystem.EnableDevice(Mouse.current);
                    InputSystem.DisableDevice(Gamepad.current);
                }
                break;
            case InputDeviceChange.Disconnected:
                if (device is Gamepad)
                {
                    PlayerData.SetCurrentController(ControllerType.Keyboard);
                    InputSystem.EnableDevice(Keyboard.current);
                    InputSystem.EnableDevice(Mouse.current);
                    InputSystem.DisableDevice(Gamepad.current);
                }
                break;
        }
        OnControllerChange?.Invoke(PlayerData.controller);
    }

    public void CloseGame()
    {
        Application.Quit();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
