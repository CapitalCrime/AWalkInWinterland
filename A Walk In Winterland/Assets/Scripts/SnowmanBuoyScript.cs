using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowmanBuoyScript : MonoBehaviour
{
    static GameObject buoy;

    private void Awake()
    {
        if(buoy == null)
        {
            buoy = Resources.Load<GameObject>("Buoy");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent(out Snowman snowman) && !snowman.GetComponent<HasBuoy>())
        {
            GameObject newBuoy = Instantiate(buoy, snowman.transform.position, Quaternion.identity);
            snowman.SetInteractable(false, true);
            snowman.gameObject.AddComponent<HasBuoy>();
            newBuoy.transform.position = snowman.transform.position;
            snowman.transform.parent = newBuoy.transform;
        }
    }
}
