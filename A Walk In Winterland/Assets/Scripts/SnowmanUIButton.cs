using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SnowmanUIButton : MonoBehaviour
{
    public RectTransform imageHolder;
    bool opening = false;

    public void MoveImageHolder()
    {
        LeanTween.cancel(imageHolder);
        opening = !opening;
        if(opening)
        {
            LeanTween.moveX(imageHolder, -405, 0.5f).setIgnoreTimeScale(true);
        } else
        {
            LeanTween.moveX(imageHolder, 0, 0.5f).setIgnoreTimeScale(true);
        }
    }
}
