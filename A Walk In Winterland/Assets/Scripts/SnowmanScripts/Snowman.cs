using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Outline))]
public abstract class Snowman : MonoBehaviour
{
    public delegate void SnowmanDelegate(Snowman snowman);
    [HideInInspector] public static event SnowmanDelegate snowmanCreatedEvent;
    [HideInInspector] protected event UnityAction snowmanViewedEvent;
    [HideInInspector] protected event UnityAction snowmanFPSEvent;
    [HideInInspector] protected event UnityAction snowmanLeaveFPSEvent;
    [HideInInspector] public UnityAction<bool> snowmanEnableCameraEvent;
    [HideInInspector] public UnityAction<bool> snowmanRacingEvent;
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
    bool uniqueActionsEnabled = true;
    bool walkingEnabled = true;
    bool walkingStartEnabled = true;
    bool _cameraEnabled = true;
    Vector3 influenceDestination = Vector3.zero;
    float lastInfluenceTime;
    public bool cameraEnabled {
        get {
            return (_cameraEnabled && gameObject.activeSelf); 
        } 
        set {
            _cameraEnabled = value;
            snowmanEnableCameraEvent?.Invoke(value);
            if (_cameraEnabled == false && SnowmanManager.instance.CheckIfCurrentViewedSnowman(this))
            {
                SnowmanManager.instance.ActivatePlayerCamera();
            }
        } 
    }
    protected MinMax walkSpeeds = new MinMax(250, 400);
    event UnityAction queuedUntilEnabledEvents;
    protected abstract void NightArriveAction();
    protected abstract void DayArriveAction();

    protected abstract void UniqueAction();

    protected abstract void CancelUniqueAction();

    void QueuedDayActionCheck()
    {
        if (UniStorm.UniStormSystem.Instance.isDay())
        {
            DayArriveAction();
        }
    }

    public Rigidbody GetRigidbody()
    {
        return snowmanRigidbody;
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

    public void SetInfluenceDestination(Vector3 val)
    {
        if (Time.time - lastInfluenceTime < 10) return;
        influenceDestination = val;
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
        SetSnowmanEnabled(value);

        if (!value)
        {
            snowmanRigidbody.angularVelocity = Vector3.zero;
            snowmanRigidbody.velocity = Vector3.zero;
        }
        snowmanRigidbody.isKinematic = !value;
    }

    public void SetSnowmanEnabled(bool value)
    {
        ResetActionTimes();

        if (value)
        {
            EnableEvents();
        } else
        {
            DisableEvents();
            CancelUniqueAction();
        }

        enabled = value;
    }

    public void UnlockSnowman(bool value)
    {
        description.unlocked = true;
    }

    private void Awake()
    {
        thirdPersonCam = transform.GetComponentsInChildren<Cinemachine.CinemachineFreeLook>(true)[0];
        if(description == null)
        {
            Debug.LogError("Snowman description missing on snowman: " + transform.name);
        }

        if (description.randomUnlock)
        {
            description.unlocked = true;
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
        lastInfluenceTime = Time.time;
        snowmanRigidbody = GetComponent<Rigidbody>();

        walkingStartEnabled = walkingEnabled;
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

        snowmanRigidbody.centerOfMass = Vector3.zero;
        snowmanRigidbody.inertiaTensorRotation = Quaternion.identity;

        EnableSounds();
        EnableEvents();
    }

    public void AddForce(Vector3 amount)
    {
        snowmanRigidbody.AddForce(amount, ForceMode.Acceleration);
        influenceDestination = Vector3.zero;
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

    public void MoveInDirection(Vector3 forceDirection)
    {
        snowmanRigidbody.velocity = Vector3.zero;
        snowmanRigidbody.angularVelocity = Vector3.zero;
        AddForce(forceDirection);
        snowmanRigidbody.AddRelativeTorque(transform.up * forceDirection.magnitude / 3 * Mathf.Sign(transform.InverseTransformPoint(transform.position + forceDirection).x), ForceMode.Acceleration);
    }

    public void PushInDirection(Vector3 forceDirection)
    {
        AddForce(forceDirection);
        snowmanRigidbody.AddRelativeTorque(transform.up * forceDirection.magnitude / 3 * Mathf.Sign(transform.InverseTransformPoint(transform.position + forceDirection).x), ForceMode.Acceleration);
    }

    void PushRandomDirection()
    {
        float forceAmount = Random.Range(walkSpeeds.min, walkSpeeds.max);
        Quaternion forceAngle = Quaternion.Euler(0, Random.Range(0, 360), 0);
        Vector3 forceDirection = forceAngle * Vector3.right * forceAmount;
        PushInDirection(forceDirection);
    }

    protected void PauseWalkTime()
    {
        walkPauseSeconds = Time.time - lastWalkedSeconds;
    }

    protected void ResumeWalkTime()
    {
        lastWalkedSeconds = Time.time - walkPauseSeconds;
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
        if (uniqueActionsEnabled && Time.time - lastActivedUniqueActionSeconds >= uniqueActionSeconds)
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
            if(influenceDestination == Vector3.zero)
            {
                PushRandomDirection();
            }
            else
            {
                Vector3 direction = influenceDestination - transform.position;
                direction.y = 0;
                float force = direction.magnitude / (Time.fixedDeltaTime * snowmanRigidbody.mass);
                MoveInDirection(direction.normalized * force);
                lastInfluenceTime = Time.time;
            }
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

    void EnableSounds()
    {
        if (clickSoundRef.Target != null)
        {
            snowmanViewedEvent += ClickSoundPlay;
        }
    }

    void EnableEvents()
    {
        //DayCycle.nightActions += NightArriveAction;
        //DayCycle.nightActions += PauseActionTimes;
        UniStorm.UniStormSystem.Instance.OnNightArriveEvent.AddListener(NightActionTrigger);
        UniStorm.UniStormSystem.Instance.OnNightArriveEvent.AddListener(PauseActionTimes);
        //DayCycle.dayActions += DayArriveAction;
        //DayCycle.dayActions += ResumeActionTimes;
        UniStorm.UniStormSystem.Instance.OnDayArriveEvent.AddListener(DayActionTrigger);
        UniStorm.UniStormSystem.Instance.OnDayArriveEvent.AddListener(ResumeActionTimes);
    }

    void DisableSounds()
    {
        snowmanViewedEvent -= ClickSoundPlay;
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
    }

    protected virtual void OnEnable()
    {
        snowmanEnableCameraEvent?.Invoke(cameraEnabled);
        queuedUntilEnabledEvents?.Invoke();
        queuedUntilEnabledEvents = null;
    }

    protected virtual void OnDisable()
    {
        snowmanEnableCameraEvent?.Invoke(cameraEnabled);
    }

    protected virtual void OnDestroy()
    {
        DisableEvents();
        DisableSounds();
    }
}
