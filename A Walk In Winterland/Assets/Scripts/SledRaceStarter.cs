using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SledRaceStarter : MonoBehaviour
{
    [SerializeField] SledScript[] sleds;
    List<SledScript> sledStorage = new List<SledScript>();
    List<Snowman> snowmen = new List<Snowman>();
    int sledsActive = 0;
    bool raceActive = false;
    public UnityEvent resetRace;
    public UnityEvent startRace;

    private void Awake()
    {
    }
    // Start is called before the first frame update
    void Start()
    {
        foreach (SledScript sled in sleds)
        {
            sled.enabled = false;
            sled.destroyEvent += SledDestroyEvent;
            SledScript storedSled = Instantiate(sled.gameObject, sled.transform.position, sled.transform.rotation).GetComponent<SledScript>();
            storedSled.gameObject.SetActive(false);
            sledStorage.Add(storedSled);
        }
    }

    void SledDestroyEvent()
    {
        sledsActive -= 1;
        if(sledsActive <= 0)
        {
            resetRace?.Invoke();
            ResetSleds();
        }
    }

    void ResetSleds()
    {
        foreach (SledScript sled in sledStorage)
        {
            if (sled == null) continue;
            SledScript newSled = Instantiate(sled);
            newSled.gameObject.SetActive(true);
        }
        raceActive = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(!raceActive && other.TryGetComponent(out Snowman snowman) && !snowmen.Contains(snowman))
        {
            snowmen.Add(snowman);
            sleds[sledsActive].SeatSnowman(snowman);
            sledsActive += 1;
            if (sledsActive >= sleds.Length)
            {
                raceActive = true;
                startRace?.Invoke();
                foreach (SledScript sled in sleds)
                {
                    sled.enabled = true;
                }
            }
        }
    }
}
