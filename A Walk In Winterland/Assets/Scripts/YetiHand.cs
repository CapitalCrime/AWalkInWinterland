using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Animation))]
public class YetiHand : MonoBehaviour
{
    public event UnityAction<Snowman> releaseAction; 
    [SerializeField] Transform yetiHandGrabPoint;
    Snowman heldSnowman;
    Transform yetiArm;
    Animation yetiAnimation;
    public bool grabbing { get; private set; }
    public bool throwing { get; private set; }
    // Start is called before the first frame update
    void Awake()
    {
        grabbing = false;
        throwing = false;
        yetiArm = transform.parent;
        yetiAnimation = GetComponent<Animation>();
    }

    public void ReachSnowman(Snowman snowman, Vector3 centerPosition, float reachDistance)
    {
        gameObject.SetActive(true);
        heldSnowman = snowman;

        Vector3 objectDirectionFromCenter = GetReachDirection(snowman, centerPosition);

        Vector3 yetiHandPosition = GetYetiHandPosition(snowman, centerPosition, objectDirectionFromCenter, reachDistance);//Mathf.Min(reachDistance, Vector3.Magnitude(centerPosition-snowman.transform.position));

        yetiArm.position = yetiHandPosition;
        yetiArm.forward = -objectDirectionFromCenter;

        snowman.transform.position = GetSnowmanHandRangePosition(snowman, centerPosition, reachDistance);

        yetiAnimation.Play();
    }

    public void ThrowSnowman(Snowman snowman)
    {
        gameObject.SetActive(true);
        heldSnowman = snowman;
        snowman.gameObject.SetActive(true);

        snowman.transform.SetParent(yetiHandGrabPoint);
        snowman.transform.localPosition = Vector3.zero;
        snowman.transform.localRotation = Quaternion.identity;

        transform.Rotate(Vector3.up * Random.value * 180, Space.World);

        yetiAnimation.Play();
    }

    float minThrowDistance = 8;
    public void LaunchSnowman()
    {
        heldSnowman.transform.SetParent(null, true);
        heldSnowman.SetInteractable(false);
        heldSnowman.transform.eulerAngles = Vector3.zero;
        LeanTween.move(heldSnowman.gameObject, heldSnowman.transform.position - transform.forward * minThrowDistance, 0.33f).setOnComplete(() =>
        {
            heldSnowman.GetRigidbody().velocity = Vector3.zero;
            heldSnowman.SetInteractable(true);
            Vector3 moveDir = -transform.forward;
            moveDir.y = 0;
            heldSnowman.MoveInDirection(moveDir * 350);
            heldSnowman.transform.eulerAngles = Vector3.zero;
            heldSnowman = null;

            releaseAction?.Invoke(null);
        });
    }

    Vector3 GetReachDirection(Snowman snowman, Vector3 centerPosition)
    {
        Vector3 objectDirectionFromCenter = snowman.transform.position - centerPosition;
        objectDirectionFromCenter.y = 0;
        objectDirectionFromCenter = objectDirectionFromCenter.normalized;
        return objectDirectionFromCenter;
    }

    Vector3 GetYetiHandPosition(Snowman snowman, Vector3 centerPosition, Vector3 reachDirection, float reachDistance)
    {
        return centerPosition - reachDirection * reachDistance;
    }

    Vector3 GetSnowmanHandRangePosition(Snowman snowman, Vector3 centerPosition, float reachDistance)
    {
        Vector3 yetiArmPosition = GetYetiHandPosition(snowman, centerPosition, GetReachDirection(snowman, centerPosition), reachDistance);
        Vector3 snowmanPos = snowman.transform.position;
        snowmanPos.y = centerPosition.y;
        snowmanPos = yetiArmPosition + (snowmanPos - centerPosition).normalized * 8.44f;
        snowmanPos.y = yetiArmPosition.y - 1.5f;
        return snowmanPos;
    }

    public void MoveIntoHandRange(Snowman snowman, Vector3 centerPosition, float reachDistance)
    {
        Vector3 inRangePosition = GetSnowmanHandRangePosition(snowman, centerPosition, reachDistance);
        float moveSpeed = ((snowman.transform.position - centerPosition).magnitude - (inRangePosition - centerPosition).magnitude)/3;
        LeanTween.move(snowman.gameObject, inRangePosition, moveSpeed).setOnComplete(() => snowman.transform.position = inRangePosition);
    }

    public void StartThrowing()
    {
        throwing = true;
    }

    public void StopThrowing()
    {
        throwing = false;
    }

    public void StartGrabbing()
    {
        grabbing = true;
    }

    public void StopGrabbing()
    {
        grabbing = false;
    }

    public void GrabSnowman()
    {
        if (heldSnowman == null) return;

        heldSnowman.transform.SetParent(yetiHandGrabPoint);
        heldSnowman.transform.localPosition = Vector3.zero;
    }

    public void ReleaseSnowman()
    {
        if (heldSnowman == null) return;

        heldSnowman.gameObject.SetActive(false);
        heldSnowman.transform.SetParent(null);
        releaseAction?.Invoke(heldSnowman);
        heldSnowman = null;
    }
}
