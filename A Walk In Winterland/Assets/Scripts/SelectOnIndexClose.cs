using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SelectOnIndexClose : MonoBehaviour
{
    public Selectable _object;
    public Button quitButton;

    private void OnEnable()
    {
        SnowmanImageManager.triggerOnClose.AddListener(SelectThis);
        SelectThis();
    }

    private void OnDisable()
    {
        SnowmanImageManager.triggerOnClose.RemoveListener(SelectThis);
    }

    private void OnDestroy()
    {
        SnowmanImageManager.triggerOnClose.RemoveListener(SelectThis);
    }

    void SelectThis()
    {
        EventSystem.current.SetSelectedGameObject(_object.gameObject);
        Navigation navigation = quitButton.navigation;
        navigation.selectOnDown = _object;
        quitButton.navigation = navigation;
    }
}
