using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CatSnowman : Snowman
{
    [SerializeField] private FMODUnity.EmitterRef catSoundRef;
    [SerializeField] private FMODUnity.EmitterRef purrSoundRef;
    protected override void Start()
    {
        base.Start();
        snowmanViewedEvent += ClickCat;
    }

    void ClickCat()
    {
        if (catSoundRef.Target != null)
        {
            catSoundRef.Target.Play();
        }
    }

    private void OnDestroy()
    {
        snowmanViewedEvent -= ClickCat;
    }

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
