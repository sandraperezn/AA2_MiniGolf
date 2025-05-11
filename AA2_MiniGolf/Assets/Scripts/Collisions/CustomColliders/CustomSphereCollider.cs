using UnityEngine;

namespace Collisions
{
    [RequireComponent(typeof(Transform))]
    public class CustomSphereCollider : CustomCollider
    {
        [Tooltip("Radio de esta esfera (en unidades de mundo).")]
        public float sphereRadius = 0.5f;

        public override bool DetectCollision(
            Vector3 sphereCenter,
            float movingRadius,
            out Vector3 collisionNormal,
            out float penetration)
        {
            Vector3 center = transform.position;

            // Sum of the radii for collision detection
            float sumR = sphereRadius + movingRadius;
            float dist = Vector3.Distance(sphereCenter, center);

            if (dist < sumR)
            {
                // Normal from surface to the sphere
                collisionNormal = (sphereCenter - center).normalized;
                penetration = sumR - dist;
                return true;
            }

            collisionNormal = Vector3.zero;
            penetration = 0f;
            return false;
        }

        protected override void OnDrawGizmosInternal()
        {
            Gizmos.DrawWireSphere(transform.position, sphereRadius);
        }
    }
}