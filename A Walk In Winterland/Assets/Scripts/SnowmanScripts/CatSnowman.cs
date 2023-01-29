using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CatSnowman : Snowman
{
    [SerializeField] private FMODUnity.EmitterRef catSoundRef;
    protected override void Start()
    {
        base.Start();
        SnowmanManager.instance.SubscribePerformFunction(ClickCat);
    }

    void ClickCat(InputAction.CallbackContext context)
    {
        if (SnowmanManager.instance.CheckCurrentSnowman(this))
        {
            if(catSoundRef.Target != null)
            {
                catSoundRef.Target.Play();
            }
        }
    }

    private void OnDestroy()
    {
        SnowmanManager.instance.UnsubscribePerformFunction(ClickCat);
    }

    public override void NightArriveAction()
    {
    }

    public override void DayArriveAction()
    {
    }

    protected override void UniqueAction()
    {
    }
}
