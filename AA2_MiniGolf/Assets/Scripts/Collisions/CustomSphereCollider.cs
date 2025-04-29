using UnityEngine;

[RequireComponent(typeof(Transform))]
public class CustomSphereCollider : CustomCollider
{
    [Tooltip("Radio de esta esfera (en unidades de mundo).")]
    public float sphereRadius = 0.5f;

    public override bool DetectCollision(
        Vector3 sphereCenter,    // centro de la bola en movimiento
        float movingRadius,      // radio de la bola en movimiento
        out Vector3 collisionNormal,
        out float penetration)
    {
        // Centro de esta esfera estática
        Vector3 center = transform.position;

        // Suma de radios para detección
        float sumR = sphereRadius + movingRadius;
        float dist = Vector3.Distance(sphereCenter, center);

        if (dist < sumR)
        {
            // Normal desde la superficie de este collider hacia la bola
            collisionNormal = (sphereCenter - center).normalized;
            penetration = sumR - dist;
            return true;
        }

        collisionNormal = Vector3.zero;
        penetration = 0f;
        return false;
    }
}