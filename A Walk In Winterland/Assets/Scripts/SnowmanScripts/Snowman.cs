using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Snowman : MonoBehaviour
{
    public Rigidbody snowmanRigidbody;
    float uniqueActionSeconds;
    float walkSeconds;
    float lastActivedUniqueActionSeconds;
    float lastWalkedSeconds;
    public abstract void NightArriveAction();
    public abstract void DayArriveAction();

    protected abstract void UniqueAction();

    void SetWalkActivationTime()
    {
        walkSeconds = Random.Range(5, 10);
    }

    void SetUniqueActionActivationTime()
    {
        uniqueActionSeconds = Random.Range(60, 150);
    }

    private void Awake()
    {
        SetWalkActivationTime();
        SetUniqueActionActivationTime();
        lastWalkedSeconds = Time.time;
        lastActivedUniqueActionSeconds = Time.time;

        DayCycle.nightActions += NightArriveAction;
        DayCycle.dayActions += DayArriveAction;
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

        if(Time.time-lastWalkedSeconds >= walkSeconds)
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
