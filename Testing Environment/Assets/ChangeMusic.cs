using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeMusic : MonoBehaviour
{
    public FMOD.Studio.EventInstance soundInstance;
    public FMODUnity.StudioEventEmitter music1Ref;
    public FMODUnity.StudioEventEmitter music2Ref;

    private void Awake()
    {
        music1Ref.gameObject.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            music1Ref.gameObject.SetActive(false);
            music2Ref.gameObject.SetActive(true);
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            music1Ref.gameObject.SetActive(true);
            music2Ref.gameObject.SetActive(false);
        }
    }
}
