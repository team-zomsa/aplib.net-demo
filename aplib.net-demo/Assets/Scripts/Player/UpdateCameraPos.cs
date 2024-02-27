using UnityEngine;

public class UpdateCameraPos : MonoBehaviour
{
    [SerializeField] Transform cameraTransform;

    void Update()
    {
        cameraTransform.position = transform.position;
    }
}
