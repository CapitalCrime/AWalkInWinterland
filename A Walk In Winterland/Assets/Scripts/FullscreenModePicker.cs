using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using System.Linq;

public class FullscreenModePicker : MonoBehaviour
{
    TMP_Dropdown dropdownMenu;
    List<FullScreenMode> modes = new List<FullScreenMode>();
    // Start is called before the first frame update
    void Start()
    {
        if (TryGetComponent(out TMP_Dropdown dropdown))
        {
            dropdownMenu = dropdown;
        }
        else
        {
            Debug.LogError("Object" + gameObject.name + " is missing TMP_Dropdown component");
        }

        int currentIndex = 0;

        foreach (FullScreenMode mode in Enum.GetValues(typeof(FullScreenMode)))
        {
            if (mode == FullScreenMode.MaximizedWindow && Application.platform != RuntimePlatform.OSXPlayer) continue;
            if (mode == FullScreenMode.Windowed && Application.platform != RuntimePlatform.WindowsPlayer) continue;

            modes.Add(mode);
            if (Screen.fullScreenMode == mode) { currentIndex = modes.Count()-1; }

            dropdownMenu.options.Add(new TMP_Dropdown.OptionData(mode.ToString()));
        }

        dropdownMenu.SetValueWithoutNotify(currentIndex);
    }

    public void ChangeFullscreenMode(int value)
    {
        Screen.fullScreenMode = modes[value];
    }
}
