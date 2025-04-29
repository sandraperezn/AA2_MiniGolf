using UnityEngine;

[RequireComponent(typeof(Renderer))]
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
        // 1) Calcula la Y (o la distancia en la dirección normal)
        var rend = GetComponent<Renderer>();
        // si el plano está horizontal, .bounds.max.y es la altura real del tope
        float worldPlaneY = rend.bounds.max.y;

        // 2) distancia del centro de la esfera a ese "worldPlaneY"
        float d = Vector3.Dot(planeNormal.normalized, sphereCenter) - worldPlaneY;

        if (d < sphereRadius)
        {
            collisionNormal = planeNormal.normalized;
            penetration = sphereRadius - d;
            return true;
        }

        collisionNormal = Vector3.zero;
        penetration = 0f;
        return false;
    }
}