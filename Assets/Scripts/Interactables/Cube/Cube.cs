using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Cube : MonoBehaviour, ICarryable
{
    [SerializeField] private InteractablesEventChannel _events;

    [Tooltip("The speed at which the carryable object will follow its target.")]
    [SerializeField] private float _followSpeed;
    [Tooltip("The max distance we are allowed to be from the follow target before we are dropped.")]
    [SerializeField] private float _followDistance;

    public Collider2D Collider { get { return _collider; } }

    private Rigidbody2D _rb;
    private Collider2D _collider;

    private Transform _carryPoint;
    private bool _isCarried;

    private float _cachedGravityScale;

    private void Awake()
    {
        // Square the follow distance so we can compare against squared magnitude
        _followDistance = Mathf.Pow(_followDistance, 2);
    }

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
            if (CheckDistanceToTarget())
            {
                FollowTarget();
            }
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

    private bool CheckDistanceToTarget()
    {
        // Get the squared magnitude between us and the carry point
        Vector2 carryDifference = (Vector2)_carryPoint.position - _rb.position;
        float squareMag = carryDifference.sqrMagnitude;

        // Break connection if greater than max threshold
        if (squareMag > _followDistance)
        {
            // Raise the event so the player can drop item
            _events.OnItemDropped.Raise(new HeldItemDroppedEvent(this));
            return false;
        }

        // Return true if connection can be maintained
        return true;
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
