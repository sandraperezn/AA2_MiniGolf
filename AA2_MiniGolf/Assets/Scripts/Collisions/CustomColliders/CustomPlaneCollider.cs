using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class CustomPlaneCollider : CustomCollider
{
    [Tooltip("Normal of the plane in world-space")]
    public Vector3 planeNormal = Vector3.up;

    public override bool DetectCollision(
        Vector3 sphereCenter,
        float sphereRadius,
        out Vector3 collisionNormal,
        out float penetration)
    {
        // 1) Height of the plane
        MeshFilter mf = GetComponent<MeshFilter>();
        float halfHeight = mf.sharedMesh.bounds.extents.y * transform.localScale.y;
        float worldHeight = transform.position.y + halfHeight;

        // 2) Distance to plane
        Vector3 n = planeNormal.normalized;
        float d = Vector3.Dot(n, sphereCenter) - worldHeight;

        if (d < sphereRadius)
        {
            // 3) Projection point above plane
            Vector3 pointOnPlane = sphereCenter - n * d;

            // 4) Local space coordinates (includes scale)
            Vector3 local = transform.InverseTransformPoint(pointOnPlane);

            // 5) Extents of the plane in local space
            Vector3 meshExt = mf.sharedMesh.bounds.extents;

            if (Mathf.Abs(local.x) <= meshExt.x && Mathf.Abs(local.z) <= meshExt.z)
            {
                collisionNormal = n;
                penetration = sphereRadius - d;
                return true;
            }
        }

        collisionNormal = Vector3.zero;
        penetration = 0f;
        return false;
    }

    protected override void OnDrawGizmosInternal()
    {
        MeshFilter mf = GetComponent<MeshFilter>();
        Gizmos.color = Color.red;
        Gizmos.DrawWireMesh(mf.sharedMesh, transform.position, transform.rotation, transform.lossyScale);
    }
}