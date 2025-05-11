using UnityEngine;

namespace Collisions
{
    [RequireComponent(typeof(MeshFilter))]
    public class CustomMeshCollider : CustomCollider
    {
        private Vector3[] worldVerts;
        private int[] tris;

        private void Awake()
        {
            // Cache triangles and world-space verts once (static geometry)
            MeshFilter mf = GetComponent<MeshFilter>();
            Mesh mesh = mf.sharedMesh;
            tris = mesh.triangles;

            Vector3[] local = mesh.vertices;
            worldVerts = new Vector3[local.Length];
            for (int i = 0; i < local.Length; i++)
            {
                worldVerts[i] = transform.TransformPoint(local[i]);
            }
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
            for (int i = 0; i < tris.Length; i += 3)
            {
                // 1) Get the three world-space vertices
                Vector3 a = worldVerts[tris[i]];
                Vector3 b = worldVerts[tris[i + 1]];
                Vector3 c = worldVerts[tris[i + 2]];

                // 2) Plane test: project sphereCenter onto triangle plane
                Vector3 n = Vector3.Cross(b - a, c - a).normalized;
                float d = Vector3.Dot(sphereCenter - a, n);
                if (Mathf.Abs(d) > sphereRadius) continue; // too far from this plane

                Vector3 p = sphereCenter - n * d;

                // 3) Point-in-triangle test (barycentric method)
                if (!IsPointInTriangle(p, a, b, c)) continue;

                // 4) We have a hit — immediately return
                //    Ensure normal points *out* of the mesh toward the sphere
                if (Vector3.Dot(sphereCenter - p, n) < 0f)
                {
                    n = -n;
                }

                collisionNormal = n;
                penetration = sphereRadius - Mathf.Abs(d);
                return true;
            }

            return false;
        }

        // Barycentric coordinate test
        private static bool IsPointInTriangle(
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

            return u >= 0f && v >= 0f && u + v <= 1f;
        }

        protected override void OnDrawGizmosInternal()
        {
            MeshFilter mf = GetComponent<MeshFilter>();
            Gizmos.color = Color.red;
            Gizmos.DrawWireMesh(mf.sharedMesh, transform.position, transform.rotation, transform.lossyScale);
        }
    }
}