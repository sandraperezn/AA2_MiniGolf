using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class BallInputController : MonoBehaviour
{
    [Header("Force & Drag")] [Tooltip("Multiplica la distancia de drag para obtener la velocidad inicial.")]
    public float forceMultiplier = 10f;

    [Tooltip("Distancia máxima en píxeles que cuenta para el drag.")]
    public float maxDragDistance = 500f;

    [Header("Pitch Angle")] [Tooltip("Ángulo máximo de elevación en grados.")]
    public float maxPitchAngle = 60f;

    [Header("Trajectory")] [Tooltip("Número de puntos para dibujar la parábola.")]
    public int trajectoryPoints = 30;

    [Tooltip("Separación temporal entre puntos (en segundos).")]
    public float timeStep = 0.1f;

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
        // Finaliza drag
        else if (Input.GetMouseButtonUp(0) && isDragging)
        {
            isDragging = false;
            lineRenderer.enabled = false;

            // Calcula vector y fuerza
            Vector2 dragEnd = Input.mousePosition;
            Vector2 dragVec = dragStartScreen - dragEnd;
            float dragDist = Mathf.Min(dragVec.magnitude, maxDragDistance);
            float dragNorm = dragDist / maxDragDistance;

            // Ángulo de elevación en radianes
            float pitchDeg = maxPitchAngle * dragNorm;
            float pitchRad = Mathf.Deg2Rad * pitchDeg;

            // Yaw según dirección de drag en pantalla
            float yawDeg = Mathf.Atan2(dragVec.x, dragVec.y) * Mathf.Rad2Deg;
            Quaternion yawRot = Quaternion.Euler(0f, yawDeg, 0f);

            // Dirección base (hacia adelante en world-space)
            Vector3 dirForward = yawRot * Vector3.forward;

            // Elevación del vector
            Vector3 launchDir = Quaternion.AngleAxis(-pitchDeg, Vector3.right) * dirForward;
            launchDir.Normalize();

            // Velocidad inicial
            float speed = dragDist * forceMultiplier;
            rb.velocity = launchDir * speed;
        }

        // Mientras arrastras, dibuja la trayectoria
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
        float pitchRad = Mathf.Deg2Rad * pitchDeg;

        float yawDeg = Mathf.Atan2(dragVec.x, dragVec.y) * Mathf.Rad2Deg;
        Quaternion yawRot = Quaternion.Euler(0f, yawDeg, 0f);
        Vector3 dirForward = yawRot * Vector3.forward;
        Vector3 launchDir = Quaternion.AngleAxis(-pitchDeg, Vector3.right) * dirForward;
        launchDir.Normalize();

        float speed = dragDist * forceMultiplier;
        Vector3 initVel = launchDir * speed;

        // Punto de partida
        Vector3 startPos = transform.position;

        // Dibuja puntos
        for (int i = 0; i < trajectoryPoints; i++)
        {
            float t = i * timeStep;
            // s = p0 + v0*t + 0.5*g*t^2
            Vector3 gravity = Physics.gravity;
            Vector3 point = startPos + initVel * t + gravity * (0.5f * t * t);
            lineRenderer.SetPosition(i, point);
        }
    }
}