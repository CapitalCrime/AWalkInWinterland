using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager instance;
    public PlayerInput playerInputs;
    private void OnEnable()
    {
        instance = this;
    }
}
