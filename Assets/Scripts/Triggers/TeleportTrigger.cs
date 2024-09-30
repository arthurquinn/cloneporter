using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider2D))]
public class TeleportTrigger : MonoBehaviour
{
    public UnityAction OnPortalLeave { get; set; }

    private Rigidbody2D _rb;

    private const float PORTAL_TIMEOUT = 0.1f;
    private float _portalTimeoutTimer;

    private Vector2 _lastFixedPosition;

    private void Start()
    {
        _rb = GetComponentInParent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        // Decrease timer
        _portalTimeoutTimer -= Time.fixedDeltaTime;

        // Set last fixed position
        _lastFixedPosition = _rb.position;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        IPortal portal = collision.GetComponent<IPortal>();
        if (portal != null)
        {
            // Do not port if we are within the portal timeout
            if (_portalTimeoutTimer < 0)
            {
                HandleEnteredPortal(portal);
            }
        }
    }

    private void HandleEnteredPortal(IPortal portal)
    {
        // Calculate the entry ray
        Vector2 entryDirection = _rb.velocity.normalized;
        Vector2 entryPoint = _lastFixedPosition;
        Ray2D entryRay = new Ray2D(entryPoint, entryDirection);

        // Use portal interface to apply port to our rigidbody
        portal.ApplyPort(entryRay, _rb);

        // Set the portal timeout
        _portalTimeoutTimer = PORTAL_TIMEOUT;

        // Raise the portal leave event next frame
        StartCoroutine(RaisePortalLeaveEvent());
    }

    private IEnumerator RaisePortalLeaveEvent()
    {
        yield return new WaitForFixedUpdate();
        if (OnPortalLeave != null)
        {
            OnPortalLeave();
        }
    }

    //private void SetEnteredPortal(IPortal portal)
    //{
    //    _didEnterPortal = true;
    //    _enteredPortal = portal;
    //}

    //private void ClearEnteredPortal()
    //{
    //    _didEnterPortal = false;
    //    _enteredPortal = null;
    //}
}
