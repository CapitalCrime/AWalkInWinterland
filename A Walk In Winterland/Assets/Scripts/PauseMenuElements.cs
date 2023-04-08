using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenuElements : MonoBehaviour
{
    public List<GameObject> menuTabs;
    public GameObject backToMenuButton;
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
        backToMenuButton.SetActive(false);
        menuButtonsHolder.SetActive(true);
    }
}
