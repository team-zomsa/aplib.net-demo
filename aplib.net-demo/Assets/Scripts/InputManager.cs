using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    [SerializeField] Movement movement;
    [SerializeField] MouseLook mouseLook;

    PlayerInput input;
    PlayerInput.PlayerActions playerActions;
    PlayerInput.UIActions uiActions;

    Vector2 horizontalInput;
    Vector2 mouseInput;

    private void Awake()
    {
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

    private void Update()
    {
        movement.ReceiveInput(horizontalInput);
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
