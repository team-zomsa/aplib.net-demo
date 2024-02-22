using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Movement : MonoBehaviour
{
    [SerializeField] float speed = 5;
    Vector3 horizontalVelocity;
    Vector2 horizontalInput;

    [SerializeField] float gravity = -30;
    [SerializeField] float fallingGravity = -45;
    float acutalGravity;

    [SerializeField] float jumpHeight = 20;
    Vector3 verticalVelocity;
    bool jumpStarted;

    [SerializeField] LayerMask groundMask;
    float playerHeight;
    bool isGrounded;

    Rigidbody rb;
    CapsuleCollider controller;

    void Start() { 
        rb = GetComponent<Rigidbody>();
        controller = GetComponent<CapsuleCollider>();
        playerHeight = controller.height;
        rb.freezeRotation = true;
    }

    void FixedUpdate() {
        
        // Ground check by checking a sphere at the bottom of the player
        Vector3 bottomPoint = transform.TransformPoint(controller.center - 0.5f * playerHeight * Vector3.up);
        isGrounded = Physics.CheckSphere(bottomPoint, 0.1f, groundMask);

        if (isGrounded) {
            Debug.Log("Grounded");
            horizontalVelocity = Vector3.zero;
            verticalVelocity.y = 0;
        }
        else {
            // Higher gravity when falling
            acutalGravity = verticalVelocity.y < 0 ? fallingGravity : gravity;
            verticalVelocity.y += acutalGravity * Time.deltaTime;  
        }

        HorizontalMovement();
        VerticalMovement();
    }   
    
    void HorizontalMovement() {
        horizontalVelocity = transform.right * horizontalInput.x + transform.forward * horizontalInput.y;
        rb.MovePosition(rb.position + speed * Time.fixedDeltaTime * horizontalVelocity);
    }

    void VerticalMovement(){
        if (jumpStarted) {
            if (isGrounded) {
                Debug.Log("Jumping"); 
                verticalVelocity.y = Mathf.Sqrt(jumpHeight * 2f);
            }
            jumpStarted = false;
        }
        rb.MovePosition(rb.position + verticalVelocity * Time.fixedDeltaTime);
    }

    public void ReceiveInput(Vector2 _input)
    {
        horizontalInput = _input;
        // Debug.Log(horizontalInput);
    }

    public void OnJumpPressed() {
        jumpStarted = true;
        // Debug.Log("Jump started");
    }
}
