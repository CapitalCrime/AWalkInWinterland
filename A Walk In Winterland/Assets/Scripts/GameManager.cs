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
        QualitySettings.vSyncCount = 1;
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        CheckController();
        QualitySettings.SetQualityLevel(startingQuality);
        InputSystem.onDeviceChange += HandleDeviceChange;
    }

    void EnableGamepad()
    {
        if (Keyboard.current != null)
        {
            InputSystem.DisableDevice(Keyboard.current);
        }
        if (Mouse.current != null)
        {
            InputSystem.DisableDevice(Mouse.current);
        }
        if (Gamepad.current != null)
        {
            InputSystem.EnableDevice(Gamepad.current);
        }
    }

    PlayerInput inputs;

    void EnableKeyboard()
    {
        if(Keyboard.current != null)
        {
            InputSystem.EnableDevice(Keyboard.current);
        }
        if(Mouse.current != null)
        {
            InputSystem.EnableDevice(Mouse.current);
        }
        if(Gamepad.current != null)
        {
            InputSystem.DisableDevice(Gamepad.current);
        }
    }

    void CheckController()
    {
        if(Gamepad.current != null)
        {
            PlayerData.SetCurrentController(ControllerType.Controller);
            EnableGamepad();
        } else
        {
            PlayerData.SetCurrentController(ControllerType.Keyboard);
            EnableKeyboard();
        }
    }

    void HandleDeviceChange(InputDevice device, InputDeviceChange change)
    {
        switch (change)
        {
            case InputDeviceChange.Added:
                if (device is Gamepad)
                {
                    PlayerData.SetCurrentController(ControllerType.Controller);
                    EnableGamepad();
                }
                else
                {
                    PlayerData.SetCurrentController(ControllerType.Keyboard);
                    EnableKeyboard();
                }
                break;
            case InputDeviceChange.Removed:
                if (device is Gamepad)
                {
                    PlayerData.SetCurrentController(ControllerType.Keyboard);
                    EnableKeyboard();
                }
                break;
            case InputDeviceChange.Disconnected:
                if (device is Gamepad)
                {
                    PlayerData.SetCurrentController(ControllerType.Keyboard);
                    EnableKeyboard();
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
