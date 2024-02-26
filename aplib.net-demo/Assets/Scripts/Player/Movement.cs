using System.Collections;
using System.Collections.Generic;
using Unity.PlasticSCM.Editor.WebApi;
using UnityEngine;
using UnityEngine.InputSystem;

public class Movement : MonoBehaviour
{
    [SerializeField] float speed = 10;
    Vector3 horizontalVelocity;
    Vector2 horizontalInput;

    [SerializeField] float normalGravityScale = 1.7f;
    [SerializeField] float fallingGravityScale = 2.5f;
    float actualGravityScale;
    float gravity;

    [SerializeField] float jumpHeight = 7;
    Vector3 verticalVelocity;
    bool jumpStarted;
    bool isGrounded;

    [SerializeField] LayerMask groundMask;
    float playerHeight;

    Rigidbody rb;
    CapsuleCollider controller;

    void Start() { 
        rb = GetComponent<Rigidbody>();
        controller = GetComponent<CapsuleCollider>();
        playerHeight = controller.height;
        rb.freezeRotation = true;
        gravity = Physics.gravity.y;
    }

    void FixedUpdate() {
        // Ground check by checking a sphere at the bottom of the player
        Vector3 bottomPoint = transform.TransformPoint(controller.center - 0.5f * playerHeight * Vector3.up);
        isGrounded = Physics.CheckSphere(bottomPoint, 0.1f, groundMask);

        if (isGrounded) 
            Debug.Log("Grounded");  
        else {
            // Custom gravity for player
            actualGravityScale = rb.velocity.y < 0 ? fallingGravityScale : normalGravityScale;
            rb.velocity += (actualGravityScale - 1) * gravity * Time.fixedDeltaTime * Vector3.up;   
        }

        HorizontalMovement();
        VerticalMovement();
    }   
    
    void HorizontalMovement() {
        horizontalVelocity = transform.right * horizontalInput.x + transform.forward * horizontalInput.y;
        // We can choose to do horizontal movement using AddForce or MovePosition
        // rb.AddForce(speed * Time.fixedDeltaTime * horizontalVelocity, ForceMode.VelocityChange);
        rb.MovePosition(rb.position + speed * Time.fixedDeltaTime * horizontalVelocity);
    }

    void VerticalMovement(){
        if (jumpStarted) {
            if (isGrounded) {
                verticalVelocity.y = jumpHeight;
                rb.AddForce(verticalVelocity, ForceMode.Impulse);
            }
            jumpStarted = false;
        }
    }

    public void ReceiveInput(Vector2 _input)
    {
        horizontalInput = _input;
    }

    public void OnJumpPressed() {
        jumpStarted = true;
    }
}
