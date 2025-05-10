using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class CustomPlaneCollider : CustomCollider
{
    [Tooltip("Normal del plano en world-space")]
    public Vector3 planeNormal = Vector3.up;

    public override bool DetectCollision(
        Vector3 sphereCenter,
        float sphereRadius,
        out Vector3 collisionNormal,
        out float penetration)
    {
        // 1) Altura del plano
        MeshFilter mf = GetComponent<MeshFilter>();
        float halfHeight = mf.sharedMesh.bounds.extents.y * transform.localScale.y;
        float worldHeight = transform.position.y + halfHeight;

        // 2) Distancia al plano
        Vector3 n = planeNormal.normalized;
        float d = Vector3.Dot(n, sphereCenter) - worldHeight;

        if (d < sphereRadius)
        {
            // 3) Punto de proyección sobre el plano
            Vector3 pointOnPlane = sphereCenter - n * d;

            // 4) Coords en espacio local (incluye escala)
            Vector3 local = transform.InverseTransformPoint(pointOnPlane);

            // 5) Extents *no* escalados (bounds están en unidades de malla)
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
        // Mitades en cada eje, en mundo:
        float halfW = mf.sharedMesh.bounds.extents.x * transform.localScale.x;
        float halfH = mf.sharedMesh.bounds.extents.y * transform.localScale.y;
        float halfD = mf.sharedMesh.bounds.extents.z * transform.localScale.z;

        // Altura de la cara superior:
        Vector3 n = planeNormal.normalized;
        Vector3 center = transform.position + n * halfH;

        // Direcciones de los ejes locales:
        Vector3 right = transform.right * halfW;
        Vector3 forward = transform.forward * halfD;

        // Cuatro esquinas del rectángulo:
        Vector3 p1 = center + right + forward;
        Vector3 p2 = center + right - forward;
        Vector3 p3 = center - right - forward;
        Vector3 p4 = center - right + forward;

        // Dibujar líneas:
        Gizmos.DrawLine(p1, p2);
        Gizmos.DrawLine(p2, p3);
        Gizmos.DrawLine(p3, p4);
        Gizmos.DrawLine(p4, p1);

        // (Opcional) Para ver la normal:
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(center, center + n);
    }
}