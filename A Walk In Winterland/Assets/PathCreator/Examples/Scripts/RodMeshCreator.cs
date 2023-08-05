using System.Collections.Generic;
using PathCreation.Utility;
using UnityEngine;

namespace PathCreation.Examples {
    public class RodMeshCreator : PathSceneTool {
        [Header ("Rod settings")]
        [Range (0, .5f)]
        public float thickness = .15f;
        public int rodResolution = 8;
        public bool flattenSurface;

        [Header ("Material settings")]
        public Material roadMaterial;
        public Material undersideMaterial;
        public float textureTiling = 1;

        [SerializeField, HideInInspector]
        GameObject meshHolder;

        MeshFilter meshFilter;
        MeshRenderer meshRenderer;
        Mesh mesh;

        protected override void PathUpdated () {
            if (pathCreator != null) {
                AssignMeshComponents ();
                AssignMaterials ();
                CreateRoadMesh ();
            }
        }

        void CreateRoadMesh () {
            int numVerts = (path.NumPoints * 2);
            Vector3[] verts = new Vector3[numVerts * rodResolution];
            Vector2[] uvs = new Vector2[verts.Length];
            Vector3[] normals = new Vector3[verts.Length];

            int numTris = 2 * (path.NumPoints - 1) + ((path.isClosedLoop) ? 2 : 0);
            int[] rodTriangles = new int[numTris * rodResolution * 3];

            int vertIndex = 0;
            int triIndex = 0;

            // Vertices for the top of the road are layed out:
            // 0  1
            // 2  3
            // and so on... So the triangle map 0,8,1 for example, defines a triangle from top left to bottom left to bottom right.
            int[] triangleMap = { 0, 2, 1, 1, 2, 3 };
            int[] sidesTriangleMap = { 4, 6, 14, 12, 4, 14, 5, 15, 7, 13, 15, 5 };

            bool usePathNormals = !(path.space == PathSpace.xyz && flattenSurface);

            for (int i = 0; i < path.NumPoints; i++) {
                Vector3 localUp = (usePathNormals) ? Vector3.Cross(path.GetTangent(i), path.GetNormal(i)) : path.up;
                Vector3 localRight = (usePathNormals) ? path.GetNormal(i) : Vector3.Cross(localUp, path.GetTangent(i));

                // Find position to left and right of current path vertex
                Vector3 vertSideA = path.GetPoint(i) + localUp * Mathf.Abs(thickness);
                Vector3 vertSideB = path.GetPoint(i) + localUp * 2 * Mathf.Abs(thickness);

                for (int rodIndex = 0; rodIndex < rodResolution; rodIndex++)
                {
                    vertSideA = path.GetPoint(i) + Quaternion.AngleAxis((360/rodResolution) * rodIndex, path.GetTangent(i)) * localUp * Mathf.Abs(thickness);
                    vertSideB = path.GetPoint(i) + Quaternion.AngleAxis((360 / rodResolution) * (rodIndex + 1), path.GetTangent(i)) * localUp * Mathf.Abs(thickness);

                    verts[(rodIndex * 2) + vertIndex] = vertSideA;
                    verts[(rodIndex * 2) + vertIndex + 1] = vertSideB;

                    Vector3 faceNormal = Vector3.Cross(vertSideB - vertSideA, path.GetTangent(i)).normalized;
                    normals[(rodIndex * 2) + vertIndex] = faceNormal;
                    normals[(rodIndex * 2) + vertIndex + 1] = faceNormal;

                    uvs[(rodIndex * 2) + vertIndex] = new Vector2(0, path.times[i]);
                    uvs[(rodIndex * 2) + vertIndex + 1] = new Vector2(1, path.times[i]);
                }

                // Set triangle indices
                if (i < path.NumPoints - 1 || path.isClosedLoop) {
                    for (int k = 0; k < rodResolution; k++)
                    {
                        rodTriangles[triIndex + 5 + (k * 6)] = (vertIndex + k*2) % verts.Length;
                        rodTriangles[triIndex + 4 + (k * 6)] = (vertIndex + (rodResolution * 2) + k*2) % verts.Length;
                        rodTriangles[triIndex + 3 + (k * 6)] = (vertIndex + 1 + k * 2) % verts.Length;
                        rodTriangles[triIndex + 2 + (k * 6)] = (vertIndex + 1 + k * 2) % verts.Length;
                        rodTriangles[triIndex + 1 + (k * 6)] = (vertIndex + (rodResolution * 2) + k * 2) % verts.Length;
                        rodTriangles[triIndex + (k * 6)] = (vertIndex + (rodResolution * 2) + 1 + k * 2) % verts.Length;
                    }
                }

                vertIndex += 2*rodResolution;
                triIndex += 6*rodResolution;
            }

            mesh.Clear ();
            mesh.vertices = verts;
            mesh.uv = uvs;
            mesh.normals = normals;
            mesh.subMeshCount = 1;
            mesh.SetTriangles (rodTriangles, 0);
            mesh.RecalculateBounds ();
        }

        // Add MeshRenderer and MeshFilter components to this gameobject if not already attached
        void AssignMeshComponents () {

            if (meshHolder == null) {
                meshHolder = new GameObject ("Road Mesh Holder");
            }

            meshHolder.transform.rotation = Quaternion.identity;
            meshHolder.transform.position = Vector3.zero;
            meshHolder.transform.localScale = Vector3.one;

            // Ensure mesh renderer and filter components are assigned
            if (!meshHolder.gameObject.GetComponent<MeshFilter> ()) {
                meshHolder.gameObject.AddComponent<MeshFilter> ();
            }
            if (!meshHolder.GetComponent<MeshRenderer> ()) {
                meshHolder.gameObject.AddComponent<MeshRenderer> ();
            }

            meshRenderer = meshHolder.GetComponent<MeshRenderer> ();
            meshFilter = meshHolder.GetComponent<MeshFilter> ();
            if (mesh == null) {
                mesh = new Mesh ();
            }
            meshFilter.sharedMesh = mesh;
        }

        void AssignMaterials () {
            if (roadMaterial != null && undersideMaterial != null) {
                meshRenderer.sharedMaterials = new Material[] { roadMaterial };
                meshRenderer.sharedMaterials[0].mainTextureScale = new Vector3 (1, textureTiling);
            }
        }

    }
}