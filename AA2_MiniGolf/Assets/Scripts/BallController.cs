using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class BallController : MonoBehaviour
{
    [Header("Player Input")]
    public float forceMultiplier = 5f;
    public float maxVelocity = 50f;

    [Header("Friction")]
    [Range(0f,1f)]
    public float surfaceFriction = 0.4f;

    private Vector3 velocity;
    private LineRenderer lineRenderer;
    private Vector3 dragStart;
    private bool isDragging = false;
    private float ballRadius;
    private float ballMass;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
        lineRenderer.useWorldSpace = true;
        lineRenderer.enabled = false;
        
        //Corrección del radio de la bola de golf 
        var mf = GetComponent<MeshFilter>();
        float realRadius = mf.sharedMesh.bounds.extents.x * transform.localScale.x; 
        PhysicsManager.Instance.ballRadius = realRadius;
        
        ballRadius = PhysicsManager.Instance.ballRadius;
        ballMass = PhysicsManager.Instance.ballMass;
    }

    void Update()
    {
        // 1) Actualiza velocidad según físicas
        PhysicsManager.Instance.ApplyPhysics(ref velocity, Time.deltaTime);

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
            velocity += slope * PhysicsManager.Instance.gravity * Time.deltaTime;

            Vector3 vH = new Vector3(velocity.x, 0f, velocity.z);
            if (vH.magnitude > 0.01f)
            {
                Vector3 fAcc = -vH.normalized * surfaceFriction * PhysicsManager.Instance.gravity;
                vH += fAcc * Time.deltaTime;
                if (Vector3.Dot(vH, fAcc) > 0f)
                    vH = Vector3.zero;
            }
            velocity.x = vH.x;
            velocity.z = vH.z;

            // Restitución (choque)
            float e = hitC.restitution;
            Vector3 vNorm = Vector3.Project(velocity, normal);
            Vector3 vTan  = velocity - vNorm;
            velocity = vTan - vNorm * e;
        }
        else
        {
            transform.position = nextPos;
        }

        // Handle input drag to apply force to the ball
        HandleInput();
    }

    private void HandleInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            isDragging = true;
            dragStart = GetMouseWorldPosition();
            lineRenderer.enabled = true;
        }
        else if (Input.GetMouseButtonUp(0) && isDragging)
        {
            Vector3 dragEnd = GetMouseWorldPosition();
            Vector3 dragVec = (dragStart - dragEnd);              // vector de fuerza deseada
            float  dragDist = dragVec.magnitude;

            // Impulso J = F·Δt ≈ (distancia * impulseMultiplier)
            // Δv = J / m
            Vector3 deltaV = dragVec.normalized 
                             * (dragDist * forceMultiplier) 
                             / ballMass;

            velocity += deltaV;

            isDragging = false;
            lineRenderer.enabled = false;
        }

        // Line Renderer dibujado durante el drag de centro a mouse pos
        if (isDragging)
        {
            Vector3 mousePos = GetMouseWorldPosition();

            // Punto 0: centro de la bola
            Vector3 p0 = transform.position;

            // Punto 1: posición del ratón en XZ, pero a la misma Y que la bola
            Vector3 p1 = new Vector3(
                mousePos.x,
                transform.position.y,
                mousePos.z
            );

            lineRenderer.SetPosition(0, p0);
            lineRenderer.SetPosition(1, p1);
        }
    }

    private Vector3 GetMouseWorldPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane ground = new Plane(Vector3.up, Vector3.zero);
        if (ground.Raycast(ray, out float enter))
            return ray.GetPoint(enter);
        return Vector3.zero;
    }
}