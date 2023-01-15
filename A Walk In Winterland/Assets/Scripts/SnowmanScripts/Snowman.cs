using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class Snowman : MonoBehaviour
{
    public delegate void SnowmanDelegate(Snowman snowman);
    [HideInInspector] public static event SnowmanDelegate snowmanCreatedEvent;
    [SerializeField] Cinemachine.CinemachineVirtualCamera fpsCam;
    public SnowmanDescription description;
    Rigidbody snowmanRigidbody;
    float uniqueActionSeconds;
    float walkSeconds;
    float lastActivedUniqueActionSeconds;
    float lastWalkedSeconds;
    bool walkingEnabled = true;
    public abstract void NightArriveAction();
    public abstract void DayArriveAction();

    protected abstract void UniqueAction();

    public Cinemachine.CinemachineVirtualCamera GetSnowmanFPSCam()
    {
        return fpsCam;
    }
    void SetWalkActivationTime()
    {
        walkSeconds = Random.Range(description.randomWalkTimeSeconds.min, description.randomWalkTimeSeconds.max);
    }

    void SetUniqueActionActivationTime()
    {
        uniqueActionSeconds = Random.Range(description.randomUniqueActionTimeSeconds.min, description.randomUniqueActionTimeSeconds.max);
    }

    private void Awake()
    {
        if(description == null)
        {
            Debug.LogError("Snowman description missing on snowman: " + transform.name);
        }

        if(description.randomWalkTimeSeconds.min == 0 && description.randomWalkTimeSeconds.max == 0)
        {
            walkingEnabled = false;
        } else
        {
            SetWalkActivationTime();
        }
        SetUniqueActionActivationTime();
        lastWalkedSeconds = Time.time;
        lastActivedUniqueActionSeconds = Time.time;
        if(TryGetComponent(out Rigidbody rigidbody))
        {
            snowmanRigidbody = rigidbody;
        } else
        {
            Debug.LogError("No rigidbody found on snowman: "+transform.name);
        }

        DayCycle.nightActions += NightArriveAction;
        DayCycle.dayActions += DayArriveAction;
    }

    protected virtual void Start()
    {
        snowmanCreatedEvent?.Invoke(this);
    }

    void PushRandomDirection()
    {
        float forceAmount = Random.Range(200, 350);
        Quaternion forceAngle = Quaternion.Euler(0, Random.Range(0, 360), 0);
        Vector3 forceDirection = forceAngle * Vector3.right * forceAmount;
        snowmanRigidbody.AddForce(forceDirection);
        snowmanRigidbody.AddRelativeTorque(transform.up * forceDirection.magnitude / 10 * Mathf.Sign(transform.InverseTransformPoint(transform.position + forceDirection).x));
    }

    protected virtual void Update()
    {
        if(Time.time-lastActivedUniqueActionSeconds >= uniqueActionSeconds)
        {
            SetUniqueActionActivationTime();
            lastActivedUniqueActionSeconds = Time.time;
            UniqueAction();
        }

        if(walkingEnabled && Time.time-lastWalkedSeconds >= walkSeconds)
        {
            SetWalkActivationTime();
            lastWalkedSeconds = Time.time;
            PushRandomDirection();
        }
    }

    private void OnDestroy()
    {
        DayCycle.nightActions -= NightArriveAction;
        DayCycle.dayActions -= DayArriveAction;
    }
}
