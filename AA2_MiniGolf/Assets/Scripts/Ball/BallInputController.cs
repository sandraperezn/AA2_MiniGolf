using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class BallInputController : MonoBehaviour
{
    [Header("Force & Drag")]
    [Tooltip("Multiplica la distancia de drag para obtener la velocidad inicial.")]
    public float forceMultiplier = 10f;

    [Tooltip("Distancia máxima en píxeles que cuenta para el drag.")]
    public float maxDragDistance = 500f;

    [Header("Pitch Angle")]
    [Tooltip("Ángulo máximo de elevación en grados.")]
    public float maxPitchAngle = 60f;

    [Header("Trajectory")]
    [Tooltip("Número de puntos para dibujar la parábola.")]
    public int trajectoryPoints = 30;

    [Tooltip("Separación temporal entre puntos (en segundos).")]
    public float timeStep = 0.1f;

    [Header("Camera")]
    [Tooltip("Referencia a la cámara desde la que disparas.")]
    public Camera cam;

    private LineRenderer lineRenderer;
    private Vector2 dragStartScreen;
    private bool isDragging;
    private Rigidbody rb;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = trajectoryPoints;
        lineRenderer.useWorldSpace = true;
        lineRenderer.enabled = false;

        rb = GetComponent<Rigidbody>();
        if (!rb)
        {
            rb = gameObject.AddComponent<Rigidbody>();
            rb.mass = 0.045f; // masa bola de golf
            rb.useGravity = true;
        }

        if (cam == null)
            cam = Camera.main;  // referencia por defecto

    }

    private void Update()
    {
        // Inicia drag
        if (Input.GetMouseButtonDown(0))
        {
            isDragging = true;
            dragStartScreen = Input.mousePosition;
            lineRenderer.enabled = true;
        }
        // Finaliza drag y dispara
        else if (Input.GetMouseButtonUp(0) && isDragging)
        {
            isDragging = false;
            lineRenderer.enabled = false;

            // --- cálculo de vector y fuerza (igual que antes) ---
            Vector2 dragEnd = Input.mousePosition;
            Vector2 dragVec = dragStartScreen - dragEnd;
            float dragDist = Mathf.Min(dragVec.magnitude, maxDragDistance);
            float dragNorm = dragDist / maxDragDistance;

            // Pitch
            float pitchDeg = maxPitchAngle * dragNorm;

            // Yaw según drag
            float yawDeg = Mathf.Atan2(dragVec.x, dragVec.y) * Mathf.Rad2Deg;

            // --- NUEVO: base de la dirección = la cámara proyectada al plano XZ ---
            Vector3 camForwardXZ = Vector3
                .ProjectOnPlane(cam.transform.forward, Vector3.up)
                .normalized;

            // rotamos ese forwardXZ con nuestro yaw del drag
            Quaternion yawRot = Quaternion.AngleAxis(yawDeg, Vector3.up);
            Vector3 dirForward = yawRot * camForwardXZ;

            // aplicamos pitch (mismo método que antes)
            // giramos dirForward alrededor de su propio eje “right” horizontal
            Vector3 rightAxis = Vector3.Cross(Vector3.up, dirForward).normalized;
            Vector3 launchDir = Quaternion
                .AngleAxis(-pitchDeg, rightAxis)
                * dirForward;
            launchDir.Normalize();

            // velocidad final
            float speed = dragDist * forceMultiplier;
            rb.velocity = launchDir * speed;
        }

        // Mientras arrastras, dibuja la trayectoria con la misma lógica
        if (isDragging)
            DrawTrajectory();
    }

    private void DrawTrajectory()
    {
        Vector2 mousePos = Input.mousePosition;
        Vector2 dragVec = dragStartScreen - mousePos;
        float dragDist = Mathf.Min(dragVec.magnitude, maxDragDistance);
        float dragNorm = dragDist / maxDragDistance;

        float pitchDeg = maxPitchAngle * dragNorm;
        float yawDeg = Mathf.Atan2(dragVec.x, dragVec.y) * Mathf.Rad2Deg;

        Vector3 camForwardXZ = Vector3
            .ProjectOnPlane(cam.transform.forward, Vector3.up)
            .normalized;

        Quaternion yawRot = Quaternion.AngleAxis(yawDeg, Vector3.up);
        Vector3 dirForward = yawRot * camForwardXZ;

        Vector3 rightAxis = Vector3.Cross(Vector3.up, dirForward).normalized;
        Vector3 launchDir = Quaternion
            .AngleAxis(-pitchDeg, rightAxis)
            * dirForward;
        launchDir.Normalize();

        float speed = dragDist * forceMultiplier;
        Vector3 initVel = launchDir * speed;

        Vector3 startPos = transform.position;
        for (int i = 0; i < trajectoryPoints; i++)
        {
            float t = i * timeStep;
            Vector3 point = startPos
                + initVel * t
                + Physics.gravity * (0.5f * t * t);
            lineRenderer.SetPosition(i, point);
        }
    }
}