using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider2D))]
public class TeleportTrigger : MonoBehaviour
{
    [Tooltip("The bounds of the object that will be teleported.")]
    [SerializeField] private Collider2D _bounds;

    [Tooltip("The portal layer.")]
    [SerializeField] private LayerMask _portalLayer;

    public UnityAction OnTeleported { get; set; }

    private Rigidbody2D _rb;

    private Vector2 _lastFixedPosition;

    private bool _didTeleport;

    private void Start()
    {
        _rb = GetComponentInParent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        // Set last fixed position
        _lastFixedPosition = _rb.position;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        IPortal portal = collision.GetComponent<IPortal>();
        if (portal != null && !_didTeleport)
        {
            HandleEnteredPortal(portal);
        }
    }

    private void HandleEnteredPortal(IPortal portal)
    {
        // Calculate the entry ray
        Vector2 entryDirection = _rb.velocity.normalized;
        Vector2 entryPoint = _lastFixedPosition;
        Ray2D entryRay = new Ray2D(entryPoint, entryDirection);

        // Use portal interface to apply port to our rigidbody
        portal.ApplyPort(entryRay, _rb, _bounds.bounds);

        // Raise the portal leave portal event after fixed update
        StartCoroutine(RaiseTeleportedEvent());

        // Start coroutine to check when we left exit portal
        StartCoroutine(WaitForExitPortal());
    }

    private IEnumerator WaitForExitPortal()
    {
        // Set our did teleport flag
        _didTeleport = true;

        // Convert our bounds into a square for better checking
        float maxSide = Mathf.Max(_bounds.bounds.size.x, _bounds.bounds.size.y);
        Vector2 boxSize = new Vector2(maxSide, maxSide);

        while (_didTeleport)
        {
            // Check each fixed update
            yield return new WaitForFixedUpdate();

            if (CheckDidExitPortal(boxSize))
            {
                // If we exited the portal set our flag to false
                _didTeleport = false;
            }
        }
    }

    private bool CheckDidExitPortal(Vector2 boxSize)
    {
        // Check overlap box for when our bounds leaves the portal
        Collider2D overlap = Physics2D.OverlapBox(_bounds.bounds.center, boxSize, 0, _portalLayer);

        // If we overlap nothing than we fully exited the exit portal
        return overlap == null;
    }

    private IEnumerator RaiseTeleportedEvent()
    {
        yield return new WaitForFixedUpdate();
        if (OnTeleported != null)
        {
            OnTeleported();
        }
    }
}
