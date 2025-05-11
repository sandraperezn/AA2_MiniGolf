using UnityEngine;

public class PhysicsManager : Singleton<PhysicsManager>
{
    #region Constants

    public const float Gravity = 9.81f;
    public const float AirDensity = 1.225f;
    public const float DragCoefficient = 0.47f;

    #endregion

    [Header("Ball Properties"), SerializeField]
    private float ballMass = 0.1f;

    public float BallRadius { get; set; }
    public Vector3 BallVelocity { get; private set; }

    // Applies forces (gravity, air) modifying velocity
    public void ApplyPhysics(ref Vector3 velocity, float dt)
    {
        // Vertical gravity
        velocity.y -= Gravity * dt;

        // Air resistance (when moving)
        if (velocity.magnitude > 0f)
        {
            float area = Mathf.PI * BallRadius * BallRadius;
            float dragMag = 0.5f * AirDensity * velocity.sqrMagnitude * DragCoefficient * area;
            Vector3 dragAcc = -velocity.normalized * (dragMag / ballMass);
            velocity += dragAcc * dt;
        }

        BallVelocity = velocity;
    }
}