using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RacerSnowman : Snowman
{
    enum RacerState
    {
        Relaxing,
        Drifting
    }

    Coroutine driftingRoutine;
    RacerState racerState = RacerState.Relaxing;
    float driftingTime = 0;
    float timeSinceChangeDriftDirection = 0;
    bool turnRight = true;
    [SerializeField] private FMOD.Studio.EventInstance carSoundInstance;
    [SerializeField] private FMODUnity.EmitterRef carSoundRef;
    [SerializeField] private FMODUnity.EmitterRef carSongRef;

    IEnumerator switchDriftingState()
    {
        while (carSoundRef.Target.IsPlaying())
        {
            yield return null;
        }

        racerState = RacerState.Drifting;
        driftingTime = 0;
        timeSinceChangeDriftDirection = 0;
        if (carSongRef.Target != null)
        {
            carSongRef.Target.SetParameter("FinishLoop", 0);
            carSongRef.Target.Stop();
            carSongRef.Target.Play();
        }

        while (driftingTime < 15)
        {
            if(timeSinceChangeDriftDirection > 2.5f)
            {
                turnRight = Random.value > 0.5f ? true : false;
                timeSinceChangeDriftDirection = 0;
            }
            driftingTime += Time.deltaTime;
            timeSinceChangeDriftDirection += Time.deltaTime;
            yield return null;
        }

        racerState = RacerState.Relaxing;
        ResetActionTimes();
        snowmanRigidbody.velocity = snowmanRigidbody.velocity.normalized * 4;
        float turnDirection = turnRight ? 150f : -150f;
        snowmanRigidbody.AddRelativeTorque(Vector3.up * turnDirection, ForceMode.Acceleration);
        if (carSongRef.Target != null)
        {
            carSongRef.Target.SetParameter("FinishLoop", 1);
        }
        yield return null;
    }
    public override void DayArriveAction()
    {
    }

    public override void NightArriveAction()
    {
    }

    protected override void UniqueAction()
    {
        if(carSoundRef.Target != null)
        {
            carSoundRef.Target.Play();
        }

        if(driftingRoutine != null)
        {
            StopCoroutine(driftingRoutine);
        }
        driftingRoutine = StartCoroutine(switchDriftingState());
    }

    protected override void Start()
    {
        base.Start();
        walkSpeeds = new MinMax(300, 450);
    }

    protected override void Update()
    {
        if(racerState == RacerState.Relaxing)
        {
            base.Update();
        } else if(racerState == RacerState.Drifting)
        {
            float turnDirection = turnRight ? 1f : -1f;
            snowmanRigidbody.AddRelativeTorque(Vector3.up * turnDirection, ForceMode.Acceleration);
            Vector3 newVel = transform.forward * 18;
            newVel.y = snowmanRigidbody.velocity.y;
            snowmanRigidbody.velocity = newVel;
        }
        transform.eulerAngles = Vector3.up * transform.eulerAngles.y;
    }
}
