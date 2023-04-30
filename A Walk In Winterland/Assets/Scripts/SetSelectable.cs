using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SetSelectable : MonoBehaviour
{
    public Selectable _object;
    public Button quitButton;
    private void OnEnable()
    {
        EventSystem.current.SetSelectedGameObject(_object.gameObject);
        Navigation navigation = quitButton.navigation;
        navigation.selectOnDown = _object;
        quitButton.navigation = navigation;
    }
}
