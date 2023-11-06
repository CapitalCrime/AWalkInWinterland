using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SledRaceFinish : MonoBehaviour
{
    [SerializeField] List<Snowman> snowmenPlacements = new List<Snowman>();
    bool startRace = false;
    public void ResetPlacements()
    {
        snowmenPlacements = new List<Snowman>();
        startRace = false;
    }

    public void DisplayPlacements()
    {

    }

    public void StartRace()
    {
        startRace = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(startRace && other.TryGetComponent(out SledScript sled))
        {
            if (!snowmenPlacements.Contains(sled.seatedSnowman))
            {
                snowmenPlacements.Add(sled.seatedSnowman);
            }
        }
    }
}
