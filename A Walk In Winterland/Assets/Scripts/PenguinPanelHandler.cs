using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PenguinPanelHandler : MonoBehaviour
{
    // Start is called before the first frame update
    public CanvasGroup canvasGroup;

    public void EnablePanel(bool value)
    {
        gameObject.SetActive(value);
        if (value)
        {
            LeanTween.cancel(gameObject);
            LeanTween.value(gameObject, 0, 1, 1).setOnUpdate((
                float val) => canvasGroup.alpha = val
                );
        }
        else
        {
            canvasGroup.alpha = 0;
        }
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
