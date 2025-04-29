using UnityEngine;

public abstract class CustomCollider : MonoBehaviour
{
    [Range(0f,1f)]
    public float restitution = 0.8f;  // coeficiente de restitución por collider

    // Devuelve true si hay colisión, y te da la normal y penetración
    public abstract bool DetectCollision(
        Vector3 sphereCenter,
        float sphereRadius,
        out Vector3 collisionNormal,
        out float penetration);
    
    protected virtual void OnEnable()
    {
        CustomCollisionManager.Instance.Register(this);
    }

    protected virtual void OnDisable()
    {
        CustomCollisionManager.Instance.Unregister(this);
    }
}