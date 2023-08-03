using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class YetiCave : MonoBehaviour
{
    [SerializeField] YetiHand yetiGrabHand;
    [SerializeField] YetiHand yetiThrowHand;
    [SerializeField] float reachDistance = 3;
    SphereCollider yetiCaveBounds;
    Queue<Snowman> linedUpSnowmen = new Queue<Snowman>();
    Queue<Snowman> captiveSnowmen = new Queue<Snowman>();
    // Start is called before the first frame update
    void Start()
    {
        yetiGrabHand.releaseAction += ResetGrabTime;
        yetiThrowHand.releaseAction += ResetThrowingTime;
        yetiThrowHand.gameObject.SetActive(false);
        yetiGrabHand.gameObject.SetActive(false);
        yetiCaveBounds = GetComponent<SphereCollider>();
        yetiCaveBounds.isTrigger = true;
    }

    float grabTime = 0;
    float throwTime = -1;

    // Update is called once per frame
    void Update()
    {
        if(Time.time - grabTime >= 3 && !yetiGrabHand.grabbing && linedUpSnowmen.Count > 0)
        {
            Debug.Log("Grabbed!");
            yetiGrabHand.StartGrabbing();
            Snowman snowman = linedUpSnowmen.Dequeue();
            yetiGrabHand.ReachSnowman(snowman, transform.position, reachDistance);
        }

        if (throwTime != -1 && Time.time - throwTime >= 6 && !yetiThrowHand.throwing && captiveSnowmen.Count > 0)
        {
            Debug.Log("Thrown!");
            throwTime = -1;
            yetiThrowHand.StartThrowing();
            yetiThrowHand.ThrowSnowman(captiveSnowmen.Dequeue());
        }
    }

    void ResetGrabTime(Snowman snowman)
    {
        grabTime = Time.time;
        yetiGrabHand.StopGrabbing();

        captiveSnowmen.Enqueue(snowman);
        if (throwTime == -1)
        {
            throwTime = Time.time;
        }
    }

    void ResetThrowingTime(Snowman _)
    {
        throwTime = Time.time;
        yetiThrowHand.StopThrowing();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent(out Snowman snowman))
        {
            foreach(Snowman queueSnowman in linedUpSnowmen)
            {
                if (queueSnowman.GetInstanceID() == snowman.GetInstanceID()) return;
            }
            snowman.SetInteractable(false);
            if (linedUpSnowmen.Count == 0)
            {
                grabTime = Time.time;
            }
            yetiGrabHand.MoveIntoHandRange(snowman, transform.position, reachDistance);
            linedUpSnowmen.Enqueue(snowman);
        }
    }
}
