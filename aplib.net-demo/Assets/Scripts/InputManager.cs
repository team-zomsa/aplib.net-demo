using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    [SerializeField] private MouseLock _mouseLock;

    [SerializeField] private Transform _playerTransform;

    // TODO: Change when inventory is added
    // Doing it this way for now, change when inventory is implemented.
    private Weapon _activeWeapon;

    private Vector2 _horizontalInput;

    private PlayerInput _input;
    private PlayerInput.PlayerActions _playerActions;
    private Movement _playerMovement;
    private ResetRigidbody _playerRespawn;
    private PlayerInput.UIActions _uiActions;

    public static InputManager Instance { get; private set; }

    /// <summary>
    /// Make this class a singleton and subscribe to the player's input events.
    /// </summary>
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }

        //DontDestroyOnLoad(gameObject);

        _input = new PlayerInput();
        _playerActions = _input.Player;
        _uiActions = _input.UI;
        _playerMovement = _playerTransform.GetComponent<Movement>();
        _playerRespawn = _playerTransform.GetComponent<ResetRigidbody>();
        List<Weapon> _playerWeapons = new(collection: _playerTransform.GetComponentsInChildren<Weapon>());
        _activeWeapon = _playerWeapons[0];

        _playerActions.Move.performed += inputContext => _horizontalInput = inputContext.ReadValue<Vector2>();
        _playerActions.Jump.performed += _ => _playerMovement.OnJumpDown();
        _playerActions.Jump.canceled += _ => _playerMovement.OnJumpUp();
        _playerActions.Respawn.performed += _ => _playerRespawn.ResetObject();
        _playerActions.Fire.performed += _ => _activeWeapon.UseWeapon();
        _uiActions.ShowMouse.performed += _ => _mouseLock.OnShowMousePressed();
        _uiActions.Click.performed += _ => _mouseLock.OnLeftMousePressed();
    }

    /// <summary>
    /// Pass the input to the movement script.
    /// </summary>
    private void Update() => _playerMovement.ReceiveHorizontalInput(_horizontalInput);

    private void OnEnable() => _input?.Enable();

    private void OnDisable() => _input?.Disable();

    /// <summary>
    /// Get the change in mouse position since the last frame.
    /// </summary>
    public Vector2 GetMouseDelta() => _playerActions.Look.ReadValue<Vector2>();
}
