using Cinemachine;
using UnityEngine;

public class CinemachinePovExtension : CinemachineExtension
{
    [SerializeField]
    private float _horizontalSpeed = 10f;
    [SerializeField]
    private float _verticalSpeed = 10f;
    [SerializeField]
    private float _clampAngle = 80f;

    private InputManager _inputManager;
    private CameraManager _cameraManager;
    private Vector3 _startingRotation;

    /// <summary>
    /// Initialise the input manager, camera manager, and starting rotation.
    /// </summary>
    protected override void Awake()
    {
        _inputManager = InputManager.Instance;
        _cameraManager = CameraManager.Instance;
        _startingRotation = transform.localRotation.eulerAngles;
        base.Awake();
    }

    /// <summary>
    /// Override void to use Cinemachine with Unity's new input system.
    /// </summary>
    protected override void PostPipelineStageCallback(CinemachineVirtualCameraBase vcam, CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
    {
        if (vcam.Follow)
        {
            if (stage == CinemachineCore.Stage.Aim)
            {
                // Make sure to not perform this piece of code when the input manager is null,
                // as that means the game is not running
                if (_inputManager == null)
                    return;

                Vector2 deltaInput = Cursor.lockState == CursorLockMode.Locked ? _inputManager.GetMouseDelta() : Vector2.zero;
                _startingRotation.x += deltaInput.x * _horizontalSpeed * Time.deltaTime;
                _startingRotation.y += deltaInput.y * _verticalSpeed * Time.deltaTime;
                _startingRotation.y = Mathf.Clamp(_startingRotation.y, -_clampAngle, _clampAngle);
                _cameraManager.PlayerTransform.localRotation = Quaternion.Euler(0f, _startingRotation.x, 0f);
                state.RawOrientation = Quaternion.Euler(-_startingRotation.y, _startingRotation.x, 0f);
            }
        }
    }
}
