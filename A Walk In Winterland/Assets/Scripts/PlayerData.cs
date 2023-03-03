using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public static class PlayerData
{
    const float maxCameraSpeed = 300;
    const float minCameraSpeed = 100;
    static float cameraSpeed = 1;

    public static UnityEvent<float> setCameraSpeedEvent = new UnityEvent<float>();

    public static void SetCameraSpeed(float valueNormalized)
    {
        cameraSpeed = valueNormalized;
        setCameraSpeedEvent?.Invoke(GetCameraSpeed());
    }

    public static float GetCameraSpeed()
    {
        return minCameraSpeed + (cameraSpeed * (maxCameraSpeed - minCameraSpeed));
    }
}
