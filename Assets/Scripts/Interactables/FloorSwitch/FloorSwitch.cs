using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorSwitch : MonoBehaviour
{
    [SerializeField] private FloorSwitchEventChannel _switchEventChannel;
    [SerializeField] private FloorSwitchTrigger _trigger;

    private Animator _animator;

    private int _isPressedID;

    private void Awake()
    {
        _isPressedID = Animator.StringToHash("isPressed");
    }

    private void Start()
    {
        _animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        AttachTriggerEvents();
    }

    private void OnDisable()
    {
        DetachTriggerEvents();
    }

    private void AttachTriggerEvents()
    {
        if (_trigger != null)
        {
            _trigger.OnSwitchActivated += HandleSwitchActivated;
            _trigger.OnSwitchDeactivated += HandleSwitchDeactivated;
        }
    }

    private void DetachTriggerEvents()
    {
        if (_trigger != null )
        {
            _trigger.OnSwitchActivated -= HandleSwitchActivated;
            _trigger.OnSwitchDeactivated -= HandleSwitchDeactivated;
        }
    }

    private void HandleSwitchActivated(Collider2D collision)
    {
        _animator.SetBool(_isPressedID, true);
        _switchEventChannel.OnFloorSwitchActivated.Raise(new FloorSwitchActivatedEvent(gameObject.name));
    }

    private void HandleSwitchDeactivated(Collider2D collision)
    {
        _animator.SetBool(_isPressedID, false);
        _switchEventChannel.OnFloorSwitchDeactivated.Raise(new FloorSwitchDeactivatedEvent(gameObject.name));
    }
}
