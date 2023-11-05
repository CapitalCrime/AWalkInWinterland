using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpySnowman : Snowman
{
    [SerializeField] GameObject[] spySnowmanParts;
    Transform disguiseHolder;
    bool disguised = false;
    [SerializeField] private FMODUnity.EmitterRef disguiseSoundRef;
    [SerializeField] ParticleSystem disguiseParticles;
    protected override void DayArriveAction()
    {
    }

    protected override void NightArriveAction()
    {

    }

    public Snowman FindRandomSpawnedSnowman(List<Snowman> snowmen)
    {
        int randomIndex = Random.Range(0, snowmen.Count);
        if (randomIndex < snowmen.Count)
        {
            return snowmen[randomIndex];
        }
        else
        {
            return null;
        }
    }

    void Disguise(bool disguised)
    {
        //TODO: Create disguise particle effect and trigger it
        disguiseParticles.gameObject.SetActive(true);

        if (disguised)
        {
            //Hide spy parts
            foreach (GameObject part in spySnowmanParts)
            {
                part.SetActive(false);
            }

            //Find existing snowmen, pick one
            List<Snowman> snowmen = SnowmanManager.instance.GetSnowmanList();
            snowmen.Remove(this);
            if (snowmen.Count == 0) return;
            Snowman pickedSnowman = FindRandomSpawnedSnowman(snowmen);

            //Take every ACTIVE mesh renderer, set their local positions correctly, and add to snowman disguise holder
            foreach(MeshRenderer snowmanPart in pickedSnowman.GetComponentsInChildren<MeshRenderer>())
            {
                if (!snowmanPart.gameObject.activeSelf) continue;

                //Accounting for mesh part offset
                Vector3 localPosition =
                    Vector3.Scale(pickedSnowman.transform.InverseTransformPoint(snowmanPart.transform.position), snowmanPart.transform.lossyScale);
                GameObject newPart = Instantiate(snowmanPart.gameObject, disguiseHolder);
                newPart.transform.localPosition = localPosition;
                //Accounting for mesh part scaling
                newPart.transform.localScale = snowmanPart.transform.lossyScale;
            }
        }
        else
        {
            foreach (Transform costume in disguiseHolder)
            {
                Destroy(costume.gameObject);
            }
            foreach (GameObject part in spySnowmanParts)
            {
                part.SetActive(true);
            }
        }

        GetComponent<Outline>().RecalcultateOutline();
    }

    protected override void UniqueAction()
    {
        if(disguiseSoundRef.Target != null)
        {
            disguiseSoundRef.Target.Play();
        }

        disguised = !disguised;

        Disguise(disguised);
    }

    protected override void CancelUniqueAction()
    {
        disguised = false;
        Disguise(disguised);
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        disguiseParticles.gameObject.SetActive(false);
    }

    protected override void Start()
    {
        base.Start();
        disguiseParticles.gameObject.SetActive(false);

        disguiseHolder = new GameObject().transform;
        disguiseHolder.parent = transform;
        disguiseHolder.localPosition = Vector3.zero;
        disguiseHolder.name = "DisguiseHolder";
    }
}
