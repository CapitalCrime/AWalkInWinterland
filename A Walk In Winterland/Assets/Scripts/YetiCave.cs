using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class YetiCave : MonoBehaviour
{
    [SerializeField] YetiHand yetiHand;
    [SerializeField] float reachDistance = 3;
    SphereCollider yetiCaveBounds;
    Queue<Snowman> linedUpSnowmen = new Queue<Snowman>();
    // Start is called before the first frame update
    void Start()
    {
        yetiHand.releaseAction += ResetGrabTime;
        yetiHand.gameObject.SetActive(false);
        yetiCaveBounds = GetComponent<SphereCollider>();
        yetiCaveBounds.isTrigger = true;
    }

    float grabTime = 0;

    // Update is called once per frame
    void Update()
    {
        if(Time.time - grabTime >= 3 && !yetiHand.grabbing && linedUpSnowmen.Count > 0)
        {
            Debug.Log("Grabbed!");
            yetiHand.StartGrabbing();
            yetiHand.ReachSnowman(linedUpSnowmen.Dequeue(), transform.position, reachDistance);
        }
    }

    void ResetGrabTime()
    {
        grabTime = Time.time;
        yetiHand.StopGrabbing();
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
            yetiHand.MoveIntoHandRange(snowman, transform.position, reachDistance);
            linedUpSnowmen.Enqueue(snowman);
        }
    }
}
