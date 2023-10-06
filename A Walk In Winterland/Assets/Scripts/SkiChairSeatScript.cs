using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PathCreation.Examples.PathFollower))]
[RequireComponent(typeof(BoxCollider))]
public class SkiChairSeatScript : MonoBehaviour
{
    enum MovementType
    {
        Stopped,
        Moving
    }

    MovementType currentMovement;
    [SerializeField] List<Transform> seats;
    [SerializeField] SkiliftData skiliftData;
    [SerializeField] int startPointIndex;
    [SerializeField] Vector3 seatForwardDirection;
    BoxCollider chairCollider;
    Vector3 stopPoint;
    int currentPointIndex;
    float normalChairSpeed;
    PathCreation.Examples.PathFollower pathFollower;
    Snowman[] passengers;
    int passengersOnboard = 0;

    private void Awake()
    {
        chairCollider = GetComponent<BoxCollider>();
        pathFollower = GetComponent<PathCreation.Examples.PathFollower>();
        passengers = new Snowman[seats.Count];
        normalChairSpeed = pathFollower.speed;
        currentPointIndex = skiliftData.GetValidIndex(startPointIndex);
        stopPoint = skiliftData.GetPointByIndex(currentPointIndex);
        skiliftData.signalMove += MoveChair;
        Debug.Log(transform.name + " distance is " + pathFollower.GetDistanceByPoint(stopPoint));
        Debug.Log("Total length = " + pathFollower.GetTotalLength());
    }

    private void Start()
    {
        currentMovement = MovementType.Stopped;
        pathFollower.SetDistanceByPoint(stopPoint);
    }

    float targetDistance = 0;

    void MoveChair()
    {
        if (currentMovement == MovementType.Stopped)
        {
            skiliftData.GetNextIndex(ref currentPointIndex);
            stopPoint = skiliftData.GetPointByIndex(currentPointIndex);
            pathFollower.speed = normalChairSpeed;
            currentMovement = MovementType.Moving;
            targetDistance = pathFollower.GetDistanceTravelled() - pathFollower.GetDistanceByPoint(stopPoint);
            if(targetDistance < 0)
            {
                targetDistance = pathFollower.GetDistanceByPoint(stopPoint) - pathFollower.GetTotalLength();
            }
            Debug.Log(transform.name+" current distance travelled "+pathFollower.GetDistanceTravelled());
            Debug.Log(transform.name + " target point distance " + pathFollower.GetDistanceByPoint(stopPoint));
        }
    }

    // Update is called once per frame
    void Update()
    {
        switch (currentMovement)
        {
            case MovementType.Moving:
                if(pathFollower.GetDistanceTravelled() < targetDistance)//if (Vector3.Magnitude(pathFollower.GetPointByDistance() - stopPoint) < 1)
                {
                    pathFollower.speed = 0;
                    pathFollower.SetDistanceByPoint(stopPoint);
                    for(int i = 0; i < passengers.Length; i++)
                    {
                        RemovePassenger(i);
                    }
                    currentMovement = MovementType.Stopped;
                }
                break;
            case MovementType.Stopped:
                pathFollower.speed = 0;
                if (passengersOnboard == seats.Count)
                {
                    skiliftData.MoveChairs();
                }
                break;
            default:
                break;
        }
    }

    void SeatPassenger(Snowman snowman, int index)
    {
        passengers[index] = snowman;
        snowman.SetInteractable(false);
        snowman.transform.SetParent(seats[index].transform);
        snowman.transform.localPosition = Vector3.zero;
    }

    void RemovePassenger(int index)
    {
        if(passengers[index] != null)
        {
            passengersOnboard--;
            passengers[index].transform.SetParent(null);
            Vector3 seatRemoveDirection = transform.TransformDirection(seatForwardDirection);
            seatForwardDirection.y = 0;
            passengers[index].transform.position = seats[index].position + seatRemoveDirection * skiliftData.GetDropDistanceAtIndex(currentPointIndex);
            passengers[index].SetInteractable(true);
            passengers[index].transform.rotation = Quaternion.Euler(0, passengers[index].transform.rotation.eulerAngles.y, 0);
            passengers[index] = null;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(passengersOnboard < seats.Count && other.TryGetComponent(out Snowman snowman))
        {
            Debug.Log("Snowman hit the chair!");
            int nullIndex = -1;
            for(int i = 0; i<passengers.Length; i++)
            {
                if (passengers[i] == null)
                {
                    nullIndex = i;
                }
                else if (passengers[i].GetInstanceID() == snowman.GetInstanceID())
                {
                    return;
                }
            }
            if (nullIndex != -1)
            {
                SeatPassenger(snowman, nullIndex);
                passengersOnboard++;
            }
        }
    }
}
