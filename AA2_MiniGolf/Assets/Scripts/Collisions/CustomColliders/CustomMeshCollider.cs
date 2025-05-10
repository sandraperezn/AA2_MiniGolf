using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class CustomMeshCollider : CustomCollider
{
    private Mesh mesh;
    private Vector3[] vertices;
    private int[] triangles;

    private void Awake()
    {
        // Cache mesh data
        MeshFilter mf = GetComponent<MeshFilter>();
        mesh = mf.sharedMesh;
        vertices = mesh.vertices;
        triangles = mesh.triangles;
    }

    /// <summary>
    /// Discrete sphere-vs-mesh collision. 
    /// Returns true if the sphere at sphereCenter with radius hits the mesh,
    /// and outputs the outward collision normal & penetration depth.
    /// </summary>
    public override bool DetectCollision(
        Vector3 sphereCenter,
        float sphereRadius,
        out Vector3 collisionNormal,
        out float penetration)
    {
        collisionNormal = Vector3.zero;
        penetration = 0f;

        // We'll pick the *smallest* distance (deepest penetration) across all triangles:
        float bestDistSqr = float.MaxValue;
        bool didHit = false;

        // Loop all triangles
        for (int i = 0; i < triangles.Length; i += 3)
        {
            // Transform triangle vertices into world space
            Vector3 v0 = transform.TransformPoint(vertices[triangles[i]]);
            Vector3 v1 = transform.TransformPoint(vertices[triangles[i + 1]]);
            Vector3 v2 = transform.TransformPoint(vertices[triangles[i + 2]]);

            // 1) Find the closest point on this triangle to the sphere center
            Vector3 closest = ClosestPointOnTriangle(sphereCenter, v0, v1, v2);

            // 2) Compute squared distance
            Vector3 diff = sphereCenter - closest;
            float distSqr = diff.sqrMagnitude;

            // 3) Check overlap
            if (distSqr <= sphereRadius * sphereRadius)
            {
                // We have a hit; check if it's the *deepest* so far
                if (distSqr < bestDistSqr)
                {
                    bestDistSqr = distSqr;
                    float dist = Mathf.Sqrt(distSqr);
                    penetration = sphereRadius - dist;

                    // Normal is from contact point → center (or face-normal if exactly coincident)
                    if (dist > 1e-6f)
                    {
                        collisionNormal = diff / dist;

                        // Si el punto más cercano está dentro del triángulo, invierte la normal
                        Vector3 faceNormal = Vector3.Cross(v1 - v0, v2 - v0).normalized;
                        if (Vector3.Dot(collisionNormal, faceNormal) < 0f)
                            collisionNormal = -collisionNormal;
                    }
                    else
                    {
                        collisionNormal = Vector3.Cross(v1 - v0, v2 - v0).normalized;
                    }

                    didHit = true;
                }
            }
        }

        return didHit;
    }

    /// <summary>
    /// Ericson’s method: closest point on triangle (a,b,c) to point p.
    /// Handles all vertex/edge/face regions.
    /// </summary>
    private Vector3 ClosestPointOnTriangle(Vector3 p, Vector3 a, Vector3 b, Vector3 c)
    {
        // Compute vectors        
        Vector3 ab = b - a;
        Vector3 ac = c - a;
        Vector3 ap = p - a;

        // Compute barycentric coords (region tests)
        float d1 = Vector3.Dot(ab, ap);
        float d2 = Vector3.Dot(ac, ap);
        if (d1 <= 0f && d2 <= 0f) return a; // barycentric (1,0,0)

        Vector3 bp = p - b;
        float d3 = Vector3.Dot(ab, bp);
        float d4 = Vector3.Dot(ac, bp);
        if (d3 >= 0f && d4 <= d3) return b; // (0,1,0)

        float vc = d1 * d4 - d3 * d2;
        if (vc <= 0f && d1 >= 0f && d3 <= 0f) // edge AB
        {
            float v = d1 / (d1 - d3);
            return a + ab * v;
        }

        Vector3 cp = p - c;
        float d5 = Vector3.Dot(ab, cp);
        float d6 = Vector3.Dot(ac, cp);
        if (d6 >= 0f && d5 <= d6) return c; // (0,0,1)

        float vb = d5 * d2 - d1 * d6;
        if (vb <= 0f && d2 >= 0f && d6 <= 0f) // edge AC
        {
            float w = d2 / (d2 - d6);
            return a + ac * w;
        }

        float va = d3 * d6 - d5 * d4;
        // inside face region
        float denom = va + vb + vc;
        float v2 = vb / denom;
        float w2 = vc / denom;
        return a + ab * v2 + ac * w2;
    }

    // Optional: draw mesh in editor to debug contact regions
    protected override void OnDrawGizmosInternal()
    {
        if (!mesh)
        {
            mesh = GetComponent<MeshFilter>().sharedMesh;
            triangles = mesh.triangles;
            vertices = mesh.vertices;
        }

        Gizmos.color = Color.red;
        for (int i = 0; i < triangles.Length; i += 3)
        {
            Vector3 v0 = transform.TransformPoint(vertices[triangles[i]]);
            Vector3 v1 = transform.TransformPoint(vertices[triangles[i + 1]]);
            Vector3 v2 = transform.TransformPoint(vertices[triangles[i + 2]]);
            Gizmos.DrawLine(v0, v1);
            Gizmos.DrawLine(v1, v2);
            Gizmos.DrawLine(v2, v0);
        }
    }
}