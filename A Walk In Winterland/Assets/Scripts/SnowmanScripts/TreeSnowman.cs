using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeSnowman : Snowman
{
    public GameObject treeNose;
    public GameObject treeNoseLeaves;
    public float growthStage = 0.8f;
    public ParticleSystem growthParticles;
    public Light growthParticleLight;
    float growthLightIntensityBase;
    public float growthTime = 6;
    public AnimationCurve curve;
    protected override void CancelUniqueAction()
    {
    }

    void GrowNose()
    {
        if (growthStage == 1) return;

        LeanTween.cancel(treeNose);
        growthParticles.gameObject.SetActive(true);
        growthParticles.Play();
        growthStage = Mathf.Clamp(growthStage + 0.15f, 0, 1);
        LeanTween.scale(treeNose, Vector3.one * growthStage, growthTime).setEaseOutBounce();
        LeanTween.value(0, 1, growthTime).setOnUpdate(
            (float val) => { growthParticleLight.intensity = growthLightIntensityBase * curve.Evaluate(val); }
            );
        if(growthStage >= 0.85f && treeNoseLeaves.activeSelf == false)
        {
            treeNoseLeaves.transform.localScale = new Vector3(0,0,1);
            treeNoseLeaves.SetActive(true);
            LeanTween.scale(treeNoseLeaves, Vector3.one, growthTime).setEaseInOutBounce();
        }
    }

    protected override void DayArriveAction()
    {
        GrowNose();
    }

    protected override void NightArriveAction()
    {
    }

    protected override void UniqueAction()
    {
    }

    protected override void Start()
    {
        base.Start();
        growthStage = Mathf.Clamp(growthStage, 0, 1);
        growthParticles.gameObject.SetActive(false);
        growthLightIntensityBase = growthParticleLight.intensity;
        growthParticleLight.intensity = 0;
        ParticleSystem.MainModule main = growthParticles.main;
        main.startLifetime = growthTime;
        treeNose.transform.localScale = Vector3.one * growthStage;
        if (growthStage >= 0.85f && treeNoseLeaves.activeSelf == false)
        {
            treeNoseLeaves.transform.localScale = Vector3.one;
            treeNoseLeaves.SetActive(true);
        }
    }
}
