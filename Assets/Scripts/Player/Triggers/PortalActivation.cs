using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(BoxCollider2D))]
public class PortalActivation : MonoBehaviour
{
    [SerializeField] private PlayerEventChannel _playerEvents;

    private Rigidbody2D _rb;
    private PlayerMovement _movement;

    private const float PORTAL_TIMEOUT = 0.1f;

    private bool _firstPortalEntry;
    private PortalColor _lastInPortalColor;
    private float _lastInPortalTime;

    private void Awake()
    {
        _firstPortalEntry = true;
    }

    private void Start()
    {
        // Get parent components
        _rb = GetComponentInParent<Rigidbody2D>();
        _movement = GetComponentInParent<PlayerMovement>();
    }

    private void FixedUpdate()
    {
        // Decrease timer
        _lastInPortalTime -= Time.fixedDeltaTime;
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
        // Ignore collision if we entered the portal within the portal timeout time and the portal is
        //   of the opposite color
        // This effectively means we are ignoring the entry of the opposite colored portal as soon as
        //   we teleport there
        bool didEnterPortalWithinTimer = _lastInPortalTime > 0 && portal.Color != _lastInPortalColor;
        if (_firstPortalEntry || !didEnterPortalWithinTimer)
        {
            _firstPortalEntry = false;
            _lastInPortalColor = portal.Color;
            _lastInPortalTime = PORTAL_TIMEOUT;

            HandlePortalEntered(portal);
        }
    }

    private void HandlePortalEntered(IPortal portal)
    {
        // Get our entry ray
        Vector2 entryDirection = _rb.velocity.normalized;
        Vector2 entryPoint = _movement.LastFixedPosition;
        Ray2D entryRay = new Ray2D(entryPoint, entryDirection);

        // Use portal interface to apply port to our rigidbody
        portal.ApplyPort(entryRay, _rb);

        // Raise portal leave event
        StartCoroutine(RaisePortalLeaveEvent());
    }

    private IEnumerator RaisePortalLeaveEvent()
    {
        // Run this event next fixed update frame (after we teleported)
        yield return new WaitForFixedUpdate();
        _playerEvents.OnPortalLeave.Raise(new PlayerLeavePortalEvent());
    }
}
