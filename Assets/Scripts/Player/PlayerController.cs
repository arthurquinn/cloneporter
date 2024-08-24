using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private PlayerMoveStats _stats;

    [Header("Checks")]
    [SerializeField] private Transform _groundCheck;
    [SerializeField] private Vector2 _groundCheckSize;

    [Header("Layers")]
    [SerializeField] private LayerMask _groundLayer;

    // Components
    private Rigidbody2D _rb;
    private PlayerInputActions _inputs;

    // Trackers
    public bool IsFacingRight { get; private set; }
    public bool StartJump { get; private set; }
    public bool IsJumping { get; private set; }
    public bool IsJumpFalling { get; private set; }
    public bool IsJumpCut { get; private set; }

    // Timers
    public float LastPressedJumpTime { get; private set; }
    public float LastOnGroundTime { get; private set; }

    // Inputs
    private Vector2 _moveInput;

    private void Awake()
    {
        _inputs = new PlayerInputActions();   
    }

    private void Start()
    {
        // Get components
        _rb = GetComponent<Rigidbody2D>();

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
            if (Physics2D.OverlapBox(_groundCheck.position, _groundCheckSize, 0, _groundLayer))
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
        }
    }

    private void FixedUpdate()
    {
        HandleMovement();
        HandleJump();
        HandleGravity();
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
        if (IsJumpCut)
        {
            // Higher gravity scale if player released jump button early
            _rb.gravityScale = _stats.gravityScale * _stats.jumpCutGravityMult;
        }
        else if (_rb.velocity.y < -0.01f)
        {
            // Higher gravity scale if we are falling
            _rb.gravityScale = _stats.gravityScale * _stats.fallGravityMult;
        }
        else
        {
            // Regular gravity scale
            _rb.gravityScale = _stats.gravityScale;
        }
    }

    private void CheckFacingDirection(bool movingRight)
    {
        // If we are facing the wrong way
        if (movingRight != IsFacingRight)
        {
            // Invert character scale
            Vector2 localScale = transform.localScale;
            localScale.x *= -1;
            transform.localScale = localScale;

            // Change facing right check
            IsFacingRight = !IsFacingRight;
        }
    }

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
    }

    #endregion
}
