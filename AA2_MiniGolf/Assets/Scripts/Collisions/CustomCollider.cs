using System;
using UnityEngine;

namespace Collisions
{
    public abstract class CustomCollider : MonoBehaviour
    {
        [Range(0f, 1f)] public float restitution = 0.8f;
        [SerializeField, Range(0f, 1f)] private float surfaceFriction = 0.4f;
        [SerializeField] private bool isTrigger;
    
        public bool IsTrigger => isTrigger;
        public float SurfaceFriction => surfaceFriction;
        public Action OnTriggerEnterEvent;
    
        // Returns true if there is collision and gives the normal and penetration
        public abstract bool DetectCollision(
            Vector3 sphereCenter,
            float sphereRadius,
            out Vector3 collisionNormal,
            out float penetration);

        // Internal method for drawing collider-specific gizmos
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
}