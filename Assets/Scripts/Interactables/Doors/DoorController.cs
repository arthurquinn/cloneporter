using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DoorController : MonoBehaviour
{
    [Header("Linked Floor Switch")]
    [Tooltip("A mapping of floor switch names to what entities they affect.")]
    [SerializeField] private FloorSwitchMap _floorSwitchMap;
    [SerializeField] private FloorSwitchEventChannel _floorSwitchChannel;

    private BoxCollider2D[] _colliders;
    private Animator _animator;
    private int _isOpenID;

    private string _linkedFloorSwitchName;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _colliders = GetComponentsInChildren<BoxCollider2D>();
        _isOpenID = Animator.StringToHash("isOpen");
    }

    private void Start()
    {
        SetLinkedFloorSwitchName();
    }

    private void OnEnable()
    {
        _floorSwitchChannel.OnFloorSwitchActivated.Subscribe(HandleFloorSwitchActivated);
        _floorSwitchChannel.OnFloorSwitchDeactivated.Subscribe(HandleFloorSwitchDeactivated);
    }

    private void OnDisable()
    {
        _floorSwitchChannel.OnFloorSwitchActivated.Unsubscribe(HandleFloorSwitchActivated);
        _floorSwitchChannel.OnFloorSwitchDeactivated.Unsubscribe(HandleFloorSwitchDeactivated);
    }

    private void SetLinkedFloorSwitchName()
    {
        foreach (FloorSwitchKeyValue pair in _floorSwitchMap.Map)
        {
            if (pair.EntityID == gameObject.name)
            {
                _linkedFloorSwitchName = pair.SwitchID;
            }
        }
    }

    private void HandleFloorSwitchActivated(FloorSwitchActivatedEvent e)
    {
        // If this event was fired by the switch we are linked to
        if (_linkedFloorSwitchName == e.SwitchName)
        {
            OpenDoor();
        }
    }

    private void HandleFloorSwitchDeactivated(FloorSwitchDeactivatedEvent e)
    {
        // If this event was fired by the switch we are linked to
        if (_linkedFloorSwitchName == e.SwitchName)
        {
            CloseDoor();
        }
    }

    private void OpenDoor()
    {
        _animator.SetBool(_isOpenID, true);
        SetCollidersEnabled(false);
    }

    private void CloseDoor()
    {
        _animator.SetBool(_isOpenID, false);
        SetCollidersEnabled(true);
    }

    private void SetCollidersEnabled(bool enabled)
    {
        for (int i = 0; i < _colliders.Length; i++)
        {
            _colliders[i].enabled = enabled;
        }
    }
}
