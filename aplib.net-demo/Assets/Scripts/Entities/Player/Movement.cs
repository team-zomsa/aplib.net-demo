// This program has been developed by students from the bachelor Computer Science at Utrecht
// University within the Software Project course.
// Copyright Utrecht University (Department of Information and Computing Sciences)

using UnityEngine;

/// <summary>
/// This component handles the player's movement, jumping, and gravity.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(EntitySound))]
[RequireComponent(typeof(Timer))]
[RequireComponent(typeof(Animator))]
public class Movement : MonoBehaviour
{
    [SerializeField] private float _maxSpeedGround = 7;
    [SerializeField] private float _maxSpeedAir = 7;
    [SerializeField] private float _acceleration = 300;
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
    [SerializeField] private float _groundDrag = 0.9f;
    [SerializeField] private float _airDrag = 0.2f;
    [SerializeField] private float _slopeAngle = 40;
    [SerializeField] private float _slopeCheckRayExtraLength = 0.3f;
    [SerializeField] private float _wallCheckMaxDistance = 0.5f;
    private float _playerHeight;
    private float _playerRadius;
    private Vector3 _bottomPoint;
    private bool _isGrounded;

    private Rigidbody _rigidbody;
    private CapsuleCollider _controller;
    private Animator _animator;

    [SerializeField]
    private Transform _playerVisTransform;

    private EntitySound _footStep;
    private Timer _timer;

    /// <summary>
    /// Initialize the player's rigidbody and collider, and freeze the player's rotation.
    /// </summary>
    private void Start()
    {
        // Link the components of this object to the variables
        _rigidbody = GetComponent<Rigidbody>();
        _controller = GetComponent<CapsuleCollider>();
        _animator = GetComponent<Animator>();
        _footStep = GetComponent<EntitySound>();
        _timer = GetComponent<Timer>();

        _playerHeight = _controller.height;
        _playerRadius = _controller.radius;
        _gravity = Physics.gravity.y;
    }

    private void OnEnable()
    {
        InputManager.Instance.Moved += ReceiveHorizontalInput;
        InputManager.Instance.Jumped += OnJumped;
        InputManager.Instance.StoppedJump += OnStoppedJump;
    }

    private void OnDisable()
    {
        InputManager.Instance.Moved -= ReceiveHorizontalInput;
        InputManager.Instance.Jumped -= OnJumped;
        InputManager.Instance.StoppedJump -= OnStoppedJump;
    }

