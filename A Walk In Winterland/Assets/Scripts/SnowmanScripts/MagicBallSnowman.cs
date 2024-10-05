using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MagicBallSnowman : Snowman
{
    public TextMeshProUGUI text;

    public List<string> fortunes = new List<string>();

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

    void CreateFortune()
    {
        LeanTween.cancel(text.gameObject);
        LTSeq seq = LeanTween.sequence();
        text.transform.localRotation = Quaternion.identity;

        seq.append(LeanTween.scale(text.gameObject, Vector3.zero, 1).setOnComplete(() =>
        {
            text.text = fortunes[Random.Range(0, fortunes.Count)];

        }));
        seq.append(LeanTween.scale(text.gameObject, Vector3.one, 1));
        LeanTween.rotateAroundLocal(text.gameObject, Vector3.forward, 360, 2);
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        if(gameObject.activeSelf == false)
        {
            LeanTween.cancel(text.rectTransform.gameObject);
            text.transform.localScale = Vector3.one;
            text.transform.localEulerAngles = Vector3.zero;
        }
    }

    protected override void Start()
    {
        base.Start();
        snowmanViewedEvent += CreateFortune;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        snowmanViewedEvent -= CreateFortune;
    }
}
