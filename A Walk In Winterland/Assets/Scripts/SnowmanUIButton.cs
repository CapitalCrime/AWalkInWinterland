using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class SnowmanUIButton : MonoBehaviour
{
    public RectTransform imageHolder;
    private InputActionMap nonMenuMap;
    [SerializeField] private InputActionReference openIndex;
    [HideInInspector] public bool opening = false;

    private void Start()
    {
        nonMenuMap = InputManager.instance.playerInputs.actions.FindActionMap("NonMenu");
        openIndex.action.performed += OpenAction;
    }

    void OpenAction(InputAction.CallbackContext context)
    {
        MoveImageHolder();
    }

    public void MoveImageHolder()
    {
        LeanTween.cancel(imageHolder);
        opening = !opening;
        if(opening)
        {
            LeanTween.moveX(imageHolder, -415, 0.5f).setIgnoreTimeScale(true);
            nonMenuMap.Disable();
            PauseScript.snowmanIndexOpen = true;
            SnowmanImageManager.EnableButtons();
        } else
        {
            LeanTween.moveX(imageHolder, 0, 0.5f).setIgnoreTimeScale(true);
            SnowmanImageManager.DisableButtons();
            PauseScript.snowmanIndexOpen = false;
            if (!PauseScript.isPaused())
            {
                nonMenuMap.Enable();
            }
        }
    }

    private void Update()
    {
        
    }
}
