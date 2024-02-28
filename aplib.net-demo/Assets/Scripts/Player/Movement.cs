using UnityEngine;

public class Movement : MonoBehaviour
{
    [SerializeField] private float _maxSpeed = 7;
    [SerializeField] private float _acceleration = 100;
    private Vector3 _horizontalVelocity;
    private Vector2 _horizontalInput;

    [SerializeField] private float _normalGravityScale = 1.7f;
    [SerializeField] private float _fallingGravityScale = 2.5f;
    private float _actualGravityScale;
    private float _gravity;

    [SerializeField] private float _jumpHeight = 7;
    [SerializeField] private float _airMovementScale = 0.5f;
    private bool _jumpPressed;

    [SerializeField] private LayerMask _groundMask;
    [SerializeField] private float _groundDrag = 0.7f;
    [SerializeField] private float _slopeAngle = 40;
    private float _playerHeight;
    private float _playerRadius;
    private Vector3 _bottomPoint;
    private bool _isGrounded;

    private Rigidbody _rigidbody;
    private CapsuleCollider _controller;

    /// <summary>
    /// Initialize the player's rigidbody and collider, and freeze the player's rotation.
    /// </summary>
    private void Start() { 
        _rigidbody = GetComponent<Rigidbody>();
        _controller = GetComponent<CapsuleCollider>();
        _playerHeight = _controller.height;
        _playerRadius = _controller.radius;
        _rigidbody.freezeRotation = true;
        _gravity = Physics.gravity.y;
    }

