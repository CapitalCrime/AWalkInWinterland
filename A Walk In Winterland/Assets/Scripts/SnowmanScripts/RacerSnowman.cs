using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RacerSnowman : Snowman
{
    public override void DayArriveAction()
    {
    }

    public override void NightArriveAction()
    {
    }

    protected override void UniqueAction()
    {
    }

    protected override void Start()
    {
        base.Start();
        walkSpeeds = new MinMax(300, 450);
    }
}
