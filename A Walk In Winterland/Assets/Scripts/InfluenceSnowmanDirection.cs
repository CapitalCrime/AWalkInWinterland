using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfluenceSnowmanDirection : MonoBehaviour
{
    public Transform destinationPoint;
    private void OnTriggerEnter(Collider other)
    {
        if (destinationPoint == null) return;
        if(other.TryGetComponent(out Snowman snowman))
        {
            snowman.SetInfluenceDestination(destinationPoint.position);
        }
    }
}
