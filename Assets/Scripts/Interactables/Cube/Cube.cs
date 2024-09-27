using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Cube : MonoBehaviour, ICarryable
{
    [Tooltip("The speed at which the carryable object will follow its target.")]
    [SerializeField] private float _followSpeed;

    public Collider2D Collider { get { return _collider; } }

    private Rigidbody2D _rb;
    private Collider2D _collider;

    private Transform _carryPoint;
    private bool _isCarried;

    private float _cachedGravityScale;

    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _collider = GetComponent<Collider2D>();

        // Cache our gravity scale so we can reapply it when dropped
        _cachedGravityScale = _rb.gravityScale;
    }

    private void FixedUpdate()
    {
        if (_isCarried)
        {
            FollowTarget();
        }
    }

    public void Pickup(Transform carryPoint)
    {
        // Disable gravity
        _rb.gravityScale = 0;

        // Set trackers
        _carryPoint = carryPoint;
        _isCarried = true;
    }

    public void Drop()
    {
        // Reapply gravity
        _rb.gravityScale = _cachedGravityScale;

        // Unset trackers
        _carryPoint = null;
        _isCarried = false;
    }

    private void FollowTarget()
    {
        // Calculate target velocity to carry point
        Vector2 targetDirection = (Vector2)_carryPoint.position - _rb.position;
        Vector2 targetVelocity = targetDirection * _followSpeed;

        // Lerp the velocity to target position for a dampened movement effect to target
        Vector2 velocity = Vector2.Lerp(_rb.velocity, targetVelocity, 0.5f);

        // Set the calculate velocity to our rigidbody
        _rb.velocity = velocity;
    }
}
