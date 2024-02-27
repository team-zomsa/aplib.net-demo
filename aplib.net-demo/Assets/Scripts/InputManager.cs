using UnityEngine;

public class InputManager : MonoBehaviour
{
    [SerializeField] private Movement _movement;
    [SerializeField] private MouseLook _mouseLook;

    private PlayerInput _input;
    private PlayerInput.PlayerActions _playerActions;
    private PlayerInput.UIActions _uiActions;

    private Vector2 _horizontalInput;
    private Vector2 _mouseInput;

    public static InputManager Instance { get; private set; }

    /// <summary>
    /// Make this class a singleton and subscribe to the player's input events.
    /// </summary>
    private void Awake()
    {
        // Setup singleton
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
        } else {
            Instance = this;
        }
        DontDestroyOnLoad(gameObject);

        _input = new PlayerInput();
        _playerActions = _input.Player;
        _uiActions = _input.UI;

        _playerActions.Move.performed += ctx => _horizontalInput = ctx.ReadValue<Vector2>();
        _playerActions.Jump.performed += _ => _movement.OnJumpPressed();
        _playerActions.LookX.performed += ctx => _mouseInput.x = ctx.ReadValue<float>();  
        _playerActions.LookY.performed += ctx => _mouseInput.y = ctx.ReadValue<float>();
        _uiActions.ShowMouse.performed += _ => _mouseLook.OnShowMousePressed();
        _uiActions.Click.performed += _ => _mouseLook.OnLeftMousePressed();
    }

    /// <summary>
    /// Pass the input to the movement and mouse look scripts.
    /// </summary>
    private void Update()
    {
        _movement.ReceiveHorizontalInput(_horizontalInput);
        _mouseLook.ReveiveMouseInput(_mouseInput);
    }

    private void OnEnable()
    {
        _input.Enable();
    }

    private void OnDisable()
    {
        _input.Disable();
    }
}
