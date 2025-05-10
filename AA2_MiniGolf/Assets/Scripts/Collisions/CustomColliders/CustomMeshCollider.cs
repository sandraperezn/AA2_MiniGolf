using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class CustomMeshCollider : CustomCollider
{
    private Mesh mesh;
    private Vector3[] vertices;
    private int[] triangles;

    private void Awake()
    {
        mesh = GetComponent<MeshFilter>().sharedMesh;
        vertices = mesh.vertices;
        triangles = mesh.triangles;
    }

    public override bool DetectCollision(
        Vector3 sphereCenter,
        float sphereRadius,
        out Vector3 collisionNormal,
        out float penetration)
    {
        collisionNormal = Vector3.zero;
        penetration = 0f;

        // Recorre los triángulos de la malla
        for (int i = 0; i < triangles.Length; i += 3)
        {
            // Extraer los tres vértices de cada triángulo
            Vector3 v0 = transform.TransformPoint(vertices[triangles[i]]);
            Vector3 v1 = transform.TransformPoint(vertices[triangles[i + 1]]);
            Vector3 v2 = transform.TransformPoint(vertices[triangles[i + 2]]);

            // Verificamos si la esfera colisiona con este triángulo
            if (CheckTriangleCollision(sphereCenter, sphereRadius, v0, v1, v2, out collisionNormal, out penetration))
            {
                return true;
            }
        }

        return false;
    }

    private bool CheckTriangleCollision(
        Vector3 sphereCenter,
        float sphereRadius,
        Vector3 v0,
        Vector3 v1,
        Vector3 v2,
        out Vector3 collisionNormal,
        out float penetration)
    {
        collisionNormal = Vector3.zero;
        penetration = 0f;

        // Calculamos la normal del triángulo usando el producto cruzado
        Vector3 edge1 = v1 - v0;
        Vector3 edge2 = v2 - v0;
        collisionNormal = Vector3.Cross(edge1, edge2).normalized;

        // Proyectamos el centro de la esfera sobre el plano del triángulo
        float distance = Vector3.Dot(sphereCenter - v0, collisionNormal);

        // Si la distancia es mayor que el radio, no hay colisión
        if (Mathf.Abs(distance) > sphereRadius)
            return false;

        // Verificamos si el centro de la esfera está dentro del triángulo proyectado
        if (IsPointInTriangle(sphereCenter, v0, v1, v2))
        {
            penetration = sphereRadius - Mathf.Abs(distance);
            return true;
        }

        return false;
    }

    private bool IsPointInTriangle(Vector3 point, Vector3 v0, Vector3 v1, Vector3 v2)
    {
        // Calculamos los vectores de los bordes
        Vector3 edge1 = v1 - v0;
        Vector3 edge2 = v2 - v0;
        Vector3 edge3 = v0 - v1;

        // Vectores de los puntos con respecto a los vértices
        Vector3 p0 = point - v0;
        Vector3 p1 = point - v1;
        Vector3 p2 = point - v2;

        // Producto cruzado entre los vectores del triángulo y los del punto
        float areaOrig = Vector3.Cross(edge1, edge2).magnitude;
        float area1 = Vector3.Cross(edge1, p0).magnitude;
        float area2 = Vector3.Cross(edge2, p1).magnitude;
        float area3 = Vector3.Cross(edge3, p2).magnitude;

        // Verificamos si el área total coincide con las áreas parciales (lo que indica si está dentro)
        return Mathf.Approximately(areaOrig, area1 + area2 + area3);
    }

    protected override void OnDrawGizmosInternal()
    {
        // Dibujar la malla usando Gizmos
        Gizmos.color = Color.red;

        for (int i = 0; i < triangles.Length; i += 3)
        {
            // Vértices del triángulo
            Vector3 v0 = transform.TransformPoint(vertices[triangles[i]]);
            Vector3 v1 = transform.TransformPoint(vertices[triangles[i + 1]]);
            Vector3 v2 = transform.TransformPoint(vertices[triangles[i + 2]]);

            // Dibujar el triángulo con Gizmos
            Gizmos.DrawLine(v0, v1);
            Gizmos.DrawLine(v1, v2);
            Gizmos.DrawLine(v2, v0);
        }
    }
}