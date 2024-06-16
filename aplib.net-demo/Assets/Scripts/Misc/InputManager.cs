using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : Singleton<InputManager>, PlayerInput.IPlayerActions, UiInput.IUIActions
{
    #region Input events
    /// <summary>
    /// Event triggered when input for player movement is registered.
    /// </summary>
    public event Action<Vector2> Moved;

    /// <summary>
    /// Event triggered when input for player looking is registered.
    /// </summary>
    public event Action<Vector2> Looked;

    /// <summary>
    /// Event triggered when the key for player firing is pressed.
    /// </summary>
    public event Action Fired;

    /// <summary>
    /// Event triggered when the key for player jumping is pressed.
    /// </summary>
    public event Action Jumped;

    /// <summary>
    /// Event triggered when the key for stopping player jumping is pressed.
    /// </summary>
    public event Action StoppedJump;

    /// <summary>
    /// Event triggered when the key for player respawning is pressed.
    /// </summary>
    public event Action Respawned;

    /// <summary>
    /// Event triggered when the key for using an item is pressed.
    /// </summary>
    public event Action UsedItem;

    /// <summary>
    /// Event triggered when the key for switching an item is pressed.
    /// </summary>
    public event Action SwitchedItem;

    /// <summary>
    /// Event triggered when the key for selecting the next item is pressed.
    /// </summary>
    public event Action NextItem;

    /// <summary>
    /// Event triggered when the key for selecting the previous item is pressed.
    /// </summary>
    public event Action PreviousItem;

    /// <summary>
    /// Event triggered when the key for player navigation is pressed.
    /// </summary>
    public event Action Navigated;

    /// <summary>
    /// Event triggered when the key for player submission is pressed.
    /// </summary>
    public event Action Submitted;

    /// <summary>
    /// Event triggered when the key for player cancellation is pressed.
    /// </summary>
    public event Action Canceled;

    /// <summary>
    /// Event triggered when the key for player pointing is pressed.
    /// </summary>
    public event Action Pointed;

    /// <summary>
    /// Event triggered when the key for player clicking is pressed.
    /// </summary>
    public event Action Clicked;

    /// <summary>
    /// Event triggered when the key for player scrolling is pressed.
    /// </summary>
    public event Action Scrolled;

    /// <summary>
    /// Event triggered when the key for player middle clicking is pressed.
    /// </summary>
    public event Action MiddleClicked;

    /// <summary>
    /// Event triggered when the key for player right clicking is pressed.
    /// </summary>
    public event Action RightClicked;

    /// <summary>
    /// Event triggered when the key for tracked device position change is pressed.
    /// </summary>
    public event Action TrackedDevicePositionChanged;

    /// <summary>
    /// Event triggered when the key for tracked device orientation change is pressed.
    /// </summary>
    public event Action TrackedDeviceOrientationChanged;

    /// <summary>
    /// Event triggered when the key for showing the mouse is pressed.
    /// </summary>
    public event Action MouseShown;

    /// <summary>
    /// Event triggered when the key for toggling settings is pressed.
    /// </summary>
    public event Action ToggledSettings;

    /// <summary>
    /// Event triggered when the key for toggling help is pressed.
    /// </summary>
    public event Action ToggledHelp;
    #endregion

    /// <summary>
    /// Reference to the player input actions.
    /// </summary>
    public PlayerInput.PlayerActions PlayerActions => _playerInput.Player;

    /// <summary>
    /// Reference to the UI input actions.
    /// </summary>
    public UiInput.UIActions UiActions => _uiInput.UI;

    private PlayerInput _playerInput;
    private UiInput _uiInput;

    #region Propagate events
    /// <summary>
    /// Callback for the move input action.
    /// </summary>
    /// <param name="context">The input action context.</param>
    public void OnMove(InputAction.CallbackContext context)
    {
        if (context.performed) Moved?.Invoke(context.ReadValue<Vector2>());
    }

    /// <summary>
    /// Callback for the look input action.
    /// </summary>
    /// <param name="context">The input action context.</param>
    public void OnLook(InputAction.CallbackContext context)
    {
        if (context.performed) Looked?.Invoke(context.ReadValue<Vector2>());
    }

    /// <summary>
    /// Callback for the fire input action.
    /// </summary>
    /// <param name="context">The input action context.</param>
    public void OnFire(InputAction.CallbackContext context)
    {
        if (context.performed) Fired?.Invoke();
    }

    /// <summary>
    /// Callback for the jump input action.
    /// </summary>
    /// <param name="context">The input action context.</param>
    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed) Jumped?.Invoke();
        else if (context.canceled) StoppedJump?.Invoke();
    }

    /// <summary>
    /// Callback for the respawn input action.
    /// </summary>
    /// <param name="context">The input action context.</param>
    public void OnRespawn(InputAction.CallbackContext context)
    {
        if (context.performed) Respawned?.Invoke();
    }

    /// <summary>
    /// Callback for the use item input action.
    /// </summary>
    /// <param name="context">The input action context.</param>
    public void OnUseItem(InputAction.CallbackContext context)
    {
        if (context.performed) UsedItem?.Invoke();
    }

    /// <summary>
    /// Callback for the switch item input action.
    /// </summary>
    /// <param name="context">The input action context.</param>
    public void OnSwitchItem(InputAction.CallbackContext context)
    {
        if (context.performed) SwitchedItem?.Invoke();
    }

    /// <summary>
    /// Callback for the next item input action.
    /// </summary>
    /// <param name="context">The input action context.</param>
    public void OnNextItem(InputAction.CallbackContext context)
    {
        if (context.performed) NextItem?.Invoke();
    }

    /// <summary>
    /// Callback for the previous item input action.
    /// </summary>
    /// <param name="context">The input action context.</param>
    public void OnPreviousItem(InputAction.CallbackContext context)
    {
        if (context.performed) PreviousItem?.Invoke();
    }

    /// <summary>
    /// Callback for the navigate input action.
    /// </summary>
    /// <param name="context">The input action context.</param>
    public void OnNavigate(InputAction.CallbackContext context)
    {
        if (context.performed) Navigated?.Invoke();
    }

    /// <summary>
    /// Callback for the submit input action.
    /// </summary>
    /// <param name="context">The input action context.</param>
    public void OnSubmit(InputAction.CallbackContext context)
    {
        if (context.performed) Submitted?.Invoke();
    }

    /// <summary>
    /// Callback for the cancel input action.
    /// </summary>
    /// <param name="context">The input action context.</param>
    public void OnCancel(InputAction.CallbackContext context)
    {
        if (context.performed) Canceled?.Invoke();
    }

    /// <summary>
    /// Callback for the point input action.
    /// </summary>
    /// <param name="context">The input action context.</param>
    public void OnPoint(InputAction.CallbackContext context)
    {
        if (context.performed) Pointed?.Invoke();
    }

    /// <summary>
    /// Callback for the click input action.
    /// </summary>
    /// <param name="context">The input action context.</param>
    public void OnClick(InputAction.CallbackContext context)
    {
        if (context.performed) Clicked?.Invoke();
    }

    /// <summary>
    /// Callback for the scroll wheel input action.
    /// </summary>
    /// <param name="context">The input action context.</param>
    public void OnScrollWheel(InputAction.CallbackContext context)
    {
        if (context.performed) Scrolled?.Invoke();
    }

    /// <summary>
    /// Callback for the middle click input action.
    /// </summary>
    /// <param name="context">The input action context.</param>
    public void OnMiddleClick(InputAction.CallbackContext context)
    {
        if (context.performed) MiddleClicked?.Invoke();
    }

    /// <summary>
    /// Callback for the right click input action.
    /// </summary>
    /// <param name="context">The input action context.</param>
    public void OnRightClick(InputAction.CallbackContext context)
    {
        if (context.performed) RightClicked?.Invoke();
    }

    /// <summary>
    /// Callback for the tracked device position input action.
    /// </summary>
    /// <param name="context">The input action context.</param>
    public void OnTrackedDevicePosition(InputAction.CallbackContext context)
    {
        if (context.performed) TrackedDevicePositionChanged?.Invoke();
    }

    /// <summary>
    /// Callback for the tracked device orientation input action.
    /// </summary>
    /// <param name="context">The input action context.</param>
    public void OnTrackedDeviceOrientation(InputAction.CallbackContext context)
    {
        if (context.performed) TrackedDeviceOrientationChanged?.Invoke();
    }

    /// <summary>
    /// Callback for the show mouse input action.
    /// </summary>
    /// <param name="context">The input action context.</param>
    public void OnShowMouse(InputAction.CallbackContext context)
    {
        if (context.performed) MouseShown?.Invoke();
    }

    /// <summary>
    /// Callback for the open settings input action.
    /// </summary>
    /// <param name="context">The input action context.</param>
    public void OnOpenSettings(InputAction.CallbackContext context)
    {
        if (context.performed) ToggledSettings?.Invoke();
    }

    /// <summary>
    /// Callback for the open help input action.
    /// </summary>
    /// <param name="context">The input action context.</param>
    public void OnOpenHelp(InputAction.CallbackContext context)
    {
        if (context.performed) ToggledHelp?.Invoke();
    }
    #endregion

    /// <summary>
    /// Disable player input.
    /// </summary>
    public void DisablePlayerInput() => PlayerActions.Disable();

    /// <summary>
    /// Enable player input.
    /// </summary>
    public void EnablePlayerInput() => PlayerActions.Enable();

    protected override void Awake()
    {
        _playerInput = new PlayerInput();
        _uiInput = new UiInput();

        base.Awake();
    }

    /// <summary>
    /// Enable all input.
    /// </summary>
    private void OnEnable()
    {
        PlayerActions.Enable();
        UiActions.Enable();

        PlayerActions.SetCallbacks(this);
        UiActions.SetCallbacks(this);
    }

    /// <summary>
    /// Disable all input.
    /// </summary>
    private void OnDisable()
    {
        PlayerActions.Disable();
        UiActions.Disable();

        PlayerActions.RemoveCallbacks(this);
        UiActions.RemoveCallbacks(this);
    }
}
