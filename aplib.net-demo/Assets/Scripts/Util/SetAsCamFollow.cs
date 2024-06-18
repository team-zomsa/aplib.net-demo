using Cinemachine;
using UnityEngine;

/// <summary>
/// This component sets the object as the follow target of the CinemachineVirtualCamera.
/// </summary>
public class SetAsCamFollow : MonoBehaviour
{
    private void Awake()
    {
        CinemachineVirtualCamera cinemachineVirtualCamera = FindObjectOfType<CinemachineVirtualCamera>();

        if (cinemachineVirtualCamera) cinemachineVirtualCamera.Follow = transform;
        else Debug.LogWarning("No CinemachineVirtualCamera found in scene.");
    }
}
