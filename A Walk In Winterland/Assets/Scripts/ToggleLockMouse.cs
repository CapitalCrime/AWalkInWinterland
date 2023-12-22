using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleLockMouse : MonoBehaviour
{
    private void Awake()
    {
        if(TryGetComponent(out Toggle toggle))
        {
            toggle.isOn = PlayerData.lockMouse;
        }
    }
    public void ToggleLock(bool value)
    {
        PlayerData.lockMouse = value;
    }
}
