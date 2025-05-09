using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class BallController : MonoBehaviour
{
    [Header("Player Input")]
    public float forceMultiplier = 5f;
    public float maxVelocity = 50f;
    [Header("Friction")]
    [Range(0f, 1f)]
    public float surfaceFriction = 0.4f;

    private Vector3 velocity;
    private float ballRadius;
    private float ballMass;

    private void Start()
    {
        // Corrección del radio de la bola de golf 
        MeshFilter mf = GetComponent<MeshFilter>();
        float realRadius = mf.sharedMesh.bounds.extents.x * transform.localScale.x;
        PhysicsManager.Instance.ballRadius = realRadius;
        ballRadius = PhysicsManager.Instance.ballRadius;
        ballMass = PhysicsManager.Instance.ballMass;
    }

    private void Update()
    {
        // 1) Actualiza velocidad según físicas
        PhysicsManager.Instance.ApplyPhysics(ref velocity);

        // 2) Limita magnitud
        if (velocity.magnitude > maxVelocity)
            velocity = velocity.normalized * maxVelocity;

        // 3) Posición tentativa
        Vector3 currentPos = transform.position;
        Vector3 nextPos = currentPos + velocity * Time.deltaTime;

        // 4) Comprobar colisión contra suelo u otros CustomCollider
        if (CustomCollisionManager.Instance.CheckCollision(
                nextPos,
                PhysicsManager.Instance.ballRadius,
                out CustomCollider hitC,
                out Vector3 normal,
                out float penetration))
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