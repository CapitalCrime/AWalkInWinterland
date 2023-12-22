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

public enum ControllerType
{
    Keyboard,
    Controller
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
    const float maxCameraSpeed = 350;
    const float minCameraSpeed = 50;
    const float maxSnowmanCameraSpeed = 500;
    const float minSnowmanCameraSpeed = 100;
    static float cameraSpeed = 0.5f;
    static float snowmanCameraSpeed = 0.5f;
    public static float musicVolume = 1;
    public static float ambienceVolume = 1;
    public static float sfxVolume = 1;
    public static ControllerType controller { get; private set; }
    public static bool lockMouse = false;

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

    public static void SetCurrentController(ControllerType controller)
    {
        PlayerData.controller = controller;
    }

    public static Resolution[] GetResolutions()
    {
        return Screen.resolutions;
    }

    static float TrueCameraSpeed(float normalizedCameraSpeed, float minSpeed, float maxSpeed)
    {
        return minSpeed + (normalizedCameraSpeed * (maxSpeed - minSpeed));
    }

    public static float getNormalizedSnowmanCameraSpeed()
    {
        return snowmanCameraSpeed;
    }

    public static float getNormalizedCameraSpeed()
    {
        return cameraSpeed;
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
