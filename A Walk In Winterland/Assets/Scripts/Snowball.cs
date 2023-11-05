using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Snowball : MonoBehaviour
{
    public Rigidbody rigidbody;
    [SerializeField] private FMODUnity.EmitterRef snowballSoundRef;
    private void Awake()
    {
        rigidbody.AddForce(transform.forward * 2000);
        Destroy(gameObject, 3);
    }
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Snowball hit: " + other.name);
        if (other.isTrigger == true) return;
        if(other.CompareTag("Snowman"))
        {
            if(other.TryGetComponent(out Snowman snowman))
            {
                snowman.AddForce(transform.forward * 400);
            }
        }
        if(snowballSoundRef.Target != null)
        {
            snowballSoundRef.Target.Play();
        }
        Destroy(gameObject);
    }
}
