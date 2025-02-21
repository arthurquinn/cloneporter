using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementLeavePortalState : IPlayerMovementState
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
        _controller.SetMovement(_controller.Stats.accelAfterPortal, _controller.Stats.deccelAfterPortal);

        // If we immediately exited to ground and are not overlapping a portal
        if (_controller.IsGrounded && !IsOverlappingPortal())
        {
            if (_controller.IsMoving)
            {
                _controller.TransitionToState(_controller.RunningState);
            }
            else
            {
                _controller.TransitionToState(_controller.IdleState);
            }
        }
    }

    private bool ShouldConserveMomentum(float targetVelocity)
    {
        return _controller.Stats.doConserveMomentum && // If we should conserve momentum
            Mathf.Abs(_controller.Rigidbody2D.velocity.x) > Mathf.Abs(targetVelocity) && // and our movement is faster than our max move speed
            Mathf.Sign(_controller.Rigidbody2D.velocity.x) == Mathf.Sign(targetVelocity);  // and the movement is in the same direction
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (!IsOverlappingPortal())
        {
            TransitionToNextState();
        }
    }

    private bool IsOverlappingPortal()
    {
        // Turn player collider into square and use it to check for overlapping portals
        float maxSide = Mathf.Max(_controller.Collider2D.bounds.size.x, _controller.Collider2D.bounds.size.y);
        Vector2 checkSize = new Vector2(maxSide, maxSide);
        Collider2D portalCollider = Physics2D.OverlapBox(_controller.Collider2D.bounds.center, checkSize * .98f, 0.0f, _controller.PortalLayer);

        // Return true if we are colliding, false otherwise
        return portalCollider != null;
    }

    public void EnterState()
    {
        _controller.SetDefaultGravity();
    }

    public void ExitState()
    {

    }

    #endregion

    private void TransitionToNextState()
    {
        // If we have a jump buffered and we are grounded then transition directly into the jump state
        if (_controller.LastJumpInput > 0 && _controller.IsGrounded)
        {
            _controller.TransitionToState(_controller.JumpingState);
        }

        // If we are falling transition into the falling state
        else if (_controller.IsFalling)
        {
            _controller.TransitionToState(_controller.FallingState);
        }

        // If we are moving transition into the running state
        else if (_controller.IsMoving)
        {
            _controller.TransitionToState(_controller.RunningState);
        }

        // If nothing else applies we are idle
        else
        {
            _controller.TransitionToState(_controller.IdleState);
        }
    }
}
