using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class CustomMeshCollider : CustomCollider
{
    [Header("Hole Triangles")] [Tooltip("Indices of triangles to skip (e.g. holes).")] [SerializeField]
    private List<int> holeTriangles = new List<int>();

    private Vector3[] worldVerts;
    private int[] tris;

    private void Awake()
    {
        // Cache triangles & world-space verts once (static geometry)
        MeshFilter mf = GetComponent<MeshFilter>();
        Mesh mesh = mf.sharedMesh;
        tris = mesh.triangles;

        Vector3[] local = mesh.vertices;
        worldVerts = new Vector3[local.Length];
        for (int i = 0; i < local.Length; i++)
            worldVerts[i] = transform.TransformPoint(local[i]);
    }

    public override bool DetectCollision(
        Vector3 sphereCenter,
        float sphereRadius,
        out Vector3 collisionNormal,
        out float penetration)
    {
        collisionNormal = Vector3.zero;
        penetration = 0f;

        // Brute-force every triangle
        for (int i = 0, triIdx = 0; i < tris.Length; i += 3, triIdx++)
        {
            // 1) Skip hole tris
            if (holeTriangles.Contains(triIdx))
                continue;

            // 2) Get the three world-space vertices
            Vector3 A = worldVerts[tris[i]];
            Vector3 B = worldVerts[tris[i + 1]];
            Vector3 C = worldVerts[tris[i + 2]];

            // 3) Plane test: project sphereCenter onto triangle plane
            Vector3 n = Vector3.Cross(B - A, C - A).normalized;
            float d = Vector3.Dot(sphereCenter - A, n);
            if (Mathf.Abs(d) > sphereRadius)
                continue; // too far from this plane

            Vector3 P = sphereCenter - n * d;

            // 4) Point-in-triangle test (barycentric method)
            if (!IsPointInTriangle(P, A, B, C))
                continue;

            // 5) We have a hit — immediately return
            //    Ensure normal points *out* of the mesh toward the sphere
            if (Vector3.Dot(sphereCenter - P, n) < 0f)
                n = -n;

            collisionNormal = n;
            penetration = sphereRadius - Mathf.Abs(d);
            return true;
        }

        return false;
    }

    // Barycentric coordinate test, identical to your friend's:
    private bool IsPointInTriangle(
        Vector3 p, Vector3 a, Vector3 b, Vector3 c)
    {
        Vector3 v0 = c - a;
        Vector3 v1 = b - a;
        Vector3 v2 = p - a;

        float d00 = Vector3.Dot(v0, v0);
        float d01 = Vector3.Dot(v0, v1);
        float d11 = Vector3.Dot(v1, v1);
        float d20 = Vector3.Dot(v2, v0);
        float d21 = Vector3.Dot(v2, v1);

        float denom = d00 * d11 - d01 * d01;
        if (Mathf.Abs(denom) < 1e-6f) return false;

        float u = (d11 * d20 - d01 * d21) / denom;
        float v = (d00 * d21 - d01 * d20) / denom;

        return (u >= 0f) && (v >= 0f) && (u + v <= 1f);
    }

    // No extra gizmos by default
    protected override void OnDrawGizmosInternal()
    {
    }
}