using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorldUIHide : MonoBehaviour
{
    [SerializeField] float maxAlpha = 1;
    float maxDistance = 35;
    float minDistance = 8;
    Camera mainCamera;
    Image[] images;
    Button button;
    bool showButton = false;

    private void Awake()
    {
        GameManager.snowmanIndexOpen += SetAlwaysShow;
    }

    private void Start()
    {
        mainCamera = Camera.main;
        images = transform.GetComponentsInChildren<Image>();
        button = transform.GetComponentInChildren<Button>();
    }

    Vector3 GetMousePos(float mouseDistance)
    {
        Vector3 mousePos = new Vector2(Screen.width / 2, Screen.height / 2);
        if (PlayerData.controller == ControllerType.Keyboard)
        {
            mousePos = UnityEngine.InputSystem.Mouse.current.position.ReadValue();
        }
        mousePos.z = mousePos.z + mouseDistance;
        return mousePos;
    }

    public void SetAlwaysShow(bool value)
    {
        showButton = value;
    }

    void EnableImages(bool value, float setAlpha = -1)
    {
        if (images[0].enabled != value)
        {
            if (button != null) { button.enabled = value; }
            foreach (Image image in images)
            {
                image.enabled = value;
                if(setAlpha != -1)
                {
                    Color color = image.color;
                    color.a = setAlpha;
                    image.color = color;
                }
            }
        }
    }

    void FadeImages(float distance, float minDistance, float maxDistance)
    {
        foreach(Image image in images)
        {
            Color color = image.color;
            color.a = maxAlpha - maxAlpha*((distance - minDistance) / (maxDistance - minDistance));
            image.color = color;
        }
    }

    private void Update()
    {
        if (PauseScript.isPaused())
        {
            EnableImages(false);
            return;
        }
        if (Vector3.Dot((transform.position - transform.forward*15 - mainCamera.transform.position).normalized, transform.forward) < 0)
        {
            EnableImages(false);
        }
        else
        {
            if (!showButton)
            {
                Vector3 mousePos = GetMousePos(mainCamera.farClipPlane);
                Vector3 mouseDirection = (mainCamera.ScreenToWorldPoint(mousePos) - mainCamera.transform.position).normalized;
                float distance = Vector3.Cross(mouseDirection, transform.position - mainCamera.transform.position).magnitude;
                if (distance > maxDistance)
                {
                    EnableImages(false);
                }
                else
                {
                    EnableImages(true);
                    FadeImages(distance, minDistance, maxDistance);
                }
            } else
            {
                EnableImages(true, maxAlpha);
            }
        }
    }
}
