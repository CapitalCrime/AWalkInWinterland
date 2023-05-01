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
        float snowmanNumber = currentIndex + 1;

        if (snowmanNumber < pageSize)
        {
            scrollbar.value = 0;
        }
        else
        {
            scrollbar.value = Mathf.Ceil((snowmanNumber - pageSize) / snowmenPerRow) / Mathf.Ceil((buttonDictionary.Count - pageSize) / snowmenPerRow);
        }
    }

    void ScrollMenuDirection()
    {
        if (!isOpen || !gameObject.activeSelf) return;

        int changeAmount = (int)scrollDirection.x * (int)snowmenPerRow + (int)-scrollDirection.y;
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

            if (changeAmount > 0)
            {
                currentIndex++;
            }
            else
            {
                currentIndex--;
            }
        }

        SelectButton(true);
    }

    public void SelectButton(bool value)
    {
        if (value && (!isOpen || !gameObject.activeSelf)) return;

        if (buttonDictionary.TryGetValue(snowmanDescriptions[currentIndex], out SnowmanImageCreator button))
        {
            button.SelectButton(value);
            SetScrollBarValue();
            if (!value)
            {
                triggerOnClose?.Invoke();
            }
        }
    }

    public static void DisableButtons()
    {
        if (instance == null) return;
        instance.scrollMenu.action.performed -= instance.ScrollMenu;
        instance.isOpen = false;
        instance.SelectButton(instance.isOpen);
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
