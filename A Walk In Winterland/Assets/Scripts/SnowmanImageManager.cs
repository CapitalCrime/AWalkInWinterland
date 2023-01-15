using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SnowmanImageManager : MonoBehaviour
{
    public SnowmanImageCreator imagePrefab;
    SnowmanDescription[] snowmanDescriptions;
    Dictionary<SnowmanDescription, SnowmanImageCreator> buttonDictionary;
    public RectTransform imageHolder;
    public Scrollbar scrollbar;
    float maxImageXPosition;

    private void Awake()
    {
        snowmanDescriptions = Resources.LoadAll<SnowmanDescription>("SnowmanDescriptions");
        buttonDictionary = new Dictionary<SnowmanDescription, SnowmanImageCreator>();
        foreach (SnowmanDescription desc in snowmanDescriptions)
        {
            SnowmanImageCreator button = Instantiate(imagePrefab, imageHolder.transform);
            button.Init(null);
            buttonDictionary.Add(desc, button);
        }
        if (imageHolder.childCount <= 12)
        {
            maxImageXPosition = 0;
            scrollbar.gameObject.SetActive(false);
        }
        else
        {
            maxImageXPosition = 130 * (Mathf.CeilToInt(imageHolder.childCount / 4.0f) - 3);
            scrollbar.size = 65 / maxImageXPosition;
        }
        Snowman.snowmanCreatedEvent += UpdateSnowmanButton;
    }

    void UpdateSnowmanButton(Snowman snowman)
    {
        if(buttonDictionary.TryGetValue(snowman.description, out SnowmanImageCreator button))
        {
            button.Init(snowman.description.image);
            button.PairButtonWithSnowman(snowman);
        }
    }

    // Update is called once per frame
    void Update()
    {
        imageHolder.anchoredPosition = new Vector2(scrollbar.value * -maxImageXPosition, -(imageHolder.sizeDelta.y/2));
    }
}
