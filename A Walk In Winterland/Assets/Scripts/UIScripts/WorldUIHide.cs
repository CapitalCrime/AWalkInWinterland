using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorldUIHide : MonoBehaviour
{
    [SerializeField] float maxAlpha = 1;
    float maxDistance = 20;
    float minDistance = 8;
    Camera mainCamera;
    Image[] images;
    Button button;
    Vector3 gizmoPosition;
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

    void EnableImages(bool value)
    {
        if (value)
        {
            if (!images[0].enabled)
            {
                if (button != null) { button.enabled = true; }
                foreach (Image image in images)
                {
                    image.enabled = true;
                }
            }
        } else
        {
            if (images[0].enabled)
            {
                if (button != null) { button.enabled = false; }
                foreach (Image image in images)
                {
                    image.enabled = false;
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
        if (Vector3.Dot((transform.position - mainCamera.transform.position).normalized, transform.forward) < 0)
        {
            EnableImages(false);
        } else
        {
            Vector3 mousePos = GetMousePos(mainCamera.farClipPlane);
            Vector3 mouseDirection = (mainCamera.ScreenToWorldPoint(mousePos) - mainCamera.transform.position).normalized;
            float distance = Vector3.Cross(mouseDirection, transform.position - mainCamera.transform.position).magnitude;
            if (distance > 20)
            {
                EnableImages(false);
            } else
            {
                EnableImages(true);
                FadeImages(distance, minDistance, maxDistance);
            }
        }
    }
}
