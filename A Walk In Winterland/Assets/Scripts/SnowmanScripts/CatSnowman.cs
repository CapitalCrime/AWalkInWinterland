using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CatSnowman : Snowman
{
    [SerializeField] private FMODUnity.EmitterRef purrSoundRef;

    protected override void NightArriveAction()
    {
    }

    protected override void DayArriveAction()
    {
    }

    protected override void UniqueAction()
    {
        if(purrSoundRef.Target != null)
        {
            purrSoundRef.Target.Play();
        }
    }

    protected override void CancelUniqueAction()
    {
    }
}
