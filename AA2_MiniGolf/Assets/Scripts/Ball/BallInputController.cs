using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallInputController : MonoBehaviour
{
    [Header("Input Settings")]
    public float forceMultiplier = 5f;
    public float ballMass = 0.045f;

    private LineRenderer lineRenderer;
    private Vector2 dragStartScreen;
    private bool isDragging = false;
    private Vector3 velocity;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
        lineRenderer.useWorldSpace = true;
        lineRenderer.enabled = false;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            isDragging = true;
            dragStartScreen = Input.mousePosition;
            lineRenderer.enabled = true;
        }
        else if (Input.GetMouseButtonUp(0) && isDragging)
        {
            Vector2 dragEndScreen = Input.mousePosition;
            Vector2 dragVecScreen = (dragStartScreen - dragEndScreen);  // vector de fuerza deseada en pantalla
            float dragDist = dragVecScreen.magnitude;

            // Convert screen space direction to world space direction
            // We're just using the x,z components since we're on a horizontal plane
            Vector3 dragVec = new Vector3(
                dragVecScreen.x,
                0,
                dragVecScreen.y
            ).normalized * (dragDist / 100f); // Scale factor for reasonable forces

            // Impulso J = F·Δt ≈ (distancia * impulseMultiplier)
            // Δv = J / m
            Vector3 deltaV = dragVec.normalized
                             * (dragDist * forceMultiplier)
                             / ballMass;

            velocity += deltaV;

            isDragging = false;
            lineRenderer.enabled = false;
        }

        // Line Renderer dibujado durante el drag
        if (isDragging)
        {
            // Punto 0: centro de la bola
            Vector3 p0 = transform.position;

            // Calculate direction in screen space
            Vector2 currentMousePos = Input.mousePosition;
            Vector2 dragDirection = dragStartScreen - currentMousePos;

            // Convert to a scaled world direction
            Vector3 dragDirectionWorld = new Vector3(
                dragDirection.x,
                0,
                dragDirection.y
            ).normalized * (dragDirection.magnitude / 100f);

            // Punto 1: posición basada en la dirección del arrastre
            Vector3 p1 = transform.position + dragDirectionWorld;

            lineRenderer.SetPosition(0, p0);
            lineRenderer.SetPosition(1, p1);
        }
    }
}
