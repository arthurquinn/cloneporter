using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public interface ICarryable
{
    Collider2D Collider { get; }

    void Pickup(Transform carryPoint);
    void Drop();
}

public class ItemPickup : MonoBehaviour
{
    [SerializeField] private Collider2D _playerCollider;
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
        // Pickup item
        _cachedItem.Pickup(_holdPosition);

        // Remove item from cache and store in held item
        _heldItem = _cachedItem;
        _cachedItem = null;

        // Disable collisions between us and the carried item
        Physics2D.IgnoreCollision(_playerCollider, _heldItem.Collider, true);
    }

    private void DropItem()
    {
        // Reenable collisions between us and the previously held item
        Physics2D.IgnoreCollision(_playerCollider, _heldItem.Collider, false);

        // Drop the item
        _heldItem.Drop();
        _heldItem = null;
    }
}
