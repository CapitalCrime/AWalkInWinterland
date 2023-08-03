using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class SnowmanImageManager : MonoBehaviour
{
    public static SnowmanImageManager instance;
    public static UnityEvent triggerOnClose = new UnityEvent();
    public SnowmanImageCreator imagePrefab;
    [SerializeField] private InputActionReference scrollMenu;
    SnowmanDescription[] snowmanDescriptions;
    Dictionary<SnowmanDescription, SnowmanImageCreator> buttonDictionary;
    public RectTransform imageHolder;
    public Scrollbar scrollbar;
    float maxImageXPosition;
    float snowmenPerRow = 4;
    float snowmenVisibleColumn = 3;
    float snowmenInputScrollVisibleColumn = 2;
    [SerializeField] int currentIndex = 0;
    bool isOpen = false;

    private void Awake()
    {
        instance = this;
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
            maxImageXPosition = 135 * (Mathf.CeilToInt(imageHolder.childCount / snowmenPerRow) - snowmenVisibleColumn);
            scrollbar.size = 70 / maxImageXPosition;
        }
        Snowman.snowmanCreatedEvent += UpdateSnowmanButton;
    }

    public static void UpdateCurrentSnowmanIndex(SnowmanDescription description)
    {
        if (instance == null) return;
        for(int i = 0; i < instance.snowmanDescriptions.Length; i++)
        {
            if(description == instance.snowmanDescriptions[i])
            {
                instance.currentIndex = i;
                break;
            }
        }
        instance.SelectButton(true);
    }

    void UpdateSnowmanButton(Snowman snowman)
    {
        if(buttonDictionary.TryGetValue(snowman.description, out SnowmanImageCreator button))
        {
            button.Init(snowman.description.image);
            button.PairButtonWithSnowman(snowman);
        }
    }

    Vector2 scrollDirection = Vector2.zero;

    void ScrollMenu(InputAction.CallbackContext context)
    {
        if(context.phase == InputActionPhase.Performed)
        {
            scrollDirection = context.ReadValue<Vector2>();
            lastScrollTime = 0;
        }
        if (context.phase == InputActionPhase.Canceled)
        {
            scrollDirection = Vector2.zero;
        }
    }

    void SetScrollBarValue()
    {
        float pageSize = snowmenPerRow * snowmenVisibleColumn;
        float snowmanNumber = currentIndex + 1 + (snowmenPerRow* (snowmenVisibleColumn- snowmenInputScrollVisibleColumn));

        if (snowmanNumber < pageSize)
        {
            scrollbar.value = 0;
        }
        else
        {
            int changeAmount = (int)scrollDirection.x * (int)snowmenPerRow + (int)-scrollDirection.y;
            float snowmanNumberEffector = Mathf.Ceil((snowmanNumber - pageSize) / snowmenPerRow);
            scrollbar.value = Mathf.Clamp(snowmanNumberEffector / Mathf.Ceil((buttonDictionary.Count - pageSize) / snowmenPerRow)
                ,0
                ,1);
        }
    }

    void ScrollMenuDirection()
    {
        if (!isOpen || !gameObject.activeSelf) return;

        //Move index horizontally and vertically by input
        int changeAmount = (int)scrollDirection.x * (int)snowmenPerRow + (int)-scrollDirection.y;

        //Handle wrap around at end points, else increase index by direction
        if(currentIndex == 0 && changeAmount < 0)
        {
            currentIndex = buttonDictionary.Count - 1;
        } else if(currentIndex == buttonDictionary.Count - 1 && changeAmount > 0)
        {
            currentIndex = 0;
        } else
        {
            int lastIndex = currentIndex;
            currentIndex += changeAmount;
            if(lastIndex < buttonDictionary.Count-1 && currentIndex > buttonDictionary.Count - 1 && buttonDictionary.Count % 4 != 0)
            {
                currentIndex = buttonDictionary.Count - 1;
            }
        }

        //If next snowman is undiscovered, keep scrolling until unlocked one is found
        while (true)
        {
            //Handle wrap around and clamp between end points
            if (currentIndex > buttonDictionary.Count - 1)
            {
                currentIndex -= buttonDictionary.Count-1;
            }
            if (currentIndex < 0)
            {
                currentIndex += buttonDictionary.Count-1;
            }
            currentIndex = Mathf.Clamp(currentIndex, 0, buttonDictionary.Count - 1);

            //If unlocked snowman is found, break out of while loop
            if (buttonDictionary.TryGetValue(snowmanDescriptions[(int)currentIndex], out SnowmanImageCreator tempButton))
            {
                if (tempButton.getPairedSnowman() != null) break;
            }

            //If unlocked snowman is not found, increase index to check for next one
            if (changeAmount > 0)
            {
                currentIndex++;
            }
            else
            {
                currentIndex--;
            }
        }

        //After snowman is found, select the index button
        SelectButton(true);
    }

    public void SelectButton(bool value)
    {
        //If snowman menu is not open, don't select button. Basically nulls input when index isn't open.
        if (value && (!isOpen || !gameObject.activeSelf)) return;

        //Checks to see if current index snowman is linked to a button
        if (buttonDictionary.TryGetValue(snowmanDescriptions[currentIndex], out SnowmanImageCreator button))
        {
            //Select button and scroll menu to correct spot
            button.SelectButton(value);
            SetScrollBarValue();
        }
    }

    public static void DisableButtons()
    {
        if (instance == null) return;
        instance.scrollMenu.action.performed -= instance.ScrollMenu;
        instance.isOpen = false;
        instance.SelectButton(instance.isOpen);
        //If closing menu, trigger close menu events
        triggerOnClose?.Invoke();
    }

    public static void EnableButtons()
    {
        if (instance == null) return;
        instance.scrollMenu.action.performed += instance.ScrollMenu;
        instance.isOpen = true;
        instance.SelectButton(instance.isOpen);
    }

    private void OnDisable()
    {
        triggerOnClose?.Invoke();
        triggerOnClose.RemoveAllListeners();
    }

    float scrollWaitTime = 0.25f;
    float lastScrollTime = 0;
    // Update is called once per frame
    void Update()
    {
        imageHolder.anchoredPosition = new Vector2(scrollbar.value * -maxImageXPosition, -(imageHolder.sizeDelta.y/2));
        if(scrollDirection != Vector2.zero && Time.time - lastScrollTime > scrollWaitTime)
        {
            lastScrollTime = Time.time;
            ScrollMenuDirection();
        }
    }
}
