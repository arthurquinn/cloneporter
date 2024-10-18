using DG.Tweening;
using UnityEngine;

public class BurnoutMovement : MonoBehaviour
{
    private Rigidbody2D _rb;

    [Header("Rotations")]
    [Tooltip("The amount of time to rotate from one rotation to another.")]
    [SerializeField] private float _rotationTime;
    [Tooltip("The amount of time to hold the current rotation.")]
    [SerializeField] private float _holdTime;
    [Tooltip("The rotations to cycle between.")]
    [SerializeField] private float[] _holdRotations;

    private float _currentHoldTime;
    private int _currentRotationIndex;

    private BurnoutAttackable _attackable;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _attackable = GetComponent<BurnoutAttackable>();

        // Init the hold timer
        ResetHoldTimer();
    }

    private void OnEnable()
    {
        _attackable.OnDeath += HandleDeath;
    }

    private void OnDisable()
    {
        _attackable.OnDeath -= HandleDeath;
    }

    private void Start()
    {
        // Initialize  the rotation
        _rb.rotation = GetCurrentRotation();
    }

    private void FixedUpdate()
    {
        // Decrement hold rotation timer
        _currentHoldTime -= Time.fixedDeltaTime;

        // Handle rotations
        RotateOnTimer();
    }

    private void RotateOnTimer()
    {
        // If our timer expired then start the tween to rotate to our next value
        if (_currentHoldTime < 0)
        {
            // Disable the hold timer until the tween completes
            _currentHoldTime = float.MaxValue;

            // Calculate the delta angle and apply it to rigidbody
            float targetRotation = IncrementToNextRotation();
            float deltaAngle = Mathf.DeltaAngle(_rb.rotation, targetRotation);
            _rb.DORotate(_rb.rotation + deltaAngle, _rotationTime)
                .SetEase(Ease.InOutQuad)
                .OnComplete(ResetHoldTimer);
        }
    }

    private void ResetHoldTimer()
    {
        _currentHoldTime = _holdTime;
    }

    private float IncrementToNextRotation()
    {
        // Increment and cycle to the first index if we reached the max
        _currentRotationIndex = (_currentRotationIndex + 1) % _holdRotations.Length;

        // Return the current rotation if any are set in the array
        if (_holdRotations.Length > 0)
        {
            return _holdRotations[_currentRotationIndex];
        }

        // Otherwise return 0
        return 0f;
    }

    private float GetCurrentRotation()
    {
        // Make sure we have any rotations available
        if (_holdRotations.Length > 0)
        {
            return _holdRotations[_currentRotationIndex];
        }

        // Otherwise return 0
        return 0f;
    }

    private void HandleDeath()
    {
        // Disable this script
        enabled = false;
    }
}
