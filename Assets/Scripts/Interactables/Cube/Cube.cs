using Cloneporter.UI;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Cube : MonoBehaviour, ICarryable
{
    private const float CARRIED_ANG_DRAG = 2f;
    private const float NEARBY_DRAG = 20f;
    private const float DROP_THRESHOLD = 0.01f;

    [SerializeField] private InteractablesEventChannel _events;

    [Tooltip("The speed at which the carryable object will follow its target.")]
    [SerializeField] private float _followSpeed;
    [Tooltip("The max distance we are allowed to be from the follow target before we are dropped.")]
    [SerializeField] private float _followDistance;

    [Header("References")]
    [Tooltip("The game object containing the canvas for the interact hint.")]
    [SerializeField] private GameObject _hintHUD;

    public Collider2D Collider { get { return _collider; } }

    private Rigidbody2D _rb;
    private Collider2D _collider;

    private Transform _carryPoint;
    private bool _isCarried;

    private float _cachedGravityScale;
    private float _cachedAngularDrag;

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
        _cachedAngularDrag = _rb.angularDrag;
    }

    private void FixedUpdate()
    {
        if (_isCarried)
        {
            // Drop if we are within follow distance
            bool shouldDrop = !IsWithinFollowDistance();
            if (shouldDrop)
            {
                // Raise the event so the player can drop item
                _events.OnItemDropped.Raise(new HeldItemDroppedEvent(this));
            }
            else
            {
                FollowTarget();
            }
        }

        // Set angular drag
        if (_isCarried)
        {
            _rb.angularDrag = CARRIED_ANG_DRAG;
        }
        else
        {
            _rb.angularDrag = _cachedAngularDrag;
        }

        // Remove any linear drag if we get a vertical velocity
        if (Mathf.Abs(_rb.velocity.y) > DROP_THRESHOLD)
        {
            _rb.drag = 0;
        }
    }

    public void SetNearby(bool isNearby)
    {
        // Show and hide the hint HUD depending on if we are in pickup range
        _hintHUD.SetActive(isNearby);

        // Add some linear drag to the cube so it's harder for the player to
        //   push it out of pickup range
        // Without this it can be annoying to run into the cube to try and pick it up
        //   then it moves away from you when you bump into it
        if (isNearby)
        {
            _rb.drag = NEARBY_DRAG;
        }
        else
        {
            _rb.drag = 0;
        }
    }

    public void SetPosition(Vector2 position)
    {
        _rb.isKinematic = true;
        _rb.position = position;
        _rb.isKinematic = false;
    }

    public void Pickup(Transform carryPoint)
    {
        // Disable gravity
        _rb.gravityScale = 0;

        // Set trackers
        _carryPoint = carryPoint;
        _isCarried = true;

        // Disable the hint HUD
        _hintHUD.SetActive(false);
    }

    public void Drop()
    {
        // Reapply gravity
        _rb.gravityScale = _cachedGravityScale;

        // Unset trackers
        _carryPoint = null;
        _isCarried = false;

        // Toggle our trigger area to force update
        // This allows the player to drop and pickup without needing to exit and re-enter our trigger area
        _collider.enabled = false;
        _collider.enabled = true;
    }

    private bool IsWithinFollowDistance()
    {
        // Get the squared magnitude between us and the carry point
        Vector2 carryDifference = (Vector2)_carryPoint.position - _rb.position;
        float squareMag = carryDifference.sqrMagnitude;

        // Return true if we are within the follow distance
        return squareMag <= _followDistance;
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
