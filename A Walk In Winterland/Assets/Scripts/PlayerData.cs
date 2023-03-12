using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Res
{
    public int width;
    public int height;
    public Res(int width, int height)
    {
        this.width = width;
        this.height = height;
    }
}

public static class PlayerData
{
    static List<Res> resolutions = new List<Res>()
    {
        new Res(2560,1440),
        new Res(1920,1080),
        new Res(1280,720),
        new Res(854,480),
        new Res(640,360)
    };
    const float maxCameraSpeed = 200;
    const float minCameraSpeed = 100;
    const float maxSnowmanCameraSpeed = 300;
    const float minSnowmanCameraSpeed = 100;
    static float cameraSpeed = 1;
    static float snowmanCameraSpeed = 1;

    public static UnityEvent<float> setPlayerCameraSpeedEvent = new UnityEvent<float>();
    public static UnityEvent<float> setSnowmanCameraSpeedEvent = new UnityEvent<float>();

    public static void SetCameraSpeed(float valueNormalized)
    {
        cameraSpeed = valueNormalized;
        setPlayerCameraSpeedEvent?.Invoke(GetPlayerCameraSpeed());
    }

    public static void SetSnowmanCameraSpeed(float valueNormalized)
    {
        snowmanCameraSpeed = valueNormalized;
        setSnowmanCameraSpeedEvent?.Invoke(GetSnowmanCameraSpeed());
    }

    public static Resolution[] GetResolutions()
    {
        return Screen.resolutions;
    }

    static float TrueCameraSpeed(float normalizedCameraSpeed, float minSpeed, float maxSpeed)
    {
        return minSpeed + (normalizedCameraSpeed * (maxSpeed - minSpeed));
    }

    public static float GetSnowmanCameraSpeed()
    {
        return TrueCameraSpeed(snowmanCameraSpeed, minSnowmanCameraSpeed, maxSnowmanCameraSpeed);
    }

    public static float GetPlayerCameraSpeed()
    {
        return TrueCameraSpeed(cameraSpeed, minCameraSpeed, maxCameraSpeed);
    }
}
