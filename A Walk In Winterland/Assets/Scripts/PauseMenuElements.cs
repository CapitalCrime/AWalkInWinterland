using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PauseMenuElements : MonoBehaviour
{
    public List<GameObject> menuTabs;
    public GameObject menuButtonsHolder;
    void Start()
    {
        DeactivateAllTabs();
    }

    public void DeactivateAllTabs()
    {
        foreach (GameObject tab in menuTabs)
        {
            tab.SetActive(false);
        }
        menuButtonsHolder.SetActive(true);
    }
}
