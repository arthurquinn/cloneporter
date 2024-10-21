using DG.Tweening;
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
    [SerializeField] private float _delayAfterStop;

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

        // Start the animation
        _rb.DOMove(_elevatorTarget.position, _elevatorLiftTime)
            .SetEase(Ease.InCubic)
            .SetUpdate(UpdateType.Fixed)
            .OnComplete(RaiseStopEvent);
    }

    private void RaiseStopEvent()
    {
        // Start coroutine to wait for delay
        StartCoroutine(RaiseStopEventDelay());
    }

    private IEnumerator RaiseStopEventDelay()
    {
        // Wait the delay amount
        yield return new WaitForSeconds(_delayAfterStop);

        // Raise the event
        _elevatorEvents.OnElevatorStop.Raise(new ElevatorUpStopEvent());
    }

    #region Animator Callback Events

    public void OnDoorCloseAnimationComplete()
    {
        StartElevatorUp();
    }

    #endregion
}
