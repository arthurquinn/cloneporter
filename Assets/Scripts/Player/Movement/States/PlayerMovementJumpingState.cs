using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementJumpingState : IPlayerMovementState
{
    private IPlayerMovementController _controller;

    #region State Methods

    public void Awake(IPlayerMovementController controller)
    {
        _controller = controller;
    }

    public void Start()
    {

    }

    public void Update()
    {

    }

    public void FixedUpdate()
    {
        _controller.SetMovement();

        FixedUpdateStateTransitions();
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {

    }

    public void EnterState()
    {
        _controller.SetDefaultGravity();

        Jump();
    }

    public void ExitState()
    {

    }

    #endregion

    private void Jump()
    {
        // Get our jump force
        float force = _controller.Stats.jumpForce;

        // If we are falling, subtract our current velocity from our jump force
        //   This is useful if we jump using coyote time and need to offset our current
        //   negative velocity
        if (_controller.IsFalling)
        {
            force -= _controller.Rigidbody2D.velocity.y;
        }

        // Apply the jump force to our rigidbody
        _controller.Rigidbody2D.AddForce(Vector2.up * force, ForceMode2D.Impulse);
    }

    private void FixedUpdateStateTransitions()
    {
        if (_controller.IsFalling)
        {
            _controller.TransitionToState(_controller.FallingState);
        }
    }
}
