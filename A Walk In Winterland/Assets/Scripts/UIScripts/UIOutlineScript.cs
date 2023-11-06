using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIOutlineScript : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    enum UIType
    {
        Image,
        Button
    }
    [SerializeField] UIType uiType;
    Image image;
    [SerializeField] Material outlineMaterial;
    private void Awake()
    {
        switch (uiType)
        {
            case UIType.Button:
                image = (Image)GetComponent<Button>().targetGraphic;
                break;
            case UIType.Image:
                image = GetComponent<Image>();
                break;
        }
    }

    public void OnDeselect(BaseEventData eventData)
    {
        Debug.Log(image);
        Debug.Log("Deselected " + gameObject.name);
        image.material = null;
    }

    public void OnSelect(BaseEventData eventData)
	{
        Debug.Log(image);
        Debug.Log("Selected " + gameObject.name);
        image.material = outlineMaterial;
	}
}
