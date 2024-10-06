using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct KnockbackAttack
{
    public Vector2 Direction { get; private set; }
    public float Force { get; private set; }

    public KnockbackAttack(Vector2 direction, float force)
    {
        Direction = direction;
        Force = force;
    }
}

public class PlayerMovementKnockbackState : IPlayerMovementState
{
    private IPlayerMovementController _controller;

    public KnockbackAttack Attack { get; set; }

    // TODO: Maybe store these in the attack?
    private float KNOCKBACK_DECEL_MULT = 0.35f;
    private float KNOCKBACK_END_VEL_THRESHOLD = 0.45f;

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
        // Decelerate to 0 movement
        float targetVelocity = 0f;
        float accelRate = _controller.Stats.runDeccelAmount * KNOCKBACK_DECEL_MULT;
        float velocityDiff = targetVelocity - _controller.Rigidbody2D.velocity.x;
        float movement = velocityDiff * accelRate;
        _controller.Rigidbody2D.AddForce(movement * Vector2.right, ForceMode2D.Force);

        // Check transition to state
        FixedUpdateStateTransitions();
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {

    }

    public void EnterState()
    {
        _controller.SetDefaultGravity();

        Knockback();
    }

    public void ExitState()
    {

    }

    #endregion

    private void FixedUpdateStateTransitions()
    {
        if (Mathf.Abs(_controller.Rigidbody2D.velocity.x) < KNOCKBACK_END_VEL_THRESHOLD)
        {
            if (_controller.IsGrounded && _controller.LastJumpInput > 0)
            {
                _controller.TransitionToState(_controller.JumpingState);
            }

            else if (_controller.IsFalling)
            {
                _controller.TransitionToState(_controller.FallingState);
            }

            else if (_controller.IsGrounded && _controller.IsMoving)
            {
                _controller.TransitionToState(_controller.RunningState);
            }

            else
            {
                _controller.TransitionToState(_controller.IdleState);
            }
        }
    }

    private void Knockback()
    {
        _controller.Rigidbody2D.AddForce(Attack.Direction * Attack.Force, ForceMode2D.Impulse);
    }
}
