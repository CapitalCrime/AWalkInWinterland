using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PauseScript : MonoBehaviour
{
    private InputAction pauseAction;
    private InputActionMap playMap;
    private InputActionMap nonMenuMap;
    [SerializeField] GameObject pauseMenu;
    public UnityEvent<bool> OnFullscreen;
    static bool paused = false;
    public static bool snowmanIndexOpen = false;
    private bool fullscreen;

    private void Awake()
    {
        fullscreen = Screen.fullScreen;
        pauseAction = InputManager.instance.playerInputs.actions.FindActionMap("Menu").FindAction("Pause");
        playMap = InputManager.instance.playerInputs.actions.FindActionMap("PlayMode");
        nonMenuMap = InputManager.instance.playerInputs.actions.FindActionMap("NonMenu");
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
        if(pauseMenu.TryGetComponent(out PauseMenuElements elements))
        {
            elements.DeactivateAllTabs();
        }

        if (paused)
        {
            OnFullscreen?.Invoke(Screen.fullScreen);
            Time.timeScale = 0;
            if(AudioSettings.instance != null)
            {
                AudioSettings.instance.PauseSFX(true);
                //AudioSettings.instance.PauseMusic(true);
                //AudioSettings.instance.PauseAmbience(true);
                AudioSettings.instance.SetMusicVolumeRelative(0.33f);
                AudioSettings.instance.SetAmbienceVolumeRelative(0.33f);
            }
            playMap.Disable();
            nonMenuMap.Disable();
        } else
        {
            Time.timeScale = 1;
            if (AudioSettings.instance != null)
            {
                AudioSettings.instance.PauseSFX(false);
                //AudioSettings.instance.PauseMusic(false);
                //AudioSettings.instance.PauseAmbience(false);
                AudioSettings.instance.SetMusicVolumeRelative(1);
                AudioSettings.instance.SetAmbienceVolumeRelative(1);
            }
            playMap.Enable();
            if (!snowmanIndexOpen)
            {
                nonMenuMap.Enable();
            }
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
