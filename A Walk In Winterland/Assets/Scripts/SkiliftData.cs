using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SkiliftData : MonoBehaviour
{
    [Serializable]
    public class SkiliftDropPoint
    {
        [SerializeField]float dropDistance;
        [SerializeField]Transform dropPoint;

        public Transform GetDropPoint()
        {
            return dropPoint;
        }

        public float GetDropDistance()
        {
            return dropDistance;
        }
    }
    [SerializeField][NonReorderable] List<SkiliftDropPoint> stopPoints;
    public event UnityAction signalMove;

    public void MoveChairs()
    {
        signalMove?.Invoke();
    }

    public Vector3 GetPointByIndex(int index)
    {
        return stopPoints[index].GetDropPoint().position;
    }

    public float GetDropDistanceAtIndex(int index)
    {
        return stopPoints[index].GetDropDistance();
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
