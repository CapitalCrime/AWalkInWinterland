using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class ScreenResolutionPicker : MonoBehaviour
{
    TMP_Dropdown dropdownMenu;
    List<Res> resolutions;
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
        int currentIndex = -1;

        for(int i = 0; i < resolutions.Count; i++)
        {
            dropdownMenu.options.Add(new TMP_Dropdown.OptionData(resolutions[i].width + " x " + resolutions[i].height));
        }

        if(GameManager.instance != null)
        {
            currentIndex = GameManager.instance.startingResolution;
        } else
        {
            currentIndex = 0;
        }

        dropdownMenu.value = currentIndex;
        ChangeResolution(currentIndex);
    }

    public void ChangeResolution(int value)
    {
        Screen.SetResolution(resolutions[value].width, resolutions[value].height, Screen.fullScreen);
    }
}