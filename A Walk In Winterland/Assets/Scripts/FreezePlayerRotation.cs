using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreezePlayerRotation : MonoBehaviour
{
    private void FixedUpdate()
    {
        transform.rotation = Quaternion.identity;
    }
}
