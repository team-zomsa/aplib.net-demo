using System.Collections;
using System.Collections.Generic;
using Unity.PlasticSCM.Editor.WebApi;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Movement : MonoBehaviour
{
    [SerializeField] float maxSpeed = 7;
    [SerializeField] float acceleration = 100;
    Vector3 horizontalVelocity;
    Vector2 horizontalInput;

    [SerializeField] float normalGravityScale = 1.7f;
    [SerializeField] float fallingGravityScale = 2.5f;
    float actualGravityScale;
    float gravity;

    [SerializeField] float jumpHeight = 7;
    [SerializeField] float airMovementMultiplier = 0.5f;
    Vector3 verticalVelocity;
    bool jumpPressed;

    [SerializeField] LayerMask groundMask;
    [SerializeField] float groundDrag;
    [SerializeField] float slopeAngle = 40;
    float playerHeight;
    bool isGrounded;

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
        // Ground check by checking a sphere at the bottom of the player, more consistent than ray
        Vector3 bottomPoint = transform.TransformPoint(controller.center - 0.5f * playerHeight * Vector3.up);
        isGrounded = Physics.CheckSphere(bottomPoint, 0.15f, groundMask);

        MovePlayer();
        HandleJump();
    }   
    
    void MovePlayer() {
        horizontalVelocity = transform.right * horizontalInput.x + transform.forward * horizontalInput.y;
        bool isOnSlope = OnSlope(out Vector3 directionOnSlope, out RaycastHit downHit);

        if (isGrounded) {
            rb.drag = groundDrag; 
            Debug.Log("Grounded");
            if (isOnSlope) {
                rb.AddForce(maxSpeed * acceleration * Time.fixedDeltaTime * 1.5f * directionOnSlope);
                // Apply gravity but perpendicular to the slope
                rb.velocity += (actualGravityScale - 1) * gravity * Time.fixedDeltaTime * downHit.normal;
            }
            else {
                rb.AddForce(maxSpeed * acceleration * Time.fixedDeltaTime * horizontalVelocity.normalized);
            }   
        }
        else {
            rb.drag = 0;  
            rb.AddForce(maxSpeed * acceleration * Time.fixedDeltaTime * airMovementMultiplier * horizontalVelocity.normalized); 
            // Custom gravity for player
            actualGravityScale = rb.velocity.y < 0 ? fallingGravityScale : normalGravityScale;
            rb.velocity += (actualGravityScale - 1) * gravity * Time.fixedDeltaTime * Vector3.up;
        }

        rb.useGravity = !isOnSlope;
        LimitVelocity(isOnSlope);
    }

    void LimitVelocity(bool isOnSlope){
        // limiting speed on slope
        if (isOnSlope) {   
            if (rb.velocity.magnitude > maxSpeed)
                rb.velocity = rb.velocity.normalized * maxSpeed;
        }
        else {
            Vector3 rbHorizontalVelocity = new(rb.velocity.x, 0, rb.velocity.z);
            if (rbHorizontalVelocity.magnitude > maxSpeed) {
                Vector3 limitedVelocity = rbHorizontalVelocity.normalized * maxSpeed;
                rb.velocity = new Vector3(limitedVelocity.x, rb.velocity.y, limitedVelocity.z);
            }
        }
        Debug.DrawRay(transform.position, rb.velocity, Color.red);
    }

    bool OnSlope(out Vector3 directionOnSlope, out RaycastHit downHit) {
        if (Physics.Raycast(transform.position, Vector3.down, out downHit, playerHeight * 0.5f + 0.3f, groundMask)){
            float angle = Vector3.Angle(Vector3.up, downHit.normal);
            if (angle < slopeAngle && angle != 0) {
                Debug.Log("On climbable slope: " + angle);
                directionOnSlope = Vector3.ProjectOnPlane(horizontalVelocity, downHit.normal).normalized;
                return true;
            }
        }
        directionOnSlope = Vector3.zero;
        return false;
    }

    void HandleJump(){
        if (jumpPressed) {
            if (isGrounded) {
                verticalVelocity.y = jumpHeight;
                rb.AddForce(verticalVelocity, ForceMode.Impulse);
            }
            jumpPressed = false;
        }
    }

    public void ReceiveInput(Vector2 _input) => horizontalInput = _input;

    public void OnJumpPressed() => jumpPressed = true;
    
}
