using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "Description", menuName = "ScriptableObjects/SnowmanDescription")]
public class SnowmanDescription : ScriptableObject
{
    [Serializable]
    public struct MinMax
    {
        public MinMax(float min, float max)
        {
            this.min = min;
            this.max = max;
        }
        [Min(0)]public float min;
        [Min(0)]public float max;
    }
    public string snowmanName;
    public Sprite image;
    public bool unlocked = false;
    public bool randomUnlock = true;
    public MinMax randomUniqueActionTimeSeconds;
    public MinMax randomWalkTimeSeconds = new MinMax(5, 10);
}
