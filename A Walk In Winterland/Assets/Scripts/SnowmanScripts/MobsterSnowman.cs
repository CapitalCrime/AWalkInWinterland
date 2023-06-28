using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobsterSnowman : Snowman
{
    public Transform cannonFirePoint;
    public GameObject snowballPrefab;
    [SerializeField] private FMODUnity.EmitterRef snowcannonSoundRef;
    Coroutine shootRoutine;
    public LayerMask snowmanMask;

    protected override void DayArriveAction()
    {
    }

    protected override void NightArriveAction()
    {
    }

    IEnumerator ShootAction()
    {
        Collider[] snowmen = Physics.OverlapSphere(transform.position, 25, snowmanMask);
        Transform closestSnowman = null;
        float closestSnowmanDistance = Mathf.Infinity;
        foreach(Collider snowman in snowmen)
        {
            if (snowman.transform == transform) continue;
            float distance = Vector3.SqrMagnitude(transform.position - snowman.transform.position);
            if (distance < closestSnowmanDistance)
            {
                closestSnowmanDistance = distance;
                closestSnowman = snowman.transform;
            }
        }
        yield return null;
        if(closestSnowman != null)
        {
            Quaternion startRotation = transform.rotation;
            Vector3 endDirection = (closestSnowman.position - cannonFirePoint.position);
            endDirection.y = 0;
            Quaternion endRotation = Quaternion.identity;
            float timer = 0;
            while (timer < 1)
            {
                snowmanRigidbody.angularVelocity = Vector3.zero;
                endDirection = (closestSnowman.position - (cannonFirePoint.position-cannonFirePoint.transform.forward)).normalized;
                endDirection.y = 0;
                endRotation = Quaternion.LookRotation(endDirection);
                snowmanRigidbody.rotation = Quaternion.Lerp(startRotation, endRotation, timer);
                timer += Time.deltaTime*1.25f;
                yield return null;
            }
            snowmanRigidbody.rotation = endRotation;
            yield return new WaitForSeconds(0.25f);
        }
        snowmanRigidbody.AddForce(-cannonFirePoint.forward * 150);
        Instantiate(snowballPrefab, cannonFirePoint.position, cannonFirePoint.rotation);
        if (snowcannonSoundRef.Target != null)
        {
            snowcannonSoundRef.Target.Play();
        }
        EnableWalking(true);
        yield return null;
    }

    protected override void UniqueAction()
    {
        if(shootRoutine != null)
        {
            StopCoroutine(shootRoutine);
        }
        EnableWalking(false);
        shootRoutine = StartCoroutine(ShootAction());
    }
}
