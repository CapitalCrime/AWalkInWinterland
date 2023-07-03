using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneWayGateScript : MonoBehaviour
{
    [SerializeField] Vector3 openDirection = Vector3.forward;
    [SerializeField] Vector3 physicsColliderSize;
    [SerializeField] Animation openAnimation;
    BoxCollider objectCollider;
    BoxCollider physicsCollider;
    // Start is called before the first frame update
    void Start()
    {
        objectCollider = gameObject.GetComponent<BoxCollider>();
        openDirection = transform.TransformDirection(openDirection);

        physicsCollider = gameObject.AddComponent<BoxCollider>();
        physicsCollider.size = objectCollider.size + physicsColliderSize;
        physicsCollider.center = objectCollider.center;
        physicsCollider.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Snowman")) return;
        if (Physics.ComputePenetration(physicsCollider, transform.position, transform.rotation, 
            other, other.transform.position, other.transform.rotation,
            out Vector3 direction, out float depth))
        {
            if(Vector3.Dot(openDirection, direction) > 0)
            {
                if(openAnimation != null && !openAnimation.isPlaying)
                {
                    openAnimation.Play();
                }
                if (other.TryGetComponent(out Snowman snowman))
                {
                    snowman.MoveInDirection(openDirection * 300);
                }
                Physics.IgnoreCollision(objectCollider, other, true);
            } else
            {
                Physics.IgnoreCollision(objectCollider, other, false);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
