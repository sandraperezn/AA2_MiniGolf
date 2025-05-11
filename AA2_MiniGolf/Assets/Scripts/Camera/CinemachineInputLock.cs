using Cinemachine;
using UnityEngine;

namespace Camera
{
    public class CinemachineInputLock : MonoBehaviour
    {
        public CinemachineFreeLook freeLookCamera;
        public string inputAxisX = "Mouse X";
        public string inputAxisY = "Mouse Y";

        private void Update()
        {
            if (Input.GetMouseButton(1)) // Click derecho presionado
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
                freeLookCamera.m_XAxis.m_InputAxisName = inputAxisX;
                freeLookCamera.m_YAxis.m_InputAxisName = inputAxisY;
            }
            else
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                // Desactiva el input
                freeLookCamera.m_XAxis.m_InputAxisName = "";
                freeLookCamera.m_YAxis.m_InputAxisName = "";

                // Reinicia los valores de entrada (elimina la inercia acumulada)
                freeLookCamera.m_XAxis.m_InputAxisValue = 0;
                freeLookCamera.m_YAxis.m_InputAxisValue = 0;
            }
        }
    }
}