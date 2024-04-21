using Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] public Transform PlayerRotation;
    [SerializeField] public Transform PlayerCamFollow;
    [SerializeField] public CinemachineVirtualCamera CinemachineCamera;
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
    }
}
