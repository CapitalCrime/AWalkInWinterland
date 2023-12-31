using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BoingKit;

public class BalloonSnowman : Snowman
{
    BoingBehavior[] boingBehaviours = new BoingBehavior[0];
    protected override void Start()
    {
        base.Start();
        boingBehaviours = gameObject.GetComponentsInChildren<BoingBehavior>();
    }
    protected override void CancelUniqueAction()
    {
    }

    protected override void DayArriveAction()
    {
    }

    protected override void NightArriveAction()
    {
    }

    protected override void UniqueAction()
    {
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        foreach(BoingBehavior boing in boingBehaviours)
        {
            boing.enabled = true;
        }
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        foreach (BoingBehavior boing in boingBehaviours)
        {
            boing.enabled = false;
        }
    }
}
