using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasManager : MonoBehaviour
{
    public static CanvasManager instance;
    public PenguinPanelHandler penguinPanelHandler;
    private void Awake()
    {
        instance = this;
    }

    public static void EnablePenguinPanel()
    {
        if (instance == null || instance.penguinPanelHandler == null) return;
        instance.penguinPanelHandler.EnablePanel(true);
    }

    public static void DisablePenguinPanel()
    {
        if (instance == null || instance.penguinPanelHandler == null) return;
        instance.penguinPanelHandler.EnablePanel(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
