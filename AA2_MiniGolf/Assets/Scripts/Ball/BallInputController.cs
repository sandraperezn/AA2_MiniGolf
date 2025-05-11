using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class BallInputController : MonoBehaviour
{
    [Header("Force & Drag")] [Tooltip("Multiplica la distancia de drag para obtener la velocidad inicial.")]
    public float forceMultiplier = 0.025f;

    [Tooltip("Distancia máxima en píxeles que cuenta para el drag.")]
    public float maxDragDistance = 350f;

    [Header("Pitch & Yaw")] [Tooltip("Ángulo máximo de elevación en grados.")]
    public float maxPitchAngle = 15f;

    [Header("Trajectory")] [Tooltip("Número de puntos para dibujar la parábola.")]
    public int trajectoryPoints = 30;

    [Tooltip("Separación temporal entre puntos (en segundos).")]
    public float timeStep = 0.1f;

    private LineRenderer lineRenderer;
    private BallController ballController;
    private Vector2 dragStartScreen;
    private bool isDragging;
    private Camera mainCamera;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = trajectoryPoints;
        lineRenderer.useWorldSpace = true;
        lineRenderer.enabled = false;

        ballController = GetComponent<BallController>();
        mainCamera = Camera.main;
    }

    private void Update()
    {
        // Al empezar el drag
        if (Input.GetMouseButtonDown(0))
        {
            isDragging = true;
            dragStartScreen = Input.mousePosition;
            lineRenderer.enabled = true;
        }
        // Al soltar: lanzamos la pelota
        else if (Input.GetMouseButtonUp(0) && isDragging)
        {
            isDragging = false;
            lineRenderer.enabled = false;

            // Calculamos fuerza y dirección
            Vector2 dragEnd = Input.mousePosition;
            Vector2 dragVec = dragStartScreen - dragEnd;
            float dragDist = Mathf.Min(dragVec.magnitude, maxDragDistance);
            float dragNorm = dragDist / maxDragDistance;

            float pitchDeg = maxPitchAngle * dragNorm;
            float yawDeg = Mathf.Atan2(dragVec.x, dragVec.y) * Mathf.Rad2Deg;

            // Base: proyección de la vista de la cámara en XZ
            Vector3 baseDir = Vector3.ProjectOnPlane(mainCamera.transform.forward, Vector3.up).normalized;
            Quaternion yawRot = Quaternion.AngleAxis(yawDeg, Vector3.up);
            Vector3 dirH = yawRot * baseDir;

            // Aplicamos pitch girando sobre el eje “right”
            Vector3 rightAxis = Vector3.Cross(Vector3.up, dirH).normalized;
            Vector3 launchDir = Quaternion.AngleAxis(-pitchDeg, rightAxis) * dirH;
            launchDir.Normalize();

            float speed = dragDist * forceMultiplier;

            // Lanzamos la bola en la dirección calculada
            ballController.Launch(launchDir * speed);
        }

        if (isDragging)
            DrawTrajectory();
    }

    private void DrawTrajectory()
    {
        // Igual que antes, pero usando tu propia gravedad
        Vector2 mousePos = Input.mousePosition;
        Vector2 dragVec = dragStartScreen - mousePos;
        float dragDist = Mathf.Min(dragVec.magnitude, maxDragDistance);
        float dragNorm = dragDist / maxDragDistance;

        float pitchDeg = maxPitchAngle * dragNorm;
        float yawDeg = Mathf.Atan2(dragVec.x, dragVec.y) * Mathf.Rad2Deg;

        Vector3 baseDir = Vector3.ProjectOnPlane(mainCamera.transform.forward, Vector3.up).normalized;
        Quaternion yawRot = Quaternion.AngleAxis(yawDeg, Vector3.up);
        Vector3 dirH = yawRot * baseDir;
        Vector3 rightAxis = Vector3.Cross(Vector3.up, dirH).normalized;
        Vector3 launchDir = Quaternion.AngleAxis(-pitchDeg, rightAxis) * dirH;
        launchDir.Normalize();

        float speed = dragDist * forceMultiplier;
        Vector3 initVel = launchDir * speed;

        Vector3 startPos = transform.position;
        float g = PhysicsManager.Gravity;

        for (int i = 0; i < trajectoryPoints; i++)
        {
            float t = i * timeStep;
            // s = p0 + v0*t + ½·(–g·up)·t²
            Vector3 point = startPos
                            + initVel * t
                            - Vector3.up * (0.5f * g * t * t);
            lineRenderer.SetPosition(i, point);
        }
    }
}