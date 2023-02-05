using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class Snowman : MonoBehaviour
{
    public delegate void SnowmanDelegate(Snowman snowman);
    [HideInInspector] public static event SnowmanDelegate snowmanCreatedEvent;
    [HideInInspector] protected event UnityAction snowmanViewedEvent;
    [SerializeField] Cinemachine.CinemachineVirtualCamera fpsCam;
    public Cinemachine.CinemachineFreeLook thirdPersonCam;
    public SnowmanDescription description;
    protected Rigidbody snowmanRigidbody;
    float uniqueActionSeconds;
    float walkSeconds;
    float lastActivedUniqueActionSeconds;
    float lastWalkedSeconds;
    bool walkingEnabled = true;
    bool walkingStartEnabled = true;
    protected MinMax walkSpeeds = new MinMax(200, 350);
    public abstract void NightArriveAction();
    public abstract void DayArriveAction();

    protected abstract void UniqueAction();

    public Cinemachine.CinemachineVirtualCamera GetSnowmanFPSCam()
    {
        return fpsCam;
    }

    public Cinemachine.CinemachineFreeLook GetSnowmanThirdCam()
    {
        return thirdPersonCam;
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
        description.unlocked = true;

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
        DayCycle.dayActions += ResetActionTimes;
        walkingStartEnabled = walkingEnabled;
    }

    public void EnableWalking(bool value)
    {
        if (!walkingStartEnabled) return;
        if(walkingEnabled == false && value == true)
        {
            lastWalkedSeconds = Time.time;
        }
        walkingEnabled = value;
    }

    protected virtual void Start()
    {
        snowmanCreatedEvent?.Invoke(this);
    }

    public void AddForce(Vector3 amount)
    {
        snowmanRigidbody.AddForce(amount, ForceMode.Acceleration);
    }

    public void OnViewEvent()
    {
        snowmanViewedEvent?.Invoke();
    }

    void PushRandomDirection()
    {
        float forceAmount = Random.Range(walkSpeeds.min, walkSpeeds.max);
        Quaternion forceAngle = Quaternion.Euler(0, Random.Range(0, 360), 0);
        Vector3 forceDirection = forceAngle * Vector3.right * forceAmount;
        snowmanRigidbody.AddForce(forceDirection, ForceMode.Acceleration);
        snowmanRigidbody.AddRelativeTorque(transform.up * forceDirection.magnitude/3 * Mathf.Sign(transform.InverseTransformPoint(transform.position + forceDirection).x), ForceMode.Acceleration);
    }

    void ResetActionTimes()
    {
        lastWalkedSeconds = Time.time;
        lastActivedUniqueActionSeconds = Time.time;
    }

    protected void UniqueCheck()
    {
        if (Time.time - lastActivedUniqueActionSeconds >= uniqueActionSeconds)
        {
            SetUniqueActionActivationTime();
            lastActivedUniqueActionSeconds = Time.time;
            UniqueAction();
        }
    }

    protected void WalkCheck()
    {
        if (walkingEnabled && Time.time - lastWalkedSeconds >= walkSeconds)
        {
            SetWalkActivationTime();
            lastWalkedSeconds = Time.time;
            PushRandomDirection();
        }
    }

    void PerformActions()
    {
        if (!DayCycle.IsSunUp()) return;

        UniqueCheck();
        WalkCheck();
    }

    protected virtual void Update()
    {
        PerformActions();
    }

    private void OnDestroy()
    {
        DayCycle.nightActions -= NightArriveAction;
        DayCycle.dayActions -= DayArriveAction;
        DayCycle.dayActions -= ResetActionTimes;
    }
}
