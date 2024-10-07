using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;

public interface IPlayerMovementController
{
    // Components
    PlayerMovementStats Stats { get; }
    Rigidbody2D Rigidbody2D { get; }
    BoxCollider2D Collider2D { get; }

    // Inputs and Timers
    Vector2 MoveInput { get; }
    float LastGroundedTime { get; }
    float LastJumpInput { get; }

    // Trackers
    bool IsMoving { get; }
    bool IsFalling { get; }
    bool IsGrounded { get; }

    // Layer Masks
    LayerMask PortalLayer { get; }

    // States
    PlayerMovementIdleState IdleState { get; }
    PlayerMovementRunningState RunningState { get; }
    PlayerMovementJumpingState JumpingState { get; }
    PlayerMovementFallingState FallingState { get; }
    PlayerMovementLeavePortalState LeavePortalState { get; }
    PlayerMovementKnockbackState KnockbackState { get; }

    // Methods
    void TransitionToState(IPlayerMovementState state);

    // Helpers
    void SetDefaultGravity();
    void SetMovement();
    void SetMovement(float accelAmount, float decelAmount);
    void ConsumeJumpInput();
}

public interface IPlayerMovementState
{
    // Lifecycle Methods
    void Awake(IPlayerMovementController controller);
    void Start();
    void Update();
    void FixedUpdate();

    // Handlers
    void OnCollisionEnter2D(Collision2D collision);

    // Transition Methods
    void EnterState();
    void ExitState();
}

public class PlayerMovement : MonoBehaviour, IPlayerMovementController, ISnappable
{
    private const float NO_MULTIPLIER = 1.0f;
    private const float FALLING_THRESHOLD = -0.01f;

    [Header("Player Movement Stats")]
    [Tooltip("Stats that define player movement.")]
    [SerializeField] private PlayerMovementStats _stats;

    [Header("Ground Check")]
    [Tooltip("The transform representing the center of our ground check.")]
    [SerializeField] private Transform _groundCheck;
    [Tooltip("The size of our ground check overlap box.")]
    [SerializeField] private Vector2 _groundCheckSize;
    [Tooltip("The layers to be considered ground.")]
    [SerializeField] private LayerMask _groundLayers;

    [Header("Teleport Trigger")]
    [Tooltip("The teleport triggerr allows objects with rigidbodies to pass through portals.")]
    [SerializeField] private TeleportTrigger _teleportTrigger;
    [SerializeField] private LayerMask _portalLayer;

    public Rigidbody2D Rigidbody2D { get { return _rb; } }
    public PlayerMovementStats Stats { get { return _stats; } }
    public BoxCollider2D Collider2D { get { return _collider; } }

    public Vector2 MoveInput { get { return _moveInput; } }
    public float LastGroundedTime { get { return _lastGroundedTime; } }
    public float LastJumpInput { get { return _lastJumpInput; } }

    public bool IsMoving { get { return _moveInput.x != 0; } }
    public bool IsFalling { get { return _rb.velocity.y < FALLING_THRESHOLD; } }
    public bool IsGrounded { get { return _lastGroundedTime == _stats.coyoteTime; } }

    public LayerMask PortalLayer { get { return _portalLayer; } }

    public PlayerMovementIdleState IdleState { get { return _idleState; } }
    public PlayerMovementRunningState RunningState { get { return _runningState; } }
    public PlayerMovementJumpingState JumpingState { get { return _jumpState; } }
    public PlayerMovementFallingState FallingState { get { return _fallingState; } }
    public PlayerMovementLeavePortalState LeavePortalState {  get { return _leavePortalState; } }
    public PlayerMovementKnockbackState KnockbackState { get { return _knockbackState; } }

    private Rigidbody2D _rb;
    private PlayerInputActions _input;
    private BoxCollider2D _collider;

    private Vector2 _moveInput;
    private float _lastGroundedTime;
    private float _lastJumpInput;
    private bool _isFacingRight;

    private PlayerMovementIdleState _idleState;
    private PlayerMovementRunningState _runningState;
    private PlayerMovementJumpingState _jumpState;
    private PlayerMovementFallingState _fallingState;
    private PlayerMovementLeavePortalState _leavePortalState;
    private PlayerMovementKnockbackState _knockbackState;

    private IPlayerMovementState _currentState;
    private IPlayerMovementState[] _allStates;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _input = new PlayerInputActions();
        _collider = GetComponent<BoxCollider2D>();

        _isFacingRight = true;

        _idleState = new PlayerMovementIdleState();
        _runningState = new PlayerMovementRunningState();
        _jumpState = new PlayerMovementJumpingState();
        _fallingState = new PlayerMovementFallingState();
        _leavePortalState = new PlayerMovementLeavePortalState();
        _knockbackState = new PlayerMovementKnockbackState();

