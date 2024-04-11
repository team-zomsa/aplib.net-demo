using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] public Transform PlayerRotation;
    [SerializeField] public Transform PlayerCamFollow;
    [SerializeField] private Cinemachine.CinemachineVirtualCamera _cinemachineCamera;
    public static CameraManager Instance { get; private set; }

    /// <summary>
    /// Make this class a singleton.
    /// </summary>
    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
        DontDestroyOnLoad(gameObject);

        if (_cinemachineCamera == null)
            _cinemachineCamera = FindObjectOfType<Cinemachine.CinemachineVirtualCamera>();

        _cinemachineCamera.Follow = PlayerCamFollow;
    }
}
