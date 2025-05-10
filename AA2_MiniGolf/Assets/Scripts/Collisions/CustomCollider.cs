using System;
using UnityEngine;

public abstract class CustomCollider : MonoBehaviour
{
    [Range(0f, 1f)] public float restitution = 0.8f; // coeficiente de restitución por collider
    [SerializeField, Range(0f, 1f)] private float surfaceFriction = 0.4f;
    [SerializeField] private bool isTrigger; // Si es un trigger, no colisiona, pero sí detecta
    
    public bool IsTrigger => isTrigger;
    public float SurfaceFriction => surfaceFriction;

    public Action OnTriggerEnterEvent;
    
    // Devuelve true si hay colisión, y te da la normal y penetración
    public abstract bool DetectCollision(
        Vector3 sphereCenter,
        float sphereRadius,
        out Vector3 collisionNormal,
        out float penetration);

    // Método interno para dibujar gizmos específicos de cada collider
    protected abstract void OnDrawGizmosInternal();

    private void OnEnable() => CustomCollisionManager.Instance.Register(this);

    private void OnDisable()
    {
        if (!CustomCollisionManager.Instance) return;
        CustomCollisionManager.Instance.Unregister(this);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        OnDrawGizmosInternal();
    }
}