        _allStates = new IPlayerMovementState[]
        {
            _idleState,
            _runningState,
            _jumpState,
            _fallingState,
            _leavePortalState,
            _knockbackState,
        };

        RunForAllStates(state => state.Awake(this));
    }

    private void OnEnable()
    {
        _input.Player.Movement.Enable();

        _input.Player.Jump.Enable();
        _input.Player.Jump.performed += HandleJumpInput;

        _teleportTrigger.OnPortalLeave += HandlePortalLeave;
    }

    private void OnDisable()
    {
        _input.Player.Movement.Disable();

        _input.Player.Jump.performed -= HandleJumpInput;
        _input.Player.Jump.Disable();

        _teleportTrigger.OnPortalLeave -= HandlePortalLeave;
    }

    private void Start()
    {
        RunForAllStates(state => state.Start());

        TransitionToState(_idleState);
    }

    private void Update()
    {
        // Read the current move input
        _moveInput = _input.Player.Movement.ReadValue<Vector2>();

        // Set the facing direction
        SetFacingDirection();

        // Update the state
        _currentState.Update();
    }

    private void FixedUpdate()
    {
        // Update tracking state timers
        UpdateTimers();

        // Update the state
        _currentState.FixedUpdate();

        // Fall speed is hard capped in all states
        StartCoroutine(HardCapFallSpeed());
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        _currentState.OnCollisionEnter2D(collision);
    }

    private void RunForAllStates(Action<IPlayerMovementState> action)
    {
        for (int i = 0; i < _allStates.Length; i++)
        {
            action.Invoke(_allStates[i]);
        }
    }

    public void TransitionToState(IPlayerMovementState state)
    {
        Debug.Log("Transition from " + _currentState + " to " + state);

        // Exit the previous state if any
        if (_currentState != null)
        {
            _currentState.ExitState();
        }

        // Enter the new state
        state.EnterState();

        // Cache the current state
        _currentState = state;
    }

    public void SetDefaultGravity()
    {
        _rb.gravityScale = _stats.gravityScale;
    }

    public void SetMovement()
    {
        SetMovement(NO_MULTIPLIER, NO_MULTIPLIER);
    }

    public void SetMovement(float accelMult, float decelMult)
    {
        float targetVelocity = _moveInput.x * _stats.runMaxSpeed;
        float accelRate = (Mathf.Abs(targetVelocity) > 0.01f) ?
            _stats.runAccelAmount * accelMult :
            _stats.runDeccelAmount * decelMult;

        float velocityDiff = targetVelocity - _rb.velocity.x;
        float movement = velocityDiff * accelRate;

        _rb.AddForce(movement * Vector2.right, ForceMode2D.Force);
    }

    public void ConsumeJumpInput()
    {
        _lastJumpInput = 0;
    }

    private void SetFacingDirection()
    {
        // If we are inputting movement
        if (_moveInput.x != 0)
        {
            // If we should face right and are not
            bool shouldFaceRight = _moveInput.x > 0;
            if (shouldFaceRight != _isFacingRight)
            {
                // Flip our rotation 180 degrees on the y to change visual facing direction
                transform.rotation *= Quaternion.Euler(0, 180, 0);

                // Update is facing right tracker
                _isFacingRight = !_isFacingRight;
            }
        }
    }

    private IEnumerator HardCapFallSpeed()
    {
        // Wait for end of all fixed updates to hard cap fall speed
        yield return new WaitForFixedUpdate();
        if (IsFalling)
        {
            float limitY = Mathf.Max(_rb.velocity.y, -_stats.maxFallSpeed);
            _rb.velocity = new Vector2(_rb.velocity.x, limitY);
        }
    }

    private void UpdateTimers()
    {
        _lastJumpInput -= Time.fixedDeltaTime;
        _lastGroundedTime -= Time.fixedDeltaTime;

        // Check for ground
        if (Physics2D.OverlapBox(_groundCheck.position, _groundCheckSize, 0, _groundLayers))
        {
            _lastGroundedTime = _stats.coyoteTime;
        }
    }

    private void HandleJumpInput(InputAction.CallbackContext context)
    {
        _lastJumpInput = _stats.jumpInputBufferTime;
    }

    private void HandlePortalLeave()
    {
        TransitionToState(_leavePortalState);
    }

    public void Knockback(KnockbackAttack attack)
    {
        if (_currentState != _knockbackState)
        {
            // Set the knockback force and origin
            _knockbackState.Attack = attack;

            // Transition to state
            TransitionToState(_knockbackState);
        }
    }

    public void SnapOffset(Vector2 offset)
    {
        Vector2 newPosition = _rb.position + offset;
        _rb.position = newPosition;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(_groundCheck.position, _groundCheckSize);
    }
}
