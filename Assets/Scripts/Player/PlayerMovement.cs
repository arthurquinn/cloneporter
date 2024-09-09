using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using System.Runtime.CompilerServices;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private PlayerMoveStats _stats;

    [Header("Checks")]
    [SerializeField] private Transform _groundCheck;
    [SerializeField] private Vector2 _groundCheckSize;
    [SerializeField] private Transform _weaponRestPoint;

    [Header("Layers")]
    [SerializeField] private LayerMask _standableTerrainLayers;
    [SerializeField] private LayerMask _portalLayer;
    [SerializeField] private LayerMask _ignoreCollisionsInPortal;

    [Header("Events")]
    [SerializeField] private UnityEvent _onJumpStart;
    [SerializeField] private UnityEvent<Vector2> _onGunPositionChanged;

    [Header("IK Objects")]
    [SerializeField] private Transform _gunTarget;
    [SerializeField] private Transform _gunEffector;

    // Components
    private Rigidbody2D _rb;
    private PlayerInputActions _inputs;
    private BoxCollider2D _boxCollider;

    // Trackers
    public bool IsFacingRight { get; private set; }
    public bool StartJump { get; private set; }
    public bool IsJumping { get; private set; }
    public bool IsJumpFalling { get; private set; }
    public bool IsJumpCut { get; private set; }

    // Timers
    public float LastPressedJumpTime { get; private set; }
    public float LastOnGroundTime { get; private set; }
    public float LastInPortalTime { get; private set; }

    // Cached Values
    public Vector2 LastFixedPosition { get; private set; }

    // Inputs
    private Vector2 _moveInput;

    // Collision detections
    private float _rayPortalCheckVert;
    private float _rayPortalCheckHoriz;
    private Vector2 _portalEnterCheck;

    // Constant values
    private float SNAP_X_OFFSET = 0.05f;

    private void Awake()
    {
        _inputs = new PlayerInputActions();   
    }

    private void Start()
    {
        // Get components
        _rb = GetComponent<Rigidbody2D>();
        _boxCollider = GetComponent<BoxCollider2D>();

        // Initialize values
        _rb.gravityScale = _stats.gravityScale;
        IsFacingRight = true;
    }

    private void OnEnable()
    {
        _inputs.Enable();
        _inputs.Player.Jump.started += OnJumpStarted;
        _inputs.Player.Jump.canceled += OnJumpCanceled;
    }

    private void OnDisable()
    {
        _inputs.Disable();
    }

    private void Update()
    {
        // Update timers
        LastPressedJumpTime -= Time.deltaTime;
        LastOnGroundTime -= Time.deltaTime;
        LastInPortalTime -= Time.deltaTime;

        // Get player input
        _moveInput = _inputs.Player.Movement.ReadValue<Vector2>();

        // Check player facing direction
        if (_moveInput.x != 0)
        {
            CheckFacingDirection(_moveInput.x > 0);
        }

        // Check player collisions
        if (!IsJumping)
        {
            // Check for ground
            if (Physics2D.OverlapBox(_groundCheck.position, _groundCheckSize, 0, _standableTerrainLayers))
            {
                LastOnGroundTime = _stats.coyoteTime;
            }
        }

        // Jump checks
        // If we are jumping and we started falling
        if (IsJumping && _rb.velocity.y < 0)
        {
            IsJumping = false;
            IsJumpFalling = true;
        }

        // If we are on the ground
        if (!IsJumping && LastOnGroundTime > 0)
        {
            // Then we are no longer jump falling
            IsJumpFalling = false;
            IsJumpCut = false;
        }

        // Set start jump if conditions for jump on jump input are true
        if (LastPressedJumpTime > 0 && LastOnGroundTime > 0 && !IsJumping)
        {
            StartJump = true;
            if (_onJumpStart != null)
            {
                _onJumpStart.Invoke();
            }
        }
    }

    private void FixedUpdate()
    {
        if (LastInPortalTime < 0)
        {
            HandlePortalInteractions();
            HandleMovement();
            HandleJump();
        }
        HandleGravity();
    }

    private void HandlePortalInteractions()
    {
        // Calculate raycast lengths
        _rayPortalCheckVert = _boxCollider.bounds.extents.y + Mathf.Abs(_rb.velocity.y) * Time.fixedDeltaTime;
        _rayPortalCheckHoriz = _boxCollider.bounds.extents.x + Mathf.Abs(_rb.velocity.x) * Time.fixedDeltaTime;

        // Check raycast hits
        // TODO: Not getting collision if both inside?? check this
        bool didHitPortal = CheckPortalRaycast(Vector2.up, _rayPortalCheckVert) ||
            CheckPortalRaycast(Vector2.down, _rayPortalCheckVert) ||
            CheckPortalRaycast(Vector2.left, _rayPortalCheckHoriz) ||
            CheckPortalRaycast(Vector2.right, _rayPortalCheckHoriz);

        // Set collision state
        if (didHitPortal)
        {
            //DisableCollisions(_ignoreCollisionsInPortal);
        }
        else
        {
            //EnableCollisions(_ignoreCollisionsInPortal);
        }

        LastFixedPosition = _rb.position;
    }

    private bool CheckPortalRaycast(Vector2 direction, float distance)
    {
        return Physics2D.Raycast(_boxCollider.bounds.center, direction, distance, _portalLayer).collider != null;
    }

    private void EnableCollisions(LayerMask collisionMask)
    {
        // Turn on collision for specified ignore layers (i.e. stop ignoring them)
        LayerMask current = Physics2D.GetLayerCollisionMask(gameObject.layer);
        LayerMask newMask = collisionMask | current;
        Physics2D.SetLayerCollisionMask(gameObject.layer, newMask);
    }

    private void DisableCollisions(LayerMask collisionMask)
    {
        // Turn off collision for specified ignore layers
        LayerMask current = Physics2D.GetLayerCollisionMask(gameObject.layer);
        LayerMask newMask = ~collisionMask & current;
        Physics2D.SetLayerCollisionMask(gameObject.layer, newMask);
    }

    private void HandleMovement()
    {
        // Target velocity is our move input times our max speed
        float targetVelocity = _moveInput.x * _stats.runMaxSpeed;

        // Our acceleration rate will be our run acceleration if we have a target velocity (i.e. player is inputting a
        //   horizontal movement command)
        // Else (the player is not inputting movement) we will use our deceleration amount
        // This ensures we have a different acceleration when the player is moving vs. when we are returning to idle
        float accelRate = (Mathf.Abs(targetVelocity) > 0.01f) ? _stats.runAccelAmount : _stats.runDeccelAmount;

        // The difference between our current and target velocity
        float velocityDiff = targetVelocity - _rb.velocity.x;

        // The force will be the difference in velocity multiplied by the acceleration rate
        float movement = velocityDiff * accelRate;

        // Apply the force to rigid body
        _rb.AddForce(movement * Vector2.right, ForceMode2D.Force);
    }

    private void HandleJump()
    {
        if (StartJump)
        {
            // Update trackers
            // Last pressed jump time is set to 0 so we don't initiate another jump
            //   from one input
            StartJump = false;
            IsJumping = true;
            LastPressedJumpTime = 0;

            // Subtract current y velocity from the jump force if player is falling
            // This ensures that if they are falling, the expected jump force remains the same
            float force = _stats.jumpForce;
            if (_rb.velocity.y < 0)
            {
                force -= _rb.velocity.y;
            }

            // Apply the force to the rigidbody
            _rb.AddForce(Vector2.up * force, ForceMode2D.Impulse);
        }
    }

    private void HandleGravity()
    {

        //if (IsJumpCut)
        //{
        //    // Higher gravity scale if player released jump button early
        //    _rb.gravityScale = _stats.gravityScale * _stats.jumpCutGravityMult;
        //}
        //else if (_rb.velocity.y < -0.01f)
        //{
        //    // Higher gravity scale if we are falling
        //    _rb.gravityScale = _stats.gravityScale * _stats.fallGravityMult;
        //}
        //else
        //{
        //    // Regular gravity scale
        //    _rb.gravityScale = _stats.gravityScale;
        //}

        // Cap maximum fall speed
        _rb.velocity = new Vector2(_rb.velocity.x, Mathf.Max(_rb.velocity.y, -_stats.maxFallSpeed));
        _rb.gravityScale = _stats.gravityScale;
    }

    private void CheckFacingDirection(bool movingRight)
    {
        // If we are facing the wrong way
        if (movingRight != IsFacingRight)
        {
            // Rotate character 180 degrees on the y axis to "flip"
            transform.rotation *= Quaternion.Euler(0, 180, 0);

            // Change facing right check
            IsFacingRight = !IsFacingRight;
        }
    }

    #region Player Event Handlers

    public void SnapAboveCollider(Collider2D collider)
    {
        Vector2 highestColliderPoint = collider.bounds.max;
        Vector2 lowestPlayerPoint = _boxCollider.bounds.min;

        // Snap the player above the collider if we are below it
        // This is useful for the player walking directly into switches without
        //   having to jump on top of them
        if (lowestPlayerPoint.y < highestColliderPoint.y)
        {
            Vector2 offset = Vector2.zero;
            offset.y = highestColliderPoint.y - lowestPlayerPoint.y;
            offset.x = highestColliderPoint.x > lowestPlayerPoint.x ? SNAP_X_OFFSET : -SNAP_X_OFFSET;
            _rb.MovePosition(_rb.position += offset);
        }
    }

    public void OnPortalEntered(IPortal portal)
    {
        if (LastInPortalTime < 0)
        {
            // Get our entry ray
            Vector2 entryDirection = _rb.velocity.normalized;
            Vector2 entryPoint = LastFixedPosition;
            Ray2D entryRay = new Ray2D(entryPoint, entryDirection);

            // Use portal interface to apply port to our rigidbody
            portal.ApplyPort(entryRay, _rb);

            // Set our last in portal time
            LastInPortalTime = _stats.portalInputTimeout;
        }
    }

    private void InvokeGunPositionChanged()
    {
        _onGunPositionChanged.Invoke(_gunEffector.position);
    }

    public void AimGun(Vector2 aimPosition)
    {
        // Change the position of our gun target to the aim position
        _gunTarget.position = aimPosition;

        // Invoke the event on gun position changed using the effector
        //   to represent the tip of our gun
        InvokeGunPositionChanged();
    }

    public void ResetGunPose()
    {
        // Change our gun position to the weapon rest position
        _gunTarget.position = _weaponRestPoint.position;

        // Invoke gun position changed event
        InvokeGunPositionChanged();
    }

    #endregion

    #region Input Callbacks

    public void OnJumpStarted(InputAction.CallbackContext context)
    {
        LastPressedJumpTime = _stats.jumpInputBufferTime;
    }

    public void OnJumpCanceled(InputAction.CallbackContext context)
    {
        // Set jump cut if player is jumping and they are moving upwards
        if (IsJumping && _rb.velocity.y > 0)
        {
            IsJumpCut = true;
        }
    }

    #endregion

    #region Editor Methods

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(_groundCheck.position, _groundCheckSize);

        //Gizmos.color = Color.red;
        //Gizmos.DrawLine(_boxCollider.bounds.center, (Vector2)_boxCollider.bounds.center + (Vector2.up * _rayPortalCheckVert));
        //Gizmos.DrawLine(_boxCollider.bounds.center, (Vector2)_boxCollider.bounds.center + (Vector2.down * _rayPortalCheckVert));
        //Gizmos.DrawLine(_boxCollider.bounds.center, (Vector2)_boxCollider.bounds.center + (Vector2.left * _rayPortalCheckHoriz));
        //Gizmos.DrawLine(_boxCollider.bounds.center, (Vector2)_boxCollider.bounds.center + (Vector2.right * _rayPortalCheckHoriz));

        //Gizmos.color = Color.blue;
        //Gizmos.DrawWireCube(_boxCollider.bounds.center, _portalEnterCheck);
    }

    #endregion
}
