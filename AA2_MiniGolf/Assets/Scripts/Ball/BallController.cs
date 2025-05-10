using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class BallController : MonoBehaviour
{
    private const float MaxVelocity = 50f;
    private Vector3 velocity;

    private void Start()
    {
        // Corrección del radio de la bola de golf 
        MeshFilter mf = GetComponent<MeshFilter>();
        float realRadius = mf.sharedMesh.bounds.extents.x * transform.localScale.x;
        PhysicsManager.Instance.ballRadius = realRadius;
    }

    private void Update()
    {
        // 1) Actualiza velocidad según físicas
        PhysicsManager.Instance.ApplyPhysics(ref velocity);

        // 2) Limita magnitud
        if (velocity.magnitude > MaxVelocity)
            velocity = velocity.normalized * MaxVelocity;

        // 3) Posición tentativa
        Vector3 currentPos = transform.position;
        Vector3 nextPos = currentPos + velocity * Time.deltaTime;

        // 4) Comprobar colisión contra suelo u otros CustomCollider
        if (CustomCollisionManager.Instance.CheckCollision(
                nextPos,
                PhysicsManager.Instance.ballRadius,
                out CustomCollider hitC,
                out Vector3 normal,
                out float penetration,
                out float surfaceFriction))
        {
            // Posicionar justo fuera del collider
            transform.position = nextPos + normal * (penetration + 0.001f);

            // Si es suelo horizontal, cancelar Y
            if (normal.y > 0.5f)
                velocity.y = 0f;

            // Aplicar componente de pendiente y fricción en XZ
            Vector3 slope = Vector3.ProjectOnPlane(Vector3.down, normal).normalized;
            velocity += PhysicsManager.Instance.gravity * Time.deltaTime * slope;

            Vector3 vH = new(velocity.x, 0f, velocity.z);
            if (vH.magnitude > 0.01f)
            {
                Vector3 fAcc = PhysicsManager.Instance.gravity * surfaceFriction * -vH.normalized;
                vH += fAcc * Time.deltaTime;
                if (Vector3.Dot(vH, fAcc) > 0f)
                    vH = Vector3.zero;
            }

            velocity.x = vH.x;
            velocity.z = vH.z;

            // Restitución (choque)
            float e = hitC.restitution;
            Vector3 vNorm = Vector3.Project(velocity, normal);
            Vector3 vTan = velocity - vNorm;
            velocity = vTan - vNorm * e;
        }
        else
        {
            transform.position = nextPos;
        }
    }
}