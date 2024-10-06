using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public interface IPlayerMovementController
{
    // Components
    PlayerMovementStats Stats { get; }
    Rigidbody2D Rigidbody2D { get; }

    // Inputs and Timers
    Vector2 MoveInput { get; }
    float LastGroundedTime { get; }
    float LastJumpInput { get; }

    // Trackers
    bool IsMoving { get; }
    bool IsFalling { get; }

    // States
    PlayerMovementIdleState IdleState { get; }
    PlayerMovementRunningState RunningState { get; }
    PlayerMovementJumpingState JumpingState { get; }
    PlayerMovementFallingState FallingState { get; }

    // Methods
    void TransitionToState(IPlayerMovementState state);

    // Helpers
    void SetDefaultGravity();
    void SetDefaultMovement();
}

public interface IPlayerMovementState
{
    // Lifecycle Methods
    void Awake(IPlayerMovementController controller);
    void Start();
    void Update();
    void FixedUpdate();

    // Transition Methods
    void EnterState();
    void ExitState();
}

public class PlayerMovement : MonoBehaviour, IPlayerMovementController
{
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

    public Rigidbody2D Rigidbody2D { get { return _rb; } }
    public PlayerMovementStats Stats { get { return _stats; } }

    public Vector2 MoveInput { get { return _moveInput; } }
    public float LastGroundedTime { get { return _lastGroundedTime; } }
    public float LastJumpInput { get { return _lastJumpInput; } }

    public bool IsMoving { get { return _moveInput.x != 0; } }
    public bool IsFalling { get { return _rb.velocity.y < FALLING_THRESHOLD; } }

    public PlayerMovementIdleState IdleState { get { return _idleState; } }
    public PlayerMovementRunningState RunningState { get { return _runningState; } }
    public PlayerMovementJumpingState JumpingState { get { return _jumpState; } }
    public PlayerMovementFallingState FallingState { get { return _fallingState; } }

    private Rigidbody2D _rb;
    private PlayerInputActions _input;

    private Vector2 _moveInput;
    private float _lastGroundedTime;
    private float _lastJumpInput;
    private bool _isFacingRight;

    private PlayerMovementIdleState _idleState;
    private PlayerMovementRunningState _runningState;
    private PlayerMovementJumpingState _jumpState;
    private PlayerMovementFallingState _fallingState;

    private IPlayerMovementState _currentState;
    private IPlayerMovementState[] _allStates;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _input = new PlayerInputActions();

        _isFacingRight = true;

        _idleState = new PlayerMovementIdleState();
        _runningState = new PlayerMovementRunningState();
        _jumpState = new PlayerMovementJumpingState();
        _fallingState = new PlayerMovementFallingState();

        _allStates = new IPlayerMovementState[]
        {
            _idleState,
            _runningState,
            _jumpState,
            _fallingState,
        };

        RunForAllStates(state => state.Awake(this));
    }

    private void OnEnable()
    {
        _input.Player.Movement.Enable();

        _input.Player.Jump.Enable();
        _input.Player.Jump.performed += HandleJumpInput;
    }

    private void OnDisable()
    {
        _input.Player.Movement.Disable();

        _input.Player.Jump.performed -= HandleJumpInput;
        _input.Player.Jump.Disable();
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

    public void SetDefaultMovement()
    {
        float targetVelocity = _moveInput.x * _stats.runMaxSpeed;
        float accelRate = (Mathf.Abs(targetVelocity) > 0.01f) ? _stats.runAccelAmount : _stats.runDeccelAmount;

        float velocityDiff = targetVelocity - _rb.velocity.x;
        float movement = velocityDiff * accelRate;

        _rb.AddForce(movement * Vector2.right, ForceMode2D.Force);
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

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(_groundCheck.position, _groundCheckSize);
    }
}
