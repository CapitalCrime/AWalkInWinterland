using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpySnowman : Snowman
{
    [SerializeField] GameObject[] spySnowmanPart;
    [SerializeField] Transform disguiseHolder;
    bool disguised = false;
    [SerializeField] private FMODUnity.EmitterRef disguiseSoundRef;
    protected override void DayArriveAction()
    {
    }

    protected override void NightArriveAction()
    {

    }

    void Disguise(bool disguised)
    {
        //TODO: Create disguise particle effect and trigger it


        if (disguised)
        {
            foreach (GameObject part in spySnowmanPart)
            {
                part.SetActive(false);
            }
            //TODO: Find surrounding snowmen, pick one,
            //then take every ACTIVE object with mesh renderer in selected snowman and copy to spy snowman disguise holder

        }
        else
        {
            foreach (Transform costume in disguiseHolder)
            {
                Destroy(costume);
            }
            foreach (GameObject part in spySnowmanPart)
            {
                part.SetActive(true);
            }
        }
    }

    protected override void UniqueAction()
    {
        if(disguiseSoundRef.Target != null)
        {
            disguiseSoundRef.Target.Play();
        }

        disguised = !disguised;

        Disguise(disguised);
    }

    protected override void CancelUniqueAction()
    {
        disguised = false;
        Disguise(disguised);
    }

    protected override void Start()
    {
        base.Start();
    }
}
