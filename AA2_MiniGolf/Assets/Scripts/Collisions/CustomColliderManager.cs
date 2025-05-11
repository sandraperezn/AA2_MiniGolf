using System.Collections.Generic;
using UnityEngine;

public class CustomCollisionManager : Singleton<CustomCollisionManager>
{
    private readonly List<CustomCollider> colliders = new();
    public void Register(CustomCollider c) => colliders.Add(c);
    public void Unregister(CustomCollider c) => colliders.Remove(c);

    // Check if a sphere (center, radius) collides with a registered CustomCollider
    // Returns the first collision, its normal and penetration
    public bool CheckCollision(
        Vector3 center, float radius,
        out CustomCollider hitCollider,
        out Vector3 normal,
        out float penetration,
        out float surfaceFriction)
    {
        foreach (CustomCollider c in colliders)
        {
            if (c.DetectCollision(center, radius, out normal, out penetration))
            {
                hitCollider = c;
                surfaceFriction = c.SurfaceFriction;

                // If is a trigger, detect but don't collide
                if (c.IsTrigger)
                {
                    c.OnTriggerEnterEvent?.Invoke();
                    penetration = 0;
                    normal = Vector3.zero;
                    return false;
                }

                return true;
            }
        }

        hitCollider = null;
        normal = Vector3.zero;
        surfaceFriction = 0;
        penetration = 0;
        return false;
    }
}