using UnityEngine;
using Cinemachine;

public class CinemachineInputLock : MonoBehaviour
{
    public CinemachineFreeLook freeLookCamera;
    public string inputAxisX = "Mouse X";
    public string inputAxisY = "Mouse Y";

    void Update()
    {
        if (Input.GetMouseButton(1)) // Click derecho presionado
        {
            freeLookCamera.m_XAxis.m_InputAxisName = inputAxisX;
            freeLookCamera.m_YAxis.m_InputAxisName = inputAxisY;
        }
        else
        {
            // Desactiva el input
            freeLookCamera.m_XAxis.m_InputAxisName = "";
            freeLookCamera.m_YAxis.m_InputAxisName = "";

            // Reinicia los valores de entrada (elimina la inercia acumulada)
            freeLookCamera.m_XAxis.m_InputAxisValue = 0;
            freeLookCamera.m_YAxis.m_InputAxisValue = 0;
        }
    }
}