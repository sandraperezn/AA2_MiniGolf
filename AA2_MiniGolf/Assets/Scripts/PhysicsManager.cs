using UnityEngine;

public class PhysicsManager : MonoBehaviour
{
    public static PhysicsManager Instance { get; private set; }

    [Header("Physical Properties")]
    public float gravity = 9.81f;
    public float airDensity = 1.225f;
    public float dragCoefficient = 0.47f;
    public float ballRadius = 0.5f;
    public float ballMass = 0.045f;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
    
    //Aplica sólo las fuerzas (gravedad, aire, etc.) modificando la velocidad.
    // No toca ningún transform.
    public void ApplyPhysics(ref Vector3 velocity, float deltaTime)
    {
        // Gravedad vertical
        velocity.y -= gravity * deltaTime;

        // Resistencia del aire (opcional) en todo el espacio
        if (velocity.magnitude > 0f)
        {
            float area = Mathf.PI * ballRadius * ballRadius;
            float dragMag = 0.5f * airDensity * velocity.sqrMagnitude * dragCoefficient * area;
            Vector3 dragAcc = -velocity.normalized * (dragMag / ballMass);
            velocity += dragAcc * deltaTime;
        }
    }

    // Prevents instantiation via new
    private PhysicsManager() {}
}