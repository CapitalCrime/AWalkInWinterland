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
    const float maxCameraSpeed = 300;
    const float minCameraSpeed = 100;
    static float cameraSpeed = 1;

    public static UnityEvent<float> setCameraSpeedEvent = new UnityEvent<float>();

    public static void SetCameraSpeed(float valueNormalized)
    {
        cameraSpeed = valueNormalized;
        setCameraSpeedEvent?.Invoke(GetCameraSpeed());
    }

    public static Resolution[] GetResolutions()
    {
        return Screen.resolutions;
    }

    public static float GetCameraSpeed()
    {
        return minCameraSpeed + (cameraSpeed * (maxCameraSpeed - minCameraSpeed));
    }
}
