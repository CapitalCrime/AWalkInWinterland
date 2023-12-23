using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class DOFAdjuster : MonoBehaviour
{
    public VolumeProfile profile;
    public float adjustSpeed = 8;
    [SerializeField] LayerMask hitMask;
    float hitDistance = 0;
    float maxDistance = 1000;
    DepthOfField dof;
    float beforePauseDistance = 0;

    // Start is called before the first frame update
    void Start()
    {
        if(profile == null)
        {
            Debug.LogError("Camera missing volume profile");
        }
        if(profile.TryGet(out DepthOfField tempDOF))
        {
            dof = tempDOF;
        } else
        {
            Debug.LogError("Camera volume profile is missing Depth of Field component");
        }
    }

    // Update is called once per frame
    void Update()
    {
        Ray cameraRay = new Ray(transform.position, transform.forward);
        RaycastHit hitInfo;
        Debug.DrawRay(transform.position, transform.forward * 5, Color.red);
        if(Physics.Raycast(cameraRay, out hitInfo, maxDistance, hitMask))
        {
            hitDistance = hitInfo.distance+0.5f;
        } else
        {
            hitDistance = Mathf.Clamp(hitDistance + 0.5f, 5, maxDistance);
        }

        AdjustDOFDistance();
    }

    public void SetPauseDistance()
    {
        beforePauseDistance = dof.focusDistance.value;
        dof.active = false;
    }

    public void ResetPlayDistance()
    {
        dof.active = true;
        dof.focusDistance.value = beforePauseDistance;
    }

    void AdjustDOFDistance()
    {
        float difference = dof.focusDistance.value - hitDistance;
        dof.focusDistance.value = Mathf.Clamp(Mathf.Lerp(dof.focusDistance.value, hitDistance, Time.deltaTime * adjustSpeed * (100/Mathf.Abs(difference))), 0.5f, maxDistance);
    }

    private void OnDestroy()
    {
        dof.focusDistance.value = 100;
        dof.active = true;
    }
}
