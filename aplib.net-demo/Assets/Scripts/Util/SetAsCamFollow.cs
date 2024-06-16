using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetAsCamFollow : MonoBehaviour
{
    private void Awake()
    {
        CinemachineVirtualCamera cinemachineVirtualCamera = FindObjectOfType<CinemachineVirtualCamera>();

        if (cinemachineVirtualCamera) cinemachineVirtualCamera.Follow = transform;
        else Debug.LogWarning("No CinemachineVirtualCamera found in scene.");
    }
}
