using UnityEngine;

public class PhysicsManager : Singleton<PhysicsManager>
{
    [Header("Physical Properties")]
    public float gravity = 9.81f;
    public float airDensity = 1.225f;
    public float dragCoefficient = 0.47f;
    
    [Header("Ball Properties")]
    public float ballRadius = 0.5f;
    public float ballMass = 0.045f;

    // Aplica sólo las fuerzas (gravedad, aire, etc.) modificando la velocidad.
    // No toca ningún transform.
    public void ApplyPhysics(ref Vector3 velocity)
    {
        // Gravedad vertical
        velocity.y -= gravity * Time.deltaTime;

        // Resistencia del aire (opcional) en todo el espacio
        if (velocity.magnitude > 0f)
        {
            float area = Mathf.PI * ballRadius * ballRadius;
            float dragMag = 0.5f * airDensity * velocity.sqrMagnitude * dragCoefficient * area;
            Vector3 dragAcc = -velocity.normalized * (dragMag / ballMass);
            velocity += dragAcc * Time.deltaTime;
        }
    }
}