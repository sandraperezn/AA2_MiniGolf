using UnityEngine;

namespace Ball
{
    [RequireComponent(typeof(LineRenderer))]
    public class BallInputController : MonoBehaviour
    {
        [SerializeField] private float forceMultiplier = 0.025f;

        [Tooltip("Max distance for dragging (pixels)"), SerializeField]
        private float maxDragDistance = 350f;

        [Tooltip("Max pitch of the trajectory (degrees)"), SerializeField]
        private float maxPitchAngle = 15f;

        private const int TrajectoryPoints = 30;
        private LineRenderer lineRenderer;
        private BallController ballController;
        private Vector2 dragStartScreen;
        private bool isDragging;
        private UnityEngine.Camera mainCamera;

        private void Awake()
        {
            lineRenderer = GetComponent<LineRenderer>();
            lineRenderer.positionCount = TrajectoryPoints;
            lineRenderer.useWorldSpace = true;
            lineRenderer.enabled = false;

            ballController = GetComponent<BallController>();
            mainCamera = UnityEngine.Camera.main;
        }

        private void Update()
        {
            // Drag begins
            if (Input.GetMouseButtonDown(0))
            {
                isDragging = true;
                dragStartScreen = Input.mousePosition;
                lineRenderer.enabled = true;
            }

            //  Drag released
            else if (Input.GetMouseButtonUp(0) && isDragging)
            {
                isDragging = false;
                lineRenderer.enabled = false;

                // Calculate force and direction
                Vector2 dragEnd = Input.mousePosition;
                Vector2 dragVector = dragStartScreen - dragEnd;
                float dragDistance = Mathf.Min(dragVector.magnitude, maxDragDistance);
                float dragNormal = dragDistance / maxDragDistance;

                float pitchAngle = maxPitchAngle * dragNormal;
                float yawAngle = Mathf.Atan2(dragVector.x, dragVector.y) * Mathf.Rad2Deg;

                // Base direction: project camera view on XZ plane
                Vector3 baseDirection = Vector3.ProjectOnPlane(mainCamera.transform.forward, Vector3.up).normalized;
                Quaternion yawRotation = Quaternion.AngleAxis(yawAngle, Vector3.up);
                Vector3 dirH = yawRotation * baseDirection;

                // Apply pitch by rotating around the “right” axis
                Vector3 rightAxis = Vector3.Cross(Vector3.up, dirH).normalized;
                Vector3 launchDirection = Quaternion.AngleAxis(-pitchAngle, rightAxis) * dirH;
                launchDirection.Normalize();

                float speed = dragDistance * forceMultiplier;

                // Launch the ball in the calculated direction
                ballController.Launch(launchDirection * speed);
            }

            if (isDragging)
            {
                DrawTrajectory();
            }
        }

        private void DrawTrajectory()
        {
            Vector2 mousePos = Input.mousePosition;
            Vector2 dragVec = dragStartScreen - mousePos;
            float dragDistance = Mathf.Min(dragVec.magnitude, maxDragDistance);
            float dragNormal = dragDistance / maxDragDistance;

            float pitchAngle = maxPitchAngle * dragNormal;
            float yawAngle = Mathf.Atan2(dragVec.x, dragVec.y) * Mathf.Rad2Deg;

            Vector3 baseDirection = Vector3.ProjectOnPlane(mainCamera.transform.forward, Vector3.up).normalized;
            Quaternion yawRotation = Quaternion.AngleAxis(yawAngle, Vector3.up);
            Vector3 dirH = yawRotation * baseDirection;
            Vector3 rightAxis = Vector3.Cross(Vector3.up, dirH).normalized;
            Vector3 launchDirection = Quaternion.AngleAxis(-pitchAngle, rightAxis) * dirH;
            launchDirection.Normalize();

            float speed = dragDistance * forceMultiplier;
            Vector3 initialVelocity = launchDirection * speed;

            Vector3 startPosition = transform.position;

            const float g = PhysicsManager.Gravity;
            const float timeStep = 0.1f;
            for (int i = 0; i < TrajectoryPoints; i++)
            {
                float step = i * timeStep;
                // s = p0 + v0*t + ½·(–g·up)·t²
                Vector3 point = startPosition
                                + initialVelocity * step
                                - Vector3.up * (0.5f * g * step * step);
                lineRenderer.SetPosition(i, point);
            }
        }
    }
}