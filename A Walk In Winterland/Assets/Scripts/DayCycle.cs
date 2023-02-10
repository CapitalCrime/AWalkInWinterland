using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DayCycle : MonoBehaviour
{
    public static event UnityAction dayActions;
    public static event UnityAction nightActions;
    static DayCycle instance;
    bool currentlyDay;
    private void Awake()
    {
        instance = this;
        currentlyDay = IsSunUp();
    }

    public static bool IsSunUp()
    {
        return Vector3.Dot(Vector3.up, instance.transform.forward) < 0.3f;
    }

    void Update()
    {
        transform.Rotate(Vector3.right * Time.deltaTime * (IsSunUp() ? 0.5f : 2));
        if(IsSunUp() && !currentlyDay)
        {
            currentlyDay = true;
            dayActions?.Invoke();
        }
        if(!IsSunUp() && currentlyDay)
        {
            currentlyDay = false;
            nightActions?.Invoke();
        }
    }
}