    /// <summary>
    /// Draw a red wire sphere at the bottom of the player to visualize the ground check area.
    /// </summary>
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(_bottomPoint, _playerRadius);
    }

    /// <summary>
    /// Main update loop where we do a ground check, move the player, and handle jumping.
    /// Updates once per physics update.
    /// </summary>
    private void FixedUpdate()
    {
        // Ground check by checking a sphere at the bottom of the player, more consistent than ray
        _bottomPoint = transform.TransformPoint(_controller.center - Vector3.up * (_playerHeight * 0.55f - _playerRadius));
        _isGrounded = Physics.CheckSphere(_bottomPoint, _playerRadius, _groundMask);
        _animator.SetBool("PlayerGrounded", _isGrounded);

        MovePlayer();
        HandleJump();
    }

    /// <summary>
    /// Move the player based on the input and the ground check using AddForce.
    /// Also applies custom gravity and limits the player's velocity.
    /// </summary>
    private void MovePlayer()
    {
        _horizontalVelocity = _playerVisTransform.right * _horizontalInput.x + _playerVisTransform.forward * _horizontalInput.y;
        bool isOnSlope = OnWalkableSlope(out Vector3 directionOnSlope, out RaycastHit downHit);
        bool isWallColliding = WalkingAgainstWall(out Vector3 wallNormal);
        _rigidbody.useGravity = !isOnSlope;

        if (_isGrounded)
        {
            _rigidbody.drag = _groundDrag;

            if (isOnSlope)
            {
                _rigidbody.AddForce(_maxSpeedGround * _acceleration * Time.fixedDeltaTime * directionOnSlope);

                // Apply gravity but perpendicular to the slope, to prevent sliding
                _rigidbody.velocity += (_actualGravityScale - 1) * _gravity * Time.fixedDeltaTime * downHit.normal;
            }
            else
            {
                _rigidbody.AddForce(_maxSpeedGround * _acceleration * Time.fixedDeltaTime * _horizontalVelocity.normalized);
            }

        }
        else
        {
            _rigidbody.drag = _airDrag;
            _rigidbody.AddForce(_maxSpeedGround * _acceleration * Time.fixedDeltaTime * _airMovementScale * _horizontalVelocity.normalized);

            // Custom gravity for player while falling
            _actualGravityScale = _rigidbody.velocity.y < 0 ? _fallingGravityScale : _normalGravityScale;
            _rigidbody.velocity += (_actualGravityScale - 1) * _gravity * Time.fixedDeltaTime * Vector3.up;
        }

        if (isWallColliding) HandleWallCollision(wallNormal);
        LimitVelocity(isOnSlope);

        // Lastly, play a footstep sound when the player is grounded, at certain intervals
        if (_isGrounded && _rigidbody.velocity.magnitude > 0.25f && _timer.IsFinished())
        {
            _timer.Reset();
            _footStep.Step();
        }

        _animator.SetFloat("PlayerVelocity", _rigidbody.velocity.magnitude);
    }

    /// <summary>
    /// Add force along the wall, to prevent sticking.
    /// </summary>
    /// <param name="wallNormal">The normal of the wall the player is walking against</param>
    private void HandleWallCollision(Vector3 wallNormal)
    {
        float angleMultiplier = Mathf.Clamp01(1.2f - Vector3.Angle(_horizontalVelocity, -wallNormal) / 90);
        Vector3 directionOnWall = Vector3.ProjectOnPlane(_horizontalVelocity, wallNormal).normalized;
        _rigidbody.AddForce(_maxSpeedGround * _acceleration * Time.fixedDeltaTime * angleMultiplier * directionOnWall);
    }

    /// <summary>
    ///  Limit the player's velocity to the max speed.
    ///  Also accounts for vertical velocity when on a slope.
    /// </summary>
    /// <param name="isOnWalkableSlope">Is the player on a walkable slope</param>
    private void LimitVelocity(bool isOnSlope)
    {
        if (isOnSlope && _rigidbody.velocity.magnitude > _maxSpeedGround)
        {
            _rigidbody.velocity = _rigidbody.velocity.normalized * _maxSpeedGround;
            return;
        }

        Vector3 rigidbodyHorizontalVelocity = new(_rigidbody.velocity.x, 0, _rigidbody.velocity.z);
        Vector3 rigidbodyVerticalVelocity = new(0, _rigidbody.velocity.y, 0);

        // Horizontal and vertical speeds are checked seperate from eachother to avoid rapid small bunny hops
        if (rigidbodyHorizontalVelocity.magnitude > _maxSpeedGround)
        {
            Vector3 horizontalVelocityLimited = rigidbodyHorizontalVelocity.normalized * _maxSpeedGround;
            _rigidbody.velocity = new Vector3(horizontalVelocityLimited.x, _rigidbody.velocity.y, horizontalVelocityLimited.z);
        }

        if (rigidbodyVerticalVelocity.magnitude > _maxSpeedAir)
        {
            Vector3 verticalVelocityLimited = rigidbodyVerticalVelocity.normalized * _maxSpeedAir;
            _rigidbody.velocity = new Vector3(_rigidbody.velocity.x, verticalVelocityLimited.y, _rigidbody.velocity.z);
        }
    }

    /// <summary>
    /// Check if the player is on a walkable slope.
    /// If so, return movement direction adjusted to the slope, and the hit point.
    /// </summary>
    /// <param name="directionOnSlope">Movement direction projected on the slope as a plane</param>
    /// <param name="downHit">RaycastHit of the downward slope check</param>
    /// <returns>A boolean that signifies whether the player is on a walkable slope or not</returns>
    private bool OnWalkableSlope(out Vector3 directionOnSlope, out RaycastHit downHit)
    {
        // Shoot a ray down to check if the player is on a slope
        if (Physics.Raycast(transform.position, Vector3.down, out downHit, _playerHeight * 0.5f + _slopeCheckRayExtraLength, _groundMask))
        {
            float angle = Vector3.Angle(Vector3.up, downHit.normal);
            if (angle < _slopeAngle && angle != 0)
            {
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
    private bool WalkingAgainstWall(out Vector3 wallNormal)
    {
        if (Physics.SphereCast(transform.position, _playerRadius, _horizontalVelocity, out RaycastHit hit, _wallCheckMaxDistance, _groundMask))
        {
            wallNormal = hit.normal;
            return true;
        }
        wallNormal = Vector3.zero;
        return false;
    }

    /// <summary>
    /// Handle the player's jump input and apply a force to the player's rigidbody.
    /// </summary>
    private void HandleJump()
    {
        if (_jumpPressed && _isGrounded) _rigidbody.AddForce(Vector3.up * _jumpHeight, ForceMode.Impulse);
    }

    /// <summary>
    /// Receive the player's input from the InputManager, result of an InputAction.
    /// </summary>
    /// <param name="input">The horizontal input in a Vector2.</param>
    public void ReceiveHorizontalInput(Vector2 input) => _horizontalInput = input;

    /// <summary>
    /// Set the jumpPressed flag to true when the player presses the jump button.
    /// </summary>
    public void OnJumped() => _jumpPressed = true;

    /// <summary>
    /// Set the jumpPressed flag to false when the player lets go of the jump button.
    /// </summary>
    public void OnStoppedJump() => _jumpPressed = false;
}
