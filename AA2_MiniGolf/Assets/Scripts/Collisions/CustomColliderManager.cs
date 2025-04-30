using System.Collections.Generic;
using UnityEngine;

public class CustomCollisionManager : Singleton<CustomCollisionManager>
{
    public void Register(CustomCollider c) => colliders.Add(c);
    public void Unregister(CustomCollider c) => colliders.Remove(c);

    private List<CustomCollider> colliders = new();

    //Comprueba si una esfera (center, radius) choca con alguno de tus CustomCollider.
    // Devuelve el primero que choque, su normal y penetración.
    public bool CheckCollision(
        Vector3 center, float radius,
        out CustomCollider hitCollider,
        out Vector3 normal,
        out float penetration)
    {
        foreach (CustomCollider c in colliders)
        {
            if (c.DetectCollision(center, radius, out normal, out penetration))
            {
                hitCollider = c;
                return true;
            }
        }
        hitCollider = null;
        normal = Vector3.zero;
        penetration = 0;
        return false;
    }
}