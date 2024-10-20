using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TutorialLevel02 : MonoBehaviour
{
    [Header("Event Channels")]
    [SerializeField] private PlayerEventChannel _playerEvents;

    [Header("Trigger Areas")]
    [Tooltip("The trigger area for the pickup item text.")]
    [SerializeField] private GenericTrigger _pickupItemTrigger;
    [Tooltip("The trigger area for the velocity hint text.")]
    [SerializeField] private GenericTrigger _velocityHintTrigger;
    [Tooltip("The trigger area to hide the velocity hint text.")]
    [SerializeField] private GenericTrigger _velocityHintHide;

    [Header("Info Texts")]
    [Tooltip("The pickup item text game object.")]
    [SerializeField] private GameObject _pickupItemText;
    [Tooltip("The drop item text game object.")]
    [SerializeField] private GameObject _dropItemText;
    [Tooltip("The velocity hint text game object.")]
    [SerializeField] private GameObject _velocityHintText;

    private bool _didPickup;
    private bool _didDrop;

    private Canvas _canvas;

    private void Awake()
    {
        AssignCamera();
    }

    private void OnEnable()
    {
        _pickupItemTrigger.OnTriggerEnter += HandlePickupItemTriggerEnter;
        _velocityHintTrigger.OnTriggerEnter += HandleVelocityHintTriggerEnter;
        _velocityHintHide.OnTriggerEnter += HandleVelocityHintTriggerExit;


        _playerEvents.OnPickupItem.Subscribe(HandleItemPickup);
        _playerEvents.OnDropItem.Subscribe(HandleItemDrop);
    }

    private void OnDisable()
    {
        _pickupItemTrigger.OnTriggerEnter -= HandlePickupItemTriggerEnter;
        _velocityHintTrigger.OnTriggerEnter -= HandleVelocityHintTriggerEnter;
        _velocityHintHide.OnTriggerEnter -= HandleVelocityHintTriggerExit;

        _playerEvents.OnPickupItem.Unsubscribe(HandleItemPickup);
        _playerEvents.OnDropItem.Unsubscribe(HandleItemDrop);
    }

    private void HandlePickupItemTriggerEnter()
    {
        if (!_didPickup)
        {
            _pickupItemText.SetActive(true);
            _didPickup = true;
        }
    }

    private void HandleItemPickup(PlayerPickupItemEvent @event)
    {
        if (!_didDrop)
        {
            _pickupItemText.SetActive(false);
            _dropItemText.SetActive(true);
            _didDrop = true;
        }
    }

    private void HandleItemDrop(PlayerDropItemEvent @event)
    {
        _dropItemText.SetActive(false);
    }

    private void HandleVelocityHintTriggerEnter()
    {
        _velocityHintText.SetActive(true);
    }

    private void HandleVelocityHintTriggerExit()
    {
        _velocityHintText.SetActive(false);
    }

    private void AssignCamera()
    {
        _canvas = GetComponentInChildren<Canvas>();
        if (_canvas != null)
        {
            _canvas.renderMode = RenderMode.ScreenSpaceCamera;
            _canvas.worldCamera = Camera.main;
            _canvas.sortingLayerName = "HUD";
        }
    }
}
