// This program has been developed by students from the bachelor Computer Science at Utrecht
// University within the Software Project course.
// Copyright Utrecht University (Department of Information and Computing Sciences)

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
