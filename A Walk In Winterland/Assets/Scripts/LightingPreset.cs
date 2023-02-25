using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Lighting Preset", menuName ="Lighting/Lighting Preset", order =1)]
public class LightingPreset : ScriptableObject
{
    public Gradient skyGradient;
    public Gradient equatorGradient;
}
