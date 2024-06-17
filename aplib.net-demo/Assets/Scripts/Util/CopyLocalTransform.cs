using UnityEngine;


public class CopyLocalTransform : MonoBehaviour
{
    [SerializeField]
    [Tooltip("The target object to copy the transform from if the target name is unspecified.")]
    private Transform _target;

    [SerializeField]
    [Tooltip("The name of the target object to find in the scene.")]
    private string _targetName;

    [Header("Copy Position")]

    [SerializeField]
    private bool _copyPositionX = true;

    [SerializeField]
    private bool _copyPositionY = true;

    [SerializeField]
    private bool _copyPositionZ = true;

    [Header("Copy Rotation")]

    [SerializeField]
    private bool _copyRotationX = true;

    [SerializeField]
    private bool _copyRotationY = true;

    [SerializeField]
    private bool _copyRotationZ = true;

    private void Awake()
    {
        if (_targetName != string.Empty)
            _target = GameObject.Find(_targetName).transform;
    }

    private void Update()
    {
        if (_target == null)
            return;

        float positionX = _copyPositionX ? _target.localPosition.x : transform.localPosition.x;
        float positionY = _copyPositionY ? _target.localPosition.y : transform.localPosition.y;
        float positionZ = _copyPositionZ ? _target.localPosition.z : transform.localPosition.z;

        transform.localPosition = new Vector3(positionX, positionY, positionZ);

        float rotationX = _copyRotationX ? _target.localRotation.eulerAngles.x : transform.localRotation.eulerAngles.x;
        float rotationY = _copyRotationY ? _target.localRotation.eulerAngles.y : transform.localRotation.eulerAngles.y;
        float rotationZ = _copyRotationZ ? _target.localRotation.eulerAngles.z : transform.localRotation.eulerAngles.z;

        transform.localRotation = Quaternion.Euler(rotationX, rotationY, rotationZ);
    }
}
