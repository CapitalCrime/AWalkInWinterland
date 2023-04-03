using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public int startingQuality = 1;

    private void Awake()
    {
        //QualitySettings.vSyncCount = 1;
        Application.targetFrameRate = 60;
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        QualitySettings.SetQualityLevel(startingQuality);
    }

    public void CloseGame()
    {
        Application.Quit();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
