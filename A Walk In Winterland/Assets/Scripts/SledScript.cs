using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody))]
public class SledScript : MonoBehaviour
{
    Rigidbody rb;
    [SerializeField] float maxSpeed = 5;
    float lastHit = 0;
    [SerializeField] LayerMask ignoreLayers;
    [SerializeField] LayerMask groundMask;
    [SerializeField] Transform sledSeat;
    public event UnityAction destroyEvent;
    public Snowman seatedSnowman { get; private set; }
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        rb.centerOfMass = new Vector3(0, -3f, 0);
        lastHit = Time.time;
    }

    private void OnDisable()
    {
        if (rb == null) return;
        rb.isKinematic = true;
    }

    private void OnEnable()
    {
        if (rb == null) return;
        rb.isKinematic = false;
    }

    public void SeatSnowman(Snowman snowman)
    {
        seatedSnowman = snowman;
        seatedSnowman.SetInteractable(false);
        seatedSnowman.transform.SetParent(sledSeat);
        foreach(Collider collider in seatedSnowman.transform.GetComponentsInChildren<Collider>())
        {
            collider.enabled = false;
        }
        seatedSnowman.transform.localPosition = Vector3.zero;
    }

    public void UnseatSnowman()
    {
        if (seatedSnowman == null || seatedSnowman.gameObject == null) return;
        seatedSnowman.transform.SetParent(null);
        seatedSnowman.SetInteractable(true);
        foreach (Collider collider in seatedSnowman.transform.GetComponentsInChildren<Collider>())
        {
            collider.enabled = true;
        }
        seatedSnowman = null;
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
        if (Mathf.Abs(offCentreCross.y) < 0.1f && offCentreDot > 0) return;
        float rotateSmooth = Mathf.Clamp(Mathf.Abs(offCentreCross.y), 0.3f, 1);

        Vector3 rotateAxis = transform.up;
        
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

    private void OnApplicationQuit()
    {
        seatedSnowman = null;
        destroyEvent = null;
    }

    private void OnDestroy()
    {
        UnseatSnowman();
        destroyEvent?.Invoke();
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
