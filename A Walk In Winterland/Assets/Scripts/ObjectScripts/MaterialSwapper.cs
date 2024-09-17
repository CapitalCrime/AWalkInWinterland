using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MaterialSwapper : MonoBehaviour
{
    [SerializeField][NonReorderable] List<MaterialLOD> materialLODs;
    [SerializeField] Collider customBounds;
    static LODController playerController;
    Collider bounds;
    float boundSize = 1;
    MeshRenderer meshRenderer;
    int lastIndex = 0;
    int currentIndex = 0;

    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();

        if(customBounds != null)
        {
            bounds = customBounds;
        } else
        {
            BoxCollider tempCollider = new BoxCollider();
            tempCollider.size = Vector3.one;
            tempCollider.center = transform.position;

            bounds = tempCollider;
        }

        boundSize = bounds.bounds.size.magnitude;
        if (playerController == null)
        {
            playerController = LODController.instance;
        }
        if(meshRenderer == null)
        {
            Debug.Log("Missing Mesh Renderer component");
            enabled = false;
        }
        if(playerController == null)
        {
            Debug.Log("No LODController component attached to any object");
            enabled = false;
        }
        if(materialLODs == null)
        {
            Debug.Log("Missing material LODS");
            enabled = false;
        }

        SquareAllDistances();
    }

    void SquareAllDistances()
    {
        foreach(MaterialLOD materialLOD in materialLODs)
        {
            materialLOD.SetMaxDistance(materialLOD.GetMaxDistance() * materialLOD.GetMaxDistance());
        }
    }

    void CheckChangeLOD()
    {
        float currentDistance = 0;
        if (boundSize < 3)
        {
            currentDistance = (playerController.transform.position - transform.position).sqrMagnitude;
        } else
        {
            currentDistance = bounds.bounds.SqrDistance(playerController.transform.position);
        }
        if (currentIndex > 0 && currentDistance < materialLODs[currentIndex - 1].GetMaxDistance())
        {
            currentIndex = Mathf.Clamp(currentIndex - 1, 0, materialLODs.Count - 1);
        } else if (currentDistance > materialLODs[currentIndex].GetMaxDistance())
        {
            currentIndex = Mathf.Clamp(currentIndex+1, 0, materialLODs.Count-1);
        }
    }

    void SwapLOD()
    {
        if(currentIndex != lastIndex)
        {
            meshRenderer.material = materialLODs[currentIndex].GetMaterial();
        }
        lastIndex = currentIndex;
    }

    // Update is called once per frame
    void Update()
    {
        CheckChangeLOD();
        SwapLOD();
    }
}

[Serializable]
public class MaterialLOD
{
    [SerializeField] Material material;
    [SerializeField] float maxDistance;

    public MaterialLOD(Material material, float maxDistance)
    {
        this.material = material;
        this.maxDistance = maxDistance;
    }

    public Material GetMaterial()
    {
        return material;
    }

    public float GetMaxDistance()
    {
        return maxDistance;
    }

    public void SetMaxDistance(float amount)
    {
        maxDistance = amount;
    }
}
