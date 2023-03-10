using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PauseScript : MonoBehaviour
{
    private InputAction pauseAction;
    private InputActionMap playMap;
    [SerializeField] GameObject pauseMenu;
    public UnityEvent<bool> OnFullscreen;
    static bool paused = false;
    private bool fullscreen;
    private void Awake()
    {
        fullscreen = Screen.fullScreen;
        pauseAction = InputManager.instance.playerInputs.actions.FindActionMap("Menu").FindAction("Pause");
        playMap = InputManager.instance.playerInputs.actions.FindActionMap("PlayMode");
        pauseAction.performed += PauseGame;
        pauseAction.actionMap.Enable();
    }

    public static bool isPaused()
    {
        return paused;
    }

    private void OnDisable()
    {
        pauseAction.performed -= PauseGame;
    }

    private void PauseGame(InputAction.CallbackContext context)
    {
        paused = !paused;
        pauseMenu.SetActive(paused);
        if (paused)
        {
            OnFullscreen?.Invoke(Screen.fullScreen);
            Time.timeScale = 0;
            if(AudioSettings.instance != null)
            {
                AudioSettings.instance.PauseSFX(true);
            }
            playMap.Disable();
        } else
        {
            Time.timeScale = 1;
            if (AudioSettings.instance != null)
            {
                AudioSettings.instance.PauseSFX(false);
            }
            playMap.Enable();
        }
    }

    private void Update()
    {
        if (fullscreen != Screen.fullScreen)
        {
            fullscreen = Screen.fullScreen;
            OnFullscreen?.Invoke(Screen.fullScreen);
        }
    }
}
