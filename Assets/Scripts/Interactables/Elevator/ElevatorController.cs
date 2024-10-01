using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private ElevatorEventChannel _elevatorEvents;

    [Tooltip("The trigger to control when elevator should close and start.")]
    [SerializeField] private ElevatorTrigger _elevatorTrigger;

    [Tooltip("The child game object whose colliders make up the walls of the elevator used by the composite.")]
    [SerializeField] private GameObject _elevatorWalls;

    [Tooltip("The target that elevator will ascend to.")]
    [SerializeField] private Transform _elevatorTarget;

    [Header("Elevator Stats")]
    [SerializeField] private float _elevatorLiftTime;

    private Rigidbody2D _rb;
    private Animator _animator;
    private int _isClosedID;

    private void Awake()
    {
        _isClosedID = Animator.StringToHash("isClosed");
    }

    private void Start()
    {
        // Get components
        _rb = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        _elevatorTrigger.OnPlayerEnter += HandlePlayerEnter;
    }

    private void OnDisable()
    {
        _elevatorTrigger.OnPlayerEnter -= HandlePlayerEnter;
    }

    private void HandlePlayerEnter()
    {
        // Activate the elevator wall colliders
        ActivateWalls();

        // Start the door close animation
        StartDoorCloseAnimation();
    }

    private void ActivateWalls()
    {
        _elevatorWalls.SetActive(true);
    }

    private void StartDoorCloseAnimation()
    {
        _animator.SetBool(_isClosedID, true);
    }

    private void StartElevatorUp()
    {
        // Raise the elevator up event
        _elevatorEvents.OnElevatorUp.Raise(new ElevatorUpStartEvent());

        // Start the elevator up coroutine
        StartCoroutine(DoElevatorUp());
    }

    private IEnumerator DoElevatorUp()
    {
        // Initialze the start and end positions, and the lift timer
        Vector2 startPosition = transform.position;
        Vector2 endPosition = _elevatorTarget.position;
        float liftTime = 0;
        float timeStep = 0;

        // Lerp to target position each fixed update
        while (timeStep < 1)
        {
            // Calculate the current time step
            liftTime += Time.fixedDeltaTime;
            timeStep = Mathf.Clamp01(liftTime / _elevatorLiftTime);

            // Lerp to target
            Vector2 currentTarget = Vector2.Lerp(startPosition, endPosition, timeStep);

            // Move rigidbody to target
            _rb.MovePosition(currentTarget);

            // Wait for next fixed update frame
            yield return new WaitForFixedUpdate();
        }

        // Raise the elevator stop event
        _elevatorEvents.OnElevatorStop.Raise(new ElevatorUpStopEvent());
    }

    #region Animator Callback Events

    public void OnDoorCloseAnimationComplete()
    {
        StartElevatorUp();
    }

    #endregion
}
