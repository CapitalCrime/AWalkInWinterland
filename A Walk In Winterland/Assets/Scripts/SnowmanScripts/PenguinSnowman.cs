using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PenguinSnowman : Snowman
{
    public Animation penguinAnimation;
    protected override void Start()
    {
        base.Start();
        snowmanFPSEvent += CanvasManager.EnablePenguinPanel;
        snowmanLeaveFPSEvent += CanvasManager.DisablePenguinPanel;
    }
    public override void DayArriveAction()
    {
    }

    public override void NightArriveAction()
    {
    }

    protected override void UniqueAction()
    {
        penguinAnimation.Play();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        snowmanFPSEvent -= CanvasManager.EnablePenguinPanel;
        snowmanLeaveFPSEvent -= CanvasManager.DisablePenguinPanel;
    }
}
