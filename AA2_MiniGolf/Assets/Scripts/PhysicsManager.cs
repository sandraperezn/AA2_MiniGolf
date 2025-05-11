using UnityEngine;
using Utils;

public class PhysicsManager : Singleton<PhysicsManager>
{
    #region Constants

    public const float Gravity = 9.81f;
    public const float AirDensity = 1.225f;
    public const float DragCoefficient = 0.47f;

    #endregion

    [Header("Ball Properties")] public float ballMass = 0.1f;

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
            float dragMagnitude = 0.5f * AirDensity * velocity.sqrMagnitude * DragCoefficient * area;
            Vector3 dragAcceleration = -velocity.normalized * (dragMagnitude / ballMass);
            velocity += dragAcceleration * dt;
        }

        BallVelocity = velocity;
    }
}