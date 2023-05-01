using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class SnowmanImageCreator : MonoBehaviour
{
    [SerializeField] Image image;
    Snowman pairedSnowman;
    [SerializeField] Button button;

    public void Init(Sprite image)
    {
        if(image != null)
        {
            this.image.sprite = image;
        }
    }

    public Snowman getPairedSnowman()
    {
        return pairedSnowman;
    }

    public void SelectButton(bool value)
    {
        if (value)
        {
            EventSystem.current.SetSelectedGameObject(button.gameObject);
        } else
        {
            if(EventSystem.current.currentSelectedGameObject == button.gameObject)
            {
                EventSystem.current.SetSelectedGameObject(null);
            }
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
