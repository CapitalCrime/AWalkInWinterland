using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroySled : MonoBehaviour
{
    public GameObject destroySledParticleEffect;
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<SledScript>())
        {
            if(destroySledParticleEffect != null)
            {
                GameObject particleSystem = Instantiate(destroySledParticleEffect, other.transform.position, other.transform.rotation);
                ParticleSystem.MainModule main = particleSystem.GetComponent<ParticleSystem>().main;
                main.startColor = other.gameObject.GetComponentInChildren<MeshRenderer>().material.color;
            }
            Destroy(other.gameObject);
        }
    }
}
