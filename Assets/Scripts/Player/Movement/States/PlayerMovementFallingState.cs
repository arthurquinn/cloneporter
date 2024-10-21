using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementFallingState : IPlayerMovementState
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
        // Player has default movement while falling
        _controller.SetMovement();

        FixedUpdateStateTransitions();

        Debug.Log(_controller.Rigidbody2D.velocity);
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {

    }

    public void EnterState()
    {
        _controller.SetDefaultGravity();
    }

    public void ExitState()
    {

    }

    #endregion

    private void FixedUpdateStateTransitions()
    {
        // If we just reached ground and we have a buffered jump input
        //   This will occur if the player just reached ground and there is a jump input buffered
        //   In this case we can transition directly from falling to jumping
        // Check the last grounded time instead of checking grounded directly so that we can jump within
        //   the coyote time buffer after we run off of a ledge
        if (_controller.LastGroundedTime > 0 && _controller.LastJumpInput > 0)
        {
            _controller.TransitionToState(_controller.JumpingState);
        }

        // If we reached the ground and are inputting movement then transition directly to the running state
        else if (_controller.IsGrounded && _controller.IsMoving)
        {
            _controller.TransitionToState(_controller.RunningState);
        }

        // Else if we reached ground then transition to idle
        else if (_controller.IsGrounded)
        {
            _controller.TransitionToState(_controller.IdleState);
        }
    }
}
