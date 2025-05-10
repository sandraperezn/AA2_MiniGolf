using UnityEngine;

/// <summary>
/// Super-simple sphere vs. mesh collider with no culling whatsoever.
/// </summary>
[RequireComponent(typeof(MeshFilter))]
public class CustomMeshCollider : CustomCollider
{
    private Vector3[] _verts;
    private int[]     _tris;

    private void Awake()
    {
        var mf = GetComponent<MeshFilter>();
        var m  = mf.sharedMesh;
        _verts = m.vertices;
        _tris  = m.triangles;
    }

    /// <summary>
    /// Walk every triangle, project the sphere center onto it,
    /// and pick the deepest‐penetrating one.
    /// </summary>
    public override bool DetectCollision(
        Vector3 sphereCenter,
        float   sphereRadius,
        out Vector3 collisionNormal,
        out float   penetration)
    {
        collisionNormal = Vector3.zero;
        penetration     = 0f;

        bool   hitAny   = false;
        float  bestPen  = 0f;
        Vector3 bestNorm = Vector3.zero;

        // brute-force every tri
        for (int i = 0; i < _tris.Length; i += 3)
        {
            // world-space verts
            Vector3 A = transform.TransformPoint(_verts[_tris[i]]);
            Vector3 B = transform.TransformPoint(_verts[_tris[i + 1]]);
            Vector3 C = transform.TransformPoint(_verts[_tris[i + 2]]);

            // closest point on tri:
            Vector3 P = ClosestPointOnTriangle(sphereCenter, A, B, C);
            Vector3 delta = sphereCenter - P;
            float   dist  = delta.magnitude;

            if (dist < sphereRadius)
            {
                float pen = sphereRadius - dist;
                if (!hitAny || pen > bestPen)
                {
                    hitAny  = true;
                    bestPen = pen;
                    bestNorm = (dist > Mathf.Epsilon)
                        ? delta.normalized
                        : Vector3.Cross(B - A, C - A).normalized;
                }
            }
        }

        if (hitAny)
        {
            penetration     = bestPen;
            collisionNormal = bestNorm;
            return true;
        }

        return false;
    }

    protected override void OnDrawGizmosInternal()
    {
        throw new System.NotImplementedException();
    }

    // Ericson’s algorithm for closest point on triangle
    private static Vector3 ClosestPointOnTriangle(
        Vector3 p,
        Vector3 a,
        Vector3 b,
        Vector3 c)
    {
        Vector3 ab = b - a, ac = c - a, ap = p - a;
        float d1 = Vector3.Dot(ab, ap), d2 = Vector3.Dot(ac, ap);
        if (d1 <= 0f && d2 <= 0f) return a;

        Vector3 bp = p - b;
        float d3 = Vector3.Dot(ab, bp), d4 = Vector3.Dot(ac, bp);
        if (d3 >= 0f && d4 <= d3) return b;

        float vc = d1 * d4 - d3 * d2;
        if (vc <= 0f && d1 >= 0f && d3 <= 0f)
        {
            float v = d1 / (d1 - d3);
            return a + v * ab;
        }

        Vector3 cp = p - c;
        float d5 = Vector3.Dot(ab, cp), d6 = Vector3.Dot(ac, cp);
        if (d6 >= 0f && d5 <= d6) return c;

        float vb = d5 * d2 - d1 * d6;
        if (vb <= 0f && d2 >= 0f && d6 <= 0f)
        {
            float w = d2 / (d2 - d6);
            return a + w * ac;
        }

        float va = d3 * d6 - d5 * d4;
        float denom = 1f / (va + vb + vc);
        float v2 = vb * denom, w2 = vc * denom;
        return a + ab * v2 + ac * w2;
    }
}
