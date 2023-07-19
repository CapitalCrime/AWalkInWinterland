using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SkiliftData : MonoBehaviour
{
    [SerializeField] List<Transform> stopPoints;
    public event UnityAction signalMove;

    public void MoveChairs()
    {
        signalMove?.Invoke();
    }

    public Vector3 GetPointByIndex(int index)
    {
        return stopPoints[index].position;
    }

    public int GetValidIndex(int index)
    {
        if(index >= stopPoints.Count)
        {
            index = stopPoints.Count - 1;
        }else if(index < 0)
        {
            index = 0;
        }
        return index;
    }

    public void GetNextIndex(ref int index)
    {
        if(index < stopPoints.Count-1)
        {
            index++;
        } else
        {
            index = 0;
        }
    }
}
