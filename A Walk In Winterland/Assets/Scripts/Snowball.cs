using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Snowball : MonoBehaviour
{
    public Rigidbody rigidbody;
    private void Awake()
    {
        rigidbody.AddForce(transform.forward * 2000);
        Destroy(gameObject, 3);
    }
    private void OnTriggerEnter(Collider other)
    {
        //if (other.gameObject == gameObject) return;

        Debug.Log("Snowball hit: " + other.name);
        if(other.CompareTag("Snowman"))
        {
            if(other.TryGetComponent(out Snowman snowman))
            {
                snowman.AddForce(transform.forward * 400);
            }
        }
        Destroy(gameObject);
    }
}
