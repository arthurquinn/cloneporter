using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementIdleState : IPlayerMovementState
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
    }

    public void ExitState()
    {

    }

    #endregion

    private void FixedUpdateStateTransitions()
    {
        if (_controller.LastGroundedTime > 0 && _controller.LastJumpInput > 0)
        {
            _controller.TransitionToState(_controller.JumpingState);
        }

        else if (_controller.IsFalling)
        {
            _controller.TransitionToState(_controller.FallingState);
        }

        else if (_controller.IsMoving)
        {
            _controller.TransitionToState(_controller.RunningState);
        }
    }
}
