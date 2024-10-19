using UnityEngine;
using UnityEngine.Events;

public class DoorController : MonoBehaviour
{
    [Header("Door Activation Map")]
    [Tooltip("Used to link doors to entities that open them.")]
    [SerializeField] private DoorActivateMap _doorMap;

    [Header("Event Channels")]
    [SerializeField] private FloorSwitchEventChannel _floorSwitchChannel;
    [SerializeField] private BurnoutEventChannel _burnoutEventChannel;
    [SerializeField] private DoorEventChannel _doorChannel;

    public UnityAction OnOpen { get; set; }
    public UnityAction OnClose { get; set; }

    private BoxCollider2D[] _colliders;
    private Animator _animator;
    private int _isOpenID;

    private string[] _openedBy;

    // If we have a door opened by multiple entities (e.g. two switches or a switch and a laser activator)
    //   then we rely on the activation count to determine when we should open and close the door
    // Otherwise, for example if both switches are pressed and one is relased, when the deactivation event
    //   is fired the door will close even if another switch is still activated
    private int _activationCount;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _colliders = GetComponentsInChildren<BoxCollider2D>();
        _isOpenID = Animator.StringToHash("isOpen");
    }

    private void Start()
    {
        SetOpenedBy();
    }

    private void OnEnable()
    {
        _floorSwitchChannel.OnFloorSwitchActivated.Subscribe(HandleFloorSwitchActivated);
        _floorSwitchChannel.OnFloorSwitchDeactivated.Subscribe(HandleFloorSwitchDeactivated);

        _burnoutEventChannel.OnActivationChanged.Subscribe(HandleBurnoutActivationChanged);
        _burnoutEventChannel.OnDeath.Subscribe(HandleBurnoutDeath);
    }

    private void OnDisable()
    {
        _floorSwitchChannel.OnFloorSwitchActivated.Unsubscribe(HandleFloorSwitchActivated);
        _floorSwitchChannel.OnFloorSwitchDeactivated.Unsubscribe(HandleFloorSwitchDeactivated);

        _burnoutEventChannel.OnActivationChanged.Unsubscribe(HandleBurnoutActivationChanged);
        _burnoutEventChannel.OnDeath.Unsubscribe(HandleBurnoutDeath);
    }

    private void SetOpenedBy()
    {
        for (int i = 0; i < _doorMap.ActivationMap.Length; i++)
        {
            DoorActivateKeyValue pair = _doorMap.ActivationMap[i];
            if (pair.DoorID == gameObject.name)
            {
                _openedBy = pair.ActivatorKeys;
            }
        }
    }

    private void HandleBurnoutDeath(BurnoutDeathEvent @event)
    {
        if (IsOpenedBy(@event.Name))
        {
            ChangeActivationCount(1);
        }
    }

    private void HandleBurnoutActivationChanged(BurnoutReceiverActivationEvent @event)
    {
        if (IsOpenedBy(@event.Name))
        {
            if (@event.Type == BurnoutReceiverActivationType.Activated)
            {
                ChangeActivationCount(1);
            }
            else
            {
                ChangeActivationCount(-1);
            }
        }
    }

    private void HandleFloorSwitchActivated(FloorSwitchActivatedEvent e)
    {
        if (IsOpenedBy(e.SwitchName))
        {
            ChangeActivationCount(1);
        }
    }

    private void HandleFloorSwitchDeactivated(FloorSwitchDeactivatedEvent e)
    {
        if (IsOpenedBy(e.SwitchName))
        {
            ChangeActivationCount(-1);
        }
    }

    private void ChangeActivationCount(int change)
    {
        _activationCount += change;
        if (_activationCount > 0)
        {
            OpenDoor();
        }
        else
        {
            CloseDoor();
        }
    }

    private bool IsOpenedBy(string name)
    {
        // Check if we are opened by the name
        for (int i = 0; i < _openedBy.Length; i++)
        {
            if (name == _openedBy[i])
            {
                return true;
            }
        }

        // Otherwise return false
        return false;
    }

    private void OpenDoor()
    {
        _animator.SetBool(_isOpenID, true);
        RaiseOpenEvents();
    }

    private void CloseDoor()
    {
        _animator.SetBool(_isOpenID, false);
        SetCollidersEnabled(true);
        RaiseClosedEvents();
    }

    private void RaiseOpenEvents()
    {
        if (OnOpen != null)
        {
            OnOpen();
        }

        _doorChannel.OnOpen.Raise(new DoorOpenEvent(gameObject.name));
    }

    private void RaiseClosedEvents()
    {
        if (OnClose != null)
        {
            OnClose();
        }

        _doorChannel.OnClosed.Raise(new DoorClosedEvent(gameObject.name));
    }

    private void SetCollidersEnabled(bool enabled)
    {
        for (int i = 0; i < _colliders.Length; i++)
        {
            _colliders[i].enabled = enabled;
        }
    }

    #region Animator Callback Events

    public void OnDoorOpenAnimationComplete()
    {
        // Animation may complete after door closed was started if they enter and exit
        //   the switch quickly.
        // Only deactivate colliders if there is still an activation remaining
        if (_activationCount > 0)
        {
            SetCollidersEnabled(false);
        }
    }

    #endregion
}
