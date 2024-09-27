using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public interface ICarryable
{
    void Pickup(Transform carryPoint);
    void Drop();
}

public class ItemPickup : MonoBehaviour
{
    [SerializeField] private Transform _holdPosition;

    private PlayerInputActions _input;
    private ICarryable _cachedItem;
    private ICarryable _heldItem;

    private void Awake()
    {
        _input = new PlayerInputActions();
    }

    private void OnEnable()
    {
        _input.Player.Interact.Enable();
        _input.Player.Interact.performed += HandleInteract;
    }

    private void OnDisable()
    {
        _input.Player.Interact.performed -= HandleInteract;
        _input.Player.Interact.Disable();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        ICarryable carryable = collision.gameObject.GetComponent<ICarryable>();

        // Only cache a new item if we aren't already holding one
        if (carryable != null && _heldItem == null)
        {
            _cachedItem = carryable;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        _cachedItem = null;
    }

    private void HandleInteract(InputAction.CallbackContext context)
    {
        if (_heldItem != null)
        {
            DropItem();
        }
        if (_cachedItem != null)
        {
            CarryItem();
        }
    }

    private void CarryItem()
    {
        _cachedItem.Pickup(_holdPosition);
        _heldItem = _cachedItem;
        _cachedItem = null;
    }

    private void DropItem()
    {
        _heldItem.Drop();
        _heldItem = null;
    }
}
