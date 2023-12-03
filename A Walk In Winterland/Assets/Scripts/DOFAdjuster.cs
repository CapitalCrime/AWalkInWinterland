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
    DepthOfField dof;

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
        if(Physics.Raycast(cameraRay, out hitInfo, 1000, hitMask))
        {
            hitDistance = hitInfo.distance+0.5f;
        } else
        {
            hitDistance = Mathf.Clamp(hitDistance + 0.5f, 5, 1000);
        }

        AdjustDOFDistance();
    }

    void AdjustDOFDistance()
    {
        float difference = dof.focusDistance.value - hitDistance;
        dof.focusDistance.value = Mathf.Clamp(Mathf.Lerp(dof.focusDistance.value, hitDistance, Time.deltaTime * adjustSpeed * (100/Mathf.Abs(difference))), 1, 1000);
    }
}
