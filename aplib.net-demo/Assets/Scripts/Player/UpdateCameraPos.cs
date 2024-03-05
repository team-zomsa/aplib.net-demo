using UnityEngine;

public class UpdateCameraPos : MonoBehaviour
{
    [SerializeField] private Transform _cameraTransform;

    private void Update()
    {
        _cameraTransform.position = transform.position;
    }
}
