using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private PlayerMoveStats _stats;

    // Components
    private Rigidbody2D _rb;
    private PlayerInputActions _inputs;

    // Trackers
    public bool IsFacingRight { get; private set; }

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
    }

    private void OnDisable()
    {
        _inputs.Disable();
    }

    private void Update()
    {
        // Get player input
        _moveInput = _inputs.Player.Movement.ReadValue<Vector2>();

        // Check player facing direction
        if (_moveInput.x != 0)
        {
            CheckFacingDirection(_moveInput.x > 0);
        }
    }

    private void FixedUpdate()
    {
        // Target velocity is our move input times our max speed
        float targetVelocity = _moveInput.x * _stats.runMaxSpeed;

        // Our acceleration rate will be our run acceleration if we have a target velocity (i.e. player is inputting a
        //   horizontal movement command)
        // Else (the player is not inputting movement) we will use our deceleration amount
        // This ensures we have a different acceleration when the player is moving vs. when we are returning to idle
        float accelRate = (Mathf.Abs(targetVelocity) > 0.01f) ?  _stats.runAccelAmount : _stats.runDeccelAmount;

        // The difference between our current and target velocity
        float velocityDiff = targetVelocity - _rb.velocity.x;

        // The force will be the difference in velocity multiplied by the acceleration rate
        float movement = velocityDiff * accelRate;

        // Apply the force to rigid body
        _rb.AddForce(movement * Vector2.right, ForceMode2D.Force);
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
}
