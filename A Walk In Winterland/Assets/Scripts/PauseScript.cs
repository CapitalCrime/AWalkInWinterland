using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PauseScript : MonoBehaviour
{
    private InputAction pauseAction;
    private InputActionMap playMap;
    bool paused = false;
    private void Awake()
    {
        pauseAction = InputManager.instance.playerInputs.actions.FindActionMap("Menu").FindAction("Pause");
        playMap = InputManager.instance.playerInputs.actions.FindActionMap("PlayMode");
        pauseAction.performed += PauseGame;
        pauseAction.actionMap.Enable();
    }

    private void OnDisable()
    {
        pauseAction.performed -= PauseGame;
    }

    private void PauseGame(InputAction.CallbackContext context)
    {
        paused = !paused;
        if (paused)
        {
            playMap.Disable();
        } else
        {
            playMap.Enable();
        }
    }
}
