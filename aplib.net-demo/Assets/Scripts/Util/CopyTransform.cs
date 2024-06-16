using UnityEngine;


public class CopyTransform : MonoBehaviour
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

        float positionX = _copyPositionX ? _target.position.x : transform.position.x;
        float positionY = _copyPositionY ? _target.position.y : transform.position.y;
        float positionZ = _copyPositionZ ? _target.position.z : transform.position.z;

        transform.position = new Vector3(positionX, positionY, positionZ);

        float rotationX = _copyRotationX ? _target.rotation.eulerAngles.x : transform.rotation.eulerAngles.x;
        float rotationY = _copyRotationY ? _target.rotation.eulerAngles.y : transform.rotation.eulerAngles.y;
        float rotationZ = _copyRotationZ ? _target.rotation.eulerAngles.z : transform.rotation.eulerAngles.z;

        transform.rotation = Quaternion.Euler(rotationX, rotationY, rotationZ);
    }
}
