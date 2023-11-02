using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class SledScript : MonoBehaviour
{
    Rigidbody rb;
    [SerializeField] float maxSpeed = 5;
    float lastHit = 0;
    [SerializeField] LayerMask ignoreLayers;
    [SerializeField] LayerMask groundMask;
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    // Start is called before the first frame update
    void Start()
    {
        rb.centerOfMass = new Vector3(0, -3f, 0);
        lastHit = Time.time;
    }

    private void FixedUpdate()
    {
        Vector2 newVel = new Vector2(rb.velocity.x, rb.velocity.z);

        if (Physics.Raycast(transform.position + transform.forward, -Vector3.up, out RaycastHit hitInfoFront, 5, groundMask))
        {
            if (Physics.Raycast(transform.position - transform.forward, -Vector3.up, out RaycastHit hitInfoBack, 5, groundMask))
            {
                if (hitInfoBack.point.y < hitInfoFront.point.y)
                {
                    Vector3 diff = hitInfoBack.point - hitInfoFront.point;
                    newVel += new Vector2(diff.x, diff.z).normalized * Time.fixedDeltaTime * 5;
                }
                else
                {
                    Vector3 diff = hitInfoFront.point - hitInfoBack.point;
                    newVel += new Vector2(diff.x, diff.z).normalized * Time.fixedDeltaTime * 5;
                }
            }
        }

        //if (newVel.magnitude < 3 && rb.velocity.y < 0)
        //{
        //    newVel = new Vector2(transform.forward.x, rb.velocity.z).normalized * 3;
        //}else 
        
        if (newVel.magnitude > maxSpeed)
        {
            newVel = newVel.normalized * maxSpeed;
        }

        rb.velocity = new Vector3(newVel[0], rb.velocity.y, newVel[1]);// Vector3.Lerp(rb.velocity, new Vector3(newVel[0], rb.velocity.y, newVel[1]), Time.fixedDeltaTime*5);
    }

    private void Update()
    {
        if (Physics.Raycast(transform.position, -Vector3.up, out RaycastHit hitInfo, 7, groundMask))
        {
            Quaternion.LookRotation(transform.forward, hitInfo.normal);
        }

        if (rb.velocity.magnitude < 1) return;
        float offCentreDot = Vector3.Dot(transform.right, rb.velocity.normalized);
        Vector3 offCentreCross = Vector3.Cross(transform.forward, rb.velocity.normalized);
        //if (Mathf.Abs(amountOffCentre) < 0.1f) return;
        if (Mathf.Abs(offCentreCross.y) < 0.1f && offCentreDot > 0) return;
        //float rotateSmooth = Mathf.Clamp(Mathf.Abs(offCentreDot), 0.3f, 1);
        float rotateSmooth = Mathf.Clamp(Mathf.Abs(offCentreCross.y), 0.3f, 1);
        //if (amountOffCentre < 0)
        //{
        //    transform.RotateAround(transform.position, transform.up, Time.deltaTime * -70 * rotateSmooth);
        //} else
        //{
        //    transform.RotateAround(transform.position, transform.up, Time.deltaTime * 70 * rotateSmooth);
        //}

        Vector3 rotateAxis = transform.up;
        //if(Physics.Raycast(transform.position, -Vector3.up, out RaycastHit hitInfo, 5, groundMask))
        //{
        //    rotateAxis = hitInfo.normal;
        //}
        
        if (offCentreCross.y < 0)
        {
            transform.RotateAround(transform.position, rotateAxis, Time.fixedDeltaTime * -200 * rotateSmooth);
        }
        else
        {
            transform.RotateAround(transform.position, rotateAxis, Time.fixedDeltaTime * 200 * rotateSmooth);
        }
    }

    bool CheckLayerMask(int layer)
    {
        return ignoreLayers == (ignoreLayers | (1 << layer));
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(!(collision.collider is TerrainCollider) && !collision.transform.GetComponent<SledScript>() && !CheckLayerMask(collision.gameObject.layer))
        {
            if(Time.time - lastHit > 1.5f)
            {
                lastHit = Time.time;
                Vector3 hitDirection = (transform.position - collision.contacts[0].point).normalized;
                if(Mathf.Abs(hitDirection.x) > Mathf.Abs(hitDirection.z))
                {
                    hitDirection.z += (Random.value*2.5f) * Mathf.Sign(hitDirection.z);
                } else
                {
                    hitDirection.x += (Random.value*2.5f) * Mathf.Sign(hitDirection.x);
                }
                rb.velocity = hitDirection*5;
            }
        }
    }
}
