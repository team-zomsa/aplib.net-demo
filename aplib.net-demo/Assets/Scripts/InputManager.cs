using Entities;
using Entities.Weapons;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class InputManager : MonoBehaviour
{
    [SerializeField]
    private MouseLock _mouseLock;

    [SerializeField]
    private Transform _playerTransform;

    [SerializeField]
    private Inventory _inventory;

    [SerializeField]
    public EquipmentInventory equipmentInventory;

    private RespawnableComponent _playerRespawn;

    private CanvasManager _canvasManager;
    private Vector2 _horizontalInput;
    private PlayerInput _input;
    private PlayerInput.PlayerActions _playerActions;
    private Movement _playerMovement;
    private PlayerInput.UIActions _uiActions;
    private Animator _playerAnimator;

    public static InputManager Instance { get; private set; }

    /// <summary>
    /// Make this class a singleton and subscribe to the player's input events. To change controls, check
    /// Assets/Input/PlayerInput.inputactions.
    /// </summary>
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            Instance = this;
        }

        _input = new PlayerInput();
        _playerActions = _input.Player;
        _uiActions = _input.UI;
        _playerMovement = _playerTransform.GetComponent<Movement>();
        _playerRespawn = _playerTransform.GetComponent<RespawnableComponent>();
        _playerAnimator = _playerTransform.gameObject.GetComponent<Animator>();
    }

    private void Start()
    {
        _playerActions.Move.performed += inputContext => _horizontalInput = inputContext.ReadValue<Vector2>();
        _playerActions.Jump.performed += _ => _playerMovement.OnJumpDown();
        _playerActions.Jump.canceled += _ => _playerMovement.OnJumpUp();
        _playerActions.Respawn.performed += _ => _playerRespawn.Respawn();
        _playerActions.UseItem.performed += _ => _inventory.ActivateItem();
        _playerActions.NextItem.performed += _ =>
        {
            equipmentInventory.MoveNext();

            _playerAnimator.SetTrigger("SwitchWeapon");
        };
        _playerActions.PreviousItem.performed += _ => equipmentInventory.MovePrevious();
        _playerActions.SwitchItem.performed += _ => _inventory.SwitchItem();
        _playerActions.Fire.performed += _ =>
        {
            if (equipmentInventory.HasItems)
            {
                equipmentInventory.CurrentEquipment.UseEquipment();

                _playerAnimator.SetTrigger("PlayerAttack");
            }
        };
        _uiActions.ShowMouse.performed += _ => _mouseLock.OnShowMousePressed();
        _uiActions.Click.performed += _ => _mouseLock.OnLeftMousePressed();
        if (CanvasManager.Instance)
        {
            _uiActions.OpenSettings.performed += _ => CanvasManager.Instance.OnToggleSettings();
            _uiActions.OpenHelp.performed += _ => CanvasManager.Instance.OnToggleHelp();
        }
    }

    /// <summary>
    /// Pass the input to the movement script.
    /// </summary>
    private void Update() => _playerMovement.ReceiveHorizontalInput(_horizontalInput);

    /// <summary>
    /// Enable all input.
    /// </summary>
    private void OnEnable() => _input?.Enable();

    /// <summary>
    /// Disable all input.
    /// </summary>
    private void OnDisable() => _input?.Disable();

    /// <summary>
    /// Get the change in mouse position since the last frame.
    /// </summary>
    public Vector2 GetMouseDelta() => _playerActions.Look.ReadValue<Vector2>();

    /// <summary>
    /// Disable player input.
    /// </summary>
    public void DisablePlayerInput() => _input.Player.Disable();

    /// <summary>
    /// Enable player input.
    /// </summary>
    public void EnablePlayerInput() => _input.Player.Enable();
}
