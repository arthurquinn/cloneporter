using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float _maxSpeed;
    [SerializeField] private float _jumpForce;
    [SerializeField] private float _acceleration;
    [SerializeField] private float _deceleration;
    [SerializeField] private float _jumpGravityScale;

    // Components
    private PlayerInputActions _inputs;
    private Rigidbody2D _rb;

    // Inputs
    private Vector2 _moveInput;

    // Trackers
    private bool _isFacingRight = true;
    private bool _startJump;


    private void Awake()
    {
        _inputs = new PlayerInputActions();
    }

    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        _inputs.Enable();
        _inputs.Player.Jump.performed += OnJump;
    }

    private void OnDisable()
    {
        _inputs.Disable();
    }

    private void Update()
    {
        _moveInput = _inputs.Player.Movement.ReadValue<Vector2>();

        // Check facing direction
        if (_isFacingRight && _moveInput.x < 0)
        {
            ChangeFacingDirection();
        }
        else if (!_isFacingRight && _moveInput.x > 0)
        {
            ChangeFacingDirection();
        }
    }

    private void FixedUpdate()
    {
        Vector2 movement = Vector2.zero;

        // If player is pressing move
        if (_moveInput != Vector2.zero)
        {
            // Set to max speed
            movement = Vector2.right * _moveInput * _maxSpeed;

            // Lerp for acceleration
            movement = Vector2.Lerp(_rb.velocity, movement, _acceleration * Time.fixedDeltaTime);
        }
        else
        {
            // Lerp to zero for deceleration
            movement = Vector2.Lerp(_rb.velocity, Vector2.zero, _deceleration * Time.fixedDeltaTime);
        }

        // If player pressed jump
        if (_startJump)
        {
            // Apply jump velocity
            movement.y = _jumpForce;
            _startJump = false;

            // Set gravity scale
            _rb.gravityScale = _jumpGravityScale;
        }
        else
        {
            movement.y = _rb.velocity.y;
        }


        _rb.velocity = movement;
    }

    private void ChangeFacingDirection()
    {
        _isFacingRight = !_isFacingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        _startJump = true;
    }
}
