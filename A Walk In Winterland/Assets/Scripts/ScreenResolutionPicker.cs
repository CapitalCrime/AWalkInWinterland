using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class ScreenResolutionPicker : MonoBehaviour
{
    TMP_Dropdown dropdownMenu;
    Resolution[] resolutions;
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

        resolutions = PlayerData.GetResolutions();
        int currentIndex = 0;

        for(int i = 0; i < resolutions.Length; i++)
        {
            if (resolutions[i].width == Screen.currentResolution.width) currentIndex = i;
            dropdownMenu.options.Add(new TMP_Dropdown.OptionData(resolutions[i].width + " x " + resolutions[i].height));
        }

        dropdownMenu.SetValueWithoutNotify(currentIndex);
    }

    public void ChangeResolution(int value)
    {
        Screen.SetResolution(resolutions[value].width, resolutions[value].height, Screen.fullScreen);
    }
}
