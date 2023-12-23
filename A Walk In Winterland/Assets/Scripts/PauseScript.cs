using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PauseScript : MonoBehaviour
{
    static PauseScript instance;
    [SerializeField] private InputActionReference pauseAction;
    private PlayerInput playerInput;
    private InputActionMap nonMenuMap;
    [SerializeField] GameObject pauseMenu;
    [SerializeField] VolumeProfile profile;
    public UnityEvent<bool> OnFullscreen;
    public UnityEvent OnPause;
    public UnityEvent OnUnpause;
    static bool paused = false;
    public static bool snowmanIndexOpen = false;
    private bool fullscreen;

    private void Awake()
    {
        instance = this;
        fullscreen = Screen.fullScreen;
        playerInput = InputManager.instance.playerInputs;
        nonMenuMap = playerInput.actions.FindActionMap("NonMenu");
        pauseAction.action.performed += PauseGame;
        pauseAction.action.actionMap.Enable();
        GameManager.snowmanIndexOpen += SetSnowmanIndexOpen;
    }

    public static void AddListenerPause(UnityAction action)
    {
        if (instance == null) return;
        instance.OnPause.AddListener(action);
    }

    public static void RemoveListenerPause(UnityAction action)
    {
        if (instance == null) return;
        instance.OnPause.RemoveListener(action);
    }

    void SetSnowmanIndexOpen(bool value){
        snowmanIndexOpen = value;
    }

    public static bool isPaused()
    {
        return paused;
    }

    private void OnDisable()
    {
        pauseAction.action.performed -= PauseGame;
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
            if(Gamepad.all.Count > 0)
            {
                Cursor.visible = false;
            }
            SnowmanManager.instance.GetCurrentMap().Disable();
            nonMenuMap.Disable();
            OnPause?.Invoke();
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

            SnowmanManager.instance.GetCurrentMap().Enable();

            if (!snowmanIndexOpen)
            {
                nonMenuMap.Enable();
            }
            Cursor.visible = true;
            OnUnpause?.Invoke();
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
