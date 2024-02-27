using UnityEngine;

public class InputManager : MonoBehaviour
{
    [SerializeField] Movement movement;
    [SerializeField] MouseLook mouseLook;

    PlayerInput input;
    PlayerInput.PlayerActions playerActions;
    PlayerInput.UIActions uiActions;

    Vector2 horizontalInput;
    Vector2 mouseInput;

    public static InputManager Instance { get; private set; }

    /// <summary>
    /// Make this class a singleton and subscribe to the player's input events.
    /// </summary>
    private void Awake()
    {
        // Setup singleton
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
        DontDestroyOnLoad(gameObject);

        input = new PlayerInput();
        playerActions = input.Player;
        uiActions = input.UI;

        playerActions.Move.performed += ctx => horizontalInput = ctx.ReadValue<Vector2>();
        playerActions.Jump.performed += _ => movement.OnJumpPressed();
        playerActions.LookX.performed += ctx => mouseInput.x = ctx.ReadValue<float>();  
        playerActions.LookY.performed += ctx => mouseInput.y = ctx.ReadValue<float>();
        uiActions.ShowMouse.performed += _ => mouseLook.OnShowMousePressed();
        uiActions.Click.performed += _ => mouseLook.OnLeftMousePressed();
    }

    /// <summary>
    /// Pass the input to the movement and mouse look scripts.
    /// </summary>
    private void Update()
    {
        movement.ReceiveHorizontalInput(horizontalInput);
        mouseLook.ReveiveMouseInput(mouseInput);
    }

    private void OnEnable()
    {
        input.Enable();
    }

    private void OnDisable()
    {
        input.Disable();
    }

}
