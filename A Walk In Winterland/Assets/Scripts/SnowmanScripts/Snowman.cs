using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class Snowman : MonoBehaviour
{
    public delegate void SnowmanDelegate(Snowman snowman);
    [HideInInspector] public static event SnowmanDelegate snowmanCreatedEvent;
    [HideInInspector] protected event UnityAction snowmanViewedEvent;
    [HideInInspector] protected event UnityAction snowmanFPSEvent;
    [HideInInspector] protected event UnityAction snowmanLeaveFPSEvent;
    [SerializeField] Cinemachine.CinemachineVirtualCamera fpsCam;
    [SerializeField] private FMODUnity.EmitterRef clickSoundRef;
    public Cinemachine.CinemachineFreeLook thirdPersonCam;
    public SnowmanDescription description;
    protected Rigidbody snowmanRigidbody;
    float uniqueActionSeconds;
    float walkSeconds;
    float lastActivedUniqueActionSeconds;
    float lastWalkedSeconds;
    float uniqueActionPauseSeconds;
    float walkPauseSeconds;
    bool walkingEnabled = true;
    bool walkingStartEnabled = true;
    protected MinMax walkSpeeds = new MinMax(250, 400);
    event UnityAction queuedUntilEnabledEvents;
    protected abstract void NightArriveAction();
    protected abstract void DayArriveAction();

    protected abstract void UniqueAction();

    void QueuedDayActionCheck()
    {
        if (UniStorm.UniStormSystem.Instance.isDay())
        {
            DayArriveAction();
        }
    }

    void QueuedNightActionCheck()
    {
        if (!UniStorm.UniStormSystem.Instance.isDay())
        {
            NightArriveAction();
        }
    }

    void DayActionTrigger()
    {
        if (enabled)
        {
            DayArriveAction();
        } else
        {
            queuedUntilEnabledEvents += QueuedDayActionCheck;
            queuedUntilEnabledEvents -= QueuedNightActionCheck;
        }
    }

    void NightActionTrigger()
    {
        if (enabled)
        {
            NightArriveAction();
        }
        else
        {
            queuedUntilEnabledEvents += QueuedNightActionCheck;
            queuedUntilEnabledEvents -= QueuedDayActionCheck;
        }
    }

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

    private void ClickSoundPlay()
    {
        if(clickSoundRef.Target != null)
        {
            clickSoundRef.Target.Play();
        }
    }

    public void SetInteractable(bool value)
    {
        ResetActionTimes();

        if (value)
        {
            EnableEvents();
        } else
        {
            DisableEvents();
        }

        enabled = value;
        snowmanRigidbody.isKinematic = !value;

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

        walkingStartEnabled = walkingEnabled;
    }

    private void OnEnable()
    {
        queuedUntilEnabledEvents?.Invoke();
        queuedUntilEnabledEvents = null;
    }

    public void EnableWalking(bool value)
    {
        if (!walkingStartEnabled) return;
        if(value == false)
        {
            PauseWalkTime();
        }
        if(walkingEnabled == false && value == true)
        {
            ResumeWalkTime();
        }
        walkingEnabled = value;
    }

    protected virtual void Start()
    {
        snowmanCreatedEvent?.Invoke(this);

        EnableEvents();
    }

    public void AddForce(Vector3 amount)
    {
        snowmanRigidbody.AddForce(amount, ForceMode.Acceleration);
    }

    public void OnViewFPSEvent()
    {
        snowmanFPSEvent?.Invoke();
    }

    public void OnLeaveFPSEvent()
    {
        snowmanLeaveFPSEvent?.Invoke();
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
        AddForce(forceDirection);
        snowmanRigidbody.AddRelativeTorque(transform.up * forceDirection.magnitude/3 * Mathf.Sign(transform.InverseTransformPoint(transform.position + forceDirection).x), ForceMode.Acceleration);
    }

    protected void PauseWalkTime()
    {
        walkPauseSeconds = Time.time - lastWalkedSeconds;
        Debug.Log("Walk pause time: " + walkPauseSeconds);
    }

    protected void ResumeWalkTime()
    {
        lastWalkedSeconds = Time.time - walkPauseSeconds;
        Debug.Log("Walk resume time: " + (Time.time- lastWalkedSeconds));
    }

    protected void PauseActionTimes()
    {
        uniqueActionPauseSeconds = Time.time - lastActivedUniqueActionSeconds;
        PauseWalkTime();
    }

    protected void ResumeActionTimes()
    {
        lastActivedUniqueActionSeconds = Time.time - uniqueActionPauseSeconds;
        ResumeWalkTime();
    }

    protected void ResetActionTimes()
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
        //if (!DayCycle.IsSunUp()) return;
        if (!UniStorm.UniStormSystem.Instance.isDay()) return;

        UniqueCheck();
        WalkCheck();
    }

    protected virtual void Update()
    {
        PerformActions();
    }

    void EnableEvents()
    {
        if (clickSoundRef.Target != null)
        {
            snowmanViewedEvent += ClickSoundPlay;
        }

        //DayCycle.nightActions += NightArriveAction;
        //DayCycle.nightActions += PauseActionTimes;
        UniStorm.UniStormSystem.Instance.OnNightArriveEvent.AddListener(NightActionTrigger);
        UniStorm.UniStormSystem.Instance.OnNightArriveEvent.AddListener(PauseActionTimes);
        //DayCycle.dayActions += DayArriveAction;
        //DayCycle.dayActions += ResumeActionTimes;
        UniStorm.UniStormSystem.Instance.OnDayArriveEvent.AddListener(DayActionTrigger);
        UniStorm.UniStormSystem.Instance.OnDayArriveEvent.AddListener(ResumeActionTimes);
    }

    void DisableEvents()
    {
        //DayCycle.nightActions -= NightArriveAction;
        //DayCycle.nightActions -= PauseActionTimes;
        UniStorm.UniStormSystem.Instance.OnNightArriveEvent.RemoveListener(NightActionTrigger);
        UniStorm.UniStormSystem.Instance.OnNightArriveEvent.RemoveListener(PauseActionTimes);
        //DayCycle.dayActions -= DayArriveAction;
        //DayCycle.dayActions -= ResumeActionTimes;
        UniStorm.UniStormSystem.Instance.OnDayArriveEvent.RemoveListener(DayActionTrigger);
        UniStorm.UniStormSystem.Instance.OnDayArriveEvent.RemoveListener(ResumeActionTimes);
        snowmanViewedEvent -= ClickSoundPlay;
    }

    protected virtual void OnDestroy()
    {
        DisableEvents();
    }
}
