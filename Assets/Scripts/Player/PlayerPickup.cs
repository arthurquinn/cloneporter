using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerPickup : MonoBehaviour
{
    [SerializeField] private Transform _holdPoint;

    // Holds the object that is within the collider
    // May or may not be actually held by player
    private ICarryable _carryBuffer;

    private ICarryable _carriedItem;

    private PlayerInputActions _input;

    private void Awake()
    {
        _input = new PlayerInputActions();
    }

    private void OnEnable()
    {
        _input.Player.Interact.Enable();
    }

    private void OnDisable()
    {
        _input.Player.Interact.Disable();
    }

    private void Start()
    {
        _input.Player.Interact.performed += OnInteract;
    }

    private void Update()
    {
        if (_carriedItem != null)
        {
            _carriedItem.UpdateCarryPosition(_holdPoint.position);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        ICarryable carryable = collision.GetComponent<ICarryable>();
        if (carryable != null)
        {
            _carryBuffer = carryable;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        _carryBuffer = null;
    }

    private void OnInteract(InputAction.CallbackContext context)
    {
        if (CanPickupItem())
        {
            _carriedItem = _carryBuffer;
            _carriedItem.StartCarry();
        }
        else if (CanDropItem())
        {
            _carriedItem.StopCarry();
            _carriedItem = null;
        }
    }

    private bool CanPickupItem()
    {
        // If we are not currently holding an item, and if our carry buffer holds a potential item
        return _carriedItem == null && _carryBuffer != null;
    }

    private bool CanDropItem()
    {
        // If we are carrying an object
        return _carriedItem != null;
    }
}
