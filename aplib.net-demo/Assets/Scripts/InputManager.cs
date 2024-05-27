using Entities.Weapons;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    [SerializeField] private MouseLock _mouseLock;
    [SerializeField] private Transform _playerTransform;
    [SerializeField] private Inventory _inventory;
    [SerializeField] private EquipmentInventory _equipmentInventory;
    private CanvasManager _canvasManager;
    private ResetRigidbody _playerRespawn;
    private Movement _playerMovement;

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

        _playerActions.Move.performed += inputContext => _horizontalInput = inputContext.ReadValue<Vector2>();
        _playerActions.Jump.performed += _ => _playerMovement.OnJumpDown();
        _playerActions.Jump.canceled += _ => _playerMovement.OnJumpUp();
        _playerActions.Respawn.performed += _ => _playerRespawn.ResetObject();
        _playerActions.UseItem.performed += _ => _inventory.ActivateItem();
        _playerActions.NextItem.performed += _ => _equipmentInventory.MoveNext();
        _playerActions.PreviousItem.performed += _ => _equipmentInventory.MovePrevious();
        _playerActions.SwitchItem.performed += _ => _inventory.SwitchItem();
        if (_equipmentInventory.HasItems)
            _playerActions.Fire.performed += _ => _equipmentInventory.CurrentEquipment.UseEquipment();
        _uiActions.ShowMouse.performed += _ => _mouseLock.OnShowMousePressed();
        _uiActions.Click.performed += _ => _mouseLock.OnLeftMousePressed();
        if (CanvasManager.Instance)
            _uiActions.OpenSettings.performed += _ => CanvasManager.Instance.OnToggleSettings();
    }

    /// <summary>
    /// Get the change in mouse position since the last frame.
    /// </summary>
    public Vector2 GetMouseDelta() => _playerActions.Look.ReadValue<Vector2>();

    /// <summary>
    /// Pass the input to the movement script.
    /// </summary>
    private void Update() => _playerMovement.ReceiveHorizontalInput(_horizontalInput);

    /// <summary>
    /// Disable player input.
    /// </summary>
    public void DisablePlayerInput() => _input.Player.Disable();

    /// <summary>
    /// Enable player input.
    /// </summary>
    public void EnablePlayerInput() => _input.Player.Enable();

    /// <summary>
    /// Enable all input.
    /// </summary>
    private void OnEnable() => _input?.Enable();

    /// <summary>
    /// Disable all input.
    /// </summary>
    private void OnDisable() => _input?.Disable();
}
