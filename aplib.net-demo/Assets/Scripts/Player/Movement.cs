using UnityEngine;

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
    [SerializeField] float airMovementScale = 0.5f;
    bool jumpPressed;

    [SerializeField] LayerMask groundMask;
    [SerializeField] float groundDrag;
    [SerializeField] float slopeAngle = 40;
    float playerHeight;
    bool isGrounded;

    Rigidbody rb;
    CapsuleCollider controller;

    /// <summary>
    /// Initialize the player's rigidbody and collider, and freeze the player's rotation.
    /// </summary>
    void Start() { 
        rb = GetComponent<Rigidbody>();
        controller = GetComponent<CapsuleCollider>();
        playerHeight = controller.height;
        rb.freezeRotation = true;
        gravity = Physics.gravity.y;
    }

    /// <summary>
    /// Main update loop where we do a ground check, move the player, and handle jumping.
    /// Updates once per physics update.
    /// </summary>
    void FixedUpdate() {
        // Ground check by checking a sphere at the bottom of the player, more consistent than ray
        Vector3 bottomPoint = transform.TransformPoint(controller.center - 0.5f * playerHeight * Vector3.up);
        isGrounded = Physics.CheckSphere(bottomPoint, 0.15f, groundMask);

        MovePlayer();
        HandleJump();
    }   
    
    /// <summary>
    /// Move the player based on the input and the ground check using AddForce.
    /// Also applies custom gravity and limits the player's velocity.
    /// </summary>
    void MovePlayer() {
        horizontalVelocity = transform.right * horizontalInput.x + transform.forward * horizontalInput.y;
        bool isOnSlope = OnWalkableSlope(out Vector3 directionOnSlope, out RaycastHit downHit);
        rb.useGravity = !isOnSlope;

        if (isGrounded) {
            rb.drag = groundDrag; 
            Debug.Log("Grounded");
            if (isOnSlope) {
                rb.AddForce(maxSpeed * acceleration * Time.fixedDeltaTime * 1.5f * directionOnSlope);
                // Apply gravity but perpendicular to the slope, to prevent sliding
                rb.velocity += (actualGravityScale - 1) * gravity * Time.fixedDeltaTime * downHit.normal;
            }
            else {
                rb.AddForce(maxSpeed * acceleration * Time.fixedDeltaTime * horizontalVelocity.normalized);
            }   
        }
        else {
            rb.drag = 0;  
            rb.AddForce(maxSpeed * acceleration * Time.fixedDeltaTime * airMovementScale * horizontalVelocity.normalized); 
            // Custom gravity for player while falling
            actualGravityScale = rb.velocity.y < 0 ? fallingGravityScale : normalGravityScale;
            rb.velocity += (actualGravityScale - 1) * gravity * Time.fixedDeltaTime * Vector3.up;
        }

        LimitVelocity(isOnSlope);
    }

    /// <summary>
    ///  Limit the player's velocity to the max speed.
    ///  Also accounts for vertical velocity when on a slope.
    /// </summary>
    /// <param name="isOnWalkableSlope"></param>
    void LimitVelocity(bool isOnWalkableSlope){
        if (isOnWalkableSlope) {   
            if (rb.velocity.magnitude > maxSpeed) {
                rb.velocity = rb.velocity.normalized * maxSpeed;
            }
        } else {
            Vector3 rbHorizontalVelocity = new(rb.velocity.x, 0, rb.velocity.z);
            if (rbHorizontalVelocity.magnitude > maxSpeed) {
                Vector3 limitedVelocity = rbHorizontalVelocity.normalized * maxSpeed;
                rb.velocity = new Vector3(limitedVelocity.x, rb.velocity.y, limitedVelocity.z);
            }
        }
        // Draw a ray to visualize the player's velocity and direction
        Debug.DrawRay(transform.position, rb.velocity, Color.red);
    }

    /// <summary>
    /// Check if the player is on a walkable slope.
    /// If so, return movement direction adjusted to the slope, and the hit point.
    /// </summary>
    /// <param name="directionOnSlope">Movement direction projected on the slope as a plane</param>
    /// <param name="downHit">RaycastHit of the downward slope check</param>
    /// <returns></returns>
    bool OnWalkableSlope(out Vector3 directionOnSlope, out RaycastHit downHit) {
        // Shoot a ray down to check if the player is on a slope
        if (Physics.Raycast(transform.position, Vector3.down, out downHit, playerHeight * 0.5f + 0.3f, groundMask)){
            float angle = Vector3.Angle(Vector3.up, downHit.normal);
            if (angle < slopeAngle && angle != 0) {
                directionOnSlope = Vector3.ProjectOnPlane(horizontalVelocity, downHit.normal).normalized;
                return true;
            }
        }
        directionOnSlope = Vector3.zero;
        return false;
    }

    /// <summary>
    /// Handle the player's jump input and apply a force to the player's rigidbody.
    /// </summary>
    void HandleJump(){
        if (jumpPressed && isGrounded) {
            rb.AddForce(Vector3.up * jumpHeight, ForceMode.Impulse);
        }
        jumpPressed = false;
    }

    /// <summary>
    /// Receive the player's input from the InputManager, result of an InputAction.
    /// </summary>
    /// <param name="_input"></param>
    public void ReceiveHorizontalInput(Vector2 _input) => horizontalInput = _input;

    /// <summary>
    /// Set the jumpPressed flag to true when the player presses the jump button.
    /// Result of an InputAction.
    /// </summary>
    public void OnJumpPressed() => jumpPressed = true;
    
}
