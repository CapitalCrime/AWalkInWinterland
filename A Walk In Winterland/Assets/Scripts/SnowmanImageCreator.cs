using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SnowmanImageCreator : MonoBehaviour
{
    [SerializeField] Image image;
    Snowman pairedSnowman;

    public void Init(Sprite image)
    {
        if(image != null)
        {
            this.image.sprite = image;
        }
    }

    public void PairButtonWithSnowman(Snowman snowman)
    {
        pairedSnowman = snowman;
    }

    public void ViewSnowman()
    {
        if (PauseScript.isPaused()) return;
        if (pairedSnowman == null) return;
        SnowmanManager.instance.ActivateSnowmanCamera(pairedSnowman);
    }
}