    /// <summary>
    /// Draw a red wire sphere at the bottom of the player to visualize the ground check area.
    /// </summary>
    void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(_bottomPoint, _playerRadius);
    }

    /// <summary>
    /// Main update loop where we do a ground check, move the player, and handle jumping.
    /// Updates once per physics update.
    /// </summary>
    private void FixedUpdate() {
        // Ground check by checking a sphere at the bottom of the player, more consistent than ray
        _bottomPoint = transform.TransformPoint(_controller.center - Vector3.up * (_playerHeight * 0.55f - _playerRadius));
        _isGrounded = Physics.CheckSphere(_bottomPoint, _playerRadius, _groundMask);

        MovePlayer();
        HandleJump();
    }   
    
    /// <summary>
    /// Move the player based on the input and the ground check using AddForce.
    /// Also applies custom gravity and limits the player's velocity.
    /// </summary>
    private void MovePlayer() {
        _horizontalVelocity = transform.right * _horizontalInput.x + transform.forward * _horizontalInput.y;
        bool isOnSlope = OnWalkableSlope(out Vector3 directionOnSlope, out RaycastHit downHit);
        bool isWalkningAgainstWall = WalkingAgainstWall(out Vector3 directionOnWall);
        _rigidbody.useGravity = !isOnSlope;

        if (_isGrounded) {
            _rigidbody.drag = _groundDrag; 
            Debug.Log("Grounded");
            if (isOnSlope) {
                _rigidbody.AddForce(_maxSpeed * _acceleration * Time.fixedDeltaTime * 1.5f * directionOnSlope);
                
                // Apply gravity but perpendicular to the slope, to prevent sliding
                _rigidbody.velocity += (_actualGravityScale - 1) * _gravity * Time.fixedDeltaTime * downHit.normal;
            }
            else if (isWalkningAgainstWall) {
                // Change the direction of the force to be along the wall
                _rigidbody.AddForce(_maxSpeed * _acceleration * Time.fixedDeltaTime * 0.5f * directionOnWall);
            }
            else {
                _rigidbody.AddForce(_maxSpeed * _acceleration * Time.fixedDeltaTime * _horizontalVelocity.normalized);
            }   
        }
        else {
            _rigidbody.drag = 0;  
            _rigidbody.AddForce(_maxSpeed * _acceleration * Time.fixedDeltaTime * _airMovementScale * _horizontalVelocity.normalized);

            // Custom gravity for player while falling
            _actualGravityScale = _rigidbody.velocity.y < 0 ? _fallingGravityScale : _normalGravityScale;
            _rigidbody.velocity += (_actualGravityScale - 1) * _gravity * Time.fixedDeltaTime * Vector3.up;
        }

        LimitVelocity(isOnSlope);
    }

    /// <summary>
    ///  Limit the player's velocity to the max speed.
    ///  Also accounts for vertical velocity when on a slope.
    /// </summary>
    /// <param name="isOnWalkableSlope">Is the player on a walkable slope</param>
    private void LimitVelocity(bool isOnWalkableSlope){
        if (isOnWalkableSlope) {
            if (_rigidbody.velocity.magnitude > _maxSpeed) _rigidbody.velocity = _rigidbody.velocity.normalized * _maxSpeed;   
        } else {
            Vector3 rigidbodyHorizontalVelocity = new(_rigidbody.velocity.x, 0, _rigidbody.velocity.z);
            if (rigidbodyHorizontalVelocity.magnitude > _maxSpeed) {
                Vector3 limitedVelocity = rigidbodyHorizontalVelocity.normalized * _maxSpeed;
                _rigidbody.velocity = new Vector3(limitedVelocity.x, _rigidbody.velocity.y, limitedVelocity.z);
            }
        }
        
        // Draw a ray to visualize the player's velocity and direction
        Debug.Log("Current velocity: " + _rigidbody.velocity.magnitude);
        Debug.DrawRay(transform.position, _rigidbody.velocity, Color.red);
    }

    /// <summary>
    /// Check if the player is on a walkable slope.
    /// If so, return movement direction adjusted to the slope, and the hit point.
    /// </summary>
    /// <param name="directionOnSlope">Movement direction projected on the slope as a plane</param>
    /// <param name="downHit">RaycastHit of the downward slope check</param>
    /// <returns>A boolean that signifies whether the player is on a walkable slope or not</returns>
    private bool OnWalkableSlope(out Vector3 directionOnSlope, out RaycastHit downHit) {
        // Shoot a ray down to check if the player is on a slope
        if (Physics.Raycast(transform.position, Vector3.down, out downHit, _playerHeight * 0.5f + 0.3f, _groundMask)) {
            float angle = Vector3.Angle(Vector3.up, downHit.normal);
            if (angle < _slopeAngle && angle != 0) {
                directionOnSlope = Vector3.ProjectOnPlane(_horizontalVelocity, downHit.normal).normalized;
                return true;
            }
        }
        
        directionOnSlope = Vector3.zero;
        return false;
    }

    /// <summary>
    /// Check if the player is walking against a wall.
    /// If so, return the wall's normal.
    /// </summary>
    /// <param name="wallNormal">The normal of the wall the player is walking against</param>
    /// <returns>A boolean that signifies whether the player is walking against a wall or not</returns>
    private bool WalkingAgainstWall(out Vector3 directionOnWall) {
        if (Physics.Raycast(transform.position, _horizontalVelocity, out RaycastHit hit, _playerRadius * 4f, _groundMask)) {
            directionOnWall = Vector3.ProjectOnPlane(_horizontalVelocity, hit.normal).normalized;
            return true;
        }
        directionOnWall = Vector3.zero;
        return false;
    }

    /// <summary>
    /// Handle the player's jump input and apply a force to the player's rigidbody.
    /// </summary>
    private void HandleJump(){
        if (_jumpPressed && _isGrounded) _rigidbody.AddForce(Vector3.up * _jumpHeight, ForceMode.Impulse);
        
        _jumpPressed = false;
    }

    /// <summary>
    /// Receive the player's input from the InputManager, result of an InputAction.
    /// </summary>
    /// <param name="input">The horizontal input in a Vector2</param>
    public void ReceiveHorizontalInput(Vector2 input) => _horizontalInput = input;

    /// <summary>
    /// Set the jumpPressed flag to true when the player presses the jump button.
    /// Result of an InputAction.
    /// </summary>
    public void OnJumpPressed() => _jumpPressed = true;
}
