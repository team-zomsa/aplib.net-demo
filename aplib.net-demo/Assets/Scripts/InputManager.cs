using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    [SerializeField] private MouseLock _mouseLock;
    [SerializeField] private Transform _playerTransform;
    [SerializeField] private Inventory _inventory;
    [SerializeField] private WeaponInventory _weaponInventory;
    private ResetRigidbody _playerRespawn;
    private Movement _playerMovement;
    // TODO: Change when inventory is added
    // Doing it this way for now, change when inventory is implemented.

    private PlayerInput _input;
    private PlayerInput.PlayerActions _playerActions;
    private PlayerInput.UIActions _uiActions;

    private Vector2 _horizontalInput;

    public static InputManager Instance { get; private set; }

    /// <summary>
    /// Make this class a singleton and subscribe to the player's input events. To change controls, check Assets/Input/PlayerInput.inputactions.
    /// </summary>
    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
        DontDestroyOnLoad(gameObject);

        _input = new PlayerInput();
        _playerActions = _input.Player;
        _uiActions = _input.UI;
        _playerMovement = _playerTransform.GetComponent<Movement>();
        _playerRespawn = _playerTransform.GetComponent<ResetRigidbody>();
        List<Weapon> playerWeapons = new(_playerTransform.GetComponentsInChildren<Weapon>());

        _playerActions.Move.performed += inputContext => _horizontalInput = inputContext.ReadValue<Vector2>();
        _playerActions.Jump.performed += _ => _playerMovement.OnJumpDown();
        _playerActions.Jump.canceled += _ => _playerMovement.OnJumpUp();
        _playerActions.Respawn.performed += _ => _playerRespawn.ResetObject();
        _playerActions.UseItem.performed += _ => _inventory.ActivateItem();
        _playerActions.SwitchItem.performed += _ => _inventory.SwitchItem();
        _playerActions.SwitchWeapon.performed += _ => _weaponInventory.SwitchItem();
        _playerActions.Fire.performed += _ => _weaponInventory.ActivateItem();
        _uiActions.ShowMouse.performed += _ => _mouseLock.OnShowMousePressed();
        _uiActions.Click.performed += _ => _mouseLock.OnLeftMousePressed();
    }

    /// <summary>
    /// Get the change in mouse position since the last frame.
    /// </summary>
    public Vector2 GetMouseDelta() => _playerActions.Look.ReadValue<Vector2>();

    /// <summary>
    /// Pass the input to the movement script.
    /// </summary>
    private void Update() => _playerMovement.ReceiveHorizontalInput(_horizontalInput);

    private void OnEnable() => _input?.Enable();

    private void OnDisable() => _input?.Disable();
}
