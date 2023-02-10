using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CatSnowman : Snowman
{
    [SerializeField] private FMODUnity.EmitterRef purrSoundRef;

    public override void NightArriveAction()
    {
    }

    public override void DayArriveAction()
    {
    }

    protected override void UniqueAction()
    {
        if(purrSoundRef.Target != null)
        {
            purrSoundRef.Target.Play();
        }
    }
}
