using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class PlayerMovementRunningState : IPlayerMovementState
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
        _controller.SetDefaultMovement();

        FixedUpdateStateTransitions();
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
        // If we inputted jump then transition to jumping state
        if (_controller.LastJumpInput > 0)
        {
            _controller.TransitionToState(_controller.JumpingState);
        }

        // If we are not inputting movement then transition to idle state
        else if (!_controller.IsMoving)
        {
            _controller.TransitionToState(_controller.IdleState);
        }
    }
}
