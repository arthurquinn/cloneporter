using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(BoxCollider2D))]
public class PortalActivation : MonoBehaviour
{
    [SerializeField] private Transform _portalActivationCenter;

    [SerializeField] private UnityEvent<IPortal> _onPortalEntered;

    private Vector2 _lastBoxSize;
    private Vector2 _boxSize;

    private BoxCollider2D _boxCollider;
    private Rigidbody2D _rb;

    private const float BOX_SIZE_MULTIPLIER = 0.20f;

    private void Start()
    {
        // Get own box collider
        _boxCollider = GetComponent<BoxCollider2D>();
        _boxCollider.transform.position = _portalActivationCenter.position;

        // Get parent rigidbody
        _rb = GetComponentInParent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        // Calculate box size based on speed
        _boxSize.x = Mathf.Max(0.01f, Mathf.Abs(_rb.velocity.x) * Time.fixedDeltaTime * BOX_SIZE_MULTIPLIER);
        _boxSize.y = Mathf.Max(0.01f, Mathf.Abs(_rb.velocity.y) * Time.fixedDeltaTime * BOX_SIZE_MULTIPLIER);

        // If the box size changed, update our box collider
        if (_boxSize != _lastBoxSize)
        {
            _boxCollider.size = _boxSize;
        }

        // Set the current box size to the last box size
        _lastBoxSize = _boxSize;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        IPortal portal = collision.GetComponent<IPortal>();
        if (portal != null)
        {
            Debug.Log("Collision");
            _onPortalEntered.Invoke(portal);
        }
    }
}
