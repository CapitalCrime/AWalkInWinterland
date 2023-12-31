using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.Events;

public class SnowmanUIButton : MonoBehaviour
{
    public RectTransform imageHolder;
    private InputActionMap nonMenuMap;
    [SerializeField] private InputActionReference openIndex;
    [HideInInspector] public bool opening = false;
    [SerializeField] UnityEvent<bool> openAction;

    private void Start()
    {
        nonMenuMap = InputManager.instance.playerInputs.actions.FindActionMap("NonMenu");
        GameManager.OnControllerChange += OnControllerChange;
        PauseScript.RemoveListenerPause(InstantClose);
        PauseScript.AddListenerPause(InstantClose);
    }

    void OnControllerChange(ControllerType controllerType)
    {
        switch (controllerType)
        {
            case ControllerType.Controller:
                break;
            case ControllerType.Keyboard:
                break;
        }
    }

    void EnableNonMenuMap(bool value)
    {
        if (value)
        {
            if (!PauseScript.isPaused())
            {
                nonMenuMap.Enable();
            }
        } else
        {
            if(PlayerData.controller != ControllerType.Keyboard)
            {
                nonMenuMap.Disable();
            }
        }
    }

    void OpenAction(InputAction.CallbackContext context)
    {
        MoveImageHolder();
    }

    void InstantClose()
    {
        opening = false;
        LeanTween.cancel(imageHolder);
        imageHolder.anchoredPosition = new Vector2(0, imageHolder.anchoredPosition.y);
        GameManager.IndexOpen(false);
    }

    void ImageHolderActions(bool open)
    {
        if (open)
        {
            LeanTween.moveX(imageHolder, -415, 0.5f).setIgnoreTimeScale(true);
            EnableNonMenuMap(false);
        }
        else
        {
            LeanTween.moveX(imageHolder, 0, 0.5f).setIgnoreTimeScale(true);
            EnableNonMenuMap(true);
        }
        GameManager.IndexOpen(open);
    }

    public void MoveImageHolder()
    {
        LeanTween.cancel(imageHolder);
        opening = !opening;
        ImageHolderActions(opening);
    }

    private void OnEnable()
    {
        GameManager.snowmanIndexOpen += SnowmanImageManager.ButtonToggle;
        openIndex.action.performed += OpenAction;
        PauseScript.AddListenerPause(InstantClose);
    }

    private void OnDisable()
    {
        opening = false;
        GameManager.snowmanIndexOpen -= SnowmanImageManager.ButtonToggle;
        openIndex.action.performed -= OpenAction;
        PauseScript.RemoveListenerPause(InstantClose);
        ImageHolderActions(opening);
    }

    private void OnApplicationQuit()
    {
        LeanTween.cancel(imageHolder);
    }

    private void Update()
    {
        
    }
}
