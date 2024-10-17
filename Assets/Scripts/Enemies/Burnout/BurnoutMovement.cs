using System.Collections;
using System.Collections.Generic;
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
    [SerializeField] private Quaternion[] _holdRotations;

    private float _currentHoldTime;
    private int _currentRotationIndex;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();

        // Init the hold timer
        _currentHoldTime = _holdTime;
    }

    private void Start()
    {
        // Initialize  the rotation
        Quaternion initRotation = GetCurrentRotation();
        _rb.rotation = initRotation.eulerAngles.z;
    }

    private void FixedUpdate()
    {
        // Decrement hold rotation timer
        _currentHoldTime -= Time.fixedDeltaTime;

        // If our timer expired then start the coroutine to rotate to our next value
        if (_currentHoldTime < 0)
        {
            // Kick off coroutine
            StartCoroutine(RotateToNext());

            // Disable the hold timer until the coroutine completes
            _currentHoldTime = float.MaxValue;
        }
    }

    private IEnumerator RotateToNext()
    {
        // Initialize values
        Quaternion startRotation = GetCurrentRotation();
        Quaternion endRotation = IncrementToNextRotation();
        float timer = 0;

        // Perform the rotation over rotation time
        while (timer < _rotationTime)
        {
            // Increment the timer
            timer += Time.fixedDeltaTime;

            // Calculate and apply the rotation to the rigidbody based on time step
            float timeStep = timer / _rotationTime;
            Quaternion currentRotation = Quaternion.Lerp(startRotation, endRotation, timeStep);
            _rb.rotation = currentRotation.eulerAngles.z;

            // Run the loop again next fixed update
            yield return new WaitForFixedUpdate();
        }

        // Reset the hold timer and exit
        _currentHoldTime = _holdTime;
    }

    private Quaternion IncrementToNextRotation()
    {
        // Increment and cycle to the first index if we reached the max
        _currentRotationIndex = (_currentRotationIndex + 1) % _holdRotations.Length;

        // Return the current rotation if any are set in the array
        if (_holdRotations.Length > 0)
        {
            return _holdRotations[_currentRotationIndex];
        }

        // Otherwise return the identity;
        return Quaternion.identity;
    }

    private Quaternion GetCurrentRotation()
    {
        // Make sure we have any rotations available
        if (_holdRotations.Length > 0)
        {
            return _holdRotations[_currentRotationIndex];
        }

        // Otherwise return the identity
        return Quaternion.identity;
    }
}
