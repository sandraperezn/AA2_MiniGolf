using Cinemachine;
using UnityEngine;

namespace Camera
{
    public class CameraZoom : MonoBehaviour
    {
        public CinemachineFreeLook freeLookCamera;
        public float zoomSpeed = 5f;
        public float minRadius = 5f;
        public float maxRadius = 20f;

        private void Update()
        {
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (scroll != 0f)
            {
                for (int i = 0; i < 3; i++)
                {
                    float newRadius = freeLookCamera.m_Orbits[i].m_Radius - scroll * zoomSpeed;
                    newRadius = Mathf.Clamp(newRadius, minRadius, maxRadius);
                    freeLookCamera.m_Orbits[i].m_Radius = newRadius;
                }
            }
        }
    }
}