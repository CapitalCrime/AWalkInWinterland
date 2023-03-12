using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class ToggleMotionBlur : MonoBehaviour
{
    public VolumeProfile profile;
    MotionBlur motionBlur;

    private void Awake()
    {
        if(profile.TryGet(out MotionBlur blur))
        {
            motionBlur = blur;
        }
        else
        {
            Debug.LogError("Profile attached to object " + gameObject.name + " is missing motion blur component");
        }

        if (gameObject.TryGetComponent(out Toggle toggle))
        {
            toggle.SetIsOnWithoutNotify(motionBlur.active);
        }
    }

    public void ToggleBlur(bool value)
    {
        motionBlur.active = value;
    }
}
