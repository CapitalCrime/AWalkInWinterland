using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class SnowmanImageManager : MonoBehaviour
{
    public static SnowmanImageManager instance;
    public SnowmanImageCreator imagePrefab;
    [SerializeField] private InputActionReference scrollMenu;
    SnowmanDescription[] snowmanDescriptions;
    Dictionary<SnowmanDescription, SnowmanImageCreator> buttonDictionary;
    public RectTransform imageHolder;
    public Scrollbar scrollbar;
    float maxImageXPosition;
    float snowmenPerRow = 4;
    float snowmenVisibleColumn = 3;
    [SerializeField] int currentIndex = 0;

    private void Awake()
    {
        instance = this;
        snowmanDescriptions = Resources.LoadAll<SnowmanDescription>("SnowmanDescriptions");
        buttonDictionary = new Dictionary<SnowmanDescription, SnowmanImageCreator>();
        foreach (SnowmanDescription desc in snowmanDescriptions)
        {
            SnowmanImageCreator button = Instantiate(imagePrefab, imageHolder.transform);
            button.Init(null, this);
            buttonDictionary.Add(desc, button);
        }
        if (imageHolder.childCount <= 12)
        {
            maxImageXPosition = 0;
            scrollbar.gameObject.SetActive(false);
        }
        else
        {
            maxImageXPosition = 135 * (Mathf.CeilToInt(imageHolder.childCount / snowmenPerRow) - snowmenVisibleColumn);
            scrollbar.size = 70 / maxImageXPosition;
        }
        Snowman.snowmanCreatedEvent += UpdateSnowmanButton;
        scrollMenu.action.performed += ScrollMenu;
    }

    public void UpdateCurrentSnowmanIndex(SnowmanDescription description)
    {
        for(int i = 0; i < snowmanDescriptions.Length; i++)
        {
            if(description == snowmanDescriptions[i])
            {
                currentIndex = i;
                return;
            }
        }
    }

    void UpdateSnowmanButton(Snowman snowman)
    {
        if(buttonDictionary.TryGetValue(snowman.description, out SnowmanImageCreator button))
        {
            button.Init(snowman.description.image);
            button.PairButtonWithSnowman(snowman);
        }
    }

    void ScrollMenu(InputAction.CallbackContext context)
    {
        Vector2 direction = context.ReadValue<Vector2>();
        int changeAmount = (int)direction.x * (int)snowmenPerRow + (int)-direction.y;
        currentIndex += changeAmount;
        while (true)
        {

            if (currentIndex > buttonDictionary.Count - 1)
            {
                currentIndex = 0;
            }
            if (currentIndex < 0)
            {
                currentIndex = buttonDictionary.Count - 1;
            }

            if (buttonDictionary.TryGetValue(snowmanDescriptions[(int)currentIndex], out SnowmanImageCreator tempButton))
            {
                if (tempButton.getPairedSnowman() != null) break;
            }

            if(changeAmount > 0)
            {
                currentIndex++;
            } else
            {
                currentIndex--;
            }
        }

        float pageSize = snowmenPerRow * snowmenVisibleColumn;
        float snowmanNumber = currentIndex + 1;

        if (snowmanNumber < pageSize)
        {
            scrollbar.value = 0;
        } else
        {
            scrollbar.value = Mathf.Ceil((snowmanNumber - pageSize)/snowmenPerRow) / Mathf.Ceil((buttonDictionary.Count - pageSize)/snowmenPerRow);
        }

        if (buttonDictionary.TryGetValue(snowmanDescriptions[currentIndex], out SnowmanImageCreator button))
        {
            button.SelectButton(true);
        }
    }

    public static void DisableButtons()
    {
        if (instance == null) return;
        if (instance.buttonDictionary.TryGetValue(instance.snowmanDescriptions[instance.currentIndex], out SnowmanImageCreator button))
        {
            button.SelectButton(false);
        }
    }

    public static void EnableButtons()
    {
        if (instance == null) return;
        if (instance.buttonDictionary.TryGetValue(instance.snowmanDescriptions[instance.currentIndex], out SnowmanImageCreator button))
        {
            button.SelectButton(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        imageHolder.anchoredPosition = new Vector2(scrollbar.value * -maxImageXPosition, -(imageHolder.sizeDelta.y/2));
    }
}
