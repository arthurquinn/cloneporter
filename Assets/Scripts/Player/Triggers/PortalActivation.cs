using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(BoxCollider2D))]
public class PortalActivation : MonoBehaviour
{
    [SerializeField] private UnityEvent<IPortal> _onPortalEntered;
    [SerializeField] private float _portalTimeout;

    private Vector2 _lastBoxSize;
    private Vector2 _boxSize;

    private BoxCollider2D _boxCollider;
    private Rigidbody2D _rb;

    private const float BOX_SIZE_MULTIPLIER = 1.5f;
    private const float BOX_SIZE_MIN_X = 0.1f;
    private const float BOX_SIZE_MIN_Y = 0.1f;

    private bool _firstPortalEntry;
    private PortalColor _lastInPortalColor;
    private float _lastInPortalTime;

    private void Awake()
    {
        _firstPortalEntry = true;
    }

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
        // Decrease timer
        _lastInPortalTime -= Time.fixedDeltaTime;

        // Calculate box size based on speed
        //_boxSize.x = Mathf.Max(BOX_SIZE_MIN_X, Mathf.Abs(_rb.velocity.x) * Time.fixedDeltaTime * BOX_SIZE_MULTIPLIER);
        //_boxSize.y = Mathf.Max(BOX_SIZE_MIN_Y, Mathf.Abs(_rb.velocity.y) * Time.fixedDeltaTime * BOX_SIZE_MULTIPLIER);

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
            HandlePortalTriggerEnter(portal);
        }
    }

    private void HandlePortalTriggerEnter(IPortal portal)
    {
        // Ignore all conditions if this is our first portal entry
        // Ignore the portal color check if we haven't collided with a portal after _portalTimeout time
        // If this isn't our first entry, and we entered a portal within the portal timeout time,
        //   then it is only a valid portal entry if our portal color is different than the last one
        //   we entered
        if (_firstPortalEntry || _lastInPortalTime < 0 || portal.Color != _lastInPortalColor)
        {
            _firstPortalEntry = false;
            _lastInPortalColor = portal.Color;
            _lastInPortalTime = _portalTimeout;
            _onPortalEntered.Invoke(portal);
        }
    }
}
