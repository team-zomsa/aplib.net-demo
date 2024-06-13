using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetAsCamFollow : MonoBehaviour
{
    private void Awake()
    {
        CinemachineVirtualCamera cinemachineVirtualCamera = FindObjectOfType<CinemachineVirtualCamera>();
        cinemachineVirtualCamera.Follow = transform;
    }
}
