using UnityEngine;

public class UpdateCameraPos : MonoBehaviour
{
    [SerializeField] private Transform cameraTransform;

    private void Update()
    {
        cameraTransform.position = transform.position;
    }
}
