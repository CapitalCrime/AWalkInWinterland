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
    Vector3 seatForwardDirectionLocal;

    private void Awake()
    {
        chairCollider = GetComponent<BoxCollider>();
        pathFollower = GetComponent<PathCreation.Examples.PathFollower>();
        passengers = new Snowman[seats.Count];
        normalChairSpeed = pathFollower.speed;
        currentPointIndex = skiliftData.GetValidIndex(startPointIndex);
        stopPoint = skiliftData.GetPointByIndex(currentPointIndex);
        skiliftData.signalMove += MoveChair;
        seatForwardDirectionLocal = transform.TransformDirection(seatForwardDirection);
    }

    private void Start()
    {
        currentMovement = MovementType.Stopped;
        pathFollower.SetDistanceByPoint(stopPoint);
    }

    void MoveChair()
    {
        if (currentMovement == MovementType.Stopped)
        {
            skiliftData.GetNextIndex(ref currentPointIndex);
            stopPoint = skiliftData.GetPointByIndex(currentPointIndex);
            pathFollower.speed = normalChairSpeed;
            currentMovement = MovementType.Moving;
        }
    }

    // Update is called once per frame
    void Update()
    {
        switch (currentMovement)
        {
            case MovementType.Moving:
                if (Vector3.Magnitude(pathFollower.GetPointByDistance() - stopPoint) < 2)
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
            passengers[index].transform.position = seats[index].position + seatForwardDirectionLocal * 10;
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