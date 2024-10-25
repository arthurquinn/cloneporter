using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementDeathState : IPlayerMovementState
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

    }

    public void OnCollisionEnter2D(Collision2D collision)
    {

    }

    public void EnterState()
    {
        // Set our death rotation based on is facing right flag
        float angularSign = _controller.IsFacingRight ? 1 : -1;

        // Apply changes to our rigidbody and collider for the death effect
        _controller.Rigidbody2D.bodyType = RigidbodyType2D.Kinematic;
        _controller.Rigidbody2D.constraints = RigidbodyConstraints2D.None;
        _controller.Rigidbody2D.angularVelocity = 15f * angularSign;
        _controller.Rigidbody2D.velocity = Vector2.up * 0.1f;
        _controller.Collider2D.enabled = false;
    }

    public void ExitState()
    {

    }

    #endregion

    private void FixedUpdateStateTransitions()
    {

    }
}
