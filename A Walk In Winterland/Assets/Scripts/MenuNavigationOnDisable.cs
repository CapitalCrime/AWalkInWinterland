using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuNavigationOnDisable : MonoBehaviour
{
    public Selectable thisObject;
    public GameObject objectBeingDeactivated;

    public Selectable activeObject;
    public Selectable deactiveObject;
    void Awake()
    {
        Reroute(objectBeingDeactivated.activeInHierarchy);
    }

    public void Reroute(bool active)
    {
        Navigation navigation = thisObject.navigation;
        navigation.selectOnUp = active ? activeObject : deactiveObject;
        thisObject.navigation = navigation;
    }
}
