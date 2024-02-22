using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateCameraPos : MonoBehaviour
{
    [SerializeField] Transform cameraTransform;

    void Update()
    {
        cameraTransform.position = transform.position;
    }
}
