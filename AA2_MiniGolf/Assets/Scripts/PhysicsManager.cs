using UnityEngine;

public class PhysicsManager : Singleton<PhysicsManager>
{
    #region  Constants
    
    public const float Gravity = 9.81f;
    public const float AirDensity = 1.225f;
    public const float DragCoefficient = 0.47f;
    
    #endregion

    [Header("Ball Properties")] public float ballMass = 1f;
    public float BallRadius { get; set; }

    // Aplica sólo las fuerzas (gravedad, aire, etc.) modificando la velocidad.
    // No toca ningún transform.
    public void ApplyPhysics(ref Vector3 velocity, float dt)
    {
        // Gravedad vertical
        velocity.y -= Gravity * dt;

        // Resistencia del aire (opcional) en todo el espacio
        if (velocity.magnitude > 0f)
        {
            float area = Mathf.PI * BallRadius * BallRadius;
            float dragMag = 0.5f * AirDensity * velocity.sqrMagnitude * DragCoefficient * area;
            Vector3 dragAcc = -velocity.normalized * (dragMag / ballMass);
            velocity += dragAcc * dt;
        }
    }
}