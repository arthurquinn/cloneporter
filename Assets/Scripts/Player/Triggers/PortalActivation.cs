using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(BoxCollider2D))]
public class PortalActivation : MonoBehaviour
{
    [SerializeField] private UnityEvent<IPortal> _onPortalEntered;

    private Vector2 _lastBoxSize;
    private Vector2 _boxSize;

    private BoxCollider2D _boxCollider;
    private Rigidbody2D _rb;

    private const float BOX_SIZE_MULTIPLIER = 0.80f;
    private const float BOX_SIZE_MIN_X = 0.5f;
    private const float BOX_SIZE_MIN_Y = 1.0f;

    private void Start()
    {
        // Get own box collider
        _boxCollider = GetComponent<BoxCollider2D>();
        _boxCollider.transform.position = transform.position;

        // Get parent rigidbody
        _rb = GetComponentInParent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        // Calculate box size based on speed
        _boxSize.x = Mathf.Max(BOX_SIZE_MIN_X, Mathf.Abs(_rb.velocity.x) * Time.fixedDeltaTime * BOX_SIZE_MULTIPLIER);
        _boxSize.y = Mathf.Max(BOX_SIZE_MIN_Y, Mathf.Abs(_rb.velocity.y) * Time.fixedDeltaTime * BOX_SIZE_MULTIPLIER);

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
            _onPortalEntered.Invoke(portal);
        }
    }
}
