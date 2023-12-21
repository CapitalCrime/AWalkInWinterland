using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PauseMenuElements : MonoBehaviour
{
    public List<GameObject> menuTabs;
    [SerializeField] GameObject menuTabHolder;
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
        menuTabHolder.SetActive(true);
    }
}
