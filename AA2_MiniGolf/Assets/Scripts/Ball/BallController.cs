using Collisions;
using UnityEngine;
using Utils;

namespace Ball
{
    [RequireComponent(typeof(MeshFilter))]
    public class BallController : MonoBehaviour
    {
        [SerializeField] private float maxVelocity = 50f;
        [SerializeField] private float skinWidth = 0.001f;
        private Vector3 velocity;

        // Offset from transform.position to the mesh’s true center (in world space)
        private Vector3 localCenterOffset;
        private Vector3 WorldCenterOffset => transform.TransformPoint(localCenterOffset) - transform.position;
        private float radius;

        private void Awake()
        {
            // figure out your real radius and center offset from the mesh’s bounds
            MeshFilter mf = GetComponent<MeshFilter>();
            Bounds b = mf.sharedMesh.bounds;
            radius = b.extents.x * transform.localScale.x;
            localCenterOffset = b.center;
            PhysicsManager.Instance.BallRadius = radius;
        }

        private void FixedUpdate()
        {
            float dt = Time.fixedDeltaTime;
            // 1) Update velocity from gravity/air/friction
            PhysicsManager.Instance.ApplyPhysics(ref velocity, dt);

            // 2) Clamp top speed
            if (velocity.sqrMagnitude > maxVelocity * maxVelocity)
                velocity = velocity.normalized * maxVelocity;

            // 3) Compute world-space start and total displacement of the sphere center
            Vector3 startCenter = transform.position + WorldCenterOffset;
            Vector3 totalDisplacement = velocity * dt;

            // 4) Sub-step so we never move more than half a radius in one go
            float maxStep = radius * 0.5f;
            int steps = Mathf.Max(1, Mathf.CeilToInt(totalDisplacement.magnitude / maxStep));

            Vector3 currentCenter = startCenter;

            for (int i = 0; i < steps; i++)
            {
                Vector3 targetCenter = currentCenter + totalDisplacement / steps;

                // 5) Check collision at targetCenter
                if (CustomCollisionManager.Instance.CheckCollision(
                        targetCenter,
                        radius,
                        out CustomCollider hitCollider,
                        out Vector3 normal,
                        out float penetration,
                        out float surfaceFriction))
                {
                    // 6) push out along the normal
                    currentCenter = targetCenter + normal * (penetration + skinWidth);

                    // — horizontal floor? zero Y
                    if (normal.y > 0.5f) velocity.y = 0f;

                    // — slope acceleration
                    Vector3 slope = Vector3.ProjectOnPlane(Vector3.down, normal).normalized;
                    velocity += PhysicsManager.Gravity * dt * slope;

                    // — friction on horizontal
                    Vector3 vH = new(velocity.x, 0, velocity.z);
                    if (vH.magnitude > 0.01f)
                    {
                        Vector3 fAcc = PhysicsManager.Gravity * surfaceFriction * -vH.normalized;
                        vH += fAcc * dt;
                        if (Vector3.Dot(vH, fAcc) > 0f) vH = Vector3.zero;
                    }

                    velocity.x = vH.x;
                    velocity.z = vH.z;

                    // — restitution
                    Vector3 normalVector = Vector3.Project(velocity, normal);
                    Vector3 tangentVector = velocity - normalVector;
                    velocity = tangentVector - normalVector * hitCollider.restitution;

                    // stop sub-stepping after a hit this frame
                    break;
                }

                currentCenter = targetCenter;
            }

            // 7) finally, write back the *transform.position* so that the mesh (which
            //    is offset by _worldCenterOffset) ends up in the right place.
            transform.position = currentCenter - WorldCenterOffset;
        }

        public void Launch(Vector3 initialVelocity)
        {
            velocity = initialVelocity;

            if (AudioManager.Instance)
            {
                AudioManager.Instance.PlaySfx(AudioManager.SfxType.Shoot);
            }
        }
    }
}