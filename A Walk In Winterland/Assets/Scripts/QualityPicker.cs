using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class QualityPicker : MonoBehaviour
{
    TMP_Dropdown dropdownMenu;
    // Start is called before the first frame update
    void Start()
    {
        if (TryGetComponent(out TMP_Dropdown dropdown))
        {
            dropdownMenu = dropdown;
        } else
        {
            Debug.LogError("Object" + gameObject.name + " is missing TMP_Dropdown component");
        }
        string[] names = QualitySettings.names;
        foreach(string name in names)
        {
            dropdownMenu.options.Add(new TMP_Dropdown.OptionData(name));
        }
        dropdownMenu.value = QualitySettings.GetQualityLevel();
        ChangeQuality(QualitySettings.GetQualityLevel());
    }

    public void ChangeQuality(int value)
    {
        QualitySettings.SetQualityLevel(value);
    }
}
